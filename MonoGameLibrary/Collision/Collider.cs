using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Collision
{
    public abstract class Collider
    {
        //private static readonly Collider s_empty = new Collider();

        /// <summary>Tạo một Collider mới.</summary>
        public Collider() { }

        /// <summary>Kiểm tra va chạm với một Collider khác.</summary>
        /// <param name="other">Collider để kiểm tra va chạm.</param>
        /// <returns>True nếu có va chạm.</returns>
        public abstract bool Intersects(Collider other);
    }
}
