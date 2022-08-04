using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Go
{
    public class ContentLibrary
    {
        public Texture2D Board { get; set; }

        public Texture2D BlackStone { get; set; }

        public Texture2D WhiteStone { get; set; }

        public Texture2D BlackStoneMarked { get; set; }

        public Texture2D WhiteStoneMarked { get; set; }

        public Texture2D KataGoMove { get; set; }

        public Texture2D KataGoBestMove { get; set; }

        public SpriteFont DefaultFont { get; set; }

        public ContentLibrary(ContentManager contentManager)
        {
            Board = contentManager.Load<Texture2D>("textures/board");
            BlackStone = contentManager.Load<Texture2D>("textures/black_stone");
            WhiteStone = contentManager.Load<Texture2D>("textures/white_stone");
            BlackStoneMarked = contentManager.Load<Texture2D>("textures/black_stone_marked");
            WhiteStoneMarked = contentManager.Load<Texture2D>("textures/white_stone_marked");
            KataGoMove = contentManager.Load<Texture2D>("textures/katago_move");
            KataGoBestMove = contentManager.Load<Texture2D>("textures/katago_best_move");

            DefaultFont = contentManager.Load<SpriteFont>("default_font");
        }
    }
}
