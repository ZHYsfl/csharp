using System;
using System.Collections.Generic;
using System.Timers;

namespace SnakeGameAI
{
    /// <summary>
    /// 游戏引擎类，负责游戏逻辑控制
    /// </summary>
    public class GameEngine
    {
        private Snake snake;
        private Food food;
        private System.Timers.Timer gameTimer;
        private GameState gameState;
        private int score;
        private int gridWidth;
        private int gridHeight;
        private int gameSpeed;

        // 事件定义
        public event EventHandler<GameUpdateEventArgs>? GameUpdated;
        public event EventHandler<GameOverEventArgs>? GameOver;
        public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;

        public GameEngine(int width, int height, int speed = 200)
        {
            gridWidth = width;
            gridHeight = height;
            gameSpeed = speed;
            
            InitializeGame();
            SetupTimer();
        }

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentState => gameState;

        /// <summary>
        /// 当前分数
        /// </summary>
        public int Score => score;

        /// <summary>
        /// 蛇对象
        /// </summary>
        public Snake Snake => snake;

        /// <summary>
        /// 食物对象
        /// </summary>
        public Food Food => food;

        /// <summary>
        /// 网格宽度
        /// </summary>
        public int GridWidth => gridWidth;

        /// <summary>
        /// 网格高度
        /// </summary>
        public int GridHeight => gridHeight;

        /// <summary>
        /// 初始化游戏
        /// </summary>
        private void InitializeGame()
        {
            // 创建蛇，初始位置在网格中央
            GamePoint startPosition = new GamePoint(gridWidth / 2, gridHeight / 2);
            snake = new Snake(startPosition);
            
            // 创建食物
            food = new Food();
            food.GenerateNewPosition(gridWidth, gridHeight, snake.Body);
            
            // 重置游戏状态
            gameState = GameState.Ready;
            score = 0;
        }

        /// <summary>
        /// 设置游戏计时器
        /// </summary>
        private void SetupTimer()
        {
            gameTimer = new System.Timers.Timer(gameSpeed);
            gameTimer.Elapsed += OnGameTick;
            gameTimer.AutoReset = true;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (gameState == GameState.Ready || gameState == GameState.Paused)
            {
                gameState = GameState.Playing;
                gameTimer.Start();
            }
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (gameState == GameState.Playing)
            {
                gameState = GameState.Paused;
                gameTimer.Stop();
            }
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            gameTimer.Stop();
            InitializeGame();
            StartGame();
        }

        /// <summary>
        /// 设置蛇的移动方向
        /// </summary>
        public void SetSnakeDirection(Direction direction)
        {
            if (gameState == GameState.Playing)
            {
                snake.SetDirection(direction);
            }
        }

        /// <summary>
        /// 游戏主循环
        /// </summary>
        private void OnGameTick(object? sender, ElapsedEventArgs e)
        {
            if (gameState != GameState.Playing)
                return;

            // 移动蛇
            bool ateFood = CheckFoodCollision();
            snake.Move(ateFood);

            // 检查碰撞
            if (CheckCollisions())
            {
                EndGame();
                return;
            }

            // 如果吃到食物，生成新食物并增加分数
            if (ateFood)
            {
                score += 10;
                food.GenerateNewPosition(gridWidth, gridHeight, snake.Body);
                OnScoreChanged(new ScoreChangedEventArgs(score));
                
                // 随着分数增加，游戏速度加快
                if (score % 50 == 0 && gameSpeed > 50)
                {
                    gameSpeed -= 10;
                    gameTimer.Interval = gameSpeed;
                }
            }

            // 触发游戏更新事件
            OnGameUpdated(new GameUpdateEventArgs(snake.Body, food.Position, score));
        }

        /// <summary>
        /// 检查是否吃到食物
        /// </summary>
        private bool CheckFoodCollision()
        {
            return snake.Head == food.Position;
        }

        /// <summary>
        /// 检查碰撞（墙壁和自身）
        /// </summary>
        private bool CheckCollisions()
        {
            GamePoint head = snake.Head;
            
            // 检查墙壁碰撞
            if (head.X < 0 || head.X >= gridWidth || head.Y < 0 || head.Y >= gridHeight)
                return true;
            
            // 检查自身碰撞
            return snake.CheckSelfCollision();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        private void EndGame()
        {
            gameState = GameState.GameOver;
            gameTimer.Stop();
            OnGameOver(new GameOverEventArgs(score));
        }

        /// <summary>
        /// 获取游戏状态信息（用于AI分析）
        /// </summary>
        public GameStateInfo GetGameStateInfo()
        {
            return new GameStateInfo
            {
                SnakeHead = snake.Head,
                SnakeBody = new List<GamePoint>(snake.Body),
                FoodPosition = food.Position,
                CurrentDirection = snake.CurrentDirection,
                Score = score,
                GridWidth = gridWidth,
                GridHeight = gridHeight
            };
        }

        /// <summary>
        /// 触发游戏更新事件
        /// </summary>
        protected virtual void OnGameUpdated(GameUpdateEventArgs e)
        {
            GameUpdated?.Invoke(this, e);
        }

        /// <summary>
        /// 触发游戏结束事件
        /// </summary>
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            GameOver?.Invoke(this, e);
        }

        /// <summary>
        /// 触发分数改变事件
        /// </summary>
        protected virtual void OnScoreChanged(ScoreChangedEventArgs e)
        {
            ScoreChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            gameTimer?.Dispose();
        }
    }

    /// <summary>
    /// 游戏更新事件参数
    /// </summary>
    public class GameUpdateEventArgs : EventArgs
    {
        public List<GamePoint> SnakeBody { get; }
        public GamePoint FoodPosition { get; }
        public int Score { get; }

        public GameUpdateEventArgs(List<GamePoint> snakeBody, GamePoint foodPosition, int score)
        {
            SnakeBody = snakeBody;
            FoodPosition = foodPosition;
            Score = score;
        }
    }

    /// <summary>
    /// 游戏结束事件参数
    /// </summary>
    public class GameOverEventArgs : EventArgs
    {
        public int FinalScore { get; }

        public GameOverEventArgs(int finalScore)
        {
            FinalScore = finalScore;
        }
    }

    /// <summary>
    /// 分数改变事件参数
    /// </summary>
    public class ScoreChangedEventArgs : EventArgs
    {
        public int NewScore { get; }

        public ScoreChangedEventArgs(int newScore)
        {
            NewScore = newScore;
        }
    }

    /// <summary>
    /// 游戏状态信息（用于AI分析）
    /// </summary>
    public class GameStateInfo
    {
        public GamePoint SnakeHead { get; set; }
        public List<GamePoint> SnakeBody { get; set; } = new();
        public GamePoint FoodPosition { get; set; }
        public Direction CurrentDirection { get; set; }
        public int Score { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
    }
}