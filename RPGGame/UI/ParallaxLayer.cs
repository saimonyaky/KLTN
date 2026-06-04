using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPGGame.UI
{
    public class ParallaxLayer
    {
        public Texture2D Texture { get; set; }
        public Vector2 Speed { get; set; }

        public ParallaxLayer(Texture2D texture, Vector2 speed)
        {
            Texture = texture;
            Speed = speed;
        }
    }
}
