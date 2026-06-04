using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using System;

namespace RPGGame.GameObjects
{
    internal class NPC : GameObject
    {
        public enum NPCState
        {
            Idle,
        }
        private NPCState _currentState = NPCState.Idle;
        private bool _faceRight = false;
        private Player _player;

        public bool IsActive { get; set; } = false;

        /// <summary> Đối tượng Animator quản lý trạng thái và khung hình. </summary>
        public Animator<NPCState> Animator { get; private set; }

        public NPC(TextureAtlas atlas, Player player)
        {
            Sprite.Scale = new Vector2(2.0f, 2.0f);
            _player = player;
            //_currentState = BanditState.Idle;

            Animator = new Animator<NPCState>(_currentState);
            Animator.AddAnimation(NPCState.Idle, atlas.GetAnimation("NPC-idle"));

            Sprite.Region = Animator.CurrentRegion; // Gán Region ban đầu
            Sprite.CenterOrigin();
        }

        public override void Initialize(Vector2 startingPosition)
        {
            base.Initialize(startingPosition);
            IsActive = true;
        }

        public override void HandleStateLogic(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Animator.SetState(_currentState);

            // Cập nhật thời gian
            Animator.Update(gameTime);

            // Gán Region và SpriteEffects
            Sprite.Region = Animator.CurrentRegion;
            Sprite.Effects = _faceRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Căn Origin nếu kích thước frame thay đổi
            Sprite.CenterOrigin();

        }


        public override Rectangle Collider()
        {
            Rectangle bounds = new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.4f),
                (int)(WorldPos.Y - Sprite.Height * 0.5f),
                (int)(Sprite.Width * 0.8),
                (int)Sprite.Height
            );

            return bounds;
        }

        
    }
}
