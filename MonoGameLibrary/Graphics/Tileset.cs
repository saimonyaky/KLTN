using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class Tileset
{
    private readonly TextureRegion[] _tiles;
    private readonly Rectangle[] _tileColliders;

    /// <summary>Lấy chiều rộng, tính bằng pixel, của mỗi ô (tile) trong tileset này.</summary>
    public int TileWidth { get; }

    /// <summary>Lấy chiều cao, tính bằng pixel, của mỗi ô (tile) trong tileset này.</summary>
    public int TileHeight { get; }

    /// <summary>Lấy tổng số cột trong tileset này.</summary>
    public int Columns { get; }

    /// <summary>Lấy tổng số dòng trong tileset này.</summary>
    public int Rows { get; }

    /// <summary>Lấy tổng số ô trong tileset này.</summary>
    public int Count { get; }

    /// <summary>Tạo một tileset mới dựa trên vùng texture đã cho với chiều rộng và chiều cao ô đã chỉ định.</summary>
    /// <param name="textureRegion">Vùng texture chứa các ô cho tileset.</param>
    /// <param name="tileWidth">Chiều rộng của mỗi ô trong tileset.</param>
    /// <param name="tileHeight">Chiều cao của mỗi ô trong tileset.</param>
    public Tileset(TextureRegion textureRegion, int tileWidth, int tileHeight)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Columns = textureRegion.Width / tileWidth;
        Rows = textureRegion.Height / tileHeight;
        Count = Columns * Rows;

        // Tạo các vùng texture (texture regions) tạo nên từng ô riêng lẻ
        _tiles = new TextureRegion[Count];
        _tileColliders = new Rectangle[Count];

        for (int i = 0; i < Count; i++)
        {
            int x = i % Columns * tileWidth;
            int y = i / Columns * tileHeight;
            _tiles[i] = new TextureRegion(textureRegion.Texture, textureRegion.SourceRectangle.X + x, textureRegion.SourceRectangle.Y + y, tileWidth, tileHeight);
            _tileColliders[i] = new Rectangle(0, 0, tileWidth, tileHeight);
        }

    }

    /// <summary>Lấy vùng texture cho ô từ tileset này tại chỉ mục (index) đã cho.</summary>
    /// <param name="index">Chỉ mục của vùng texture trong bộ ô này.</param>
    /// <returns>Vùng texture cho ô từ tileset này tại chỉ mục đã cho.</returns>
    public TextureRegion GetTile(int index) => _tiles[index];

    /// <summary>Lấy vùng texture cho ô từ tileset này tại vị trí đã cho.</summary>
    /// <param name="column">Cột của vùng texture trong tileset này.</param>
    /// <param name="row">Hàng của vùng texture trong tileset này.</param>
    /// <returns>Vùng texture cho ô từ tileset này tại vị trí đã cho.</returns>
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    /// <summary>
    /// Thiết lập vùng va chạm (collider) cho ô tại chỉ mục đã cho.
    /// </summary>
    /// <param name="index">Chỉ mục của ô trong tileset.</param>
    /// <param name="collider">Rectangle đại diện cho vùng va chạm.</param>
    public void SetTileCollider(int index, Rectangle collider)
    {
        if (index >= 0 && index < Count)
        {
            _tileColliders[index] = collider;
        }
    }

    /// <summary>
    /// Lấy vùng va chạm (collider) cho ô từ tileset này tại chỉ mục đã cho.
    /// </summary>
    /// <param name="index">Chỉ mục của ô trong tileset này.</param>
    /// <returns>Rectangle đại diện cho vùng va chạm. Trả về Rectangle.Empty nếu chỉ mục không hợp lệ.</returns>
    public Rectangle GetTileCollider(int index)
    {
        if (index < 0 || index >= Count)
        {
            // Trả về một Rectangle rỗng hoặc an toàn khi truy vấn ô trống/không hợp lệ
            return Rectangle.Empty;
        }
        return _tileColliders[index];
    }
}