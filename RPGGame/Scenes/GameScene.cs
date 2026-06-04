using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Camera;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using RPGGame.Data;
using RPGGame.GameObjects;
using RPGGame.UI;
using System;
using System.Collections.Generic;

namespace RPGGame.Scenes;

public class GameScene : Scene
{
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }
    private int _currentLevel;

    private Player _player;
    private PlayerData _data;
    private NPC _npc;
    private List<Bandit> _bandits;
    private List<Trap> _traps;
    private Boss _boss;
    private Item _item;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the map.
    private Rectangle _mapBounds;

    // The background theme song
    private Song _themeSong;

    // The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect;

    // The sound effect to play when the slime eats a bat.
    private SoundEffect _collectSoundEffect;

    // Tracks the players score.
    private int _score;

    private Camera2D _camera;
    private List<ParallaxLayer> _backgroundLayers;

    private GameSceneUI _ui;
    //private 

    private GameState _state;

    public GameScene(int levelIndex = 1, PlayerData data = null)
    {
        _currentLevel = levelIndex;
        _data = data;
    }

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // Start playing the background music.
        Core.Audio.PlaySong(_themeSong);

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        _mapBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        //_mapBounds.Inflate(-_tilemap.TileWidth, -_tilemap.TileHeight);
        _mapBounds = new Rectangle(0, 0, (int)(_tilemap.Columns * _tilemap.TileWidth), (int)(_tilemap.Rows * _tilemap.TileHeight));


        // Create any UI elements from the root element created in previous scenes.
        GumService.Default.Root.Children.Clear();

        InitializeUI();

        // Initialize a new game to be played.
        InitializeNewGame();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI element incase we came here
        // from a different scene.
        GumService.Default.Root.Children.Clear();

        // Create the game scene ui instance.
        _ui = new GameSceneUI(_player.MaxHealth);

        // Subscribe to the events from the game scene ui.
        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.ReplayButtonClick += OnReplayButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        // Change the game state back to playing.
        _state = GameState.Playing;
    }

    private void OnReplayButtonClicked(object sender, EventArgs args)
    {
        _data = PlayerData.CreateDefault();
        Core.ChangeScene(new GameScene(1, _data));
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        _data.Score -= 50;
        LoadContent();
        // Player has chosen to retry, so initialize a new game.
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to quit, so return back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

    private void InitializeNewGame()
    {
        // Calculate the position for the player, which will be at the center tile of the tile map.
        Vector2 playerPos = new Vector2(100, 500);
        //Vector2.Clamp(playerPos, Vector2.Zero, new Vector2(_mapBounds.Width, _mapBounds.Height));
        Vector2 itemPos = new Vector2(1300, 500);

        // Xóa hết quái của màn chơi cũ (nếu có)
        //_bandits.Clear();
        _npc.IsActive = false;
        _boss.IsActive = false;

        switch (_currentLevel)
        {
            case 1:
                playerPos = new Vector2(100, 500);
                Vector2 npcPos = new Vector2(125, 560);
                _npc.Initialize(npcPos);
                itemPos = new Vector2(1500, 596);
                break;
            case 2:
                playerPos = new Vector2(100, 500);
                itemPos = new Vector2(1200, 600);
                break;
            case 3:
                playerPos = new Vector2(100, 500);

                Vector2 bossPos = new Vector2(1000, 516);
                _boss.Initialize(bossPos);
                break;
        }
        // Initialize the player.
        _player.Initialize(playerPos);
        _player.CurrentHealth = _data.CurrentHealth;
        _player.ItemCount = _data.Flask;

        _item.Initialize(itemPos);

        // Reset the score.
        _score = _data.Score;

        // Set the game state to playing.
        _state = GameState.Playing;
    }

    public override void LoadContent()
    {
        // Load the bounce sound effect.
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        //SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create the player.
        _player = new Player(atlas, _bounceSoundEffect);
        _player.OnAttackDealt += CheckHit;
        _npc = new NPC(atlas, _player);

        // Create the bandit from the atlas.
        SpawnEnemies(atlas);

        CreateTrap(atlas);

        _boss = new Boss(atlas, new Vector2(500, 516), new Vector2(1500, 516), _player);

        Sprite item = atlas.CreateSprite("potion");
        _item = new Item(item);

        _camera = new Camera2D();
        _backgroundLayers = new List<ParallaxLayer>();

        // Helper function để đỡ viết code lặp lại
        void AddLayer(string path, float speedX)
        {
            var tex = Content.Load<Texture2D>(path);
            // Tốc độ Y = 0 để không bị trượt dọc (như đã bàn ở câu trước)
            _backgroundLayers.Add(new ParallaxLayer(tex, new Vector2(speedX, 0f)));
        }

        // Nạp theo thứ tự: Xa nhất (Speed thấp) -> Gần nhất (Speed cao)
        AddLayer("images/BACKGROUND", 0.0f);
        AddLayer("images/BUSH-BACKGROUND", 0.1f);
        AddLayer("images/WOODS-Fourth", 0.2f);
        AddLayer("images/WOODS-Third", 0.2f);
        AddLayer("images/WOODS-Second", 0.3f);
        AddLayer("images/VINES-Second", 0.3f);
        AddLayer("images/WOODS-First", 0.4f);

        // Create the tilemap from the XML configuration file.
        string mapName = $"images/level-{_currentLevel}.xml";
        try
        {
            _tilemap = Tilemap.FromFile(Content, mapName);
        }
        catch
        {
            // Nếu không tìm thấy map (ví dụ thắng game), quay về Title
            Console.WriteLine("Map not found, returning to Title.");
            Core.ChangeScene(new TitleScene());
            return;
        }
        //_tilemap.Scale = new Vector2(1.5f, 1.5f);

        switch (_currentLevel)
        {
            case 3:
                // Load the background theme music.
                _themeSong = Content.Load<Song>("audio/boss-theme");
                break;
            default:
                // Load the background theme music.
                _themeSong = Content.Load<Song>("audio/theme");
                break;
        }

        //// Create the bat.
        //_bat = new Bat(batAnimation, bounceSoundEffect);

        // Load the collect sound effect.
        //_collectSoundEffect = Content.Load<SoundEffect>("audio/collect");


        DebugRenderer.Initialize(Core.GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated
        _ui.Update(gameTime);
        _ui.UpdateHealth(_player.CurrentHealth);
        _ui.UpdateItemText(_player.ItemCount);
        _ui.UpdateCoinText(_score);
        //// If the game is paused, do not continue
        //if (_pausePanel.IsVisible)
        //{
        //    return;
        //}

        // If the game is in a game over state, immediately return back here.
        if (_state == GameState.GameOver)
        {
            return;
        }

        // If the pause button is pressed, toggle the pause state.
        if (GameController.Back())
        {
            TogglePause();
        }

        // At this point, if the game is paused, just return back early.
        if (_state == GameState.Paused)
        {
            return;
        }

        // Update the player.
        _player.Update(gameTime);
        if (_player.CurrentHealth <= 0)
            GameOver();

        _npc.Update(gameTime);

        _item.Update(gameTime);

        // Update the bandit.
        foreach (var bandit in _bandits)
        {
            if (bandit.IsAlive)
            {
                bandit.Update(gameTime);
                if (!bandit.IsAlive)
                    _score += 100;

            }
        }
        _bandits.RemoveAll(b => !b.IsAlive);

        foreach (var trap in _traps)
        {
            trap.Update(gameTime);
        }

        if (_boss.IsActive)
        {
            _boss.Update(gameTime);
            if(!_boss.IsActive)
                _score += 500;
        }

        Vector2 targetPosition = _player.WorldPos;
        // Tính toán biên giới hạn (Min và Max cho tâm Camera)
        // Camera không được sang trái quá một nửa màn hình
        float minX = Core.GraphicsDevice.Viewport.Width / 2f;
        // Camera không được sang phải quá (Độ rộng Map - một nửa màn hình)
        float maxX = _mapBounds.Width - (Core.GraphicsDevice.Viewport.Width / 2f);

        float minY = Core.GraphicsDevice.Viewport.Height / 2f;
        float maxY = _mapBounds.Height - (Core.GraphicsDevice.Viewport.Height / 2f);

        // Kẹp vị trí (Clamp)
        // Nếu Map nhỏ hơn màn hình thì giữ camera ở giữa map
        if (_mapBounds.Width < Core.GraphicsDevice.Viewport.Width)
        {
            targetPosition.X = _mapBounds.Width / 2f;
        }
        else
        {
            targetPosition.X = MathHelper.Clamp(targetPosition.X, minX, maxX);
        }

        if (_mapBounds.Height < Core.GraphicsDevice.Viewport.Height)
        {
            targetPosition.Y = _mapBounds.Height / 2f;
        }
        else
        {
            targetPosition.Y = MathHelper.Clamp(targetPosition.Y, minY, maxY);
        }
        // Gán vị trí đã tính toán vào Camera
        _camera.Position = targetPosition;

        // Perform collision checks.
        CollisionChecks();

    }

    private void SpawnEnemies(TextureAtlas atlas)
    {
        _bandits = new List<Bandit>();

        // Tạo một list tạm chứa các vị trí muốn spawn quái theo Level
        List<Vector2> enemyPositions = new List<Vector2>();

        // Cấu hình vị trí theo Level
        switch (_currentLevel)
        {
            case 1:
                enemyPositions.Add(new Vector2(1160, 625));
                enemyPositions.Add(new Vector2(824, 561));
                break;

            case 2:
                enemyPositions.Add(new Vector2(592, 400));
                enemyPositions.Add(new Vector2(824, 336));
                enemyPositions.Add(new Vector2(1224, 368));
                break;

            default: // Level test
                break;
        }

        // VÒNG LẶP TẠO QUÁI
        foreach (Vector2 pos in enemyPositions)
        {
            Vector2 leftBoundary = new Vector2(pos.X - 72, pos.Y);
            Vector2 rightBoundary = new Vector2(pos.X + 72, pos.Y);
            // Tạo instance (cần Atlas)
            Bandit b = new Bandit(atlas, leftBoundary, rightBoundary, _player);

            // Gọi Initialize để truyền vị trí
            b.Initialize(pos);

            // Thêm vào danh sách quản lý
            _bandits.Add(b);
        }
    }

    private void CreateTrap(TextureAtlas atlas)
    {
        _traps = new List<Trap>();
        List<Vector2> trapPositions = new List<Vector2>();

        // Cấu hình vị trí theo Level
        switch (_currentLevel)
        {
            case 1:
                break;

            case 2:
                trapPositions.Add(new Vector2(804, 590));
                trapPositions.Add(new Vector2(900, 590));
                trapPositions.Add(new Vector2(996, 590));
                break;
            case 3:
                trapPositions.Add(new Vector2(414, 648));
                trapPositions.Add(new Vector2(777, 584));
                trapPositions.Add(new Vector2(1139, 584));
                trapPositions.Add(new Vector2(1502, 648));
                break;

            default: // Level test
                break;
        }

        // VÒNG LẶP TẠO QUÁI
        foreach (Vector2 pos in trapPositions)
        {
            // Tạo instance (cần Atlas)
            Trap t = new Trap(atlas);

            // Gọi Initialize để truyền vị trí
            t.Initialize(pos);

            // 3. Thêm vào danh sách quản lý
            _traps.Add(t);
        }
    }

    private void CollisionChecks()
    {
        // Capture the current bounds of the player.
        Rectangle playerBounds = _player.Collider();

        _player.CheckTilemap(_tilemap);

        float rayLength = 2f; // Độ dài tia cực ngắn để check xem có sát đất không

        // Bắn 2 tia từ 2 góc dưới của Player
        Vector2 leftRayStart = new Vector2(playerBounds.Left + 2, playerBounds.Bottom);
        Vector2 rightRayStart = new Vector2(playerBounds.Right - 2, playerBounds.Bottom);

        // Nếu bất kỳ tia nào chạm vào Tile Solid, thì coi như đang đứng trên đất
        bool leftHit = Rectangle.Empty != _tilemap.GetWorldTileCollider(leftRayStart.X, leftRayStart.Y + rayLength);
        bool rightHit = Rectangle.Empty != _tilemap.GetWorldTileCollider(rightRayStart.X, rightRayStart.Y + rayLength);
        _player.GroundCheck = leftHit || rightHit;

        foreach (var trap in _traps)
        {
            if (playerBounds.Intersects(trap.Collider()))
            {
                _player.TakeDamage(1);
                break;
            }

        }
        _ui.HideSuggetion();

        if (playerBounds.Intersects(_item.Collider()) && !_item.IsPickedUp)
        {
            _ui.ShowSuggetion(_camera.WorldToScreen(_item.WorldPos) , "to pickup");
            _player.OnAction += PickUpItem;
        }

        if (playerBounds.Intersects(_npc.Collider()))
        {
            _ui.ShowSuggetion(_npc.WorldPos - new Vector2(_npc.Sprite.Width/2, _npc.Sprite.Height/2), "to talk");
            _player.OnAction += TalkToNPC;
        }
        else
        {
            _ui.HideSpeechBubble();
        }

        if (playerBounds.Right >= _mapBounds.Right - 10)
        {
            LoadNextLevel();
        }

        if (playerBounds.Bottom > _mapBounds.Bottom)
        {
            GameOver();
            return;
        }

    }

    private void TalkToNPC(object sender, EventArgs args)
    {
        if (_player.Collider().Intersects(_npc.Collider()))
            {
            _ui.HideSuggetion();
            _ui.ShowSpeechBubble(_npc.WorldPos);
        }
    }
    private void PickUpItem(object sender, EventArgs args)
    {
        if (_player.Collider().Intersects(_item.Collider()) && !_item.IsPickedUp)
        {
            _player.ItemCount++;
            _item.OnTriggerEnter(_player.Collider());
        }
    }
    private void CheckHit(Rectangle attackBounds)
    {
        // Duyệt qua danh sách quái mà GameScreen đang quản lý
        foreach (var bandit in _bandits)
        {
            if (attackBounds.Intersects(bandit.Collider()))
            {
                bandit.TakeDamage(1);
                // Play the bounce sound effect.
                Core.Audio.PlaySoundEffect(_bounceSoundEffect);
            }
        }
        if (_boss.IsActive && attackBounds.Intersects(_boss.Collider()))
        {
            _boss.TakeDamage(1);
            // Play the bounce sound effect.
            Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        }
    }

    private void LoadNextLevel()
    {
        if (_currentLevel < 3)
        {
            if (_bandits.Count > 0)
            {
                _ui.ShowNoticePanel();
                _player.UpdatePosition(new Vector2(-50, 0));
            }
            else
            {
                // Tăng index lên 1 và tạo Scene mới
                int nextLevel = _currentLevel + 1;
                _data.CurrentHealth = _player.CurrentHealth;
                _data.Flask = _player.ItemCount;
                _data.Score = _score;
                // Có thể thêm hiệu ứng âm thanh ở đây

                // Chuyển Scene
                Core.ChangeScene(new GameScene(nextLevel, _data));
            }
        }
        else
        {
            _state = GameState.Paused;
            _ui.ShowVictoryPanel();
        }
    }

    private void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            // We're now unpausing the game, so hide the pause panel.
            _ui.HidePausePanel();

            // And set the state back to playing.
            _state = GameState.Playing;
        }
        else
        {
            // We're now pausing the game, so show the pause panel.
            _ui.ShowPausePanel();

            // And set the state to paused.
            _state = GameState.Paused;
        }
    }

    private void GameOver()
    {
        // Show the game over panel.
        _ui.ShowGameOverPanel();

        // Set the game state to game over.
        _state = GameState.GameOver;
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.Black);
        Rectangle destRect = new Rectangle(-Core.GraphicsDevice.Viewport.Width / 2, -Core.GraphicsDevice.Viewport.Height / 2, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height);

        Core.SpriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(Vector2.Zero, Core.GraphicsDevice.Viewport), samplerState: SamplerState.LinearWrap);
        foreach (var layer in _backgroundLayers)
        {
            Core.SpriteBatch.Draw(layer.Texture, destRect, new Rectangle((int)(_camera.Position.X * layer.Speed.X), (int)(_camera.Position.Y * layer.Speed.Y), layer.Texture.Width, layer.Texture.Height), Color.White);
        }

        Core.SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(Core.GraphicsDevice.Viewport));

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        switch (_currentLevel)
        {
            case 1:
                _npc.Draw();
                _item.Draw();
                break;
            case 2:
                _item.Draw();
                break;
            case 3:
                _boss.Draw();
                break;
            default:
                break;
        }
        // Draw the bandit.
        foreach (var bandit in _bandits)
        {
            bandit.Draw();
        }

        foreach (var trap in _traps)
            trap.Draw();
        // Draw the player.
        _player.Draw();

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        //// Draw the Gum UI
        //GumService.Default.Draw();

        // Draw the UI.
        _ui.Draw();
    }
}
