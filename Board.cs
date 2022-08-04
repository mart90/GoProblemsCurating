using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Go
{
    public class Board
    {
        public List<Intersection> Intersections { get; private set; }

        public Board()
        {
            Intersections = new List<Intersection>();

            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    char xChar = x <= 7 ? (char)(x + 65) : (char)(x + 66);

                    Intersections.Add(new Intersection
                    {
                        X = x,
                        Y = y,
                        StringNotation = $"{xChar}{y + 1}"
                    });
                }
            }
        }

        private void Empty()
        {
            foreach (Intersection intersection in Intersections)
            {
                intersection.Reset();
            }
        }

        public bool PointIsKataGoMove(Point location)
        {
            int x = (int)Math.Round((double)(location.X - 32.5) / 65, 0);
            int y = 18 - (int)Math.Round((double)(location.Y - 32.5) / 65, 0);

            Intersection intersection = Intersections.Single(e => e.X == x && e.Y == y);

            return intersection.IsKataGoMove;
        }

        public bool TryMakeMove(StoneColor color, int x, int y)
        {
            Intersection intersection = Intersections.Single(e => e.X == x && e.Y == y);
            return TryPlaceStone(color, intersection);
        }

        public bool TryMakeMove(StoneColor color, string stringNotation)
        {
            Intersection intersection = Intersections.Single(e => e.StringNotation == stringNotation);
            return TryPlaceStone(color, intersection);
        }

        private bool TryPlaceStone(StoneColor color, Intersection intersection)
        {
            if (intersection.StoneColor != StoneColor.None)
            {
                return false;
            }

            intersection.StoneColor = color;

            if (intersection.X > 0)
            {
                MaybeCaptureGroup(GetNeighbor(intersection, Direction.Left));
            }
            if (intersection.X < 18)
            {
                MaybeCaptureGroup(GetNeighbor(intersection, Direction.Right));
            }
            if (intersection.Y > 0)
            {
                MaybeCaptureGroup(GetNeighbor(intersection, Direction.Down));
            }
            if (intersection.Y < 18)
            {
                MaybeCaptureGroup(GetNeighbor(intersection, Direction.Up));
            }

            return true;
        }

        public void PrepareProblem(Problem problem)
        {
            Empty();

            foreach (GameMove move in problem.GameMoves.Where(e => e.MoveNumber < problem.MoveNumber))
            {
                TryMakeMove(move.MoveNumber % 2 == 0 ? StoneColor.Black : StoneColor.White, move.Move);

                if (move.MoveNumber == problem.MoveNumber - 1)
                {
                    Intersections.Single(e => e.StringNotation == move.Move).IsLastMove = true;
                }
            }

            foreach (KataGoMove kgm in problem.KataGoMoves)
            {
                Intersections.Single(e => e.StringNotation == kgm.Move).IsKataGoMove = true;
            }
        }

        private void MaybeCaptureGroup(Intersection intersection)
        {
            var group = new List<Intersection> { intersection };

            if (IsSurroundedRecursive(intersection, null, group))
            {
                foreach (Intersection intersect in group)
                {
                    Intersections.Single(e => e.StringNotation == intersect.StringNotation).StoneColor = StoneColor.None;
                }
            }
        }

        private bool IsSurroundedRecursive(Intersection intersection, Direction? lastDirection, List<Intersection> group)
        {
            if (intersection.X > 0 && lastDirection != Direction.Right)
            {
                Intersection neighbor = GetNeighbor(intersection, Direction.Left);

                if (neighbor.StoneColor == StoneColor.None)
                {
                    return false;
                }
                
                if (neighbor.StoneColor == intersection.StoneColor)
                {
                    group.Add(neighbor);

                    if (!IsSurroundedRecursive(neighbor, Direction.Left, group))
                    {
                        return false;
                    }
                }
            }
            if (intersection.X < 18 && lastDirection != Direction.Left)
            {
                Intersection neighbor = GetNeighbor(intersection, Direction.Right);

                if (neighbor.StoneColor == StoneColor.None)
                {
                    return false;
                }

                if (neighbor.StoneColor == intersection.StoneColor)
                {
                    group.Add(neighbor);

                    if (!IsSurroundedRecursive(neighbor, Direction.Right, group))
                    {
                        return false;
                    }
                }
            }
            if (intersection.Y > 0 && lastDirection != Direction.Up)
            {
                Intersection neighbor = GetNeighbor(intersection, Direction.Down);

                if (neighbor.StoneColor == StoneColor.None)
                {
                    return false;
                }

                if (neighbor.StoneColor == intersection.StoneColor)
                {
                    group.Add(neighbor);

                    if (!IsSurroundedRecursive(neighbor, Direction.Down, group))
                    {
                        return false;
                    }
                }
            }
            if (intersection.Y < 18 && lastDirection != Direction.Down)
            {
                Intersection neighbor = GetNeighbor(intersection, Direction.Up);

                if (neighbor.StoneColor == StoneColor.None)
                {
                    return false;
                }

                if (neighbor.StoneColor == intersection.StoneColor)
                {
                    group.Add(neighbor);

                    if (!IsSurroundedRecursive(neighbor, Direction.Up, group))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private Intersection GetNeighbor(Intersection intersection, Direction direction)
        {
            return direction switch
            {
                Direction.Up => Intersections.Single(e => e.X == intersection.X && e.Y == intersection.Y + 1),
                Direction.Right => Intersections.Single(e => e.X == intersection.X + 1 && e.Y == intersection.Y),
                Direction.Down => Intersections.Single(e => e.X == intersection.X && e.Y == intersection.Y - 1),
                Direction.Left => Intersections.Single(e => e.X == intersection.X - 1 && e.Y == intersection.Y)
            };
        }
    }
}
