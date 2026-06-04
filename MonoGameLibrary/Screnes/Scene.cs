using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Scenes;

public abstract class Scene : IDisposable
{
    /// <summary>
    /// Lấy ContentManager được sử dụng để tải các tài sản (asset) dành riêng cho scene này.
    /// Các tài sản được tải thông qua ContentManager này sẽ tự động được giải phóng (unload) khi scene này kết thúc.
    /// </summary>
    protected ContentManager Content { get; }

    /// <summary>Lấy một giá trị cho biết liệu scene này đã được giải phóng (disposed) hay chưa.</summary>
    public bool IsDisposed { get; private set; }

    /// <summary>Tạo một instance scene mới.</summary>
    public Scene()
    {
        // Tạo một trình quản lý nội dung cho scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    // Finalizer (hàm kết thúc), được gọi khi đối tượng được bộ thu gom rác (garbage collector) dọn dẹp.
    ~Scene() => Dispose(false);

    /// <summary>
    /// Khởi tạo (initializes) scene.
    /// Khi override (ghi đè) phương thức này trong lớp dẫn xuất, hãy đảm bảo rằng base.Initialize() vẫn được gọi vì đây là lúc LoadContent được gọi.
    /// </summary>
    public virtual void Initialize()
    {
        LoadContent();
    }

    /// <summary>Ghi đè phương thức này để cung cấp logic tải nội dung (content) cho scene.</summary>
    public virtual void LoadContent() { }

    /// <summary>Giải phóng nội dung dành riêng cho scene.</summary>
    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    /// <summary>Cập nhật (updates) scene này.</summary>
    /// <param name="gameTime">Một bản chụp nhanh (snapshot) các giá trị thời gian cho frame (khung hình) hiện tại.</param>
    public virtual void Update(GameTime gameTime) { }


    /// <summary>Vẽ (draws) scene này.</summary>
    /// <param name="gameTime">Một bản chụp nhanh các giá trị thời gian cho frame hiện tại.</param>
    public virtual void Draw(GameTime gameTime) { }


    /// <summary>Giải phóng (disposes) scene này.</summary>
    public void Dispose()
    {
        Dispose(true);
        // Ngăn chặn Finalizer được gọi lần nữa, vì chúng ta đã giải phóng tài nguyên.
        GC.SuppressFinalize(this);
    }


    /// <summary>Giải phóng (disposes) scene này.</summary>
    /// <param name="disposing">
    /// Cho biết liệu các tài nguyên được quản lý (managed resources) có nên được giải phóng hay không.
    /// Giá trị này chỉ đúng khi được gọi từ phương thức Dispose chính.
    /// Khi được gọi từ finalizer, giá trị này sẽ là false.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
    }

}
