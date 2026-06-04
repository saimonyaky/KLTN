using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace RPGGame.GameObjects
{
    public abstract class GameObject
    {
        /// <summary>Sprite đại diện cho hình ảnh của GameObject.</summary>
        public Sprite Sprite;
        /// <summary>Tọa độ vị trí của GameObject.</summary>
        public Vector2 WorldPos { get; protected set; }

        public GameObject()
        {
            Sprite = new Sprite();
        }

        /// <summary>
        /// Initializes the game object, can be used to reset it back to an initial state.
        /// </summary>
        /// <param name="startingPosition">The position the slime should start at.</param>
        public virtual void Initialize(Vector2 startingPosition)
        {
            WorldPos = startingPosition;
        }

        /// <summary>
        /// Updates the game object.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
        public void Update(GameTime gameTime)
        {
            //// Update the animated sprite.
            //_sprite.Update(gameTime);

            // Gọi hàm xử lý logic riêng của từng GameObject (AI, Input, Physics)
            HandleStateLogic(gameTime);
        }

        // Thêm hàm abstract (hoặc virtual) để bắt buộc các lớp con thực thi logic riêng
        public abstract void HandleStateLogic(GameTime gameTime);

        public abstract Rectangle Collider();

        /// <summary>
        /// Được gọi khi một va chạm vật lý (IsTrigger=false) xảy ra với Collider khác.
        /// </summary>
        /// <param name="otherCollider">Collider bị va chạm.</param>
        public virtual void OnCollisionEnter(Rectangle otherCollider) { }

        /// <summary>
        /// Được gọi khi hai Collider (ít nhất một cái là IsTrigger=true) chồng lên nhau.
        /// </summary>
        /// <param name="otherCollider">Collider kích hoạt trigger.</param>
        public virtual void OnTriggerEnter(Rectangle otherCollider) { }

        /// <summary>Vẽ game object.</summary>
        public virtual void Draw()
        {
            Sprite.Draw(Core.SpriteBatch, WorldPos);
            DebugRenderer.DrawRectangleOutline(Core.SpriteBatch, Collider(), Color.White);
        }

    }
}
