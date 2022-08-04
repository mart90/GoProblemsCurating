using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Go
{
    public class UserInterface : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ContentLibrary _contentLibrary;

        private Problem _currentProblem;
        private ProblemDatabase _problemDatabase;
        private Board _board;

        private bool _leftMouseHeld;

        private bool _showSolution;

        private int _correctAnswers;
        private int _wrongAnswers;

        public UserInterface()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 1237;
            _graphics.PreferredBackBufferWidth = 1237;
            _graphics.ApplyChanges();

            _board = new Board(); 
            
            _problemDatabase = new ProblemDatabase();
            _problemDatabase.SetProblems();

            _correctAnswers = 0;
            _wrongAnswers = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _contentLibrary = new ContentLibrary(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentProblem == null)
            {
                _currentProblem = _problemDatabase.GetRandomProblemToTry();
                _board.PrepareProblem(_currentProblem);
            }

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released && _leftMouseHeld)
            {
                _leftMouseHeld = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && !_leftMouseHeld)
            {
                _leftMouseHeld = true;

                if (_showSolution)
                {
                    _currentProblem = _problemDatabase.GetRandomProblemToTry();
                    _board.PrepareProblem(_currentProblem);
                    _showSolution = false;
                }
                else
                {
                    Point location = mouseState.Position;
                    
                    if (_board.PointIsKataGoMove(location))
                    {
                        _correctAnswers++;
                    }
                    else
                    {
                        _wrongAnswers++;
                    }
                    
                    _showSolution = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            DrawBoard();

            _spriteBatch.DrawString(
                _contentLibrary.DefaultFont,
                _correctAnswers.ToString(),
                new Vector2(600, 5),
                Color.Green);

            _spriteBatch.DrawString(
                _contentLibrary.DefaultFont,
                _wrongAnswers.ToString(),
                new Vector2(637, 5),
                Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBoard()
        {
            _spriteBatch.Draw(_contentLibrary.Board, new Rectangle(0, 0, 1237, 1237), Color.White);

            foreach (Intersection intersection in _board.Intersections)
            {
                if (intersection.StoneColor == StoneColor.None && (!intersection.IsKataGoMove || !_showSolution))
                {
                    continue;
                }

                var invertedY = 18 - intersection.Y;
                var rectangle = new Rectangle(2 + 65 * intersection.X, 2 + 65 * invertedY, 63, 63);

                Texture2D texture;

                if (intersection.IsKataGoMove && _showSolution)
                {
                    KataGoMove katagoBestMove = _currentProblem.KataGoMoves.OrderByDescending(e => e.Winrate).First();

                    if (intersection.StringNotation == katagoBestMove.Move)
                    {
                        texture = _contentLibrary.KataGoBestMove;
                    }
                    else 
                    {
                        texture = _contentLibrary.KataGoMove;
                    }
                }
                else if (intersection.IsLastMove)
                {
                    texture = intersection.StoneColor == StoneColor.Black ? _contentLibrary.BlackStoneMarked : _contentLibrary.WhiteStoneMarked;
                }
                else
                {
                    texture = intersection.StoneColor == StoneColor.Black ? _contentLibrary.BlackStone : _contentLibrary.WhiteStone;
                }

                _spriteBatch.Draw(texture, rectangle, Color.White);

                if (intersection.IsKataGoMove && _showSolution)
                {
                    KataGoMove kataGoMove = _currentProblem.KataGoMoves.Single(e => e.Move == intersection.StringNotation);

                    string winrate = Math.Round(kataGoMove.Winrate * 100, 1).ToString();
                    int winrateXOffset = 32 - winrate.Length * 3;

                    _spriteBatch.DrawString(
                        _contentLibrary.DefaultFont,
                        winrate,
                        new Vector2(winrateXOffset + 65 * intersection.X, 12 + 65 * invertedY),
                        Color.Black);

                    string scoreLead = Math.Round(kataGoMove.ScoreLead, 1).ToString();
                    int scoreLeadXOffset = 32 - scoreLead.Length * 3;

                    _spriteBatch.DrawString(
                        _contentLibrary.DefaultFont,
                        scoreLead,
                        new Vector2(scoreLeadXOffset + 65 * intersection.X, 35 + 65 * invertedY),
                        Color.Black);
                }
            }
        }
    }
}
