using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class MouseInfo
{
    /// <summary>Trạng thái input chuột trong chu kỳ cập nhật trước đó.</summary>
    public MouseState PreviousState { get; private set; }

    /// <summary>Trạng thái input chuột trong chu kỳ cập nhật hiện tại.</summary>
    public MouseState CurrentState { get; private set; }

    /// <summary>Lấy hoặc thiết lập vị trí hiện tại của con trỏ chuột trong không gian màn hình (screen space).</summary>
    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    /// <summary>Lấy hoặc thiết lập tọa độ x hiện tại của con trỏ chuột trong không gian màn hình.</summary>
    public int X
    {
        get => CurrentState.X;
        set => SetPosition(value, CurrentState.Y);
    }

    /// <summary>Lấy hoặc thiết lập tọa độ y hiện tại của con trỏ chuột trong không gian màn hình.</summary>
    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(CurrentState.X, value);
    }

    /// <summary>Lấy sự khác biệt về vị trí con trỏ chuột giữa frame (khung hình) trước và frame hiện tại.</summary>
    public Point PositionDelta => CurrentState.Position - PreviousState.Position;

    /// <summary>Lấy sự khác biệt về vị trí x của con trỏ chuột giữa frame trước và frame hiện tại.</summary>
    public int XDelta => CurrentState.X - PreviousState.X;

    /// <summary>Lấy sự khác biệt về vị trí y của con trỏ chuột giữa frame trước và frame hiện tại.</summary>
    public int YDelta => CurrentState.Y - PreviousState.Y;

    /// <summary>Lấy giá trị tích lũy của con lăn chuột (mouse scroll wheel) kể từ khi bắt đầu trò chơi.</summary>
    public bool WasMoved => PositionDelta != Point.Zero;

    /// <summary>Lấy giá trị của con lăn chuột giữa frame trước và frame hiện tại.</summary>
    public int ScrollWheel => CurrentState.ScrollWheelValue;

    /// Gets the value of the scroll wheel between the previous and current frame.
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;


    /// <summary>Tạo một MouseInfo mới.</summary>
    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
    }

    /// <summary>Cập nhật thông tin trạng thái về input chuột.</summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    /// <summary>Trả về một giá trị cho biết liệu nút chuột được chỉ định có đang được nhấn giữ (down) hay không.</summary>
    /// <param name="button">Nút chuột cần kiểm tra.</param>
    /// <returns>true nếu nút chuột được chỉ định đang được nhấn giữ; ngược lại là false.</returns>
    public bool IsButtonDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return CurrentState.MiddleButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    /// <summary>Trả về một giá trị cho biết liệu nút chuột được chỉ định có đang được thả ra (up) hay không.</summary>
    /// <param name="button">Nút chuột cần kiểm tra.</param>
    /// <returns>true nếu nút chuột được chỉ định đang được thả ra; ngược lại là false.</returns>
    public bool IsButtonUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Released;
            case MouseButton.Middle:
                return CurrentState.MiddleButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Released;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    /// <summary>Trả về một giá trị cho biết liệu nút chuột được chỉ định có vừa mới được nhấn (just pressed) trong frame hiện tại hay không.</summary>
    /// <param name="button">Nút chuột cần kiểm tra.</param>
    /// <returns>true nếu nút chuột được chỉ định vừa mới được nhấn trong frame hiện tại; ngược lại là false.</returns>
    public bool WasButtonJustPressed(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
            case MouseButton.Middle:
                return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    /// <summary>Trả về một giá trị cho biết liệu nút chuột được chỉ định có vừa mới được thả (just released) trong frame hiện tại hay không.</summary>
    /// <param name="button">Nút chuột cần kiểm tra.</param>
    /// <returns>true nếu nút chuột được chỉ định vừa mới được thả trong frame hiện tại; ngược lại là false.</returns>
    public bool WasButtonJustReleased(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
            case MouseButton.Right:
                return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return CurrentState.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return CurrentState.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    /// <summary>Thiết lập vị trí hiện tại của con trỏ chuột trong không gian màn hình và cập nhật CurrentState với vị trí mới.</summary>
    /// <param name="x">Tọa độ x của vị trí con trỏ chuột trong không gian màn hình.</param>
    /// <param name="y">Tọa độ y của vị trí con trỏ chuột trong không gian màn hình.</param>
    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x,
            y,
            CurrentState.ScrollWheelValue,
            CurrentState.LeftButton,
            CurrentState.MiddleButton,
            CurrentState.RightButton,
            CurrentState.XButton1,
            CurrentState.XButton2
        );
    }

}
