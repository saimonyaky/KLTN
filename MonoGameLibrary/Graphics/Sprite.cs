using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite
{
    /// <summary>Lấy hoặc thiết lập vùng texture nguồn (source texture region) mà sprite này đại diện.</summary>
    public TextureRegion Region { get; set; }

    /// <summary>Lấy hoặc thiết lập mặt nạ màu (color mask) để áp dụng khi render (vẽ) sprite này.</summary>
    /// <remarks>Giá trị mặc định là Color.White (Màu Trắng)</remarks>
    public Color Color { get; set; } = Color.White;

    /// <summary>Lấy hoặc thiết lập lượng xoay (rotation), tính bằng radian, để áp dụng khi render sprite này.</summary>
    /// <remarks>Giá trị mặc định là 0.0f</remarks>
    public float Rotation { get; set; } = 0.0f;

    /// <summary>Lấy hoặc thiết lập hệ số tỉ lệ (scale factor) để áp dụng cho trục x và y khi render sprite này.</summary>
    /// <remarks>Giá trị mặc định là Vector2.One</remarks>
    public Vector2 Scale { get; set; } = Vector2.One;

    /// <summary>Lấy hoặc thiết lập điểm gốc (origin point) tọa độ xy, tương đối so với góc trên bên trái của sprite này.</summary>
    /// <remarks>Giá trị mặc định là Vector2.Zero</remarks>
    public Vector2 Origin { get; set; } = Vector2.Zero;

    /// <summary>Lấy hoặc thiết lập hiệu ứng sprite (sprite effects) để áp dụng khi render sprite này.</summary>
    /// <remarks>Giá trị mặc định là SpriteEffects.None (Không có hiệu ứng)</remarks>
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// <summary>Lấy hoặc thiết lập độ sâu lớp (layer depth) để áp dụng khi render sprite này.</summary>
    /// <remarks>Giá trị mặc định là 0.0f</remarks>
    public float LayerDepth { get; set; } = 0.0f;

    /// <summary>Lấy chiều rộng, tính bằng pixel, của sprite này.</summary>
    /// <remarks>Chiều rộng được tính bằng cách nhân chiều rộng của vùng texture nguồn với hệ số tỉ lệ trên trục x.</remarks>
    public float Width => Region.Width * Scale.X;

    /// <summary>Lấy chiều cao, tính bằng pixel, của sprite này.</summary>
    /// <remarks>Chiều cao được tính bằng cách nhân chiều cao của vùng texture nguồn với hệ số tỉ lệ trên trục y.</remarks>
    public float Height => Region.Height * Scale.Y;

    /// <summary>Tạo một sprite mới.</summary>
    public Sprite() { }

    /// <summary>Tạo một sprite mới sử dụng vùng texture nguồn được chỉ định.</summary>
    /// <param name="region">Vùng texture để sử dụng làm vùng texture nguồn cho sprite này.</param>
    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    /// <summary>Thiết lập điểm gốc (origin) của sprite này về chính giữa.</summary>
    public void CenterOrigin()
    {
        if (Region != null)
        {
            Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
        }
    }

    /// <summary>Gửi sprite này để được vẽ (draw) tới batch hiện tại.</summary>
    /// <param name="spriteBatch">Instance của SpriteBatch được sử dụng để gom nhóm (batching) các lệnh vẽ.</param>
    /// <param name="position">Vị trí tọa độ xy để render sprite này tại đó.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
    }

}
