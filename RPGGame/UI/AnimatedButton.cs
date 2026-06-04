using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace RPGGame.UI;

/// A custom button implementation that inherits from Gum's Button class to provide
/// animated visual feedback when focused.
internal class AnimatedButton : Button
{
    /// Creates a new AnimatedButton instance using graphics from the specified texture atlas.
    /// <param name="atlas">The texture atlas containing button graphics</param>
    public AnimatedButton(TextureAtlas atlas)
    {
        // Each Forms conrol has a general Visual property that has properties shared by all control types.
        // This Visual type matches the Forms type.
        // It can be casted to access controls-specific properties.
        ButtonVisual buttonVisual = (ButtonVisual)Visual;
        buttonVisual.Height = 64f;
        buttonVisual.HeightUnits = DimensionUnitType.Absolute;
        buttonVisual.Width = 96f;
        buttonVisual.WidthUnits = DimensionUnitType.RelativeToChildren;

        // Get a reference to the nine-slice background to display the button graphics
        // A nine-slice allows the button to stretch while preserving corner appearance
        NineSliceRuntime background = buttonVisual.Background;
        background.Texture = atlas.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.Color = Color.Transparent;
        // texture coordinates for the background are set down below

        TextRuntime textInstance = buttonVisual.TextInstance;
        textInstance.Text = "START";
        textInstance.Color = Color.White;
        textInstance.UseCustomFont = true;
        textInstance.CustomFontFile = "fonts/Aoboshi_One.fnt";
        textInstance.FontScale = 1f;
        textInstance.Anchor(Gum.Wireframe.Anchor.Center);
        textInstance.Width = 0;

        // Get the frame for the focused button state from the atlas
        TextureRegion focusedTextureRegion = atlas.GetRegion("focused-button");

        // Reset all state to default so we don't have unexpected variable assignments:
        buttonVisual.ButtonCategory.ResetAllStates();

        // Get the enabled (default/unfocused) state
        StateSave enabledState = buttonVisual.States.Enabled;
        enabledState.Apply = () =>
        {
            background.Color = Color.Transparent;
            textInstance.Color = Color.White;
        };

        // Create the focused state
        StateSave focusedState = buttonVisual.States.Focused;
        focusedState.Apply = () =>
        {
            background.TextureHeight = focusedTextureRegion.Height;
            background.TextureLeft = focusedTextureRegion.SourceRectangle.Left;
            background.TextureTop = focusedTextureRegion.SourceRectangle.Top;
            background.TextureWidth = focusedTextureRegion.Width;
            background.Color = Color.White;
            textInstance.Color = Color.Black;
        };

        // Create the highlighted+focused state (for mouse hover while focused)
        StateSave highlightedFocused = buttonVisual.States.HighlightedFocused;
        highlightedFocused.Apply = focusedState.Apply;

        // Create the highlighted state (for mouse hover)
        // by cloning the enabled state since they appear the same
        StateSave highlighted = buttonVisual.States.Highlighted;
        highlighted.Apply = enabledState.Apply;

        // Add event handlers for keyboard input.
        KeyDown += HandleKeyDown;

        // Add event handler for mouse hover focus.
        buttonVisual.RollOn += HandleRollOn;
    }

    /// Handles keyboard input for navigation between buttons using left/right keys.
    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Keys.Left)
        {
            // Left arrow navigates to previous control
            HandleTab(TabDirection.Up, loop: true);
        }
        if (e.Key == Keys.Right)
        {
            // Right arrow navigates to next control
            HandleTab(TabDirection.Down, loop: true);
        }
    }

    /// Automatically focuses the button when the mouse hovers over it.
    private void HandleRollOn(object sender, EventArgs e)
    {
        IsFocused = true;
    }
}
