namespace DataAccess.Model.SharedModels
{
    public class SingleGuessRequestObject
    {
        public string Team { get; set; }
        public string Guess { get; set; }
    }

    public class SingleGuessResponseObject
    {
        public string TargetTeam { get; set; }
        public string Guess { get; set; }
        public int NoOfDigitsMatchedByPositionAndValue { get; set; }
        public int NoOfDigitsMatchedByValue { get; set; }
        public long Score { get; set; }
        public bool IsValid { get; set; }
        public string ErrMessage { get; set; }
    }
}