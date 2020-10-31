using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameApi.Service.Models
{
    public class EvaluationResult
    {
        public int NoOfDigitsMatchedByValueAndPosition { get; set; }
        public int NoOfDigitsMatchedByValue { get; set; }
        public long PointsScored { get; set; }
        public string ErrMessage { get; set; }
        public bool CorrectGuess { get; set; }
    }
}
