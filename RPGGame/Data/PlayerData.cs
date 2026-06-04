using System;

namespace RPGGame.Data
{
    [Serializable] // Lưu game ra file (Save/Load)
    public class PlayerData
    {
        public int LevelIndex { get; set; } = 1;
        public int CurrentHealth { get; set; } = 5;
        public int Score { get; set; } = 0;
        public int Flask { get; set; } = 0;

        // Helper: Tạo dữ liệu mặc định cho New Game
        public static PlayerData CreateDefault()
        {
            return new PlayerData
            {
                LevelIndex = 1,
                CurrentHealth = 5,
                Score = 0,
                Flask = 0,
            };
        }
    }
}
