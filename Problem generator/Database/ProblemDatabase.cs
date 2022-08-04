using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace GoProblemGenerator.Database
{
    public class ProblemDatabase
    {
        private readonly SqliteConnection _dbConnection;

        public ProblemDatabase()
        {
            _dbConnection = new SqliteConnection("Data Source=problems.db");
            _dbConnection.Open();
        }

        public void WriteGameToDatabase(GoProblemGenerator.Game game)
        {
            using SqliteTransaction transaction = _dbConnection.BeginTransaction();

            string sql = $"INSERT INTO Game (id, fileName) VALUES ('{game.Id}', '{game.FileName}')";

            new SqliteCommand(sql, _dbConnection, transaction)
                .ExecuteNonQuery();

            foreach (Move move in game.Moves)
            {
                sql = $"INSERT INTO GameMove (move, gameId, moveNumber) VALUES (" +
                    $"'{move.StringNotation}', " +
                    $"'{game.Id}', " +
                    $"{move.Number})";

                new SqliteCommand(sql, _dbConnection, transaction)
                    .ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public void WriteProblemAnswersToDatabase(List<KataGoMove> moves)
        {
            using SqliteTransaction transaction = _dbConnection.BeginTransaction();

            foreach (KataGoMove move in moves)
            {
                string sql = "INSERT INTO KataGoMove (gameId, moveNumber, move, winrate, scoreLead) VALUES (" +
                    $"'{move.GameId}', " +
                    $"{move.MoveNumber}, " +
                    $"'{move.Move}', " +
                    $"{move.Winrate}, " +
                    $"{move.ScoreLead})";

                new SqliteCommand(sql, _dbConnection, transaction)
                    .ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public void Empty()
        {
            using SqliteTransaction transaction = _dbConnection.BeginTransaction();

            string sql = "DELETE FROM Game; DELETE FROM GameMove; DELETE FROM KataGoMove;";

            new SqliteCommand(sql, _dbConnection, transaction)
                .ExecuteNonQuery();

            transaction.Commit();
        }
    }
}
