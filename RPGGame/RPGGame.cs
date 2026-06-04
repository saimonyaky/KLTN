using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using RPGGame.Scenes;
using System;

namespace RPGGame
{
    public class RPGGame : Core
    {

        public RPGGame() : base("RPGGame", 1280, 720, false)
        {
            
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize the Gum UI service
            InitializeGum();

            UpdateUIScale();

            // Start the game with the title scene.
            ChangeScene(new TitleScene());
        }

        protected override void LoadContent()
        {
            //DebugRenderer.Initialize(GraphicsDevice);
        }

        private void InitializeGum()
        {
            // Initialize the Gum service.
            // The second parameter specifies the version of the default visuals to use.
            // V2 is the latest version.
            GumService.Default.Initialize(this, DefaultVisualsVersion.V2);

            // Tell the Gum service which content manager to use.
            // We will tell it to use the global content manager from our Core.
            GumService.Default.ContentLoader.XnaContentManager = Core.Content;

            // Register keyboard input for UI control.
            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

            // Register gamepad input for Ui control.
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

            // Customize the tab reverse UI navigation to also trigger when the keyboard
            // Up arrow key is pushed.
            FrameworkElement.TabReverseKeyCombos.Add(
               new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

            // Customize the tab UI navigation to also trigger when the keyboard
            // Down arrow key is pushed.
            FrameworkElement.TabKeyCombos.Add(
               new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

            // The assets created for the UI were done so at 1/4th the size to keep the size of the
            // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
            // the game's resolution then tell gum to zoom in by a factor of 4.
            GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth ;
            GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight * 0.75f ;
            GumService.Default.Renderer.Camera.Zoom = 1.0f;
        }

        private void UpdateUIScale()
        {
            if (GumService.Default?.Renderer?.Camera == null)
                return;

            var screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Giả sử UI được thiết kế cho 1280x720 gốc
            float baseWidth = 1280f;
            float baseHeight = 720f;

            // Tính scale theo tỉ lệ nhỏ hơn giữa width & height
            float scaleX = screenWidth / baseWidth;
            float scaleY = screenHeight / baseHeight;
            float scale = MathF.Min(scaleX, scaleY);

            // Áp dụng scale cho camera Gum
            GumService.Default.Renderer.Camera.Zoom = scale;

            // Cập nhật lại canvas nếu cần
            GumService.Default.CanvasWidth = screenWidth / scale;
            GumService.Default.CanvasHeight = screenHeight / scale;
        }

    }
}
