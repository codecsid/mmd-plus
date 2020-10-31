using System.Collections.Generic;

namespace DataAccess.Model.SharedModels
{
    public class GuessRequestBody
    {
        public List<SingleGuessRequestObject> Guesses { get; set; }
    }
}