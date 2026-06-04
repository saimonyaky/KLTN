using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RPGGame.Data;
using RPGGame.UI;
using System;

namespace RPGGame.Scenes;

public class TitleScene : Scene
{
    private const string WANDERING_TEXT = "Wandering";
    private const string KINGHT_TEXT = "Knight";

    //private SoundEffect _uiSoundEffect;
    private Panel _titleScreenButtonsPanel;
    private Panel _optionsPanel;

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _font5x;

    // The position to draw the title text at.
    private Vector2 _wanderingTextPos;
    private Vector2 _kinghtTextPos;

    // The origin to set for the title text.
    private Vector2 _wanderingTextOrigin;
    private Vector2 _kinghtTextOrigin;

    // The texture used for the background pattern.
    private Texture2D _backgroundPattern;

    // The destination rectangle for the background pattern to fill.
    private Rectangle _backgroundDestination;

    private TitlePanel _titlePanel;

    // The options button used to open the options menu.
    private AnimatedButton _optionsButton;

    // The quit button used to exit.
    private AnimatedButton _quitButton;

    // The back button used to exit the options menu back to the title menu.
    private AnimatedButton _optionsBackButton;

    // Reference to the texture atlas that we can pass to UI elements are created.
    private TextureAtlas _atlas;


    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // Can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Title text.
        Vector2 size = _font5x.MeasureString(WANDERING_TEXT);
        _wanderingTextPos = new Vector2(640, 100);
        _wanderingTextOrigin = size * 0.5f;

        size = _font5x.MeasureString(KINGHT_TEXT);
        _kinghtTextPos = new Vector2(757, 207);
        _kinghtTextOrigin = size * 0.5f;

        // Set the background pattern destination rectangle to fill the entire
        // screen background.
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("fonts/AoboshiOne-Regular");

        // Load the font for the title text.
        _font5x = Content.Load<SpriteFont>("fonts/AoboshiOne-Regular_5x");

        // Load the background pattern texture.
        _backgroundPattern = Content.Load<Texture2D>("images/background-title");

        // Load the sound effect to play when ui actions occur.
        //_uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

        // Load the texture atlas from the xml configuration file.
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        AnimatedButton startButton = new AnimatedButton(_atlas);
        startButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        startButton.Visual.Y = -196;
        startButton.Visual.Width = 256;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new AnimatedButton(_atlas);
        _optionsButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        _optionsButton.Visual.Y = -128;
        _optionsButton.Visual.Width = 256;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        _quitButton = new AnimatedButton(_atlas);
        _quitButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        _quitButton.Visual.Y = -64;
        _quitButton.Visual.Width = 256;
        _quitButton.Text = "Quit";
        _quitButton.Click += HandleExitClicked;
        _titleScreenButtonsPanel.AddChild(_quitButton);

        startButton.IsFocused = true;
    }

    private void HandleStartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        //Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Tạo dữ liệu mới tinh    
        PlayerData newData = PlayerData.CreateDefault();
        // Change to the game scene to start the game.
        Core.ChangeScene(new GameScene(1, newData));
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        //Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Set the title panel to be invisible.
        _titleScreenButtonsPanel.IsVisible = false;

        // Set the options panel to be visible.
        _optionsPanel.IsVisible = true;

        // Give the back button on the options panel focus.
        _optionsBackButton.IsFocused = true;
    }

    private void HandleExitClicked(object sender, EventArgs e)
    {
        Core.Instance.Exit();
    }

    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        TextRuntime optionsText = new TextRuntime();
        optionsText.Dock(Gum.Wireframe.Dock.Top);
        optionsText.Y = 20;
        optionsText.Text = "OPTIONS";
        optionsText.UseCustomFont = true;
        optionsText.FontScale = 2f;
        optionsText.CustomFontFile = @"fonts/Aoboshi_One.fnt";
        _optionsPanel.AddChild(optionsText);

        //DropdownSelection languageDropdown = new DropdownSelection(_atlas);
        //languageDropdown.Title = "Laguage";
        //languageDropdown.AddItems("English", "Tiếng Việt");
        //languageDropdown.Anchor(Gum.Wireframe.Anchor.Top);
        //languageDropdown.Visual.Y = 100f;
        //languageDropdown.SelectedIndex = 0;
        //_optionsPanel.AddChild(languageDropdown);

        //AnimatedComboBox difficultyComboBox = new AnimatedComboBox(_atlas);
        //difficultyComboBox.Title = "DIFFICULTY";
        //difficultyComboBox.AddItems("Easy", "Normal", "Hard");
        //difficultyComboBox.Anchor(Gum.Wireframe.Anchor.Top);
        //difficultyComboBox.Visual.Y = 100f;
        //difficultyComboBox.SelectedIndex = 1;
        //_optionsPanel.AddChild(difficultyComboBox);

        OptionsSlider musicSlider = new OptionsSlider(_atlas);
        musicSlider.Name = "MusicSlider";
        musicSlider.Text = "MUSIC";
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Visual.Y = 120f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        //var sfxSlider = new Slider();
        OptionsSlider sfxSlider = new OptionsSlider(_atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Visual.Y = 372;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new AnimatedButton(_atlas);
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -112f;
        _optionsBackButton.Y = -40f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the track.
        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // Play the UI Sound effect so the player can hear the difference in audio.
        //Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the track.
        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect
        //Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        //Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Set the title panel to be visible.
        _titleScreenButtonsPanel.IsVisible = true;

        // Set the options panel to be invisible.
        _optionsPanel.IsVisible = false;

        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _optionsButton.IsFocused = true;
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        CreateOptionsPanel();
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, Color.White * 0.5f);
        Core.SpriteBatch.End();

        if (_titleScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Title text on top of that at its original position.
            Core.SpriteBatch.DrawString(_font5x, WANDERING_TEXT, _wanderingTextPos, Color.SaddleBrown, 0.0f, _wanderingTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
            Core.SpriteBatch.DrawString(_font5x, KINGHT_TEXT, _kinghtTextPos, Color.White, 0.0f, _kinghtTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();
        }

        GumService.Default.Draw();
    }

}
