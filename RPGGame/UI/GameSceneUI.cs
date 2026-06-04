using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace RPGGame.UI
{
    internal class GameSceneUI : ContainerRuntime
    {
        // The string format to use when updating the text for the score display.
        private static readonly string s_coinFormat = "{0:D6}";

        // The sound effect to play for auditory feedback of the user interface.
        //private SoundEffect _uiSoundEffect;

        // The pause panel
        private Panel _pausePanel;

        // The resume button on the pause panel. Field is used to track reference so
        // focus can be set when the pause panel is shown.
        private AnimatedButton _resumeButton;

        private Panel _noticePanel;
        private Panel _victoryPanel;

        // The game over panel.
        private Panel _gameOverPanel;

        private SpeechBubble _speechBubble;

        // The retry button on the game over panel. Field is used to track reference
        // so focus can be set when the game over panel is shown.
        private AnimatedButton _retryButton;

        private ContainerRuntime _suggestion;
        private TextRuntime _suggestText;

        private ContainerRuntime _playerInfo;
        private List<SpriteRuntime> _heartInstances;
        private TextureRegion _heartRegion;
        private TextureRegion _brokenRegion;
        private SpriteRuntime _coinInstances;
        private int _maxHealth;
        private int _currentHealth;
        //public int MaxHealth;
        private TextRuntime _coinText;
        private TextRuntime _itemText;
        private TextRuntime _scoreText;

        /// <summary>
        /// Event invoked when the Resume button on the Pause panel is clicked.
        /// </summary>
        public event EventHandler ResumeButtonClick;

        /// <summary>
        /// Event invoked when the Quit button on either the Pause panel or the
        /// Game Over panel is clicked.
        /// </summary>
        public event EventHandler QuitButtonClick;

        /// <summary>
        /// Event invoked when the Replay button on the Game Over panel is clicked.
        /// </summary>
        public event EventHandler ReplayButtonClick;

        /// <summary>
        /// Event invoked when the Retry button on the Game Over panel is clicked.
        /// </summary>
        public event EventHandler RetryButtonClick;

        public GameSceneUI(int maxHealth)
        {
            // The game scene UI inherits from ContainerRuntime, so we set its
            // doc to fill so it fills the entire screen.
            Dock(Gum.Wireframe.Dock.Fill);

            // Add it to the root element.
            this.AddToRoot();

            // Get a reference to the content manager that was registered with the
            // GumService when it was original initialized.
            ContentManager content = GumService.Default.ContentLoader.XnaContentManager;

            // Use that content manager to load the sound effect and atlas for the
            // user interface elements
            //_uiSoundEffect = content.Load<SoundEffect>("audio/ui");
            TextureAtlas atlas = TextureAtlas.FromFile(content, "images/atlas-definition.xml");

            _maxHealth = maxHealth;
            _playerInfo = CreatePlayerInfo(atlas);            
            AddChild(_playerInfo);
            // Create the text that will display the players score and add it as
            // a child to this container.
            //_scoreText = CreateScoreText();
            //AddChild(_scoreText);

            _suggestion = CreateSuggestion(atlas);
            AddChild(_suggestion);

            // Create the Pause panel that is displayed when the game is paused and
            // add it as a child to this container
            _pausePanel = CreatePausePanel(atlas);
            AddChild(_pausePanel.Visual);

            _noticePanel = CreateNoticePanel(atlas);
            AddChild(_noticePanel.Visual);

            _victoryPanel = CreateVictoryPanel(atlas);
            AddChild(_victoryPanel.Visual);

            // Create the Game Over panel that is displayed when a game over occurs
            // and add it as a child to this container
            _gameOverPanel = CreateGameOverPanel(atlas);
            AddChild(_gameOverPanel.Visual);

            _speechBubble = new SpeechBubble(atlas);
            AddChild(_speechBubble);
        }
        private ContainerRuntime CreatePlayerInfo(TextureAtlas atlas)
        {
            ContainerRuntime container = new ContainerRuntime();
            container.Name = "HeartBarContainer";
            container.Anchor(Gum.Wireframe.Anchor.TopLeft);
            container.X = 10f;
            container.Y = 10f;
            //container.WidthUnits = DimensionUnitType.RelativeToChildren;
            //container.HeightUnits = DimensionUnitType.RelativeToChildren;

            _heartInstances = new List<SpriteRuntime>();
            _coinInstances = new SpriteRuntime();

            _heartRegion = atlas.GetRegion("heart");
            _brokenRegion = atlas.GetRegion("broken-heart");
            TextureRegion coinRegion = atlas.GetRegion("coin");
            TextureRegion flaskRegion = atlas.GetRegion("flask");

            for (int i = 0; i < _maxHealth; i++)
            {
                SpriteRuntime heart = new SpriteRuntime
                {
                    Texture = atlas.Texture,
                    TextureAddress = TextureAddress.Custom,
                    TextureHeight = _heartRegion.Height,
                    TextureLeft = _heartRegion.SourceRectangle.Left,
                    TextureTop = _heartRegion.SourceRectangle.Top,
                    TextureWidth = _heartRegion.Width,
                    Height = 50f,
                    Width = 50f,
                    Color = Color.Red,
                    Y = 0,
                };
                heart.X = i * heart.Width;
                //heart.Dock(Gum.Wireframe.Dock.Top);
                container.AddChild(heart);
                _heartInstances.Add(heart);
            }

            // Clamp giá trị để không vượt quá max hoặc nhỏ hơn 0
            

            _coinInstances = new SpriteRuntime
            {
                Texture = atlas.Texture,
                TextureAddress = TextureAddress.Custom,
                TextureHeight = coinRegion.Height,
                TextureLeft = coinRegion.SourceRectangle.Left,
                TextureTop = coinRegion.SourceRectangle.Top,
                TextureWidth = coinRegion.Width,
                Height = 50f,
                Width = 50f,
                Color = Color.Gold,
                Y = 50f
            };
            //_coinInstances.Dock(Gum.Wireframe.Dock.Bottom);
            container.AddChild(_coinInstances);

            _coinText = new TextRuntime
            {
                X = 50f,
                Y = 55f,
                UseCustomFont = true,
                CustomFontFile = @"fonts/04b_30.fnt",
                Text = string.Format(s_coinFormat, 0),
            };
            //_coinText.Anchor(Gum.Wireframe.Anchor.Top);
            container.AddChild(_coinText);

            SpriteRuntime flask = new SpriteRuntime
            {
                Texture = atlas.Texture,
                TextureAddress = TextureAddress.Custom,
                TextureHeight = flaskRegion.Height,
                TextureLeft = flaskRegion.SourceRectangle.Left,
                TextureTop = flaskRegion.SourceRectangle.Top,
                TextureWidth = flaskRegion.Width,
                Height = 50f,
                Width = 50f,
                Color = Color.OrangeRed,
                Y = 100f
            };
            container.AddChild(flask);

            _itemText = new TextRuntime
            {
                X = 100f,
                Y = 100f,
                UseCustomFont = true,
                CustomFontFile = @"fonts/04b_30.fnt",
                Text = "0",
            };
            //_coinText.Anchor(Gum.Wireframe.Anchor.Top);
            container.AddChild(_itemText);

            return container;
        }

        private ContainerRuntime CreateSuggestion(TextureAtlas atlas)
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
                Width = 260f,
                Height = 260f,
            };
            //button.Dock(Gum.Wireframe.Dock.Left);
            container.AddChild(button);

            _suggestText = new TextRuntime
            {
                UseCustomFont = true,
                CustomFontFile = @"fonts/04b_30.fnt",
                Text = "Lorem",
                FontScale = 2f,
                X = 40f,
            };
            container.AddChild(_suggestText);

            return container;
        }

        private Panel CreatePausePanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 800f;
            panel.Visual.Height = 280f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.Color = Color.SaddleBrown;
            panel.AddChild(background);

            TextRuntime text = new TextRuntime();
            text.Text = "PAUSED";
            text.UseCustomFont = true;
            text.CustomFontFile = "fonts/04b_30.fnt";
            text.FontScale = 2f;
            text.Anchor(Gum.Wireframe.Anchor.Top);
            text.Y = 40.0f;
            panel.AddChild(text);

            _resumeButton = new AnimatedButton(atlas);
            _resumeButton.Text = "RESUME";
            _resumeButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            _resumeButton.Visual.X = 36.0f;
            _resumeButton.Visual.Y = -36.0f;
            _resumeButton.Width = 150f;
            _resumeButton.Click += OnResumeButtonClicked;
            _resumeButton.GotFocus += OnElementGotFocus;
            
            panel.AddChild(_resumeButton);

            AnimatedButton quitButton = new AnimatedButton(atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            quitButton.Visual.X = -36.0f;
            quitButton.Visual.Y = -36.0f;

            quitButton.Click += OnQuitButtonClicked;
            quitButton.GotFocus += OnElementGotFocus;
            
            panel.AddChild(quitButton);

            return panel;
        }

        private Panel CreateNoticePanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 800f;
            panel.Visual.Height = 280f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.Color = Color.SaddleBrown;
            panel.AddChild(background);

            TextRuntime title = new TextRuntime
            {
                Text = "NOTICE",
                UseCustomFont = true,
                CustomFontFile = "fonts/04b_30.fnt",
                FontScale = 2f,
            };
            title.Anchor(Gum.Wireframe.Anchor.Top);
            title.Y = 40f;
            panel.AddChild(title);

            TextRuntime text = new TextRuntime
            {
                Text = "You must defeat all enemies",
                UseCustomFont = true,
                CustomFontFile = "fonts/04b_30.fnt",
                FontScale = 1f,
            };
            text.Anchor(Gum.Wireframe.Anchor.Center);
            panel.AddChild(text);

            _resumeButton = new AnimatedButton(atlas);
            _resumeButton.Text = "OK";
            _resumeButton.Anchor(Gum.Wireframe.Anchor.Bottom);
            _resumeButton.Y = -36f;
            _resumeButton.Click += OnResumeButtonClicked;
            _resumeButton.GotFocus += OnElementGotFocus;

            panel.AddChild(_resumeButton);

            return panel;
        }

        private Panel CreateVictoryPanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 800f;
            panel.Visual.Height = 280f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.Color = Color.SaddleBrown;
            panel.AddChild(background);

            TextRuntime title = new TextRuntime
            {
                Text = "VICTORY",
                UseCustomFont = true,
                WidthUnits = DimensionUnitType.RelativeToChildren,
                CustomFontFile = "fonts/04b_30.fnt",
                FontScale = 2f,
            };
            title.Anchor(Gum.Wireframe.Anchor.Top);
            title.Y = 40f;
            panel.AddChild(title);

            _scoreText = new TextRuntime
            {
                Text = "Your score: ",
                UseCustomFont = true,
                CustomFontFile = "fonts/04b_30.fnt",
                FontScale = 1f,
            };
            _scoreText.Anchor(Gum.Wireframe.Anchor.Center);
            panel.AddChild(_scoreText);

            _retryButton = new AnimatedButton(atlas);
            _retryButton.Text = "REPLAY";
            _retryButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            _retryButton.Visual.X = 36.0f;
            _retryButton.Visual.Y = -36.0f;
            _retryButton.Width = 150f;
            _retryButton.Click += OnReplayButtonClicked;
            _retryButton.GotFocus += OnElementGotFocus;

            panel.AddChild(_retryButton);

            AnimatedButton quitButton = new AnimatedButton(atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            quitButton.Visual.X = -36.0f;
            quitButton.Visual.Y = -36.0f;

            quitButton.Click += OnQuitButtonClicked;
            quitButton.GotFocus += OnElementGotFocus;

            panel.AddChild(quitButton);

            return panel;
        }
        
        private Panel CreateGameOverPanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 800f;
            panel.Visual.Height = 280f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.Color = Color.SaddleBrown;
            panel.AddChild(background);

            TextRuntime text = new TextRuntime
            {
                Text = "GAME OVER",
                WidthUnits = DimensionUnitType.RelativeToChildren,
                UseCustomFont = true,
                CustomFontFile = "fonts/04b_30.fnt",
                FontScale = 2f,
            };
            text.Anchor(Gum.Wireframe.Anchor.Top);
            text.Y = 40.0f;
            panel.AddChild(text);

            _retryButton = new AnimatedButton(atlas);
            _retryButton.Text = "RETRY";
            _retryButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            _retryButton.Visual.X = 36.0f;
            _retryButton.Visual.Y = -36.0f;
            _retryButton.Width = 125f;
            _retryButton.Click += OnRetryButtonClicked;
            _retryButton.GotFocus += OnElementGotFocus;

            panel.AddChild(_retryButton);

            AnimatedButton quitButton = new AnimatedButton(atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            quitButton.Visual.X = -36.0f;
            quitButton.Visual.Y = -36.0f;

            quitButton.Click += OnQuitButtonClicked;
            quitButton.GotFocus += OnElementGotFocus;

            panel.AddChild(quitButton);

            return panel;
        }
        
        private void OnResumeButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback.
            //Core.Audio.PlaySoundEffect(_uiSoundEffect);

            // Since the resume button was clicked, we need to hide panel.
            HidePausePanel();
            HideNoticePanel();

            // Invoke the ResumeButtonClick event
            if (ResumeButtonClick != null)
            {
                ResumeButtonClick(sender, args);
            }
        }

        private void OnReplayButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback.
            //Core.Audio.PlaySoundEffect(_uiSoundEffect);

            // Since the replay button was clicked, we need to hide panel.
            HideVictoryPanel();
            HideGameOverPanel();

            // Invoke the ReplayButtonClick event.
            if (ReplayButtonClick != null)
            {
                ReplayButtonClick(sender, args);
            }
        }

        private void OnRetryButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback.
            //Core.Audio.PlaySoundEffect(_uiSoundEffect);

            // Since the retry button was clicked, we need to hide panel.
            HideVictoryPanel();
            HideGameOverPanel();

            // Invoke the RetryButtonClick event.
            if (RetryButtonClick != null)
            {
                RetryButtonClick(sender, args);
            }
        }

        private void OnQuitButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback.
            //Core.Audio.PlaySoundEffect(_uiSoundEffect);

            // Both panels have a quit button, so hide all panels
            HidePausePanel();
            HideVictoryPanel();
            HideGameOverPanel();

            // Invoke the QuitButtonClick event.
            if (QuitButtonClick != null)
            {
                QuitButtonClick(sender, args);
            }
        }

        private void OnElementGotFocus(object sender, EventArgs args)
        {
            // A ui element that can receive focus has received focus, play the
            // ui sound effect for auditory feedback.
            //Core.Audio.PlaySoundEffect(_uiSoundEffect);
        }

        private void UpdateHeartVisuals()
        {
            // Clamp giá trị
            if (_currentHealth < 0) _currentHealth = 0;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;

            for (int i = 0; i < _heartInstances.Count; i++)
            {
                SpriteRuntime heart = _heartInstances[i];

                // Nếu index < máu hiện tại -> Vẽ tim đầy
                if (i < _currentHealth)
                {
                    heart.TextureLeft = _heartRegion.SourceRectangle.Left;
                    heart.TextureTop = _heartRegion.SourceRectangle.Top;
                    heart.TextureWidth = _heartRegion.Width;
                    heart.TextureHeight = _heartRegion.Height;
                    heart.Color = Color.Red; // Trả lại màu đỏ khi hồi máu
                }
                else // Vẽ tim vỡ
                {
                    heart.TextureLeft = _brokenRegion.SourceRectangle.Left;
                    heart.TextureTop = _brokenRegion.SourceRectangle.Top;
                    heart.TextureWidth = _brokenRegion.Width;
                    heart.TextureHeight = _brokenRegion.Height;
                    heart.Color = Color.White; // Màu trắng cho tim vỡ
                }
            }
        }

        /// <summary>
        /// Cập nhật hiển thị thanh máu dựa trên máu hiện tại của người chơi.
        /// </summary>
        public void UpdateHealth(int currentHealth)
        {
            if (_currentHealth != currentHealth)
            {
                _currentHealth = currentHealth;
                UpdateHeartVisuals();
            }
        }

        /// <summary>
        /// Updates the text on the coin display.
        /// </summary>
        /// <param name="coin">The coin to display.</param>
        public void UpdateCoinText(int coin)
        {
            _coinText.Text = string.Format(s_coinFormat, coin);
        }

        public void UpdateItemText(int item)
        {
            _itemText.Text = item.ToString();
        }

        public void ShowSuggetion(Vector2 pos, string content)
        {
            if (!_speechBubble.Visible)
            {
                _suggestText.Text = content;
                _suggestion.X = pos.X;
                _suggestion.Y = pos.Y - _suggestion.Width/2;
                _suggestion.Visible = true;
            }
        }

        public void HideSuggetion()
        {
            _suggestion.Visible = false;
        }

        /// <summary>
        /// Tells the game scene ui to show the pause panel.
        /// </summary>
        public void ShowPausePanel()
        {
            _pausePanel.IsVisible = true;

            // Give the resume button focus for keyboard/gamepad input.
            _resumeButton.IsFocused = true;

            // Ensure the other panel isn't visible.
            _noticePanel.IsVisible = false;
            _victoryPanel.IsVisible = false;
            _gameOverPanel.IsVisible = false;
        }

        /// <summary>
        /// Tells the game scene ui to hide the pause panel.
        /// </summary>
        public void HidePausePanel()
        {
            _pausePanel.IsVisible = false;
            _resumeButton.IsFocused = false;
            
        }

        public void ShowNoticePanel()
        {
            _noticePanel.IsVisible = true;
            _resumeButton.IsFocused = true;

            _pausePanel.IsVisible = false;
            _victoryPanel.IsVisible = false;
            _gameOverPanel.IsVisible = false;
        }

        public void HideNoticePanel()
        {
            _noticePanel.IsVisible = false;
            _resumeButton.IsFocused = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowVictoryPanel()
        {
            _victoryPanel.IsVisible = true;
            _scoreText.Text = "Your score: " + _coinText.Text;
            _retryButton.IsFocused = true;

            _pausePanel.IsVisible = false;
            _noticePanel.IsVisible = false;
            _gameOverPanel.IsVisible = false;
        }

        public void HideVictoryPanel()
        {
            _victoryPanel.IsVisible = false;
            _retryButton.IsFocused = false;
        }

        /// <summary>
        /// Tells the game scene ui to show the game over panel.
        /// </summary>
        public void ShowGameOverPanel()
        {
            _gameOverPanel.IsVisible = true;

            // Give the retry button focus for keyboard/gamepad input.
            _retryButton.IsFocused = true;

            // Ensure the other panel isn't visible.
            _pausePanel.IsVisible = false;
            _noticePanel.IsVisible = false;
            _victoryPanel.IsVisible = false;
        }

        /// <summary>
        /// Tells the game scene ui to hide the game over panel.
        /// </summary>
        public void HideGameOverPanel()
        {
            _gameOverPanel.IsVisible = false;
            _retryButton.IsFocused = false;
        }

        public void ShowSpeechBubble(Vector2 pos)
        {
            _speechBubble.Show(pos.X, pos.Y - 100f, "Hello Player!\n" +
                "To control your character,\n" +
                "use the A and D keys to\n" +
                "move left and right. Press\n" +
                "the Left Mouse Button to\n" +
                "perform an attack and the\n" +
                "Spacebar to jump. When\n" +
                "you want to use an item or\n" +
                "interact with an NPC,\n" +
                "simply press the F key.");
        }
        public void HideSpeechBubble()
        {
            _speechBubble.Hide();
        }
        /// <summary>
        /// Updates the game scene ui.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
        public void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
        }

        /// <summary>
        /// Draws the game scene ui.
        /// </summary>
        public void Draw()
        {
            GumService.Default.Draw();
        }

    }
}
