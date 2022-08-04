using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Go
{
    public class ProblemDatabase
    {
        private readonly SqliteConnection _dbConnection;

        public List<Problem> ProblemPool { get; private set; }

        public ProblemDatabase()
        {
            _dbConnection = new SqliteConnection("Data Source=problems.db");
            _dbConnection.Open();

            ProblemPool = new List<Problem>();
        }

        public void SetProblems()
        {
            string sql = "SELECT DISTINCT gameId, moveNumber FROM KataGoMove";

            var cmd = new SqliteCommand(sql, _dbConnection);
            SqliteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ProblemPool.Add(new Problem
                {
                    GameId = (string)reader["gameId"],
                    MoveNumber = (int)(Int64)reader["moveNumber"],
                    TriedByUser = false
                });
            }
        }

        public Problem GetRandomProblemToTry()
        {
            List<Problem> problemPool = ProblemPool
                .Where(e => !e.TriedByUser)
                .ToList();

            Problem problem = problemPool[new Random().Next(0, problemPool.Count - 1)];

            ProblemPool.Single(e => e.GameId == problem.GameId && e.MoveNumber == problem.MoveNumber).TriedByUser = true;

            SetProblemData(problem);

            return problem;
        }

        public void SetProblemData(Problem problem)
        {
            string sql = $"SELECT move, winrate, scoreLead FROM KataGoMove WHERE gameId = '{problem.GameId}' AND moveNumber = {problem.MoveNumber}";

            var cmd = new SqliteCommand(sql, _dbConnection);
            SqliteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                problem.KataGoMoves.Add(new KataGoMove
                {
                    Move = (string)reader["move"],
                    Winrate = (double)reader["winrate"],
                    ScoreLead = (double)reader["scoreLead"]
                });
            }

            sql = $"SELECT move, moveNumber FROM GameMove WHERE gameId = '{problem.GameId}'";

            cmd = new SqliteCommand(sql, _dbConnection);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                problem.GameMoves.Add(new GameMove
                {
                    Move = (string)reader["move"],
                    MoveNumber = (int)(Int64)reader["moveNumber"]
                });
            }
        }
    }
}
