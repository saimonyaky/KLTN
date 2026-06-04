using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Camera
{
    public class Camera2D
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1.0f;
        public float Rotation { get; set; }
        public Matrix Transform { get; private set; }

        public Matrix GetViewMatrix(Viewport viewport)
        {
            return GetViewMatrix(Vector2.One, viewport);
        }

        public Matrix GetViewMatrix(Vector2 parallaxFactor, Viewport viewport)
        {
            // Nhân Position với parallaxFactor
            // Nếu factor < 1, camera sẽ "di chuyển ít hơn" đối với lớp đó -> tạo cảm giác ở xa
            return Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Matrix viewMatrix = GetViewMatrix(Vector2.One, Core.GraphicsDevice.Viewport);

            // Dùng Vector2.Transform để tính toán vị trí mới
            return Vector2.Transform(worldPosition, viewMatrix);
        }
    }
}
