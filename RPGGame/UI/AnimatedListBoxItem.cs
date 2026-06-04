using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
using System;

namespace RPGGame.UI
{
    /// A custom dropdown selection control derived from Gum's ComboBox.
    public class AnimatedListBoxItem : ListBoxItem
    {
        public AnimatedListBoxItem(string text)
        {
            ListBoxItemVisual listBoxItemVisual = (ListBoxItemVisual)Visual;
            TextRuntime textInstance = listBoxItemVisual.TextInstance;
            textInstance.Text = text;
            textInstance.CustomFontFile = @"fonts/Aoboshi_One.fnt";
            textInstance.UseCustomFont = true;
            textInstance.FontScale = 1f;
            textInstance.Anchor(Gum.Wireframe.Anchor.Center);
            AddChild(textInstance);
        }
    }
}
