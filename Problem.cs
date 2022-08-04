using System.Collections.Generic;

namespace Go
{
    public class Problem
    {
        public string GameId { get; set; }

        public int MoveNumber { get; set; }

        public bool TriedByUser { get; set; }

        public List<GameMove> GameMoves { get; private set; }

        public List<KataGoMove> KataGoMoves { get; private set; }

        public Problem()
        {
            GameMoves = new List<GameMove>();
            KataGoMoves = new List<KataGoMove>();
        }
    }
}
