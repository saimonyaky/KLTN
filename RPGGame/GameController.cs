using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Input;

namespace RPGGame;

/// <summary>
/// Cung cấp một lớp trừu tượng (abstraction) input dành riêng cho game,
/// ánh xạ các input vật lý (physical inputs) tới các hành động trong game (game actions),
/// làm cầu nối giữa hệ thống input của chúng ta với các chức năng cụ thể của trò chơi.
/// </summary>
public static class GameController
{
    private static MouseInfo s_mouse => Core.Input.Mouse;
    private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
    private static GamePadInfo s_gamePad => Core.Input.GamePads[(int)PlayerIndex.One];

    /// <summary>Trả về true nếu người chơi đã kích hoạt hành động "di chuyển lên" (move up).</summary>
    public static bool MoveUp()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Up) ||
               s_keyboard.WasKeyJustPressed(Keys.W) ||
               s_keyboard.WasKeyJustPressed(Keys.Space) ||
               s_gamePad.IsButtonDown(Buttons.DPadUp) ||
               s_gamePad.IsButtonDown(Buttons.LeftThumbstickUp);
    }

    /// <summary>Trả về true nếu người chơi đã kích hoạt hành động "di chuyển xuống" (move down).</summary>
    public static bool MoveDown()
    {
        return s_keyboard.IsKeyDown(Keys.Down) ||
               s_keyboard.IsKeyDown(Keys.S) ||
               s_gamePad.IsButtonDown(Buttons.DPadDown) ||
               s_gamePad.IsButtonDown(Buttons.LeftThumbstickDown);
    }

    /// <summary>Trả về true nếu người chơi đã kích hoạt hành động "di chuyển sang trái" (move left).</summary>
    public static bool MoveLeft()
    {
        return s_keyboard.IsKeyDown(Keys.Left) ||
               s_keyboard.IsKeyDown(Keys.A) ||
               s_gamePad.IsButtonDown(Buttons.DPadLeft) ||
               s_gamePad.IsButtonDown(Buttons.LeftThumbstickLeft);
    }

    /// <summary>Trả về true nếu người chơi đã kích hoạt hành động "di chuyển sang phải" (move right).</summary>
    public static bool MoveRight()
    {
        return s_keyboard.IsKeyDown(Keys.Right) ||
               s_keyboard.IsKeyDown(Keys.D) ||
               s_gamePad.IsButtonDown(Buttons.DPadRight) ||
               s_gamePad.IsButtonDown(Buttons.LeftThumbstickRight);
    }

    /// <summary>Trả về true nếu người chơi đã kích hoạt hành động "tạm dừng" (pause).</summary>
    public static bool Back()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Escape) ||
               s_gamePad.WasButtonJustPressed(Buttons.Start);
    }

    /// <summary>
    /// Trả về true nếu người chơi đã kích hoạt nút "hành động" (action),
    /// thường được sử dụng để xác nhận (confirmation) trong menu.
    /// </summary>
    public static bool Action()
    {
        return s_keyboard.WasKeyJustPressed(Keys.E) ||
               s_gamePad.WasButtonJustPressed(Buttons.A);
    }

    public static bool Attack()
    {
        return s_keyboard.WasKeyJustPressed(Keys.J) ||
               s_gamePad.WasButtonJustPressed(Buttons.X) ||
               s_mouse.WasButtonJustReleased(MouseButton.Left);
    }

    public static bool Dash()
    {
        return s_keyboard.WasKeyJustPressed(Keys.LeftShift) ||
               s_keyboard.WasKeyJustPressed(Keys.L) ||
               s_gamePad.WasButtonJustPressed(Buttons.A);
    }

    public static bool Use()
    {
        return s_keyboard.WasKeyJustPressed(Keys.F) ||
            s_gamePad.WasButtonJustPressed(Buttons.B);
    }
}
