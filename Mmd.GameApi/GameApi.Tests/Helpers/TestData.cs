using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Model;
using DataAccess.Model.SharedModels;

namespace GameApi.Tests.Helpers
{
    public class TestData
    {
        public static Guid Round1Id = Guid.Parse("cf92d2e1-6398-43b0-a023-5a9a23666b2c");
        public static Guid Round2Id = Guid.Parse("9277a98f-ce7e-4012-98a7-530ecddf2d4b");
        public static Guid GameId = Guid.Parse("581e538d-dc16-4544-a783-ddb6462dfb37");
        public static Guid Guess1Id = Guid.Parse("581e538d-dc16-4544-a783-ddb6462dfb38");
        public static Guid Guess2Id = Guid.Parse("581e538d-dc16-4544-a783-ddb6462dfb39");
        public static IQueryable<Game> Games
        {
            get
            {
                return new List<Game>
                {
                    new Game { Id = 1, GameId = GameId, TimeStamp = DateTime.UtcNow }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Guess> Guesses
        {
            get
            {
                return new List<Guess>
                {
                    new Guess { Id = 1, GuessId = Guess1Id, GameId = GameId, RoundId = Round1Id, GuessRequest = new GuessRequestBody(), GuessResponse = new GuessResponseBody(), TimeStamp = DateTime.UtcNow, TeamId = "TestTeam1" },
                    new Guess { Id = 1, GuessId = Guess2Id, GameId = GameId, RoundId = Round1Id, GuessRequest = new GuessRequestBody(), GuessResponse = new GuessResponseBody(), TimeStamp = DateTime.UtcNow, TeamId = "TestTeam1" }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Kill> Kills
        {
            get
            {
                return new List<Kill>
                {
                    new Kill { Id = 1, RoundId = Round1Id, GameId = GameId, VictimId = "TestTeam2", KillerId = "TestTeam1", TimeStamp = DateTime.UtcNow },
                    new Kill { Id = 2, RoundId = Round1Id, GameId = GameId, VictimId = "TestTeam3", KillerId = "TestTeam1", TimeStamp = DateTime.UtcNow }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Participant> Participants
        {
            get
            {
                return new List<Participant>
                {
                    new Participant { Id = 1, GameId = GameId, RoundId = Round1Id, TeamId = "TestTeam1", IsAlive = true, JoinedAt = DateTime.UtcNow },
                    new Participant { Id = 2, GameId = GameId, RoundId = Round1Id, TeamId = "TestTeam2", IsAlive = false, JoinedAt = DateTime.UtcNow },
                    new Participant { Id = 3, GameId = GameId, RoundId = Round1Id, TeamId = "TestTeam3", IsAlive = false, JoinedAt = DateTime.UtcNow },
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Request> Requests
        {
            get
            {
                return new List<Request>
                {
                    new Request { Id = 1, RequestApi = RequestApi.Gamestatus.ToString(), RequestMethod = RequestMethod.GET, TimeStamp = DateTime.UtcNow },
                    new Request { Id = 2, GameId = GameId, RoundId = Round1Id, RequestApi = RequestApi.Gamestatus.ToString(), RequestMethod = RequestMethod.GET, TimeStamp = DateTime.UtcNow },
                    new Request { Id = 3, GameId = GameId, RoundId = Round1Id, RequestApi = RequestApi.Join.ToString(), RequestMethod = RequestMethod.POST, TimeStamp = DateTime.UtcNow },
                    new Request { Id = 4, GameId = GameId, RoundId = Round1Id, RequestApi = RequestApi.Guess.ToString(), RequestMethod = RequestMethod.POST, TimeStamp = DateTime.UtcNow }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Round> Rounds
        {
            get
            {
                return new List<Round>
                {
                    new Round { 
                        Id = 1, 
                        RoundNumber = 1, 
                        GameId = GameId,
                        RoundId = Round1Id,
                        TimeStamp = DateTime.UtcNow
                    }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Score> Scores
        {
            get
            {
                return new List<Score>
                {
                    new Score { Id = 1, GameId = GameId, RoundId = Round1Id, TeamId = "TestTeam1", GuessId = Guess1Id, PointsScored = 1000, TimeStamp = DateTime.UtcNow },
                    new Score { Id = 2, GameId = GameId, RoundId = Round1Id, TeamId = "TestTeam1", GuessId = Guess2Id, PointsScored = 1200, TimeStamp = DateTime.UtcNow },
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Team> Teams
        {
            get
            {
                return new List<Team>
                {
                    new Team { Id = 1, TeamId = "TestTeam1", SecretToken = "TestTeam1Token", Location = "TestLocation1", IsRobot = false, RegisteredAt = DateTime.UtcNow, LastUpdatedAt = DateTime.UtcNow },
                    new Team { Id = 2, TeamId = "TestTeam2", SecretToken = "TestTeam2Token", Location = "TestLocation2", IsRobot = true, RegisteredAt = DateTime.UtcNow, LastUpdatedAt = DateTime.UtcNow },
                    new Team { Id = 3, TeamId = "TestTeam3", SecretToken = "TestTeam3Token", Location = "TestLocation3", IsRobot = false, RegisteredAt = DateTime.UtcNow, LastUpdatedAt = DateTime.UtcNow }
                }
                .AsQueryable();
            }
        }

        public static IQueryable<Phase> Phases
        {
            get
            {
                return new List<Phase> { 
                    new Phase { GameId = GameId, RoundId = Round1Id, PhaseType = PhaseType.Joining, TimeStamp = DateTime.UtcNow }
                }.AsQueryable();
            }
        }
    }
}