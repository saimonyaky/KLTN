using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using System;

namespace RPGGame.GameObjects
{
    public class Item : GameObject
    {
        public string ItemName { get; set; }
        public bool IsPickedUp { get; private set; } = false;
        public Item(Sprite sprite)
        {
            Sprite = sprite;
            Sprite.Scale = new Vector2(1.5f, 1.5f);
            Sprite.CenterOrigin();
        }

        public override void Initialize(Vector2 startingPosition)
        {
            base.Initialize(startingPosition);
            IsPickedUp = false;
        }

        public override void HandleStateLogic(GameTime gameTime)
        {

        }

        // Trả về vùng va chạm dựa trên Sprite và vị trí hiện tại
        public override Rectangle Collider()
        {
            return new Rectangle(
                (int)(WorldPos.X - Sprite.Width * 0.1f),
                (int)(WorldPos.Y - Sprite.Height * 0.1f),
                (int)(Sprite.Width * 0.2),
                (int)(Sprite.Height * 0.2)
            );
        }

        // Sử dụng OnTriggerEnter để xử lý việc nhặt đồ
        public override void OnTriggerEnter(Rectangle playerCollider)
        {
            if (!IsPickedUp)
            {
                IsPickedUp = true;
                //OnItemPickedUp();
            }
        }

        private void OnItemPickedUp()
        {
            //Phát âm thanh, cộng điểm, hoặc thêm vào Inventory
            //Console.WriteLine($"Đã nhặt vật phẩm: {ItemName}");
        }

        public override void Draw()
        {
            if(!IsPickedUp)
                base.Draw();
        }
    }
}
