using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public readonly struct Circle : IEquatable<Circle>
{
    private static readonly Circle s_empty = new Circle();

    /// <summary>Tọa độ x của tâm hình tròn này.</summary>
    public readonly int X;

    /// <summary>Tọa độ y của tâm hình tròn này.</summary>
    public readonly int Y;

    /// <summary>Chiều dài, tính bằng pixel, từ tâm hình tròn này đến vành (cạnh).</summary>
    public readonly int Radius;

    /// <summary>Lấy vị trí tâm hình tròn này.</summary>
    public readonly Point Location => new Point(X, Y);

    /// <summary>Lấy một hình tròn có X=0, Y=0, và Radius=0.</summary>
    public static Circle Empty => s_empty;

    /// <summary>Lấy một giá trị cho biết liệu hình tròn này có bán kính (radius) bằng 0 và vị trí (location) là (0, 0) hay không.</summary>
    public readonly bool IsEmpty => X == 0 && Y == 0 && Radius == 0;

    /// <summary>Lấy tọa độ y của điểm cao nhất trên hình tròn này.</summary>
    public readonly int Top => Y - Radius;

    /// <summary>Lấy tọa độ y của điểm thấp nhất trên hình tròn này.</summary>
    public readonly int Bottom => Y + Radius;

    /// <summary>Lấy tọa độ x của điểm ngoài cùng bên trái trên hình tròn này.</summary>
    public readonly int Left => X - Radius;

    /// <summary>Lấy tọa độ x của điểm ngoài cùng bên phải trên hình tròn này.</summary>
    public readonly int Right => X + Radius;

    /// <summary>Tạo một hình tròn mới với vị trí và bán kính đã chỉ định.</summary>
    /// <param name="x">Tọa độ x của tâm hình tròn.</param>
    /// <param name="y">Tọa độ y của tâm hình tròn.</param>
    /// <param name="radius">Chiều dài từ tâm hình tròn đến một cạnh (bán kính).</param>
    public Circle(int x, int y, int radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    /// <summary>Tạo một hình tròn mới với vị trí và bán kính đã chỉ định.</summary>
    /// <param name="location">Tâm hình tròn.</param>
    /// <param name="radius">Bán kính</param>
    public Circle(Point location, int radius)
    {
        X = location.X;
        Y = location.Y;
        Radius = radius;
    }

    /// <summary>Trả về một giá trị cho biết liệu hình tròn được chỉ định có giao cắt (intersects) với hình tròn này hay không.</summary>
    /// <param name="other">Hình tròn khác để kiểm tra.</param>
    /// <returns>true nếu hình tròn khác giao cắt với hình tròn này; ngược lại là false.</returns>
    public bool Intersects(Circle other)
    {
        int radiiSquared = (this.Radius + other.Radius) * (this.Radius + other.Radius);
        float distanceSquared = Vector2.DistanceSquared(this.Location.ToVector2(), other.Location.ToVector2());
        return distanceSquared < radiiSquared;
    }

    /// <summary>Trả về một giá trị cho biết liệu hình tròn này và đối tượng được chỉ định có bằng nhau (equal) hay không.</summary>
    /// <param name="obj">Đối tượng để so sánh với hình tròn này.</param>
    /// <returns>true nếu hình tròn này và đối tượng được chỉ định bằng nhau; ngược lại là false.</returns>
    public override readonly bool Equals(object obj) => obj is Circle other && Equals(other);

    /// <summary>Trả về một giá trị cho biết liệu hình tròn này và hình tròn được chỉ định có bằng nhau hay không.</summary>
    /// <param name="other">Hình tròn để so sánh với hình tròn này.</param>
    /// <returns>true nếu hình tròn này và hình tròn được chỉ định bằng nhau; ngược lại là false.</returns>
    public readonly bool Equals(Circle other) => this.X == other.X &&
                                                    this.Y == other.Y &&
                                                    this.Radius == other.Radius;

    /// <summary>Trả về mã băm (hash code) cho hình tròn này.</summary>
    /// <returns>Mã băm cho hình tròn này dưới dạng số nguyên có dấu 32-bit.</returns>
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Radius);

    /// <summary>Trả về một giá trị cho biết liệu hình tròn ở phía bên trái của toán tử bằng (=) có bằng với
    /// hình tròn ở phía bên phải của toán tử bằng hay không.</summary>
    /// <param name="lhs">Hình tròn ở phía bên trái của toán tử.</param>
    /// <param name="rhs">Hình tròn ở phía bên phải của toán tử.</param>
    /// <returns>true nếu hai hình tròn bằng nhau; ngược lại là false.</returns>
    public static bool operator ==(Circle lhs, Circle rhs) => lhs.Equals(rhs);

    /// <summary>Trả về một giá trị cho biết liệu hình tròn ở phía bên trái của toán tử không bằng (!=) có không bằng với
    /// hình tròn ở phía bên phải của toán tử không bằng hay không.</summary>
    /// <param name="lhs">Hình tròn ở phía bên trái của toán tử không bằng.</param>
    /// <param name="rhs">Hình tròn ở phía bên phải của toán tử không bằng.</param>
    /// <returns>true nếu hai hình tròn không bằng nhau; ngược lại là false.</returns>
    public static bool operator !=(Circle lhs, Circle rhs) => !lhs.Equals(rhs);

}
