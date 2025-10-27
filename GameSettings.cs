using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace SnakeGameAI
{
    /// <summary>
    /// 游戏设置类，管理游戏配置和用户偏好
    /// </summary>
    public class GameSettings
    {
        private static GameSettings? instance;
        private static readonly object lockObject = new object();
        private const string SettingsFileName = "game_settings.json";

        /// <summary>
        /// 单例实例
        /// </summary>
        public static GameSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = LoadSettings();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 游戏速度（毫秒）
        /// </summary>
        public int GameSpeed { get; set; } = 200;

        /// <summary>
        /// 网格宽度
        /// </summary>
        public int GridWidth { get; set; } = 30;

        /// <summary>
        /// 网格高度
        /// </summary>
        public int GridHeight { get; set; } = 20;

        /// <summary>
        /// 单元格大小
        /// </summary>
        public int CellSize { get; set; } = 20;

        /// <summary>
        /// 蛇头颜色
        /// </summary>
        public Color SnakeHeadColor { get; set; } = Color.DarkGreen;

        /// <summary>
        /// 蛇身颜色
        /// </summary>
        public Color SnakeBodyColor { get; set; } = Color.Green;

        /// <summary>
        /// 食物颜色
        /// </summary>
        public Color FoodColor { get; set; } = Color.Red;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Black;

        /// <summary>
        /// 是否启用音效
        /// </summary>
        public bool SoundEnabled { get; set; } = true;

        /// <summary>
        /// 是否启用AI助手
        /// </summary>
        public bool AIAssistantEnabled { get; set; } = true;

        /// <summary>
        /// 最高分数
        /// </summary>
        public int HighScore { get; set; } = 0;

        /// <summary>
        /// 玩家姓名
        /// </summary>
        public string PlayerName { get; set; } = "Player";

        /// <summary>
        /// AI API密钥
        /// </summary>
        public string AIApiKey { get; set; } = "";

        /// <summary>
        /// 加载设置
        /// </summary>
        private static GameSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    string json = File.ReadAllText(SettingsFileName);
                    var settings = JsonConvert.DeserializeObject<GameSettings>(json);
                    return settings ?? new GameSettings();
                }
            }
            catch (Exception ex)
            {
                // 如果加载失败，使用默认设置
                Console.WriteLine($"加载设置失败: {ex.Message}");
            }
            
            return new GameSettings();
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 重置为默认设置
        /// </summary>
        public void ResetToDefaults()
        {
            GameSpeed = 200;
            GridWidth = 30;
            GridHeight = 20;
            CellSize = 20;
            SnakeHeadColor = Color.DarkGreen;
            SnakeBodyColor = Color.Green;
            FoodColor = Color.Red;
            BackgroundColor = Color.Black;
            SoundEnabled = true;
            AIAssistantEnabled = true;
            PlayerName = "Player";
            AIApiKey = "";
            // 注意：不重置最高分数
        }

        /// <summary>
        /// 更新最高分数
        /// </summary>
        public void UpdateHighScore(int score)
        {
            if (score > HighScore)
            {
                HighScore = score;
                SaveSettings();
            }
        }
    }

    /// <summary>
    /// 游戏统计类
    /// </summary>
    public class GameStatistics
    {
        private static GameStatistics? instance;
        private static readonly object lockObject = new object();
        private const string StatsFileName = "game_stats.json";

        /// <summary>
        /// 单例实例
        /// </summary>
        public static GameStatistics Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = LoadStatistics();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 总游戏次数
        /// </summary>
        public int TotalGames { get; set; } = 0;

        /// <summary>
        /// 总游戏时间（秒）
        /// </summary>
        public int TotalPlayTime { get; set; } = 0;

        /// <summary>
        /// 总分数
        /// </summary>
        public int TotalScore { get; set; } = 0;

        /// <summary>
        /// 最高分数
        /// </summary>
        public int HighestScore { get; set; } = 0;

        /// <summary>
        /// 平均分数
        /// </summary>
        public double AverageScore => TotalGames > 0 ? (double)TotalScore / TotalGames : 0;

        /// <summary>
        /// 最长蛇身长度
        /// </summary>
        public int LongestSnake { get; set; } = 0;

        /// <summary>
        /// AI建议使用次数
        /// </summary>
        public int AIAdviceUsed { get; set; } = 0;

        /// <summary>
        /// 加载统计数据
        /// </summary>
        private static GameStatistics LoadStatistics()
        {
            try
            {
                if (File.Exists(StatsFileName))
                {
                    string json = File.ReadAllText(StatsFileName);
                    var stats = JsonConvert.DeserializeObject<GameStatistics>(json);
                    return stats ?? new GameStatistics();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载统计数据失败: {ex.Message}");
            }
            
            return new GameStatistics();
        }

        /// <summary>
        /// 保存统计数据
        /// </summary>
        public void SaveStatistics()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(StatsFileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存统计数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 记录游戏结果
        /// </summary>
        public void RecordGame(int score, int playTime, int snakeLength)
        {
            TotalGames++;
            TotalScore += score;
            TotalPlayTime += playTime;
            
            if (score > HighestScore)
            {
                HighestScore = score;
            }
            
            if (snakeLength > LongestSnake)
            {
                LongestSnake = snakeLength;
            }
            
            SaveStatistics();
        }

        /// <summary>
        /// 记录AI建议使用
        /// </summary>
        public void RecordAIAdviceUsed()
        {
            AIAdviceUsed++;
            SaveStatistics();
        }

        /// <summary>
        /// 重置统计数据
        /// </summary>
        public void ResetStatistics()
        {
            TotalGames = 0;
            TotalPlayTime = 0;
            TotalScore = 0;
            HighestScore = 0;
            LongestSnake = 0;
            AIAdviceUsed = 0;
            SaveStatistics();
        }
    }

    /// <summary>
    /// 音效管理类
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// 播放吃食物音效
        /// </summary>
        public static void PlayEatSound()
        {
            if (GameSettings.Instance.SoundEnabled)
            {
                try
                {
                    // 使用系统音效
                    System.Media.SystemSounds.Beep.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"播放音效失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 播放游戏结束音效
        /// </summary>
        public static void PlayGameOverSound()
        {
            if (GameSettings.Instance.SoundEnabled)
            {
                try
                {
                    // 使用系统音效
                    System.Media.SystemSounds.Hand.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"播放音效失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 播放按钮点击音效
        /// </summary>
        public static void PlayClickSound()
        {
            if (GameSettings.Instance.SoundEnabled)
            {
                try
                {
                    // 使用系统音效
                    System.Media.SystemSounds.Question.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"播放音效失败: {ex.Message}");
                }
            }
        }
    }
}