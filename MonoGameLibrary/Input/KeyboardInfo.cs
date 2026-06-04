using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class KeyboardInfo 
{
    /// <summary>Lấy trạng thái input bàn phím trong chu kỳ cập nhật trước đó.</summary>
    public KeyboardState PreviousState { get; private set; }

    /// <summary>Lấy trạng thái input bàn phím trong chu kỳ input hiện tại.</summary>
    public KeyboardState CurrentState { get; private set; }

    /// <summary>Tạo một KeyboardInfo mới.</summary>
    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    /// <summary>Cập nhật thông tin trạng thái về input bàn phím.</summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    /// <summary>Trả về một giá trị cho biết liệu phím được chỉ định có đang được nhấn giữ (down) hay không.</summary>
    /// <param name="key">Phím cần kiểm tra.</param>
    /// <returns>true nếu phím được chỉ định đang được nhấn giữ; ngược lại là false.</returns>
    public bool IsKeyDown(Keys key)
    {
        return CurrentState.IsKeyDown(key);
    }

    /// <summary>Trả về một giá trị cho biết liệu phím được chỉ định có đang được thả ra (up) hay không.</summary>
    /// <param name="key">Phím cần kiểm tra.</param>
    /// <returns>true nếu phím được chỉ định đang được thả ra; ngược lại là false.</returns>
    public bool IsKeyUp(Keys key)
    {
        return CurrentState.IsKeyUp(key);
    }

    /// Returns a value that indicates if the specified key was just pressed on the current frame.
    /// <param name="key">Phím cần kiểm tra.</param>
    /// <returns>true nếu phím được chỉ định vừa mới được nhấn trong frame hiện tại; ngược lại là false.</returns>
    public bool WasKeyJustPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }

    /// Returns a value that indicates if the specified key was just released on the current frame.
    /// <param name="key">Phím cần kiểm tra.</param>
    /// <returns>true nếu phím được chỉ định vừa mới được thả trong frame hiện tại; ngược lại là false.</returns>
    public bool WasKeyJustReleased(Keys key)
    {
        return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
    }

}
