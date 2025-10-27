using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeGameAI
{
    /// <summary>
    /// 主窗体类，负责游戏界面显示和用户交互
    /// </summary>
    public partial class MainForm : Form
    {
        private GameEngine gameEngine = null!;
        private AIAssistant aiAssistant = null!;
        private Panel gamePanel = null!;
        private Label scoreLabel = null!;
        private Label statusLabel = null!;
        private Button startButton = null!;
        private Button pauseButton = null!;
        private Button restartButton = null!;
        private Button aiAdviceButton = null!;
        private Button aiTipsButton = null!;
        private TextBox aiResponseTextBox = null!;
        private List<int> gameHistory = null!;
        
        // 游戏绘制相关
        private const int CellSize = 20;
        private const int GridWidth = 20;
        private const int GridHeight = 20;
        private Brush snakeHeadBrush = new SolidBrush(Color.DarkGreen);
        private Brush snakeBodyBrush = new SolidBrush(Color.Green);
        private Brush foodBrush = new SolidBrush(Color.Red);
        private Brush backgroundBrush = new SolidBrush(Color.Black);
        private Pen gridPen = new Pen(Color.DarkGray, 1);

        public MainForm()
        {
            InitializeComponent();
            InitializeGame();
            SetupEventHandlers();
        }

        /// <summary>
        /// 初始化窗体组件
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "贪吃蛇游戏 - AI智能助手版";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.TabStop = true;

            // 创建游戏面板
            gamePanel = new Panel
            {
                Size = new Size(GridWidth * CellSize + 1, GridHeight * CellSize + 1),
                Location = new Point(20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            gamePanel.Paint += GamePanel_Paint;
            this.Controls.Add(gamePanel);

            // 创建控制面板
            CreateControlPanel();
            
            // 创建AI助手面板
            CreateAIPanel();

            gameHistory = new List<int>();
        }

        /// <summary>
        /// 创建控制面板
        /// </summary>
        private void CreateControlPanel()
        {
            Panel controlPanel = new Panel
            {
                Size = new Size(200, 300),
                Location = new Point(gamePanel.Right + 20, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // 分数标签
            scoreLabel = new Label
            {
                Text = "分数: 0",
                Location = new Point(10, 10),
                Size = new Size(180, 30),
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            controlPanel.Controls.Add(scoreLabel);

            // 状态标签
            statusLabel = new Label
            {
                Text = "状态: 准备开始",
                Location = new Point(10, 50),
                Size = new Size(180, 30),
                Font = new Font("微软雅黑", 10),
                ForeColor = Color.Green
            };
            controlPanel.Controls.Add(statusLabel);

            // 开始按钮
            startButton = new Button
            {
                Text = "开始游戏",
                Location = new Point(10, 90),
                Size = new Size(80, 35),
                Font = new Font("微软雅黑", 9)
            };
            startButton.Click += StartButton_Click;
            controlPanel.Controls.Add(startButton);

            // 暂停按钮
            pauseButton = new Button
            {
                Text = "暂停",
                Location = new Point(100, 90),
                Size = new Size(80, 35),
                Font = new Font("微软雅黑", 9),
                Enabled = false
            };
            pauseButton.Click += PauseButton_Click;
            controlPanel.Controls.Add(pauseButton);

            // 重新开始按钮
            restartButton = new Button
            {
                Text = "重新开始",
                Location = new Point(10, 135),
                Size = new Size(170, 35),
                Font = new Font("微软雅黑", 9)
            };
            restartButton.Click += RestartButton_Click;
            controlPanel.Controls.Add(restartButton);

            // 操作说明
            Label instructionLabel = new Label
            {
                Text = "操作说明:\n↑↓←→ 控制方向\nSpace 暂停/继续\nR 重新开始",
                Location = new Point(10, 180),
                Size = new Size(180, 80),
                Font = new Font("微软雅黑", 9),
                ForeColor = Color.DarkBlue
            };
            controlPanel.Controls.Add(instructionLabel);

            this.Controls.Add(controlPanel);
        }

        /// <summary>
        /// 创建AI助手面板
        /// </summary>
        private void CreateAIPanel()
        {
            Panel aiPanel = new Panel
            {
                Size = new Size(350, 400),
                Location = new Point(gamePanel.Right + 220, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // AI助手标题
            Label aiTitleLabel = new Label
            {
                Text = "🤖 AI智能助手",
                Location = new Point(10, 10),
                Size = new Size(330, 30),
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                ForeColor = Color.Purple,
                TextAlign = ContentAlignment.MiddleCenter
            };
            aiPanel.Controls.Add(aiTitleLabel);

            // AI建议按钮
            aiAdviceButton = new Button
            {
                Text = "获取游戏建议",
                Location = new Point(10, 50),
                Size = new Size(100, 35),
                Font = new Font("微软雅黑", 9),
                BackColor = Color.LightBlue
            };
            aiAdviceButton.Click += AIAdviceButton_Click;
            aiPanel.Controls.Add(aiAdviceButton);

            // AI技巧按钮
            aiTipsButton = new Button
            {
                Text = "游戏技巧",
                Location = new Point(120, 50),
                Size = new Size(100, 35),
                Font = new Font("微软雅黑", 9),
                BackColor = Color.LightGreen
            };
            aiTipsButton.Click += AITipsButton_Click;
            aiPanel.Controls.Add(aiTipsButton);

            // AI响应文本框
            aiResponseTextBox = new TextBox
            {
                Location = new Point(10, 100),
                Size = new Size(330, 280),
                Font = new Font("微软雅黑", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.LightYellow,
                Text = "欢迎使用AI智能助手！\n\n点击上方按钮获取游戏建议和技巧。\n\nAI助手可以：\n• 分析当前游戏状态\n• 提供移动策略建议\n• 分享游戏技巧\n• 评估游戏表现"
            };
            aiPanel.Controls.Add(aiResponseTextBox);

            this.Controls.Add(aiPanel);
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        private void InitializeGame()
        {
            gameEngine = new GameEngine(GridWidth, GridHeight, 200);
            aiAssistant = new AIAssistant();
        }

        /// <summary>
        /// 设置事件处理器
        /// </summary>
        private void SetupEventHandlers()
        {
            // 游戏引擎事件
            gameEngine.GameUpdated += GameEngine_GameUpdated;
            gameEngine.GameOver += GameEngine_GameOver;
            gameEngine.ScoreChanged += GameEngine_ScoreChanged;

            // 键盘事件
            this.KeyDown += MainForm_KeyDown;

            // 窗体事件
            this.Activated += MainForm_Activated;
            this.Click += MainForm_Click;
            this.FormClosing += MainForm_FormClosing;
        }

        /// <summary>
        /// 游戏面板绘制事件
        /// </summary>
        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            // 清空背景
            g.FillRectangle(backgroundBrush, 0, 0, gamePanel.Width, gamePanel.Height);
            
            // 绘制网格
            DrawGrid(g);
            
            // 绘制蛇
            DrawSnake(g);
            
            // 绘制食物
            DrawFood(g);
        }

        /// <summary>
        /// 绘制网格
        /// </summary>
        private void DrawGrid(Graphics g)
        {
            // 绘制垂直线
            for (int x = 0; x <= GridWidth; x++)
            {
                g.DrawLine(gridPen, x * CellSize, 0, x * CellSize, GridHeight * CellSize);
            }
            
            // 绘制水平线
            for (int y = 0; y <= GridHeight; y++)
            {
                g.DrawLine(gridPen, 0, y * CellSize, GridWidth * CellSize, y * CellSize);
            }
        }

        /// <summary>
        /// 绘制蛇
        /// </summary>
        private void DrawSnake(Graphics g)
        {
            if (gameEngine.Snake.Body.Count == 0) return;

            // 绘制蛇头
            GamePoint head = gameEngine.Snake.Head;
            Rectangle headRect = new Rectangle(
                head.X * CellSize + 1,
                head.Y * CellSize + 1,
                CellSize - 2,
                CellSize - 2
            );
            g.FillRectangle(snakeHeadBrush, headRect);

            // 绘制蛇身
            for (int i = 1; i < gameEngine.Snake.Body.Count; i++)
            {
                GamePoint bodyPart = gameEngine.Snake.Body[i];
                Rectangle bodyRect = new Rectangle(
                    bodyPart.X * CellSize + 1,
                    bodyPart.Y * CellSize + 1,
                    CellSize - 2,
                    CellSize - 2
                );
                g.FillRectangle(snakeBodyBrush, bodyRect);
            }
        }

        /// <summary>
        /// 绘制食物
        /// </summary>
        private void DrawFood(Graphics g)
        {
            GamePoint food = gameEngine.Food.Position;
            Rectangle foodRect = new Rectangle(
                food.X * CellSize + 2,
                food.Y * CellSize + 2,
                CellSize - 4,
                CellSize - 4
            );
            g.FillEllipse(foodBrush, foodRect);
        }

        /// <summary>
        /// 游戏更新事件处理
        /// </summary>
        private void GameEngine_GameUpdated(object? sender, GameUpdateEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                gamePanel.Invalidate(); // 重绘游戏面板
            }));
        }

        /// <summary>
        /// 游戏结束事件处理
        /// </summary>
        private void GameEngine_GameOver(object? sender, GameOverEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                statusLabel.Text = "状态: 游戏结束";
                statusLabel.ForeColor = Color.Red;
                startButton.Enabled = true;
                pauseButton.Enabled = false;
                
                gameHistory.Add(e.FinalScore);
                
                // 显示游戏结束消息
                string message = $"游戏结束！\n最终分数: {e.FinalScore}\n\n是否重新开始？";
                DialogResult result = MessageBox.Show(message, "游戏结束", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                
                if (result == DialogResult.Yes)
                {
                    RestartGame();
                }
                
                // 获取AI表现分析
                GetAIPerformanceAnalysis(e.FinalScore);
            }));
        }

        /// <summary>
        /// 分数改变事件处理
        /// </summary>
        private void GameEngine_ScoreChanged(object? sender, ScoreChangedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                scoreLabel.Text = $"分数: {e.NewScore}";
            }));
        }

        /// <summary>
        /// 键盘按下事件处理
        /// </summary>
        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // 确保游戏正在运行时才处理方向键
            if (gameEngine.CurrentState == GameState.Playing)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                    case Keys.W:
                        gameEngine.SetSnakeDirection(Direction.Up);
                        e.Handled = true;
                        break;
                    case Keys.Down:
                    case Keys.S:
                        gameEngine.SetSnakeDirection(Direction.Down);
                        e.Handled = true;
                        break;
                    case Keys.Left:
                    case Keys.A:
                        gameEngine.SetSnakeDirection(Direction.Left);
                        e.Handled = true;
                        break;
                    case Keys.Right:
                    case Keys.D:
                        gameEngine.SetSnakeDirection(Direction.Right);
                        e.Handled = true;
                        break;
                }
            }
            
            // 处理其他按键
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (gameEngine.CurrentState == GameState.Playing)
                        PauseGame();
                    else if (gameEngine.CurrentState == GameState.Paused)
                        StartGame();
                    e.Handled = true;
                    break;
                case Keys.R:
                    RestartGame();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (gameEngine.CurrentState == GameState.Ready || gameEngine.CurrentState == GameState.GameOver)
                        StartGame();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// 重写ProcessCmdKey方法以确保方向键能被正确处理
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 处理方向键
            if (gameEngine.CurrentState == GameState.Playing)
            {
                switch (keyData)
                {
                    case Keys.Up:
                    case Keys.W:
                        gameEngine.SetSnakeDirection(Direction.Up);
                        return true;
                    case Keys.Down:
                    case Keys.S:
                        gameEngine.SetSnakeDirection(Direction.Down);
                        return true;
                    case Keys.Left:
                    case Keys.A:
                        gameEngine.SetSnakeDirection(Direction.Left);
                        return true;
                    case Keys.Right:
                    case Keys.D:
                        gameEngine.SetSnakeDirection(Direction.Right);
                        return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 窗体激活事件 - 确保焦点
        /// </summary>
        private void MainForm_Activated(object? sender, EventArgs e)
        {
            this.Focus();
        }

        /// <summary>
        /// 窗体点击事件 - 确保焦点
        /// </summary>
        private void MainForm_Click(object? sender, EventArgs e)
        {
            this.Focus();
        }

        /// <summary>
        /// 开始按钮点击事件
        /// </summary>
        private void StartButton_Click(object? sender, EventArgs e)
        {
            StartGame();
            this.Focus(); // 确保焦点回到窗体
        }

        /// <summary>
        /// 暂停按钮点击事件
        /// </summary>
        private void PauseButton_Click(object? sender, EventArgs e)
        {
            PauseGame();
            this.Focus(); // 确保焦点回到窗体
        }

        /// <summary>
        /// 重新开始按钮点击事件
        /// </summary>
        private void RestartButton_Click(object? sender, EventArgs e)
        {
            RestartGame();
            this.Focus(); // 确保焦点回到窗体
        }

        /// <summary>
        /// AI建议按钮点击事件
        /// </summary>
        private async void AIAdviceButton_Click(object? sender, EventArgs e)
        {
            aiAdviceButton.Enabled = false;
            aiResponseTextBox.Text = "AI正在分析游戏状态，请稍候...";
            
            try
            {
                GameStateInfo gameState = gameEngine.GetGameStateInfo();
                string advice = await aiAssistant.GetGameAdviceAsync(gameState);
                aiResponseTextBox.Text = $"🎯 AI游戏建议:\n\n{advice}";
            }
            catch (Exception ex)
            {
                aiResponseTextBox.Text = $"获取AI建议时出错: {ex.Message}";
            }
            finally
            {
                aiAdviceButton.Enabled = true;
            }
        }

        /// <summary>
        /// AI技巧按钮点击事件
        /// </summary>
        private async void AITipsButton_Click(object? sender, EventArgs e)
        {
            aiTipsButton.Enabled = false;
            aiResponseTextBox.Text = "AI正在准备游戏技巧，请稍候...";
            
            try
            {
                string tips = await aiAssistant.GetGameTipsAsync();
                aiResponseTextBox.Text = $"💡 AI游戏技巧:\n\n{tips}";
            }
            catch (Exception ex)
            {
                aiResponseTextBox.Text = $"获取AI技巧时出错: {ex.Message}";
            }
            finally
            {
                aiTipsButton.Enabled = true;
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            gameEngine.StartGame();
            statusLabel.Text = "状态: 游戏中";
            statusLabel.ForeColor = Color.Green;
            startButton.Enabled = false;
            pauseButton.Enabled = true;
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        private void PauseGame()
        {
            gameEngine.PauseGame();
            statusLabel.Text = "状态: 已暂停";
            statusLabel.ForeColor = Color.Orange;
            startButton.Enabled = true;
            pauseButton.Enabled = false;
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void RestartGame()
        {
            gameEngine.RestartGame();
            scoreLabel.Text = "分数: 0";
            statusLabel.Text = "状态: 游戏中";
            statusLabel.ForeColor = Color.Green;
            startButton.Enabled = false;
            pauseButton.Enabled = true;
            gamePanel.Invalidate();
        }

        /// <summary>
        /// 获取AI表现分析
        /// </summary>
        private async void GetAIPerformanceAnalysis(int finalScore)
        {
            try
            {
                string analysis = await aiAssistant.AnalyzeGamePerformanceAsync(
                    finalScore, 0, gameHistory.TakeLast(5).ToList());
                aiResponseTextBox.Text = $"📊 AI表现分析:\n\n{analysis}";
            }
            catch (Exception ex)
            {
                // 忽略错误，不影响游戏体验
            }
        }

        /// <summary>
        /// 窗体关闭事件处理
        /// </summary>
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            gameEngine?.Dispose();
            aiAssistant?.Dispose();
        }
    }
}