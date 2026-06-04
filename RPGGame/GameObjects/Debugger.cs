using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class DebugRenderer
{
    private static Texture2D _pixel;

    public static void Initialize(GraphicsDevice device)
    {
        if (device == null) return;
        _pixel = new Texture2D(device, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Vẽ một hình chữ nhật đặc (filled rectangle).
    /// </summary>
    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        if (_pixel == null) return;

        spriteBatch.Draw(
            _pixel,
            rect, // Vị trí và kích thước (Destination Rectangle)
            color * 0.5f // Dùng alpha 0.5f để nhìn xuyên qua (Tùy chọn)
        );
    }

    /// <summary>
    /// Vẽ đường viền của một hình chữ nhật.
    /// </summary>
    public static void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rect, Color color, int lineWidth = 1)
    {
        // Vẽ cạnh trên
        DrawRectangle(spriteBatch, new Rectangle(rect.X, rect.Y, rect.Width, lineWidth), color);

        // Vẽ cạnh dưới
        DrawRectangle(spriteBatch, new Rectangle(rect.X, rect.Y + rect.Height - lineWidth, rect.Width, lineWidth), color);

        // Vẽ cạnh trái
        DrawRectangle(spriteBatch, new Rectangle(rect.X, rect.Y + lineWidth, lineWidth, rect.Height - lineWidth * 2), color);

        // Vẽ cạnh phải
        DrawRectangle(spriteBatch, new Rectangle(rect.X + rect.Width - lineWidth, rect.Y + lineWidth, lineWidth, rect.Height - lineWidth * 2), color);
    }
}