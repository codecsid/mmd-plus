using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model.SharedModels;
using GameApi.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameApi.Service
{
    public class EvaluationModule
    {
        private readonly IGameApiService _gameApiService;
        private readonly long CORRECT_DIGIT_SCORE = 5;
        private readonly long CORRECT_POSITION_SCORE = 10;
        private readonly long BONUS = 0;

        public EvaluationModule(IGameApiService gameApiService)
        {
            _gameApiService = gameApiService ?? throw new ArgumentNullException("gameApiService");
        }

        public async Task<EvaluationResult> EvaluateTheGuess(string guessingTeam, string targetTeam, string guess, string actualSecret)
        {
            var result = CompareGuess(guess, actualSecret);

            try
            {
                await _gameApiService.ApplyEvaluation(guessingTeam, targetTeam, result.PointsScored, result.CorrectGuess);
            }
            catch(TargetTeamDeadException)
            {
                result.PointsScored = result.NoOfDigitsMatchedByValue = result.NoOfDigitsMatchedByValueAndPosition = 0;
                result.ErrMessage = "Target Team is already dead";
            }
            catch(GuessingTeamDeadException)
            {
                result.PointsScored = result.NoOfDigitsMatchedByValue = result.NoOfDigitsMatchedByValueAndPosition = 0;
                result.ErrMessage = "Your team is dead, please try in next round";
            }
            catch(TargetAlreadyKilledException)
            {
                result.PointsScored = result.NoOfDigitsMatchedByValue = result.NoOfDigitsMatchedByValueAndPosition = 0;
                result.ErrMessage = "You have already killed this team";
            }
            catch(Exception)
            {
                result.PointsScored = result.NoOfDigitsMatchedByValue = result.NoOfDigitsMatchedByValueAndPosition = 0;
                result.ErrMessage = "Evaluation failed";
            }

            return result;
        }

        private EvaluationResult CompareGuess(string guess, string actualSecret)
        {
            if (string.IsNullOrWhiteSpace(guess) || string.IsNullOrWhiteSpace(actualSecret))
                return new EvaluationResult { NoOfDigitsMatchedByValue = 0, NoOfDigitsMatchedByValueAndPosition = 0, PointsScored = 0 };

            if (guess.Length > actualSecret.Length)
                guess = guess.Substring(0, actualSecret.Length);

            if (guess.Equals(actualSecret))
                return new EvaluationResult {
                    NoOfDigitsMatchedByValue = actualSecret.Length,
                    NoOfDigitsMatchedByValueAndPosition = actualSecret.Length,
                    PointsScored = (actualSecret.Length * (CORRECT_DIGIT_SCORE + CORRECT_POSITION_SCORE)) + BONUS,
                    CorrectGuess = true
                };

            var guessCountMap = new Dictionary<char, int>();
            var secretCountMap = new Dictionary<char, int>();
            PopulateDict(guess, guessCountMap);
            PopulateDict(actualSecret, secretCountMap);

            long score = 0;
            int digitsMatchedByValue = 0;

            // first count common occurrences
            foreach(var pair in secretCountMap)
            {
                var digit = pair.Key;
                if(guessCountMap.ContainsKey(digit))
                {
                    var commonDigits = Math.Min(guessCountMap[digit], secretCountMap[digit]);
                    score += commonDigits * CORRECT_DIGIT_SCORE;
                    digitsMatchedByValue += commonDigits;
                }
            }

            var digitsMatchedByValueAndPosition = 0;

            for (int i = 0;i < guess.Length;i++)
            {
                if (guess[i] == actualSecret[i])
                    digitsMatchedByValueAndPosition++;
            }

            score += digitsMatchedByValueAndPosition * CORRECT_POSITION_SCORE;

            return new EvaluationResult
            {
                NoOfDigitsMatchedByValue = digitsMatchedByValue,
                NoOfDigitsMatchedByValueAndPosition = digitsMatchedByValueAndPosition,
                PointsScored = score,
                CorrectGuess = digitsMatchedByValueAndPosition == actualSecret.Length
            };
        }

        private void PopulateDict(string s, Dictionary<char, int> dict)
        {
            foreach (char c in s)
            {
                if (!dict.ContainsKey(c))
                    dict[c] = 0;
                dict[c]++;
            }
        }
    }
}
