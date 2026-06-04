using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;

/// <summary>Quản lý State Machine và logic cập nhật animation.</summary>
public class Animator<T> where T : Enum
{
    private T _curState;
    private Dictionary<T, Animation> _animMap = new Dictionary<T, Animation>();
    private Queue<T> _stateQueue;
    //Stores the current animation being played.
    private Animation _animation;
    //Tracks which frame of the animation is currently being displayed.
    private int _currentFrame;
    //Keeps track of how much time has passed since the last frame change.
    private TimeSpan _elapsed;

    // Thuộc tính để lớp Sprite bên ngoài có thể truy cập và cập nhật Region
    public TextureRegion CurrentRegion { get; private set; }
    public T CurrentState => _curState;
    public int CurrentFrame => _currentFrame;
    public bool AnimationFinished { get; private set; } = false;

    /// Creates a new animater.
    public Animator() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="initialState"></param>
    public Animator(T initialState)
    {
        _curState = initialState;
        _stateQueue = new Queue<T>();
    }

    /// <summary>Adds an animation to the animation map.</summary>
    /// <param name="state">The state (Enum) this animation corresponds to.</param>
    /// <param name="animation">The animation data.</param>
    public void AddAnimation(T state, Animation animation)
    {
        if (!_animMap.ContainsKey(state))
        {
            _animMap.Add(state, animation);
        }
    }
    /// <summary>Set the current animation state and resets the frame.</summary>
    /// <param name="newState"></param>
    public void SetState(T newState)
    {
        if(_curState.Equals(newState) || !_animMap.ContainsKey(newState))
        { return; }
        if (_animation != null && _animation.ForgeFinish && _currentFrame < _animation.FrameCount - 1)
        {
            _stateQueue.Enqueue(newState);
        }
        else
        {
            _curState = newState;
            SetCurrentAnimation();
        }
        if (_animation.ForgeFinish && _currentFrame == _animation.FrameCount - 1 && _stateQueue.Count > 0)
        {
            T nextState = _stateQueue.Dequeue();
            _curState = nextState;
        }
    }

    private void SetCurrentAnimation()
    {
        if (_animMap.TryGetValue(_curState, out Animation newAnim) && newAnim != null && newAnim.FrameCount > 0)
        {
            _animation = newAnim;
            _currentFrame = 0;
            _elapsed = TimeSpan.Zero;
            AnimationFinished = false;
            CurrentRegion = _animation.GetFrame(_currentFrame);
        }
        else
        {
            _animation = null;
            CurrentRegion = null;
        }
    }

    public void Update(GameTime gameTime)
    {
        // Đảm bảo animation ban đầu được thiết lập
        if (_animation == null && _animMap.Count > 0)
        {
            SetCurrentAnimation();
        }

        if (_animation == null || _animation.FrameCount <= 1)
        {
            return;
        }

        // Cộng dồn thời gian đã trôi qua
        _elapsed += gameTime.ElapsedGameTime;

        // Kiểm tra xem đã đến lúc chuyển frame chưa
        if (_elapsed >= _animation.Delay)
        {
            // Trừ đi thời gian trễ, giữ lại phần thừa (để tính toán chính xác hơn nếu FPS thấp)
            _elapsed -= _animation.Delay;
            _currentFrame++;

            // Xử lý logic lặp (looping)
            if (_currentFrame >= _animation.FrameCount)
            {
                //AnimationFinished = true;
                _currentFrame = 0;
            }
            if (_currentFrame >= _animation.FrameCount - 1)
            {
                AnimationFinished = true;
            }

            // Cập nhật Region hiện tại
            CurrentRegion = _animation.Frames[_currentFrame];
        }
    }

    /// <summary>
    /// Thử lấy trạng thái tiếp theo từ hàng đợi (Queue).
    /// </summary>
    /// <param name="nextState">Trạng thái được lấy ra.</param>
    /// <returns>True nếu có trạng thái trong hàng đợi.</returns>
    public bool TryGetNextQueuedState(out T nextState)
    {
        if (_stateQueue.Count > 0)
        {
            nextState = _stateQueue.Dequeue();
            return true;
        }
        nextState = default;
        return false;
    }
}