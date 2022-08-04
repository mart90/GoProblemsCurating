namespace GoProblemGenerator
{
    public class KataGoAnalysisFeedback
    {
        public string Id { get; set; }

        public bool IsDuringSearch { get; set; }

        public int TurnNumber { get; set; }

        public MoveInfo[] MoveInfos { get; set; }

        public RootInfo RootInfo { get; set; }
    }

    public class MoveInfo
    {
        public string Move { get; set; }

        public double ScoreLead { get; set; }

        public int Visits { get; set; }

        public double Winrate { get; set; }
    }

    public class RootInfo
    {
        public string CurrentPlayer { get; set; }

        public double ScoreLead { get; set; }

        public double Winrate { get; set; }
    }
}
