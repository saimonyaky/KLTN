using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class GamePadInfo
{
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

    /// <summary>Lấy chỉ mục (index) của người chơi mà tay cầm này dành cho.</summary>
    public PlayerIndex PlayerIndex { get; }

    /// <summary>Lấy trạng thái input cho tay cầm này trong chu kỳ cập nhật trước đó.</summary>
    public GamePadState PreviousState { get; private set; }

    /// <summary>Lấy trạng thái input cho tay cầm này trong chu kỳ cập nhật hiện tại.</summary>
    public GamePadState CurrentState { get; private set; }

    /// <summary>Lấy một giá trị cho biết liệu tay cầm này có đang được kết nối (connected) hay không.</summary>
    public bool IsConnected => CurrentState.IsConnected;

    /// <summary>Lấy giá trị của cần analog bên trái (left thumbstick) của tay cầm này.</summary>
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

    /// <summary>Lấy giá trị của cần analog bên phải (right thumbstick) của tay cầm này.</summary>
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    /// <summary>Lấy giá trị của cò (trigger) bên trái (left trigger) của tay cầm này.</summary>
    public float LeftTrigger => CurrentState.Triggers.Left;

    /// <summary>Lấy giá trị của cò (trigger) bên phải (right trigger) của tay cầm này.</summary>
    public float RightTrigger => CurrentState.Triggers.Right;

    /// <summary>Tạo một GamePadInfo mới cho tay cầm được kết nối tại chỉ mục người chơi được chỉ định.</summary>
    /// <param name="playerIndex">Chỉ mục của người chơi dành cho tay cầm này.</param>
    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(playerIndex);
    }

    /// <summary>Cập nhật thông tin trạng thái cho input của tay cầm này.</summary>
    /// <param name="gameTime">Thời gian game hiện tại, được sử dụng để theo dõi rung (vibration).</param>
    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

            if (_vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
            }
        }
    }

    /// <summary>Trả về một giá trị cho biết liệu nút tay cầm được chỉ định có đang được nhấn giữ (down) hay không.</summary>
    /// <param name="button">Nút tay cầm cần kiểm tra.</param>
    /// <returns>true nếu nút tay cầm được chỉ định đang được nhấn giữ; ngược lại là false.</returns>
    public bool IsButtonDown(Buttons button)
    {
        return CurrentState.IsButtonDown(button);
    }

    /// <summary>Trả về một giá trị cho biết liệu nút tay cầm được chỉ định có đang được thả ra (up) hay không.</summary>
    /// <param name="button">Nút tay cầm cần kiểm tra.</param>
    /// <returns>true nếu nút tay cầm được chỉ định đang được thả ra; ngược lại là false.</returns>
    public bool IsButtonUp(Buttons button)
    {
        return CurrentState.IsButtonUp(button);
    }

    /// <summary>Trả về một giá trị cho biết liệu nút tay cầm được chỉ định có vừa mới được nhấn (just pressed) trong frame hiện tại hay không.</summary>
    /// <param name="button">Nút tay cầm cần kiểm tra.</param>
    /// <returns>true nếu nút tay cầm được chỉ định vừa mới được nhấn trong frame hiện tại; ngược lại là false.</returns>
    public bool WasButtonJustPressed(Buttons button)
    {
        return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    /// <summary>Trả về một giá trị cho biết liệu nút tay cầm được chỉ định có vừa mới được thả (just released) trong frame hiện tại hay không.</summary>
    /// <param name="button">Nút tay cầm cần kiểm tra.</param>
    /// <returns>true nếu nút tay cầm được chỉ định vừa mới được thả trong frame hiện tại; ngược lại là false.</returns>
    public bool WasButtonJustReleased(Buttons button)
    {
        return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }

    /// <summary>Thiết lập độ rung (vibration) cho tất cả các mô tơ (motor) của tay cầm này.</summary>
    /// <param name="strength">Độ mạnh của độ rung từ 0.0f (không rung) đến 1.0f (rung mạnh nhất).</param>
    /// <param name="time">Khoảng thời gian độ rung nên xảy ra.</param>
    public void SetVibration(float strength, TimeSpan time)
    {
        _vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    /// <summary>Dừng độ rung của tất cả các mô tơ cho tay cầm này.</summary>
    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }

}
