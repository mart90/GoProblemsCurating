namespace GoProblemGenerator.Database
{
    public class KataGoMove
    {
        public string GameId { get; set; }

        public int MoveNumber { get; set; }

        public string Move { get; set; }

        public double Winrate { get; set; }

        public double ScoreLead { get; set; }
    }
}
