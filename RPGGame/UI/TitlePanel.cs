using Gum.DataTypes;
using Gum.Managers;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace RPGGame.UI
{
    internal class TitlePanel : ContainerRuntime
    {
        // Reference to the text label that displays the title
        private TextRuntime _titleInstance;

        /// Gets or sets the text label for title.
        public string Title
        {
            get => _titleInstance.Text;
            set => _titleInstance.Text = value;
        }

        /// Creates a new TitlePanel instance using graphics from the specified texture atlas.
        /// <param name="atlas">The texture atlas containing title graphics.</param>
        public TitlePanel(TextureAtlas atlas)
        {
            TextureRegion dividerRegion = atlas.GetRegion("divider-fade");

            // Create the title text element
            _titleInstance = new TextRuntime();
            _titleInstance.Anchor(Gum.Wireframe.Anchor.Center);
            _titleInstance.CustomFontFile = @"fonts/Aoboshi_One.fnt";
            _titleInstance.UseCustomFont = true;
            _titleInstance.FontScale = 2f;
            _titleInstance.Text = "Replace Me";
            //_titleInstance.Height = 55f;
            _titleInstance.Y = 20f;
            _titleInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
            AddChild(_titleInstance);

            // Create the divider of the title (left)
            NineSliceRuntime leftDivider = new NineSliceRuntime();
            //leftDivider.Dock(Gum.Wireframe.Dock.Left);
            leftDivider.Texture = atlas.Texture;
            leftDivider.TextureAddress = TextureAddress.Custom;
            leftDivider.TextureHeight = dividerRegion.Height;
            leftDivider.TextureLeft = dividerRegion.SourceRectangle.Left;
            leftDivider.TextureTop = dividerRegion.SourceRectangle.Top;
            leftDivider.TextureWidth = dividerRegion.Width;
            leftDivider.Width = 100f;
            leftDivider.Height = 10f;
            leftDivider.Y = 20f + _titleInstance.Height / 2 - leftDivider.Height / 2f;
            leftDivider.X = -(_titleInstance.Width / 2f) - leftDivider.Width / 2f - 10f;
            leftDivider.WidthUnits = DimensionUnitType.Absolute;
            AddChild(leftDivider);

            // Create the divider of the title (right)
            NineSliceRuntime rightDivider = new NineSliceRuntime();
            rightDivider.Texture = atlas.Texture;
            rightDivider.TextureAddress = TextureAddress.Custom;
            rightDivider.TextureLeft = dividerRegion.SourceRectangle.Left + dividerRegion.Width;
            rightDivider.TextureTop = dividerRegion.SourceRectangle.Top;
            rightDivider.TextureWidth = -dividerRegion.Width;
            rightDivider.TextureHeight = dividerRegion.Height;
            rightDivider.X = 300;
            rightDivider.Width = 100f;
            rightDivider.Height = 10f;
            rightDivider.WidthUnits = DimensionUnitType.Absolute;
            //rightDivider.Dock(Gum.Wireframe.Dock.Right);
            AddChild(rightDivider);
        }
    }
}
