using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace RPGGame.UI
{
    internal class ContextualUI : ContainerRuntime
    {
        //private ContainerRuntime _suggestion;
        private TextRuntime _suggestText;

        public ContextualUI(TextureAtlas atlas)
        {
            ContainerRuntime container = new ContainerRuntime();
            container.WidthUnits = DimensionUnitType.Absolute;
            container.HeightUnits = DimensionUnitType.Absolute;

            TextureRegion buttonRegion = atlas.GetRegion("e-key");

            SpriteRuntime button = new SpriteRuntime
            {
                Texture = atlas.Texture,
                TextureAddress = TextureAddress.Custom,
                TextureHeight = buttonRegion.Height,
                TextureLeft = buttonRegion.SourceRectangle.Left,
                TextureTop = buttonRegion.SourceRectangle.Top,
                TextureWidth = buttonRegion.Width,
                Width = 26f,
                Height = 26f,
            };
            //button.Dock(Gum.Wireframe.Dock.Left);
            container.AddChild(button);

            _suggestText = new TextRuntime
            {
                UseCustomFont = true,
                CustomFontFile = @"fonts/04b_30.fnt",
                Text = "Lorem",
                FontScale = 2f,
                X = button.Width,
            };
            container.AddChild(_suggestText);
        }

        public void Show(Vector2 pos, string content)
        {
                _suggestText.Text = content;
                this.X = pos.X;
                this.Y = pos.Y;
                this.Visible = true;
        }

        public void Hide()
        {
            this.Visible = false;
        }
    }
}
