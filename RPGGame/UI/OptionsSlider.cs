using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace RPGGame.UI;

/// A custom slider control that inherits from Gum's Slider class.
public class OptionsSlider : Slider
{
    // Reference to the text label that displays the slider's title
    private TextRuntime _textInstance;

    // Reference to the rectangle that visually represents the current value
    private ColoredRectangleRuntime _fillRectangle;

    /// Gets or sets the text label for this slider.
    public string Text
    {
        get => _textInstance.Text;
        set => _textInstance.Text = value;
    }

    /// Creates a new OptionsSlider instance using graphics from the specified texture atlas.
    /// <param name="atlas">The texture atlas containing slider graphics.</param>
    public OptionsSlider(TextureAtlas atlas)
    {
        // Create the top-level container for all visual elements
        ContainerRuntime topLevelContainer = new ContainerRuntime();
        topLevelContainer.Height = 240f;
        topLevelContainer.Width = 1056f;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        // Create the background panel that contains everything
        NineSliceRuntime background = new NineSliceRuntime();
        background.Texture = atlas.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureWidth = backgroundRegion.Width;
        background.Dock(Gum.Wireframe.Dock.Fill);
        topLevelContainer.AddChild(background);

        // Create the text element
        _textInstance = new TextRuntime
        {
            CustomFontFile = @"fonts/Aoboshi_One.fnt",
            UseCustomFont = true,
            FontScale = 2f,
            Text = "Replace Me",
            X = 40,
            Y = 40,
            WidthUnits = DimensionUnitType.RelativeToChildren,
        };
        topLevelContainer.AddChild(_textInstance);

        // Create the container for the slider track and decorative elements
        ContainerRuntime innerContainer = new ContainerRuntime();
        innerContainer.Height = 52f;
        innerContainer.Width = 964f;
        innerContainer.X = 40f;
        innerContainer.Y = 132f;
        topLevelContainer.AddChild(innerContainer);

        // Add "MIN" text to the left end
        TextRuntime minText = new TextRuntime();
        minText.Color = Color.White;
        minText.CustomFontFile = @"fonts/Aoboshi_One.fnt";
        minText.FontScale = 1f;
        minText.UseCustomFont = true;
        minText.Text = "MIN";
        minText.Dock(Gum.Wireframe.Dock.Left);
        innerContainer.AddChild(minText);

        TextureRegion middleBackgroundRegion = atlas.GetRegion("slider-middle-background");

        // Create the middle track portion of the slider
        NineSliceRuntime middleBackground = new NineSliceRuntime();
        middleBackground.Dock(Gum.Wireframe.Dock.FillVertically);
        middleBackground.Texture = middleBackgroundRegion.Texture;
        middleBackground.TextureAddress = TextureAddress.Custom;
        middleBackground.TextureHeight = middleBackgroundRegion.Height;
        middleBackground.TextureLeft = middleBackgroundRegion.SourceRectangle.Left;
        middleBackground.TextureTop = middleBackgroundRegion.SourceRectangle.Top;
        middleBackground.TextureWidth = middleBackgroundRegion.Width;
        middleBackground.Width = 716f;
        middleBackground.WidthUnits = DimensionUnitType.Absolute;
        middleBackground.Dock(Gum.Wireframe.Dock.Left);
        middleBackground.X = 108f;
        innerContainer.AddChild(middleBackground);

        // Create the interactive track that responds to clicks
        // The special name "TrackInstance" is required for Slider functionality
        ContainerRuntime trackInstance = new ContainerRuntime();
        trackInstance.Name = "TrackInstance";
        trackInstance.Dock(Gum.Wireframe.Dock.Fill);
        trackInstance.Height = -2f;
        trackInstance.Width = -2f;
        middleBackground.AddChild(trackInstance);

        // Create the fill rectangle that visually displays the current value
        _fillRectangle = new ColoredRectangleRuntime();
        _fillRectangle.Dock(Gum.Wireframe.Dock.Left);
        _fillRectangle.Width = 90f; // Default to 90% - will be updated by value changes
        _fillRectangle.WidthUnits = DimensionUnitType.PercentageOfParent;
        trackInstance.AddChild(_fillRectangle);

        // Add "MAX" text to the right end
        TextRuntime maxText = new TextRuntime();
        maxText.Color = Color.White;
        maxText.CustomFontFile = @"fonts/Aoboshi_One.fnt";
        maxText.FontScale = 1f;
        maxText.UseCustomFont = true;
        maxText.Text = "MAX";
        maxText.Dock(Gum.Wireframe.Dock.Right);
        innerContainer.AddChild(maxText);

        // Define colors for focused and unfocused states
        Color focusedColor = Color.White;
        Color unfocusedColor = Color.Gray;

        // Create slider state category - Slider.SliderCategoryName is the required name
        StateSaveCategory sliderCategory = new StateSaveCategory();
        sliderCategory.Name = Slider.SliderCategoryName;
        topLevelContainer.AddCategory(sliderCategory);

        // Create the enabled (default/unfocused) state
        StateSave enabled = new StateSave();
        enabled.Name = FrameworkElement.EnabledStateName;
        enabled.Apply = () =>
        {
            // When enabled but not focused, use gray coloring for all elements
            background.Color = unfocusedColor;
            _textInstance.Color = unfocusedColor;
            _fillRectangle.Color = unfocusedColor;
            maxText.Color = unfocusedColor;
            minText.Color = unfocusedColor;
            middleBackground.Color = unfocusedColor;
        };
        sliderCategory.States.Add(enabled);

        // Create the focused state
        StateSave focused = new StateSave();
        focused.Name = FrameworkElement.FocusedStateName;
        focused.Apply = () =>
        {
            // When focused, use white coloring for all elements
            background.Color = focusedColor;
            _textInstance.Color = focusedColor;
            maxText.Color = focusedColor;
            minText.Color = focusedColor;
            middleBackground.Color = focusedColor;
            _fillRectangle.Color = focusedColor;
        };
        sliderCategory.States.Add(focused);

        // Create the highlighted+focused state by cloning the focused state
        StateSave highlightedFocused = focused.Clone();
        highlightedFocused.Name = FrameworkElement.HighlightedFocusedStateName;
        sliderCategory.States.Add(highlightedFocused);

        // Create the highlighted state by cloning the enabled state
        StateSave highlighted = enabled.Clone();
        highlighted.Name = FrameworkElement.HighlightedStateName;
        sliderCategory.States.Add(highlighted);

        // Assign the configured container as this slider's visual
        Visual = topLevelContainer;

        // Enable click-to-point functionality for the slider
        // This allows users to click anywhere on the track to jump to that value
        IsMoveToPointEnabled = true;

        // Add event handlers
        Visual.RollOn += HandleRollOn;
        ValueChanged += HandleValueChanged;
        ValueChangedByUi += HandleValueChangedByUi;
    }

    /// Automatically focuses the slider when the user interacts with it
    private void HandleValueChangedByUi(object sender, EventArgs e)
    {
        IsFocused = true;
    }

    /// Automatically focuses the slider when the mouse hovers over it
    private void HandleRollOn(object sender, EventArgs e)
    {
        IsFocused = true;
    }

    /// Updates the fill rectangle width to visually represent the current value
    private void HandleValueChanged(object sender, EventArgs e)
    {
        // Calculate the ratio of the current value within its range
        double ratio = (Value - Minimum) / (Maximum - Minimum);

        // Update the fill rectangle width as a percentage
        // _fillRectangle uses percentage width units, so we multiply by 100
        _fillRectangle.Width = 100 * (float)ratio;
    }
}
