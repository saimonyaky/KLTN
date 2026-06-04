using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

/// <summary>Đại diện cho một vùng hình chữ nhật trong một vân bề mặt(texture).</summary>
public class TextureRegion //Vùng texture
{
    /// <summary>Lấy hoặc Thiết lập texture nguồn mà vùng texture này là một phần của nó.</summary>
    public Texture2D Texture { get; set; }

    /// <summary>Lấy hoặc Thiết lập ranh giới hình chữ nhật nguồn (source rectangle boundary) của vùng texture này bên trong texture nguồn.</summary>
    public Rectangle SourceRectangle { get; set; }
  
    /// <summary>Gets chiều rộng, tính bằng pixels, của texture region này.</summary>
    public int Width => SourceRectangle.Width;

    /// <summary>Gets chiều cao, tính bằng pixels, của texture region này.</summary>
    public int Height => SourceRectangle.Height;

    /// <summary>Tạo một vùng texture mới.</summary>
    public TextureRegion() { }

    /// <summary>Lấy tọa độ texture chuẩn hóa (normalized texture coordinate) trên cùng của vùng này.</summary>
    public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;

    /// <summary>Lấy tọa độ texture chuẩn hóa phía dưới của vùng này.</summary>
    public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;

    /// <summary>Lấy tọa độ texture chuẩn hóa bên trái của vùng này.</summary>
    public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;

    /// <summary>Lấy tọa độ texture chuẩn hóa bên phải của vùng này.</summary>
    public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;

    /// <summary>Tạo texture region mới bằng cách sử dụng texture nguồn được chỉ định.</summary>
    /// <param name="texture">Texture được sử dụng làm texture nguồn cho texture region.</param>
    /// <param name="x">Vị trí tọa độ x của góc trên bên trái của texture region này so với góc trên bên trái của texture nguồn.</param>
    /// <param name="y">Vị trí tọa độ y của góc trên bên trái của texture region này so với góc trên bên trái của texture nguồn.</param>
    /// <param name="width">Chiều rộng, tính bằng pixels, của texture region này.</param>
    /// <param name="height">Chiều cao, tính bằng pixels, của texture region này.</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    /// <summary>Gửi vùng texture này để vẽ (draw) trong batch hiện tại.</summary>
    /// <param name="spriteBatch">Instance spritebatch được sử dụng để gom nhóm (batching) các lệnh vẽ.</param>
    /// <param name="position">Vị trí tọa độ xy để vẽ vùng texture này trên màn hình.</param>
    /// <param name="color">Mặt nạ màu (color mask) để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
    }

    /// <summary>Gửi vùng texture này để vẽ trong batch hiện tại.</summary>
    /// <param name="spriteBatch">Instance spritebatch được sử dụng để gom nhóm các lệnh vẽ.</param>
    /// <param name="position">Vị trí tọa độ xy để vẽ vùng texture này trên màn hình.</param>
    /// <param name="color">Mặt nạ màu để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="rotation">Lượng xoay (rotation), tính bằng radian, để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="origin">Tâm (center) để xoay, thay đổi tỉ lệ (scaling) và định vị khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="scale">Hệ số tỉ lệ để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="effects">Chỉ định liệu vùng texture này nên được lật ngang, lật dọc, hay cả hai khi vẽ trên màn hình.</param>
    /// <param name="layerDepth">Độ sâu của lớp (layer depth) để sử dụng khi vẽ vùng texture này trên màn hình.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(
            spriteBatch,
            position,
            color,
            rotation,
            origin,
            new Vector2(scale, scale),
            effects,
            layerDepth
        );
    }

    /// <summary>Gửi vùng texture này để vẽ trong batch hiện tại.</summary>
    /// <param name="spriteBatch">Instance spritebatch được sử dụng để gom nhóm các lệnh vẽ.</param>
    /// <param name="position">Vị trí tọa độ xy để vẽ vùng texture này trên màn hình.</param>
    /// <param name="color">Mặt nạ màu để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="rotation">Lượng xoay, tính bằng radian, để áp dụng khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="origin">Tâm để xoay, thay đổi tỉ lệ và định vị khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="scale">Lượng thay đổi tỉ lệ để áp dụng cho trục x và y khi vẽ vùng texture này trên màn hình.</param>
    /// <param name="effects">Chỉ định liệu vùng texture này nên được lật ngang, lật dọc, hay cả hai khi vẽ trên màn hình.</param>
    /// <param name="layerDepth">Độ sâu của lớp để sử dụng khi vẽ vùng texture này trên màn hình.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(
            Texture,
            position,
            SourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth
        );
    }

}