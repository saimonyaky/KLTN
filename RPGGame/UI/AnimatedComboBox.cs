using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Microsoft.Xna.Framework;

using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
using System;

namespace RPGGame.UI
{
    internal class AnimatedComboBox : ComboBox
    {
        // Reference to the text label that displays the slider's title
        private TextRuntime _textInstance;

        private TextRuntime _selectedText;

        /// Gets or sets the text label for this slider.
        public string Title
        {
            get => _textInstance.Text;
            set => _textInstance.Text = value;
        }
        public AnimatedComboBox(TextureAtlas atlas)
        {
            // Create the top-level container for all visual elements
            ContainerRuntime topLevelContainer = new ContainerRuntime();
            topLevelContainer.Height = 60f;
            topLevelContainer.Width = 640f;

            ComboBoxVisual comboBoxVisual = (ComboBoxVisual)Visual;

            TextureRegion backgroundRegion = atlas.GetRegion("button");

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
                FontScale = 1f,
                Text = "Replace Me",
                WidthUnits = DimensionUnitType.RelativeToChildren,
            };
            _textInstance.Anchor(Gum.Wireframe.Anchor.Left);
            _textInstance.X = 20f;
            topLevelContainer.AddChild(_textInstance);

            ContainerRuntime itemsContainer = new ContainerRuntime
            {
                Name = "ItemsContainer",
                X = 360,
                Height = 60f,
                //Visible = false,
            };
            topLevelContainer.AddChild(itemsContainer);

            _selectedText = new TextRuntime
            {
                Name = "TextInstance",
                Text = "(none)",
                Color = Color.White,
                UseCustomFont = true,
                CustomFontFile = "fonts/Aoboshi_One.fnt",
                FontScale = 1f
            };
            _selectedText.Anchor(Gum.Wireframe.Anchor.Center);
            itemsContainer.AddChild(_selectedText);

            ListBoxVisual listBoxVisual = comboBoxVisual.ListBoxInstance;
            listBoxVisual.Height = 120f;
            listBoxVisual.HeightUnits = DimensionUnitType.Absolute;
            listBoxVisual.Width = 360f;
            listBoxVisual.WidthUnits = DimensionUnitType.Absolute;

            TextureRegion listBoxBackgroundRegion = atlas.GetRegion("panel-background");

            // Get a reference to the nine-slice background to display the button graphics
            // A nine-slice allows the button to stretch while preserving corner appearance
            NineSliceRuntime listBoxBackground = listBoxVisual.Background;
            listBoxBackground.Texture = atlas.Texture;
            listBoxBackground.TextureAddress = TextureAddress.Custom;
            listBoxBackground.TextureHeight = listBoxBackgroundRegion.Height;
            listBoxBackground.TextureLeft = listBoxBackgroundRegion.SourceRectangle.Left;
            listBoxBackground.TextureTop = listBoxBackgroundRegion.SourceRectangle.Top;
            listBoxBackground.TextureWidth = listBoxBackgroundRegion.Width;
            listBoxBackground.Color = Color.White;
            listBoxBackground.Dock(Gum.Wireframe.Dock.Fill);

            listBoxVisual.Name = "ListBoxInstance";
            listBoxVisual.Anchor(Gum.Wireframe.Anchor.Right);
            listBoxVisual.Y = 90;
            itemsContainer.AddChild(listBoxVisual);

            // Define colors for focused and unfocused states
            Color focusedColor = Color.White;
            Color unfocusedColor = Color.Gray;

            StateSaveCategory comboBoxCategory = new StateSaveCategory();
            comboBoxCategory.Name = "ComboBoxCategory";
            topLevelContainer.AddCategory(comboBoxCategory);

            // Create the enabled (default/unfocused) state
            StateSave enabled = new StateSave();
            enabled.Name = FrameworkElement.EnabledStateName;
            enabled.Apply = () =>
            {
                background.Color = Color.Transparent;
                _textInstance.Color = unfocusedColor;
                _selectedText.Color = unfocusedColor;
            };
            comboBoxCategory.States.Add(enabled);

            // Create the focused state
            StateSave focused = new StateSave();
            focused.Name = FrameworkElement.FocusedStateName;
            focused.Apply = () =>
            {
                background.Color = focusedColor;
                _textInstance.Color = focusedColor;
                _selectedText.Color = focusedColor;
            };
            comboBoxCategory.States.Add(focused);

            // Create the highlighted, focused state by cloning the focused state
            StateSave highlightedFocused = focused.Clone();
            highlightedFocused.Name = FrameworkElement.HighlightedFocusedStateName;
            comboBoxCategory.States.Add(highlightedFocused);

            StateSave highlighted = enabled.Clone();
            highlighted.Name = FrameworkElement.HighlightedFocusedStateName;
            comboBoxCategory.States.Add(highlighted);

            Visual = topLevelContainer;

            Visual.RollOn += HandleRollOn;
            //SelectionChanged += HandleSelectionChanged;
        }

        private void HandleRollOn(object sender, EventArgs e)
        {
            IsFocused = true;
        }

        /// Adds items to the dropdown
        public void AddItems(params string[] items)
        {
            foreach (var item in items)
            {
                Items.Add(new AnimatedListBoxItem(item));
            }
        }

        private void HandleSelectionChanged(object sender, EventArgs e)
        {
            _selectedText.Text = SelectedObject?.ToString() ?? "(none)";
            // Khi chọn item thì tự thu gọn lại
            Visual.ApplyState("Collapsed");
        }
    }
}
