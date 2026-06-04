using Microsoft.Xna.Framework;
using System;

namespace MonoGameLibrary.Collision
{
    public class RectangleCollider : Collider
    {
        public Rectangle Bounds { get; private set; }
        public override bool Intersects(Collider other)
        {
            //if (other is RectangleCollider rectOther)
            //{
            //    // Rectangle vs Rectangle
            //    return this.Bounds.Intersects(rectOther.Bounds);
            //}
            //else if (other is CircleCollider circleOther)
            //{
            //    // Rectangle vs Circle (Sử dụng thuật toán tìm điểm gần nhất)
            //    return IntersectsCircle(circleOther);
            //}
            return false;
        }
    }
}
