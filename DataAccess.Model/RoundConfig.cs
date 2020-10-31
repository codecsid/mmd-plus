namespace DataAccess.Model
{
    public class RoundConfig : IEntityBase
    {
        public long Id { get; set; }
        public long JoiningDuration { get; set; }
        public long RunningDuration { get; set; }
        public long FinishedDuration { get; set; }
        public int SecretLength { get; set; }
        public int LifeLines { get; set; }
        public long Penalty { get; set; }
    }
}