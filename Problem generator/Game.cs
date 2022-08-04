using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoProblemGenerator
{
    public class Game
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string KomiString { get; set; }

        public List<Move> Moves { get; set; }

        public int PositionsAnalyzed { get; set; }

        public int PositionsToAnalyze { get; set; }

        public Game(string fileName)
        {
            FileName = fileName.Split('\\').Last();

            Id = Guid.NewGuid().ToString();

            Moves = new List<Move>();

            ParseSgf(fileName);

            int positionsToAnalyze = (Moves.Count - 1) / 5 - 1;
            PositionsToAnalyze = positionsToAnalyze > 19 ? 19 : positionsToAnalyze;
        }

        public void ParseSgf(string fileName)
        {
            string allText = File.ReadAllText(fileName).Replace("\n", string.Empty).Replace("\r", string.Empty);

            Match komiMatch = Regex.Match(allText, "KM\\[.*?\\]");

            if (!komiMatch.Success)
            {
                Console.WriteLine($"SGF parse error for file: {fileName}");
            }

            KomiString = komiMatch.Value.Split('[')[1].Replace("]", string.Empty);

            MatchCollection matches = Regex.Matches(allText, ";.\\[.*?\\]");

            int i = 0;

            foreach (Match moveMatch in matches)
            {
                string coordinates = moveMatch.Value.Split('[')[1].Replace("]", string.Empty);

                if (moveMatch.Value.Contains("[]")) // Pass
                {
                    continue;
                }

                var move = new Move
                {
                    IsBlack = moveMatch.Value.Contains(";B["),
                    X = coordinates[0] - 97,
                    Y = 18 - (coordinates[1] - 97),
                    Number = i
                };

                char xChar = move.X <= 7 ? (char)(move.X + 65) : (char)(move.X + 66);
                move.StringNotation = $"{xChar}{move.Y + 1}";

                Moves.Add(move);

                i++;
            }
        }

        public string ToKataGoQuery()
        {
            string query = "{";

            query += $"\"id\": \"{Id}\",";
            query += "\"initialStones\": [],";
            query += "\"moves\": [";

            foreach (Move move in Moves)
            {
                string color = move.IsBlack ? "B" : "W";
                query += $"[\"{color}\", \"{move.StringNotation}\"],";
            }

            query = query.Substring(0, query.Length - 1);
            query += "],\"rules\": \"tromp-taylor\",";
            query += $"\"komi\": {KomiString},";
            query += "\"boardXSize\": 19, \"boardYSize\": 19, \"analyzeTurns\": [";

            for (int i = 10; i <= 100; i += 5)
            {
                if (i >= Moves.Count)
                {
                    break;
                }

                query += $"{i},";
            }

            query = query.Substring(0, query.Length - 1);
            query += "]}";

            return query;
        }
    }
}
