using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System;

namespace RPGGame.GameObjects
{
    internal class Boss : GameObject
    {
        public enum BossState
        {
            Idle,
            Chase,
            Flee,
            Attack,
            Hurt,
            Die
        }
        private BossState _currentState = BossState.Idle;
        private bool _faceRight = false;
        private float _speed = 100f;
        private int _maxHealth = 5;
        private int _currentHealth;
        private Player _player;
        private Vector2 _rightPlatform;
        private Vector2 _leftPlatform;
        private Vector2 _velocity;
        //private float _chasingRadius = 500f;
        private float _attackRadius = 300f;
        private float _fleeRadius = 175f;
        private bool _hasChase = false;

        public bool IsActive { get; set; } = false;
        /// <summary> Đối tượng Animator quản lý trạng thái và khung hình. </summary>
        public Animator<BossState> Animator { get; private set; }

        public Boss(TextureAtlas atlas, Vector2 leftPos, Vector2 rightPos, Player player)
        {
            Sprite.Scale = new Vector2(3.0f, 3.0f);
            _currentHealth = _maxHealth;
            _velocity = Vector2.Zero;
            _player = player;
            _leftPlatform = leftPos;
            _rightPlatform = rightPos;
            //_currentState = BanditState.Idle;

            Animator = new Animator<BossState>(_currentState);
            Animator.AddAnimation(BossState.Idle, atlas.GetAnimation("boss-idle"));
            Animator.AddAnimation(BossState.Chase, atlas.GetAnimation("boss-walk"));
            Animator.AddAnimation(BossState.Flee, atlas.GetAnimation("boss-walk"));
            Animator.AddAnimation(BossState.Attack, atlas.GetAnimation("boss-attack"));
            Animator.AddAnimation(BossState.Hurt, atlas.GetAnimation("boss-hurt"));
            Animator.AddAnimation(BossState.Die, atlas.GetAnimation("boss-die"));

            Sprite.Region = Animator.CurrentRegion; // Gán Region ban đầu
            Sprite.CenterOrigin();
        }

        public override void Initialize(Vector2 startingPosition)
        {
            base.Initialize(startingPosition);
            IsActive = true;
            _currentState = BossState.Idle;
        }

        public override void HandleStateLogic(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Tính velocity
            UpdateBossState();

            // Cập nhật vị trí
            WorldPos += _velocity * deltaTime;

            Animator.SetState(_currentState);

            // Cập nhật thời gian
            Animator.Update(gameTime);

            // Gán Region và SpriteEffects
            Sprite.Region = Animator.CurrentRegion;
            Sprite.Effects = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Căn Origin nếu kích thước frame thay đổi
            Sprite.CenterOrigin();

            HandleAttackDamage();
        }

        private void UpdateBossState()
        {
            float distanceToPlayer = Vector2.Distance(WorldPos, _player.WorldPos);

            // Player có đang nằm trong phạm vi truy đuổi của Boss không?
            bool isPlayerInChaseZone = _player.WorldPos.X >= _leftPlatform.X &&
                                        _player.WorldPos.X <= _rightPlatform.X;

            switch (_currentState)
            {
                case BossState.Idle:
                    _velocity.X = 0;
                    if (distanceToPlayer < _attackRadius)
                    {
                        WorldPos += new Vector2(0, -48);
                        _currentState = BossState.Attack;
                    }
                    // Chỉ Chase khi thỏa mãn cả 2 điều kiện
                    else if (distanceToPlayer > _attackRadius && isPlayerInChaseZone)
                    {
                        _currentState = BossState.Chase;
                    }
                    else if (isPlayerInChaseZone && distanceToPlayer < _fleeRadius)
                    {
                        _currentState = BossState.Flee;
                    }
                    break;
                case BossState.Chase:
                    // Đuổi theo người chơi
                    if (_player.WorldPos.X > WorldPos.X)
                    {
                        _velocity.X = _speed;
                        _faceRight = true;
                    }
                    else
                    {
                        _velocity.X = -_speed;
                    }
                    if (distanceToPlayer < _attackRadius)
                    {
                        WorldPos += new Vector2(0, -48);
                        _currentState = BossState.Attack;
                    }
                    // Chỉ Chase khi thỏa mãn cả 2 điều kiện
                    else if (!isPlayerInChaseZone)
                    {
                        _currentState = BossState.Idle;
                    }
                    break;
                case BossState.Flee:
                    if (_player.WorldPos.X > WorldPos.X)
                    {
                        _velocity.X = (_player.WorldPos.X - _leftPlatform.X > 175) ? -_speed : _speed;
                        _faceRight = true;
                    }
                    else
                    {
                        _velocity.X = (_rightPlatform.X - _player.WorldPos.X > 175) ? _speed : -_speed;
                        _faceRight = false;

                    }
                    if (distanceToPlayer > _fleeRadius && distanceToPlayer < _attackRadius)
                    {
                        WorldPos += new Vector2(0, -48);
                        _currentState = BossState.Attack;
                    }
                    break;
                case BossState.Attack:

                    _velocity.X = 0;
                    if (Animator.AnimationFinished)
                    {
                        WorldPos += new Vector2(0, 48);

                        if (_player.WorldPos.X > WorldPos.X)
                            _faceRight = true;
                        else
                            _faceRight = false;
                        if (isPlayerInChaseZone && distanceToPlayer > _attackRadius)
                        {
                            _currentState = BossState.Chase;
                        }
                        else if (isPlayerInChaseZone && distanceToPlayer < _fleeRadius)
                        {
                            _currentState = BossState.Flee;
                        }
                        else
                        {
                            _currentState = BossState.Idle;
                        }
                    }
                    break;
                case BossState.Hurt:
                    _velocity.X = 0;
                    if (Animator.AnimationFinished)
                        _currentState = BossState.Idle;
                    if (Animator.AnimationFinished && _currentHealth <= 0)
                        _currentState = BossState.Die;
                    break;
                case BossState.Die:
                    if (Animator.AnimationFinished)
                        IsActive = false;
                    break;
                default:
                    break;
            }
        }

        public override Rectangle Collider()
        {
            Rectangle bounds = new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.1f),
                (int)(WorldPos.Y - Sprite.Height * 0.5f),
                (int)(Sprite.Width * 0.2),
                (int)Sprite.Height
            );

            return bounds;
        }

        private void HandleAttackDamage()
        {
            // Chỉ kiểm tra sát thương nếu đang ở trạng thái Attack
            if (_currentState != BossState.Attack) return;

            // Xác định khung hình gây sát thương
            if (Animator.CurrentFrame == 4)
            {
                // Tạo một vùng va chạm tạm thời (Hitbox) phía trước mặt
                Rectangle hitbox;

                if (_faceRight)
                    hitbox = new Rectangle((int)WorldPos.X + (int)Sprite.Width / 4, (int)WorldPos.Y, (int)Sprite.Width / 4, (int)Sprite.Height / 2);
                else
                    hitbox = new Rectangle((int)WorldPos.X - (int)Sprite.Width / 2, (int)WorldPos.Y, (int)Sprite.Width / 4, (int)Sprite.Height / 2);

                // 4. Kiểm tra va chạm giữa Hitbox và Collider của đối phương
                if (hitbox.Intersects(_player.Collider()))
                {
                    // Gây sát thương cho Player
                    _player.TakeDamage(1);

                }
            }
        }

        public void TakeDamage(int dmg)
        {

            _currentHealth -= dmg;
            if (_currentState == BossState.Attack)
                WorldPos += new Vector2(0, 48);
            if (_currentState != BossState.Die)
                _currentState = BossState.Hurt;
        }

        public override void Draw()
        {
            if (IsActive)
                base.Draw();
            Rectangle hitbox;

            if (_faceRight)
                hitbox = new Rectangle((int)WorldPos.X + (int)Sprite.Width / 4, (int)WorldPos.Y, (int)Sprite.Width / 4, (int)Sprite.Height / 2);
            else
                hitbox = new Rectangle((int)WorldPos.X - (int)Sprite.Width / 2, (int)WorldPos.Y, (int)Sprite.Width / 4, (int)Sprite.Height / 2);
            //DebugRenderer.DrawRectangleOutline(Core.SpriteBatch, hitbox, Color.White);
        }
    }
}
