# RPGGame - Wandering Knight

## Tổng quan

**Wandering Knight** là một game **2D Action-Platformer** (side-scrolling) được phát triển bằng **C#** trên nền tảng **MonoGame** (DesktopGL). Game kể về một hiệp sĩ lang thang phiêu lưu qua 3 level, chiến đấu với kẻ thù (Bandit), vượt qua bẫy (Trap), thu thập vật phẩm (Potion/Flask), và đối đầu với Boss ở level cuối.

---

## Tech Stack

| Thành phần       | Công nghệ                          | Version     |
|------------------|-------------------------------------|-------------|
| **Ngôn ngữ**     | C#                                  | —           |
| **Framework**    | .NET                                | 8.0         |
| **Game Engine**  | MonoGame (DesktopGL)                | 3.8.x       |
| **UI Framework** | Gum (MonoGame plugin)               | 2025.8.3.3  |
| **IDE**          | Visual Studio 2022                  | 17.x        |
| **Build**        | MSBuild + MonoGame Content Pipeline | —           |
| **Platform**     | Windows (DesktopGL - cross-platform)| —           |

### NuGet Packages
- `MonoGame.Framework.DesktopGL` 3.8.*
- `MonoGame.Content.Builder.Task` 3.8.*
- `Gum.MonoGame` 2025.8.3.3

---

## Cấu trúc Solution

```
RPGGame.sln
├── RPGGame/                    # Project chính (WinExe) - Game code
│   ├── RPGGame.csproj
│   ├── Program.cs              # Entry point
│   ├── RPGGame.cs              # Main game class (kế thừa Core)
│   ├── GameController.cs       # Input abstraction layer
│   │
│   ├── GameObjects/            # Các entity trong game
│   │   ├── GameObject.cs       # Base class trừu tượng cho mọi entity
│   │   ├── Player.cs           # Nhân vật chính - Player (FSM)
│   │   ├── Bandit.cs           # Enemy thường - AI tuần tra/đuổi/tấn công
│   │   ├── Boss.cs             # Boss cuối - AI phức tạp hơn
│   │   ├── NPC.cs              # NPC tương tác (hội thoại)
│   │   ├── Item.cs             # Vật phẩm nhặt được (Potion)
│   │   ├── Trap.cs             # Bẫy gây sát thương
│   │   └── Debugger.cs         # Debug renderer (vẽ hitbox)
│   │
│   ├── Scenes/                 # Các scene/màn hình game
│   │   ├── TitleScene.cs       # Màn hình chính (Start/Options/Quit)
│   │   └── GameScene.cs        # Scene gameplay chính (quản lý level)
│   │
│   ├── UI/                     # Hệ thống giao diện (dùng Gum)
│   │   ├── GameSceneUI.cs      # UI gameplay (HP, coin, pause, game over, victory)
│   │   ├── AnimatedButton.cs   # Custom button với animation focus
│   │   ├── AnimatedComboBox.cs # Custom combo box
│   │   ├── AnimatedListBoxItem.cs # Custom list box item
│   │   ├── ContextualUI.cs     # UI tương tác theo ngữ cảnh
│   │   ├── OptionsSlider.cs    # Slider cho Options (Volume)
│   │   ├── ParallaxLayer.cs    # Struct cho lớp parallax background
│   │   ├── SpeechBubble.cs     # Bong bóng hội thoại NPC
│   │   └── TitlePanel.cs       # Panel tiêu đề
│   │
│   ├── Data/                   # Data model
│   │   └── PlayerData.cs       # Dữ liệu player giữa các level (HP, Score, Flask)
│   │
│   └── Content/                # Assets (MonoGame Content Pipeline)
│       ├── Content.mgcb        # File cấu hình Content Pipeline
│       ├── audio/              # Âm thanh
│       │   ├── theme.mp3       # Nhạc nền gameplay
│       │   ├── boss-theme.mp3  # Nhạc nền boss fight
│       │   └── bounce.wav      # SFX đánh trúng
│       ├── fonts/              # Font chữ
│       │   ├── AoboshiOne-Regular.spritefont
│       │   ├── AoboshiOne-Regular_5x.spritefont
│       │   ├── Aoboshi_One.fnt # BitmapFont cho Gum UI
│       │   └── 04b_30.fnt      # BitmapFont pixel cho HUD
│       └── images/             # Hình ảnh
│           ├── atlas.png               # Sprite atlas chính (tất cả sprites)
│           ├── atlas-definition.xml    # Định nghĩa các region/animation trong atlas
│           ├── tilemap-definition.xml  # Định nghĩa tileset
│           ├── level-1.xml             # Dữ liệu tilemap Level 1
│           ├── level-2.xml             # Dữ liệu tilemap Level 2
│           ├── level-3.xml             # Dữ liệu tilemap Level 3
│           ├── background-title.png    # Background title screen
│           ├── BACKGROUND.png          # Parallax layer - xa nhất
│           ├── BUSH-BACKGROUND.png     # Parallax layer
│           ├── WOODS-Fourth.png        # Parallax layer
│           ├── WOODS-Third.png         # Parallax layer
│           ├── WOODS-Second.png        # Parallax layer
│           ├── VINES-Second.png        # Parallax layer
│           └── WOODS-First.png         # Parallax layer - gần nhất
│
└── MonoGameLibrary/            # Thư viện engine tự viết (Class Library)
    ├── MonoGameLibrary.csproj
    ├── Core.cs                 # Lớp Game chính (singleton) - quản lý vòng lặp game
    ├── Circle.cs               # Hình tròn (collision helper)
    │
    ├── Screnes/                # (typo: Scenes) Scene management
    │   └── Scene.cs            # Base class cho tất cả scenes (IDisposable)
    │
    ├── Graphics/               # Hệ thống đồ họa
    │   ├── Sprite.cs           # Sprite rendering
    │   ├── TextureRegion.cs    # Vùng texture (sub-texture từ atlas)
    │   ├── TextureAtlas.cs     # Quản lý sprite atlas (load từ XML)
    │   ├── Animation.cs        # Dữ liệu animation (danh sách frames)
    │   ├── Animator.cs         # Generic state machine cho animation
    │   ├── Tilemap.cs          # Tilemap rendering + collision
    │   └── Tileset.cs          # Tileset (chia texture thành tiles)
    │
    ├── Input/                  # Hệ thống input
    │   ├── InputManager.cs     # Quản lý tất cả input devices
    │   ├── KeyboardInfo.cs     # Trạng thái bàn phím
    │   ├── MouseInfo.cs        # Trạng thái chuột
    │   ├── MouseButton.cs      # Enum mouse button
    │   └── GamePadInfo.cs      # Trạng thái gamepad
    │
    ├── Camera/                 # Hệ thống camera
    │   └── Camera2D.cs         # Camera 2D với position, zoom, parallax support
    │
    ├── Audio/                  # Hệ thống âm thanh
    │   └── AudioController.cs  # Quản lý Song và SoundEffect
    │
    └── Collision/              # Hệ thống va chạm
        ├── Collider.cs         # Base collider
        └── RectangleCollider.cs # Rectangle-based collider
```

---

## Kiến trúc & Design Patterns

### 1. Scene Management (State Pattern)
- `Core` (MonoGameLibrary) quản lý vòng đời Scene: `ChangeScene()` → `TransitionScene()`
- Mỗi Scene có riêng `ContentManager` để quản lý tài nguyên scope-level
- 2 Scene hiện tại: `TitleScene` (menu) và `GameScene` (gameplay)

### 2. Game Object Hierarchy (Template Method Pattern)
```
GameObject (abstract)
├── Player       — FSM 8 states, physics, combat
├── Bandit       — Enemy AI (Patrol/Chase/Attack/Hurt/Die)
├── Boss         — Boss AI (Idle/Chase/Flee/Attack/Hurt/Die)
├── NPC          — Static, tương tác hội thoại
├── Item         — Vật phẩm nhặt được
└── Trap         — Bẫy tĩnh gây sát thương
```
- `GameObject.Update()` gọi `HandleStateLogic()` — phương thức abstract bắt buộc con ghi đè
- Mỗi entity có `Collider()` trả về `Rectangle` cho va chạm

### 3. Finite State Machine (FSM)
Mỗi entity có bộ states riêng, quản lý bằng `Animator<TState>`:

**Player States**: `Idle → Run → Jump → Fall → Dash → Attack → Hurt → Die`
- Xử lý input từ `GameController` (abstraction layer)
- Physics: Gravity (980f), Jump (-400f), Speed (200f)
- I-Frame khi bị thương (1 giây)

**Bandit States**: `Patrol → Chase → Attack → Hurt → Die`
- AI tự động tuần tra giữa 2 điểm
- Khi player vào bán kính 200px → Chase; 50px → Attack
- Chỉ chase khi player cùng tầng (kiểm tra Y)

**Boss States**: `Idle → Chase → Flee → Attack → Hurt → Die`
- AI phức tạp hơn: có Flee state (chạy xa khi player đến quá gần)
- Attack radius lớn (300px), Flee radius (175px)
- Scale 3x, HP = 5

### 4. Animation System (Generic State Machine)
```
Animator<T> where T : Enum
├── AddAnimation(state, animation)    — Đăng ký animation cho state
├── SetState(newState)                — Chuyển state + reset frame
├── Update(gameTime)                  — Cập nhật frame theo thời gian
└── Properties: CurrentRegion, CurrentFrame, AnimationFinished
```
- Animation data load từ `TextureAtlas` (XML definition → sprite atlas PNG)
- Support animation queue (`ForgeFinish` flag)

### 5. Input Abstraction Layer
`GameController` (static class) ánh xạ input vật lý → game actions:
| Action    | Keyboard           | Gamepad              | Mouse        |
|-----------|--------------------|-----------------------|--------------|
| MoveUp    | Up/W/Space         | DPad Up/LeftStick Up  | —            |
| MoveDown  | Down/S             | DPad Down/LeftStick   | —            |
| MoveLeft  | Left/A             | DPad Left/LeftStick   | —            |
| MoveRight | Right/D            | DPad Right/LeftStick  | —            |
| Jump      | Space              | DPad Up               | —            |
| Attack    | J                  | X Button              | Left Click   |
| Dash      | LeftShift/L        | A Button              | —            |
| Action    | E                  | A Button              | —            |
| Use Item  | F                  | B Button              | —            |
| Pause     | Escape             | Start                 | —            |

### 6. Event-Driven Communication
- `Player.OnAttackDealt` → `GameScene.CheckHit()` (kiểm tra hitbox vs enemies)
- `Player.OnAction` → `GameScene.PickUpItem()` / `GameScene.TalkToNPC()`
- `GameSceneUI` events → `GameScene` handlers (Resume/Retry/Replay/Quit)

---

## Luồng hoạt động chính

### Khởi động Game
```
Program.cs → new RPGGame() → Core(title, 1280, 720, false)
    → Initialize()
        → InitializeGum()         // Khởi tạo Gum UI Framework
        → UpdateUIScale()         // Scale UI theo resolution
        → ChangeScene(TitleScene) // Vào Title Screen
```

### Title Screen Flow
```
TitleScene
├── Start → tạo PlayerData mặc định → ChangeScene(GameScene(level=1))
├── Options → Toggle Options Panel (Music/SFX sliders)
└── Quit → Core.Exit()
```

### Gameplay Loop (GameScene)
```
GameScene(levelIndex, playerData)
    │
    ├── LoadContent()
    │   ├── Load TextureAtlas (atlas-definition.xml)
    │   ├── Create Player, NPC, Bandits, Boss, Items, Traps
    │   ├── Load Parallax Background (7 layers)
    │   ├── Load Tilemap (level-N.xml)
    │   └── Load Audio (theme/boss-theme)
    │
    ├── InitializeNewGame()
    │   ├── Set player position theo level
    │   ├── Spawn enemies/NPC/Boss theo level config
    │   └── Restore player state từ PlayerData
    │
    └── Update Loop (mỗi frame)
        ├── UI Update (HP bar, coin, items)
        ├── Check Pause (Escape)
        ├── Player Update (FSM + Physics + Input)
        ├── Enemies Update (AI)
        ├── Boss Update (AI)
        ├── Camera Follow Player (clamped to map bounds)
        └── CollisionChecks()
            ├── Player vs Tilemap (ground + wall)
            ├── Ground raycast (2 tia từ chân player)
            ├── Player vs Traps → TakeDamage
            ├── Player vs Item → Show suggestion (E key)
            ├── Player vs NPC → Show suggestion → SpeechBubble
            ├── Player vs Map Right Edge → LoadNextLevel
            └── Player falls → GameOver
```

### Level Progression
```
Level 1: Tutorial
  - NPC (hướng dẫn controls)
  - 2 Bandits
  - 1 Potion
  - Phải tiêu diệt hết quái mới qua level

Level 2: Intermediate
  - 3 Bandits
  - 3 Traps
  - 1 Potion

Level 3: Boss Fight
  - Boss (HP=5, 3x scale)
  - 4 Traps
  - Nhạc nền boss-theme
  - Thắng boss → Victory Panel (hiện score)
```

### Combat System
```
Player Attack:
  1. Nhấn J / Left Click / Gamepad X
  2. Chuyển sang Attack state
  3. Frame thứ 5 → Tạo hitbox (50x40px trước mặt)
  4. OnAttackDealt event → GameScene.CheckHit()
  5. Kiểm tra hitbox vs từng enemy/boss collider
  6. Trúng → enemy.TakeDamage(1)

Enemy/Boss Attack:
  1. AI chuyển sang Attack state khi player vào attack radius
  2. Frame thứ 4 → Tạo hitbox trước mặt
  3. Kiểm tra hitbox vs player collider
  4. Trúng → player.TakeDamage(1)
  5. Player có I-Frame 1 giây
```

### Scoring System
| Hành động          | Điểm |
|-------------------|------|
| Kill Bandit       | +100 |
| Kill Boss         | +500 |
| Retry (Game Over) | -50  |

### Player Data (giữ giữa levels)
```csharp
PlayerData {
    LevelIndex     = 1    // Level hiện tại
    CurrentHealth  = 5    // Máu hiện tại (max 5)
    Score          = 0    // Điểm tích lũy
    Flask          = 0    // Số potion đang giữ
}
```

---

## Hệ thống Rendering

### Parallax Background (7 layers)
```
Layer 0: BACKGROUND.png       (speed: 0.0 - static)
Layer 1: BUSH-BACKGROUND.png  (speed: 0.1)
Layer 2: WOODS-Fourth.png     (speed: 0.2)
Layer 3: WOODS-Third.png      (speed: 0.2)
Layer 4: WOODS-Second.png     (speed: 0.3)
Layer 5: VINES-Second.png     (speed: 0.3)
Layer 6: WOODS-First.png      (speed: 0.4 - gần nhất)
```
→ Vẽ trước tilemap, dùng SamplerState.LinearWrap

### Draw Order
```
1. Clear Black
2. Parallax Background (separate SpriteBatch với camera zero-parallax)
3. SpriteBatch Begin (với Camera2D transform matrix)
   a. Tilemap
   b. NPC / Item / Boss (theo level)
   c. Bandits
   d. Traps
   e. Player
4. SpriteBatch End
5. Gum UI Draw (overlay, không bị camera transform)
```

### Camera System
- `Camera2D` follow player với clamping vào map bounds
- Không cho camera ra ngoài biên map (min/max X, Y)
- Support parallax factor trong `GetViewMatrix()`
- `WorldToScreen()` để chuyển đổi tọa độ (dùng cho UI suggestion)

---

## Hệ thống UI (Gum Framework)

### Title Screen UI
- Title text: "Wandering" + "Knight" (SpriteFont 5x)
- Buttons: Start / Options / Quit (`AnimatedButton`)
- Options Panel: Music Slider + SFX Slider + Back button

### In-Game HUD (GameSceneUI)
- **Heart Bar**: Icon tim đỏ/vỡ (sprite từ atlas), tối đa 5
- **Coin Counter**: Icon coin + text format 6 chữ số
- **Flask Counter**: Icon flask + số lượng
- **Context Suggestion**: Icon phím E + text hướng dẫn (hiện khi gần NPC/Item)
- **Speech Bubble**: NineSlice background + text (hội thoại NPC)

### Overlay Panels
- **Pause Panel**: Resume + Quit buttons
- **Notice Panel**: "You must defeat all enemies" (khi cố qua level mà còn quái)
- **Game Over Panel**: Retry + Quit buttons
- **Victory Panel**: Score display + Replay + Quit buttons

---

## Content Pipeline

### Sprite Atlas
- Tất cả sprite gộp vào `atlas.png` (163KB)
- Định nghĩa region và animation trong `atlas-definition.xml`
- `TextureAtlas.FromFile()` parse XML → tạo `TextureRegion` và `Animation`

### Atlas Regions chính
| Region Name       | Mô tả                    |
|-------------------|---------------------------|
| idle, run, jump, fall, dash, attack, hurt | Player animations |
| bandit-run, bandit-attack, bandit-hurt, bandit-die | Bandit animations |
| boss-idle, boss-walk, boss-attack, boss-hurt, boss-die | Boss animations |
| NPC-idle           | NPC animation             |
| heart, broken-heart | HP icons                 |
| coin, flask, potion | Item icons/sprites       |
| trap               | Trap sprite               |
| panel-background   | UI panel NineSlice        |
| focused-button     | Button focused state      |
| speech-bubble      | Speech bubble NineSlice   |
| e-key              | UI key prompt sprite      |

### Tilemap
- Dữ liệu level trong XML (`level-1.xml`, `level-2.xml`, `level-3.xml`)
- Tileset từ `tilemap-definition.xml` → chia atlas thành tiles
- Mỗi tile có collision data optional
- `Tilemap.GetWorldTileCollider()` kiểm tra collision tại tọa độ world

---

## Collision System

### Tilemap Collision
- `Player.CheckTilemap()`: Raycast nhiều điểm quanh player collider
- Kiểm tra va chạm ngang (2 điểm: top, bottom) → đẩy lùi
- Kiểm tra va chạm dọc (2 điểm: left, right) → đặt lên sàn
- Ground Check: 2 tia ngắn (2px) từ 2 góc dưới chân player

### Entity Collision
- Dùng `Rectangle.Intersects()` đơn giản
- Hitbox attack tạo tạm thời tại frame cụ thể
- Player có I-Frame (bất tử tạm thời) sau khi bị thương

---

## Cách Build & Run

```bash
# Restore dependencies
dotnet restore RPGGame.sln

# Build
dotnet build RPGGame.sln

# Run
dotnet run --project RPGGame/RPGGame.csproj
```

### Resolution mặc định: **1280 x 720**, windowed mode

---

## Lưu ý quan trọng cho AI Agent

1. **Namespace chính**: `RPGGame` (game code), `MonoGameLibrary` (engine)
2. **Typo**: Thư mục `MonoGameLibrary/Screnes/` (đúng ra là "Scenes") - KHÔNG đổi tên vì sẽ ảnh hưởng references
3. **Comment tiếng Việt**: Code comment chủ yếu bằng tiếng Việt
4. **Gum UI**: UI không dùng XAML hay scene graph, tạo programmatically bằng C# code
5. **No Save/Load**: `PlayerData` có attribute `[Serializable]` nhưng chưa implement save/load
6. **No Dependency Injection**: Tất cả dependencies truyền trực tiếp qua constructor
7. **Static Core**: `Core` là singleton, truy cập global qua `Core.Instance`, `Core.Content`, `Core.SpriteBatch`, etc.
8. **Content Pipeline**: Assets phải được đăng ký trong `Content.mgcb` để build
9. **Scale**: Player sprite scale 2x, Boss scale 3x, Item scale 1.5x, Trap scale 1.5x
10. **Hardcoded positions**: Vị trí spawn enemies/items được hardcode trong `GameScene.SpawnEnemies()` và `InitializeNewGame()`
