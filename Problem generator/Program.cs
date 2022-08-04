using GoProblemGenerator.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GoProblemGenerator
{
    class Program
    {
        static void Main()
        {
            var database = new ProblemDatabase();

            var gamesToAnalyze = new List<Game>();

            string[] fileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/sgfs");

            foreach (string fileName in fileNames)
            {
                var game = new Game(fileName);
                gamesToAnalyze.Add(game);
                database.WriteGameToDatabase(game);
            }

            var katago = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "katago/katago.exe",
                    Arguments = "analysis -config katago/analysis_config.cfg -model katago/katanetwork.gz",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            };

            katago.Start();

            while (true)
            {
                string line = katago.StandardError.ReadLine();

                if (line.Contains("Started, ready to begin handling requests"))
                {
                    break;
                }
            }

            foreach (Game game in gamesToAnalyze)
            {
                string query = game.ToKataGoQuery();
                katago.StandardInput.WriteLine(query);
            }

            while (true)
            {
                List<KataGoMove> problemAnswers = ConvertAnalysisFeedbackToProblemAnswers(GetAnalysisResponse(katago));
                database.WriteProblemAnswersToDatabase(problemAnswers);

                gamesToAnalyze.Single(e => e.Id == problemAnswers[0].GameId).PositionsAnalyzed++;

                List<Game> finishedGames = gamesToAnalyze
                    .Where(e => e.PositionsAnalyzed == e.PositionsToAnalyze)
                    .ToList();

                foreach (Game game in finishedGames)
                {
                    Console.WriteLine($"Finished analyzing game {game.FileName}");
                    File.Move($"{AppDomain.CurrentDomain.BaseDirectory}/sgfs/{game.FileName}", $"{AppDomain.CurrentDomain.BaseDirectory}/sgfs/analyzed/{game.FileName}");
                    gamesToAnalyze.Remove(game);
                }

                if (!gamesToAnalyze.Any())
                {
                    Console.WriteLine("Finished!");
                    break;
                }
            }
        }

        static List<KataGoMove> ConvertAnalysisFeedbackToProblemAnswers(KataGoAnalysisFeedback kataGoAnalysisFeedback)
        {
            List<KataGoMove> problemAnswers = new();

            List<MoveInfo> moves = kataGoAnalysisFeedback.MoveInfos.ToList();

            MoveInfo bestMove = moves[0];

            double bestMoveWinrate = bestMove.Winrate;

            double winrateLowerLimit = bestMoveWinrate - bestMoveWinrate * 0.1;

            moves.RemoveAll(e => e.Winrate < winrateLowerLimit);

            foreach (MoveInfo move in moves)
            {
                problemAnswers.Add(new KataGoMove
                {
                    GameId = kataGoAnalysisFeedback.Id,
                    Move = move.Move,
                    MoveNumber = kataGoAnalysisFeedback.TurnNumber,
                    Winrate = move.Winrate,
                    ScoreLead = move.ScoreLead
                });
            }

            return problemAnswers;
        }

        static KataGoAnalysisFeedback GetAnalysisResponse(Process process)
        {
            string responseString = "";

            char c = (char)process.StandardOutput.Read();

            while (c != '\n')
            {
                responseString += c.ToString();
                c = (char)process.StandardOutput.Read();
            }

            responseString += c.ToString();

            if (responseString.StartsWith("{\"error\""))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<KataGoAnalysisFeedback>(responseString);
        }
    }
}
