using System.Collections.Generic;

namespace DataAccess.Model.SharedModels
{
    public class GuessResponseData : ApiResponseData
    {
        public List<SingleGuessResponseObject> Guesses { get; set; }
    }
}