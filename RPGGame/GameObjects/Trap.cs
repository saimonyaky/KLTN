using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;

namespace RPGGame.GameObjects
{
    internal class Trap : GameObject
    {
        public Trap(TextureAtlas atlas)
        {
            Sprite.Scale = new Vector2(1.5f, 1.5f);
            Sprite.Region = atlas.GetRegion("trap");
            Sprite.CenterOrigin();
        }
        public override void HandleStateLogic(GameTime gameTime)
        {

        }

        public override Rectangle Collider()
        {
            Rectangle bounds = new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.4f),
                (int)(WorldPos.Y - Sprite.Height * 0.3f),
                (int)(Sprite.Width * 0.8),
                (int)(Sprite.Height * 0.8)
            );

            return bounds;
        }
    }
}
