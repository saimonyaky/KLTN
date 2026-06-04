using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Collision;
using MonoGameLibrary.Graphics;
using System;
using static RPGGame.GameObjects.Player;

namespace RPGGame.GameObjects
{
    internal class Bandit : GameObject
    {
        public enum BanditState
        {
            Patrol,
            Chase,
            Attack,
            Hurt,
            Die
        }
        public bool IsAlive = true;

        private BanditState _currentState = BanditState.Patrol;
        private bool _faceRight = true;
        private float _speed = 100f;
        private int _maxHealth = 3;
        private int _currentHealth;
        private Player _player;
        private Vector2 _rightPlatform;
        private Vector2 _leftPlatform;
        private Vector2 _velocity;
        private float _chasingRadius = 200f;
        private float _attackRadius = 50f;

        /// <summary> Đối tượng Animator quản lý trạng thái và khung hình. </summary>
        public Animator<BanditState> Animator { get; private set; }

        public Bandit(TextureAtlas atlas, Vector2 leftPos, Vector2 rightPos, Player player)
        {
            Sprite.Scale = new Vector2(2.0f, 2.0f);
            _currentHealth = _maxHealth;
            _velocity = Vector2.Zero;
            _leftPlatform = leftPos;
            _rightPlatform = rightPos;
            _player = player;
            //_currentState = BanditState.Idle;

            Animator = new Animator<BanditState>(_currentState);
            Animator.AddAnimation(BanditState.Patrol, atlas.GetAnimation("bandit-run"));
            Animator.AddAnimation(BanditState.Chase, atlas.GetAnimation("bandit-run"));
            Animator.AddAnimation(BanditState.Attack, atlas.GetAnimation("bandit-attack"));
            Animator.AddAnimation(BanditState.Hurt, atlas.GetAnimation("bandit-hurt"));
            Animator.AddAnimation(BanditState.Die, atlas.GetAnimation("bandit-die"));

            Sprite.Region = Animator.CurrentRegion; // Gán Region ban đầu
            Sprite.CenterOrigin();
        }

        public override void Initialize(Vector2 startingPosition)
        {
            base.Initialize(startingPosition);
            IsAlive = true;
        }

        public override void HandleStateLogic(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Tính velocity
            UpdateAIState();

            // Cập nhật hướng và vị trí
            //if (_velocity.X > 0) _faceRight = true;
            //else if (_velocity.X < 0) _faceRight = false;
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

        private void UpdateAIState()
        {
            float distanceToPlayer = Vector2.Distance(WorldPos, _player.WorldPos);

            // Player có đang nằm trong phạm vi tuần tra của Enemy không?
            // Thêm một khoảng đệm (ví dụ 50px) để tránh việc Enemy khựng lại ngay lập tức khi Player vừa chạm mép
            bool isPlayerInPatrolZone = _player.WorldPos.X >= (_leftPlatform.X - 50) &&
                                        _player.WorldPos.X <= (_rightPlatform.X + 50)&&
                                        -20 <= _player.WorldPos.Y - WorldPos.Y &&
                                        _player.WorldPos.Y - WorldPos.Y <= 20;

            switch (_currentState)
            {
                case BanditState.Patrol:
                    // Đi tuần qua lại giữa 2 điểm
                    if (_faceRight)
                    {
                        _velocity.X = _speed;
                        if (WorldPos.X > _rightPlatform.X) _faceRight = false;
                    }
                    else
                    {
                        _velocity.X = -_speed;
                        if (WorldPos.X < _leftPlatform.X) _faceRight = true;
                    }
                    if (distanceToPlayer < _attackRadius)
                    {
                        _currentState = BanditState.Attack;
                    }
                    // Chỉ Chase khi thỏa mãn cả 2 điều kiện
                    else if (distanceToPlayer < _chasingRadius && isPlayerInPatrolZone)
                    {
                        _currentState = BanditState.Chase;
                    }
                    break;
                case BanditState.Chase:
                    // Đuổi theo người chơi
                    if (_player.WorldPos.X > WorldPos.X)
                    {
                        _velocity.X = _speed * 1.5f; // Đuổi nhanh hơn đi tuần
                        _faceRight = true;
                    }
                    else
                    {
                        _velocity.X = -_speed * 1.5f;
                        _faceRight = false;
                    }
                    if (distanceToPlayer < _attackRadius)
                    {
                        _currentState = BanditState.Attack;
                    }
                    // Chỉ Chase khi thỏa mãn cả 2 điều kiện
                    else if (distanceToPlayer > _chasingRadius && isPlayerInPatrolZone)
                    {
                        _currentState = BanditState.Patrol;
                    }
                    break;
                case BanditState.Attack:
                    _velocity.X = 0;
                    if (Animator.AnimationFinished)
                    {
                        if (_player.WorldPos.X > WorldPos.X)
                            _faceRight = true;
                        else
                            _faceRight = false;
                        if (distanceToPlayer < _chasingRadius && isPlayerInPatrolZone)
                        {
                            _currentState = BanditState.Chase;
                        }
                        else
                        {
                            _currentState = BanditState.Patrol;
                        }
                    }
                    break;
                case BanditState.Hurt:
                    _velocity.X = 0;
                    if (Animator.AnimationFinished && _currentHealth > 0)
                        _currentState = BanditState.Patrol;
                    else if (Animator.AnimationFinished && _currentHealth <= 0)
                        _currentState = BanditState.Die;
                    break;
                case BanditState.Die:
                    _velocity.X = 0;
                    if (Animator.AnimationFinished)
                        IsAlive = false;
                        break;
                default:
                    break;
            }
        }

        public override Rectangle Collider()
        {
            Rectangle bounds = new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.2f),
                (int)(WorldPos.Y - Sprite.Height * 0.3f),
                (int)(Sprite.Width * 0.4),
                (int)(Sprite.Height * 0.8)
            );

            return bounds;
        }

        //public override void OnTriggerEnter(Rectangle other)
        //{
        //    if (other.Tag == "PlayerAttack")
        //    {
        //        this.TakeDamage(20);
        //    }
        //}
        private void HandleAttackDamage()
        {
            // Chỉ kiểm tra sát thương nếu đang ở trạng thái Attack
            if (_currentState != BanditState.Attack) return;

            // Xác định khung hình gây sát thương
            if (Animator.CurrentFrame == 4)
            {
                // Tạo một vùng va chạm tạm thời (Hitbox) phía trước mặt
                Rectangle hitbox;

                if (_faceRight)
                    hitbox = new Rectangle((int)WorldPos.X, (int)WorldPos.Y - 20, 50, 40);
                else
                    hitbox = new Rectangle((int)WorldPos.X - 50, (int)WorldPos.Y - 20, 50, 40);

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
            if (_currentState != BanditState.Die)
                _currentState = BanditState.Hurt;
        }

        public override void Draw()
        {
            base.Draw();
            Rectangle hitbox;

            if (_faceRight)
                hitbox = new Rectangle((int)WorldPos.X, (int)WorldPos.Y - 20, 50, 40);
            else
                hitbox = new Rectangle((int)WorldPos.X - 50, (int)WorldPos.Y - 20, 50, 40);
            //DebugRenderer.DrawRectangleOutline(Core.SpriteBatch, hitbox, Color.White);
        }
    }
}
