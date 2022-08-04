namespace Go
{
    public class Intersection
    {
        public int X { get; set; }

        public int Y { get; set; }

        public StoneColor StoneColor { get; set; }

        public string StringNotation { get; set; }

        public bool IsLastMove { get; set; }

        public bool IsKataGoMove { get; set; }

        public void Reset()
        {
            StoneColor = StoneColor.None;
            IsLastMove = false;
            IsKataGoMove = false;
        }
    }
}
