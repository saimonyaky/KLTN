using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace RPGGame.GameObjects
{
    internal class Player : GameObject
    {
        public enum PlayerState
        {
            Idle,
            Run,
            Jump,
            Fall,
            Dash,
            Attack,
            Hurt,
            Die
        }
        private PlayerState _currentState;
        private bool _faceRight = true;
        private int _maxHealth = 5;
        private int _currentHealth;
        private float _speed = 200f;
        private Vector2 _velocity;
        private bool _onGround;
        private bool _hasDealtDamage = false;
        private float _iframeTimer = 0f;
        private const float JUMPPOW = 400f;
        private const float GRAVITY = 980f;
        private const float IFRAME_DURATION = 1f;
        private SoundEffect _hurtSound;

        public int ItemCount { get; set; }
        public bool GroundCheck;
        public int MaxHealth => _maxHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                // Bắn tín hiệu cho ai đang lắng nghe (Observer)
                //OnHealthChanged?.Invoke(_currentHealth);
            }
        }
        public int Coin { get; private set; }

        /// <summary> Đối tượng Animator quản lý trạng thái và khung hình. </summary>
        public Animator<PlayerState> Animator { get; private set; }

        public event EventHandler OnAction;

        // Truyền vào một Rectangle (vùng đánh)
        public event Action<Rectangle> OnAttackDealt;

        /// <summary>
        /// Creates a new Player using the specified animated sprite.
        /// </summary>
        /// <param name="sprite">The AnimatedSprite to use when drawing the player.</param>
        public Player(TextureAtlas atlas, SoundEffect hurtSound)
        {
            Sprite.Scale = new Vector2(2.0f, 2.0f);
            CurrentHealth = _maxHealth;
            _hurtSound = hurtSound;
            _velocity = Vector2.Zero;
            _currentState = PlayerState.Idle;
            _onGround = false;
            // Khởi tạo Animator
            Animator = new Animator<PlayerState>(_currentState);
            // Nạp Animation từ Atlas đã gộp
            try
            {
                Animator.AddAnimation(PlayerState.Idle, atlas.GetAnimation("idle"));
                Animator.AddAnimation(PlayerState.Run, atlas.GetAnimation("run"));
                Animator.AddAnimation(PlayerState.Jump, atlas.GetAnimation("jump"));
                Animator.AddAnimation(PlayerState.Fall, atlas.GetAnimation("fall"));
                Animator.AddAnimation(PlayerState.Dash, atlas.GetAnimation("dash"));
                Animator.AddAnimation(PlayerState.Attack, atlas.GetAnimation("attack"));
                Animator.AddAnimation(PlayerState.Hurt, atlas.GetAnimation("hurt"));
                // ... (Thêm các animation khác)

                // Đặt Origin về tâm của khung hình đầu tiên
                Sprite.Region = Animator.CurrentRegion; // Gán Region ban đầu
                Sprite.CenterOrigin();

            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Lỗi nạp Animation: {ex.Message}");
                // Xử lý lỗi nếu animation không tồn tại
            }
        }

        public override void Initialize(Vector2 startingPosition)
        {
            base.Initialize(startingPosition);
            _currentState = PlayerState.Idle;
        }

        // Phương thức bắt buộc phải ghi đè từ GameObject
        public override void HandleStateLogic(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_iframeTimer > 0) _iframeTimer -= deltaTime;

            if (!_onGround)
            {
                _velocity.Y += GRAVITY * deltaTime;
            }
            else
            {
                _velocity.Y = 0; // Reset vận tốc khi chạm đất
            }
            _velocity.X = 0; // Reset X

            ProcessInputAndState();

            if (_currentState == PlayerState.Run || _currentState == PlayerState.Jump || _currentState == PlayerState.Fall)
            {
                if (GameController.MoveLeft())
                {
                    _velocity.X = -_speed;
                    _faceRight = false;
                }
                else if (GameController.MoveRight())
                {
                    _velocity.X = _speed;
                    _faceRight = true;
                }
            }

            // Cập nhật vị trí (sử dụng protected set)
            WorldPos += _velocity * deltaTime;
            Animator.SetState(_currentState);

            // Cập nhật thời gian
            Animator.Update(gameTime);

            // Gán Region và SpriteEffects
            Sprite.Region = Animator.CurrentRegion;
            Sprite.Effects = _faceRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Căn Origin nếu kích thước frame thay đổi
            Sprite.CenterOrigin();

            HandleAction();
            HandleAttackDamage();
        }

        /// <summary>Chứa logic FSM (State Machine) thuần túy để xác định trạng thái hiện tại.</summary>
        private void ProcessInputAndState()
        {
            PlayerState playerState = _currentState;

            // Trạng thái hiện tại
            switch (_currentState)
            {
                case PlayerState.Idle:
                    if (GameController.Use() && _currentHealth < _maxHealth && ItemCount > 0)
                    {
                        ItemCount--;
                        _currentHealth++;
                    }
                    if (GameController.Dash())
                    {
                        playerState = PlayerState.Dash;
                    }
                    else if (GameController.MoveUp() && _onGround)
                    {
                        playerState = PlayerState.Jump;
                    }
                    else if (GameController.Attack() && _onGround)
                    {
                        playerState = PlayerState.Attack;
                    }
                    else if (GameController.MoveLeft() || GameController.MoveRight())
                    {
                        playerState = PlayerState.Run;
                    }
                    else if (!_onGround)
                    {
                        playerState = PlayerState.Fall;
                    }
                    break;

                case PlayerState.Run:
                    if (GameController.Dash())
                    {
                        playerState = PlayerState.Dash;
                    }
                    else if (GameController.MoveUp() && _onGround)
                    {
                        playerState = PlayerState.Jump;
                    }
                    else if (GameController.Attack())
                    {
                        playerState = PlayerState.Attack;
                    }
                    else if (!_onGround)
                    {
                        playerState = PlayerState.Fall;
                    }
                    else if (!GameController.MoveLeft() && !GameController.MoveRight())
                        playerState = PlayerState.Idle;
                    break;

                case PlayerState.Jump:
                    if (_onGround)
                    {
                        _velocity.Y = -JUMPPOW; // Áp dụng lực nhảy
                        _onGround = false;
                    }
                    else if (GameController.Dash())
                    {
                        _velocity.Y = 0;
                        playerState = PlayerState.Dash;
                    }

                    // Nếu vận tốc Y > 0 (bắt đầu rơi xuống)
                    if (_velocity.Y > 0)
                    {
                        playerState = PlayerState.Fall;
                    }
                    break;

                case PlayerState.Fall:
                    if (GameController.Dash())
                        playerState = PlayerState.Dash;
                    else if (_onGround)
                    {
                        // Chạm đất
                        playerState = (GameController.MoveLeft() || GameController.MoveRight()) ? PlayerState.Run : PlayerState.Idle;
                    }
                    break;

                case PlayerState.Dash:
                    _iframeTimer = 2f;
                    _velocity.X = 1.5f * (_faceRight ? _speed : -_speed);
                    if (Animator.AnimationFinished)
                        _iframeTimer = 0f;
                    if (GameController.MoveUp() && _onGround)
                        playerState = PlayerState.Jump;
                    else if (GameController.Attack() && _onGround)
                        playerState = PlayerState.Attack;
                    else if (!_onGround && Animator.AnimationFinished)
                        playerState = PlayerState.Fall;
                    else if (_onGround && Animator.AnimationFinished)
                    {
                        //_velocity.X = 0;
                        playerState = (GameController.MoveLeft() || GameController.MoveRight()) ? PlayerState.Run : PlayerState.Idle;
                    }
                    break;

                case PlayerState.Attack:
                    if (GameController.Dash())
                        playerState = PlayerState.Dash;
                    else if (GameController.MoveLeft() && !Animator.AnimationFinished)
                    {
                        _faceRight = false;
                    }
                    else if (GameController.MoveRight() && !Animator.AnimationFinished)
                    {
                        _faceRight = true;
                    }
                    else if (GameController.MoveUp() && _onGround)
                    {
                        playerState = PlayerState.Jump;
                    }
                    else if (Animator.AnimationFinished)
                    {
                        playerState = PlayerState.Idle;
                    }
                    break;
                case PlayerState.Hurt:
                    _velocity.X = _faceRight ? -50f : 50f;
                    if (Animator.AnimationFinished && _currentHealth>0)
                        playerState = PlayerState.Idle;
                    if (Animator.AnimationFinished && _currentHealth <= 0)
                        playerState = PlayerState.Die;
                    break;
                case PlayerState.Die:

                    //// Vẫn giữ trọng lực để nhân vật rơi xuống đất nếu chết trên không
                    //if (!_onGround) _velocity.Y += GRAVITY * deltaTime;
                    //else _velocity.Y = 0;
                    if (Animator.AnimationFinished)
                        _velocity = Vector2.Zero;

                    // Không nhận bất kỳ Input nào từ người chơi/AI tại đây
                    break;
                default:
                    // Trạng thái Attack/Hurt/Die thường tự chuyển sang trạng thái Idle/Run khi animation kết thúc
                    break;
            }

            // Cập nhật _currentState
            _currentState = playerState;

            if (GroundCheck && _velocity.Y > 0)
            {
                _onGround = true;
                //WorldPos = new Vector2(WorldPos.X, 500);
            }
            else if (!GroundCheck)
            {
                _onGround = false;
            }
        }

        public void UpdatePosition(Vector2 newPos)
        {
            WorldPos += newPos;
        }
        /// <summary>Lấy hộp va chạm (Bounding Box) hiện tại của đối tượng.</summary>
        public override Rectangle Collider()
        {
            Rectangle bounds = new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.1f),
                (int)(WorldPos.Y - Sprite.Height * 0.2f),
                (int)(Sprite.Width * 0.2),
                (int)(Sprite.Height * 0.7)
            );

            return bounds;
        }

        public void CollectCoin()
        {
            Coin++;
        }

        public void CheckTilemap(Tilemap tilemap)
        {
            Rectangle bounds = Collider();
            // Kiểm tra các điểm ở cạnh trái/phải tùy hướng di chuyển
            float xCheck = _velocity.X > 0 ? bounds.Right : bounds.Left;
            // Kiểm tra va chạm tại 3 điểm (đầu, giữa, chân) để bao quát chiều cao
            if (bounds.Intersects(tilemap.GetWorldTileCollider(xCheck, bounds.Top)) ||
                bounds.Intersects(tilemap.GetWorldTileCollider(xCheck, bounds.Bottom - 2)))
            {
                Rectangle tileRect = tilemap.GetWorldTileCollider(xCheck, bounds.Intersects(tilemap.GetWorldTileCollider(xCheck, bounds.Top)) ? bounds.Top : bounds.Bottom - 2);
                if ((bounds.X < tileRect.X? bounds.Right - tileRect.Left: tileRect.Right - bounds.Left) < (bounds.Y < tileRect.Y? bounds.Bottom - tileRect.Top: tileRect.Bottom - bounds.Top))
                {
                    if (_velocity.X > 0) WorldPos -= new Vector2(xCheck - tileRect.Left, 0);
                    else WorldPos += new Vector2(tileRect.Right - xCheck, 0);
                    _velocity.X = 0;
                }
            }
            float yCheck = _velocity.Y > 0 ? bounds.Bottom : bounds.Top;

            if (bounds.Intersects(tilemap.GetWorldTileCollider(bounds.Left, yCheck)) ||
                bounds.Intersects(tilemap.GetWorldTileCollider(bounds.Right, yCheck)))
            {
                // Position ± Collision Area (Đẩy player lên sàn)
                Rectangle tileRect = tilemap.GetWorldTileCollider(bounds.Intersects(tilemap.GetWorldTileCollider(bounds.Left, yCheck)) ? bounds.Left : bounds.Right, yCheck);
                if (_velocity.Y > 0 && bounds.Bottom > tileRect.Top)
                {
                    WorldPos += new Vector2(0, tileRect.Top - bounds.Bottom);
                }

                _velocity.Y = 0;
            }
        }

        private void HandleAction()
        {
            if (GameController.Action())
                OnAction?.Invoke(this, EventArgs.Empty);
        }
        private void HandleAttackDamage()
        {
            // Chỉ kiểm tra sát thương nếu đang ở trạng thái Attack
            if (_currentState != PlayerState.Attack)
            {
                _hasDealtDamage = false;
                return;
            }

            // Xác định khung hình gây sát thương
            if (!_hasDealtDamage && Animator.CurrentFrame == 5)
            {
                // Tạo một vùng va chạm tạm thời (Hitbox) phía trước mặt
                Rectangle hitbox;

                if (_faceRight)
                    hitbox = new Rectangle((int)WorldPos.X, (int)WorldPos.Y - 20, 50, 40);
                else
                    hitbox = new Rectangle((int)WorldPos.X - 50, (int)WorldPos.Y - 20, 50, 40);

                OnAttackDealt?.Invoke(hitbox);

                _hasDealtDamage = true;
            }
        }
        public void TakeDamage(int dmg)
        {
            bool isInvincible = _iframeTimer > 0;
            if (isInvincible) return;
            _currentHealth -= dmg;
            _iframeTimer = IFRAME_DURATION;
            _hurtSound.Play();

            if (_currentHealth > 0)
                _currentState = PlayerState.Hurt;
            else
                _currentState = PlayerState.Die;

            // Đẩy nhẹ đối tượng lùi lại (Knockback)
            //_velocity.X = _faceRight ? -200f : 200f;
        }
    }
}
