using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Tilemap
{
    private readonly Tileset _tileset;
    private readonly int[] _tiles;

    /// <summary>Lấy tổng số hàng (rows) trong tilemap này.</summary>
    public int Rows { get; }

    /// <summary>Lấy tổng số cột (columns) trong tilemap này.</summary>
    public int Columns { get; }

    /// <summary>Lấy tổng số ô (tile) trong tilemap này.</summary>
    public int Count { get; }

    /// <summary>Lấy hoặc Thiết lập hệ số tỉ lệ (scale factor) để vẽ mỗi ô.</summary>
    public Vector2 Scale { get; set; }

    /// <summary>Lấy chiều rộng, tính bằng pixel, mà mỗi ô được vẽ.</summary>
    public float TileWidth => _tileset.TileWidth * Scale.X;

    /// <summary>Lấy chiều cao, tính bằng pixel, mà mỗi ô được vẽ.</summary>
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    /// <summary>Tạo một tilemap mới.</summary>
    /// <param name="tileset">Bộ ô (tileset) được sử dụng bởi tilemap này.</param>
    /// <param name="columns">Tổng số cột trong tilemap này.</param>
    /// <param name="rows">Tổng số hàng trong tilemap này.</param>
    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int[Count];
    }

    /// <summary>Thiết lập ô tại chỉ mục (index) đã cho trong tilemap này để sử dụng ô từ tileset tại id tileset đã chỉ định.</summary>
    /// <param name="index">Chỉ mục của ô trong tilemap này.</param>
    /// <param name="tilesetID">ID tileset của ô từ tileset để sử dụng.</param>
    public void SetTile(int index, int tilesetID)
    {
        _tiles[index] = tilesetID;
    }

    /// <summary>
    /// Thiết lập ô tại cột và hàng đã cho trong tilemap này để sử dụng ô từ tileset tại id tileset đã chỉ định.
    /// </summary>
    /// <param name="column">Cột của ô trong tilemap này.</param>
    /// <param name="row">Hàng của ô trong tilemap này.</param>
    /// <param name="tilesetID">ID tileset của ô từ tileset để sử dụng.</param>
    public void SetTile(int column, int row, int tilesetID)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetID);
    }

    /// <summary>Lấy vùng texture (texture region) của ô từ tilemap này tại chỉ mục đã chỉ định.</summary>
    /// <param name="index">Chỉ mục của ô trong tilemap này.</param>
    /// <returns>Vùng texture của ô từ tilemap này tại chỉ mục đã chỉ định.</returns>
    public TextureRegion GetTile(int index)
    {
        return _tileset.GetTile(_tiles[index]);
    }

    /// <summary>Lấy vùng texture của ô từ tilemap này tại cột và hàng đã chỉ định.</summary>
    /// <param name="column">Cột của ô trong tilemap này.</param>
    /// <param name="row">Hàng của ô trong tilemap này.</param>
    /// <returns>Vùng texture của ô từ tilemap này tại cột và hàng đã chỉ định.</returns>
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    /// <summary>Vẽ (Draws) tilemap này bằng cách sử dụng sprite batch đã cho.</summary>
    /// <param name="spriteBatch">Sprite batch được sử dụng để vẽ tilemap này.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int tilesetIndex = _tiles[i];
            if (tilesetIndex >= 0)
            {
                TextureRegion tile = _tileset.GetTile(tilesetIndex);

                int x = i % Columns;
                int y = i / Columns;

                Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
                tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
            }
        }
    }

    /// <summary>Tạo một tilemap mới dựa trên tệp cấu hình tilemap xml.</summary>
    /// <param name="content">Trình quản lý nội dung (content manager) được sử dụng để tải texture cho tileset.</param>
    /// <param name="filename">Đường dẫn đến tệp xml, tương đối so với thư mục gốc nội dung (content root directory).</param>
    /// <returns>Tilemap được tạo bởi phương thức này.</returns>
    public static Tilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // Phần tử <Tileset> chứa thông tin về tileset được sử dụng bởi tilemap.
                //
                // Ví dụ
                // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
                //
                // Thuộc tính region đại diện cho các thành phần x, y, width, và height
                // của ranh giới (boundary) cho vùng texture (texture region) bên trong
                // texture tại contentPath đã chỉ định.
                //
                // Các thuộc tính tileWidth và tileHeight chỉ định chiều rộng và
                // chiều cao của mỗi ô trong tileset.
                //
                // Giá trị contentPath là contentPath đến texture để
                // tải, texture này chứa tileset.
                XElement tilesetElement = root.Element("Tileset");

                string regionAttribute = tilesetElement.Attribute("region").Value;
                string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string contentPath = tilesetElement.Value;

                // Tải texture 2d tại content path
                Texture2D texture = content.Load<Texture2D>(contentPath);

                // Tạo vùng texture từ texture
                TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                // Tạo tileset bằng cách sử dụng vùng texture
                Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                // Phần tử <Tiles> chứa các dòng chuỗi, trong đó mỗi dòng
                // đại diện cho một hàng trong tilemap. Mỗi dòng là một chuỗi
                // được phân tách bằng dấu cách, trong đó mỗi phần tử đại diện cho một cột
                // trong hàng đó. Giá trị của cột là id của ô trong
                // tileset để vẽ cho vị trí đó.
                //
                // Ví dụ:
                // <Tiles>
                //      00 01 01 02
                //      03 04 04 05
                //      03 04 04 05
                //      06 07 07 08
                // </Tiles>
                XElement tilesElement = root.Element("Tiles");

                // Tách giá trị của dữ liệu ô thành các hàng bằng cách tách dựa trên ký tự xuống dòng
                string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                // Tách giá trị của hàng đầu tiên để xác định tổng số cột
                int columnCount = rows[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Length;

                // Tạo tilemap
                Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                // Xử lý từng hàng
                for (int row = 0; row < rows.Length; row++)
                {
                    // Tách hàng thành các cột riêng lẻ
                    string[] columns = rows[row].Trim().Split(",", StringSplitOptions.RemoveEmptyEntries);

                    // Xử lý từng cột của hàng hiện tại
                    for (int column = 0; column < columnCount; column++)
                    {
                        // Lấy chỉ mục tileset cho vị trí này
                        int tilesetIndex = int.Parse(columns[column]);

                        if (tilesetIndex >= 0)
                        {
                            // Lấy vùng texture của ô đó từ tileset
                            TextureRegion region = tileset.GetTile(tilesetIndex);
                        }
                        // Thêm vùng đó vào tilemap tại vị trí hàng và cột
                        tilemap.SetTile(column, row, tilesetIndex);
                    }
                }

                return tilemap;
            }
        }
    }

    public Rectangle GetWorldTileCollider(float worldX, float worldY)
    {
        // 1. Chuyển đổi World Coordinate sang Cột/Hàng
        int column = (int)(worldX / TileWidth);
        int row = (int)(worldY / TileHeight);

        // 2. Kiểm tra biên và lấy Tileset ID
        if (column < 0 || column >= Columns || row < 0 || row >= Rows)
        {
            return Rectangle.Empty;
        }
        int tilesetID = _tiles[row * Columns + column];

        // 3. Lấy Collider cục bộ (local) từ Tileset
        Rectangle localCollider = _tileset.GetTileCollider(tilesetID);
        if(localCollider == Rectangle.Empty)
            return Rectangle.Empty;

        // 4. CHUYỂN ĐỔI COLLIDER CỤC BỘ sang TỌA ĐỘ THẾ GIỚI

        // Vị trí (X, Y) của ô gạch trong thế giới
        int tileWorldX = (int)(column * TileWidth);
        int tileWorldY = (int)(row * TileHeight);

        // Collider trong thế giới = Tile World Position + Local Collider Offset
        return new Rectangle(
            tileWorldX + localCollider.X,
            tileWorldY + localCollider.Y,
            localCollider.Width,
            localCollider.Height
        );
    }
}
