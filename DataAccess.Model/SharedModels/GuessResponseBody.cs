using System.Collections.Generic;

namespace DataAccess.Model.SharedModels
{
    public class GuessResponseBody
    {
        public List<SingleGuessResponseObject> Guesses { get; set; }
    }
}