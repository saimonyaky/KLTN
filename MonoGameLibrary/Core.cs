using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using System;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core s_instance;

    /// <summary>Lấy một tham chiếu (reference) đến instance (thể hiện) của Core.</summary>
    public static Core Instance => s_instance;

    // Scene (cảnh) hiện đang hoạt động.
    private static Scene s_activeScene;

    // Scene tiếp theo để chuyển sang, nếu có
    private static Scene s_nextScene;

    /// <summary>Lấy trình quản lý thiết bị đồ họa (graphics device manager) để kiểm soát việc trình bày đồ họa (presentation of graphics).</summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>Lấy thiết bị đồ họa (graphics device) được sử dụng để tạo các tài nguyên đồ họa và thực hiện việc render cơ bản (primitive rendering).</summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>Lấy sprite batch được sử dụng cho tất cả các thao tác render 2D.</summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>Lấy trình quản lý nội dung (content manager) được sử dụng để tải các tài sản (assets) toàn cục.</summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>Lấy một tham chiếu đến hệ thống quản lý input (đầu vào).</summary>
    public static InputManager Input { get; private set; }

    /// <summary>Lấy hoặc Thiết lập một giá trị cho biết liệu game có nên thoát khi nhấn phím Esc trên bàn phím hay không.</summary>
    public static bool ExitOnEscape { get; set; }

    /// <summary>Lấy một tham chiếu đến hệ thống điều khiển âm thanh.</summary>
    public static AudioController Audio { get; private set; }

    /// <summary>Tạo một instance Core mới.</summary>
    /// <param name="title">Tiêu đề (title) để hiển thị trên thanh tiêu đề của cửa sổ game.</param>
    /// <param name="width">Chiều rộng ban đầu, tính bằng pixel, của cửa sổ game.</param>
    /// <param name="height">Chiều cao ban đầu, tính bằng pixel, của cửa sổ game.</param>
    /// <param name="fullScreen">Cho biết liệu trò chơi có nên khởi động ở chế độ toàn màn hình (fullscreen) hay không.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Đảm bảo rằng không có nhiều hơn một Core được tạo ra.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Lưu trữ tham chiếu đến engine để truy cập thành viên toàn cục.
        s_instance = this;

        // Tạo một trình quản lý thiết bị đồ họa mới.
        Graphics = new GraphicsDeviceManager(this);

        // Thiết lập các giá trị mặc định cho đồ họa.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Áp dụng các thay đổi về trình bày đồ họa.
        Graphics.ApplyChanges();

        // Thiết lập tiêu đề cửa sổ.
        Window.Title = title;

        // Đặt trình quản lý nội dung của core thành tham chiếu đến trình quản lý nội dung của lớp Game cơ sở.
        Content = base.Content;

        // Thiết lập thư mục gốc (root directory) cho nội dung (assets).
        Content.RootDirectory = "Content";

        // Chuột hiển thị theo mặc định.
        IsMouseVisible = true;

        // Thoát bằng nút escape theo mặc định
        ExitOnEscape = true;

        // This allows you to resize:
        //Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Đặt thiết bị đồ họa của core thành tham chiếu đến thiết bị đồ họa của lớp Game cơ sở.
        GraphicsDevice = base.GraphicsDevice;

        // Tạo instance của sprite batch.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Tạo một trình quản lý input mới.
        Input = new InputManager();

        // Tạo một trình điều khiển âm thanh mới.
        Audio = new AudioController();
    }

    protected override void UnloadContent()
    {
        // Giải phóng (Dispose) trình điều khiển âm thanh.
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Cập nhật trình quản lý input.
        Input.Update(gameTime);

        // Cập nhật trình điều khiển âm thanh.
        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // Nếu có một scene tiếp theo đang chờ để được chuyển sang, thì thực hiện chuyển đổi (transition) sang scene đó.
        if (s_nextScene != null)
        {
            TransitionScene();
        }

        // Nếu có một scene đang hoạt động, hãy cập nhật nó.
        if (s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Nếu có một scene đang hoạt động, hãy vẽ (draw) nó.
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next)
    {
        // Chỉ thiết lập giá trị scene tiếp theo nếu nó không phải là instance giống như scene đang hoạt động.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        // Nếu có một scene đang hoạt động, hãy giải phóng (dispose) nó.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Buộc bộ thu gom rác (garbage collector) thu thập để đảm bảo bộ nhớ được giải phóng.
        GC.Collect();

        // Thay đổi scene đang hoạt động hiện tại thành scene mới.
        s_activeScene = s_nextScene;

        // Đặt giá trị scene tiếp theo thành null để nó không kích hoạt việc thay đổi lặp đi lặp lại.
        s_nextScene = null;

        // Nếu scene đang hoạt động hiện tại không phải là null, hãy khởi tạo (initialize) nó.
        // Hãy nhớ rằng, giống như với Game, lời gọi Initialize cũng gọi Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }
}
