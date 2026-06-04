using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class InputManager 
{
    /// <summary>Lấy thông tin trạng thái input của bàn phím (keyboard).</summary>
    public KeyboardInfo Keyboard { get; private set; }

    /// <summary>Lấy thông tin trạng thái input của chuột (mouse).</summary>
    public MouseInfo Mouse { get; private set; }

    /// <summary>Lấy thông tin trạng thái của các tay cầm chơi game (gamepad).</summary>
    public GamePadInfo[] GamePads { get; private set; }

    /// <summary>Tạo một InputManager mới.</summary>
    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[4];
        for (int i = 0; i < 4; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }

    /// <summary>Cập nhật thông tin trạng thái cho input của bàn phím, chuột và tay cầm.</summary>
    /// <param name="gameTime">Một bản chụp nhanh (snapshot) các giá trị thời gian cho frame (khung hình) hiện tại.</param>
    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        for (int i = 0; i < 4; i++)
        {
            GamePads[i].Update(gameTime);
        }
    }

}