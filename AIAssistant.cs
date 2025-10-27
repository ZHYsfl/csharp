using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SnakeGameAI
{
    /// <summary>
    /// AI助手类，集成大语言模型提供游戏建议和智能提示
    /// </summary>
    public class AIAssistant
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private readonly string apiUrl;

        public AIAssistant()
        {
            httpClient = new HttpClient();
            // 这里使用模拟的API配置，实际使用时需要配置真实的API
            apiKey = "sk-05ed547abe9e4a2aad233d34856a103f";
            apiUrl = "https://api.deepseek.com/v1";
            
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        /// <summary>
        /// 获取游戏策略建议
        /// </summary>
        public async Task<string> GetGameAdviceAsync(GameStateInfo gameState)
        {
            try
            {
                string prompt = CreateGameAnalysisPrompt(gameState);
                return await CallLanguageModelAsync(prompt);
            }
            catch (Exception ex)
            {
                // 如果API调用失败，返回本地智能建议
                return GetLocalAdvice(gameState);
            }
        }

        /// <summary>
        /// 获取下一步移动建议
        /// </summary>
        public async Task<Direction?> GetNextMoveAdviceAsync(GameStateInfo gameState)
        {
            try
            {
                string prompt = CreateMoveAdvicePrompt(gameState);
                string response = await CallLanguageModelAsync(prompt);
                return ParseDirectionFromResponse(response);
            }
            catch (Exception ex)
            {
                // 如果API调用失败，使用本地算法
                return GetLocalMoveAdvice(gameState);
            }
        }

        /// <summary>
        /// 获取游戏技巧提示
        /// </summary>
        public async Task<string> GetGameTipsAsync()
        {
            try
            {
                string prompt = "请提供一些贪吃蛇游戏的高级技巧和策略，帮助玩家提高游戏水平。";
                return await CallLanguageModelAsync(prompt);
            }
            catch (Exception ex)
            {
                return GetLocalGameTips();
            }
        }

        /// <summary>
        /// 分析游戏表现
        /// </summary>
        public async Task<string> AnalyzeGamePerformanceAsync(int score, int gameTime, List<int> recentScores)
        {
            try
            {
                string prompt = CreatePerformanceAnalysisPrompt(score, gameTime, recentScores);
                return await CallLanguageModelAsync(prompt);
            }
            catch (Exception ex)
            {
                return GetLocalPerformanceAnalysis(score, recentScores);
            }
        }

        /// <summary>
        /// 创建游戏分析提示词
        /// </summary>
        private string CreateGameAnalysisPrompt(GameStateInfo gameState)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("作为贪吃蛇游戏专家，请分析当前游戏状态并提供建议：");
            prompt.AppendLine($"蛇头位置: ({gameState.SnakeHead.X}, {gameState.SnakeHead.Y})");
            prompt.AppendLine($"食物位置: ({gameState.FoodPosition.X}, {gameState.FoodPosition.Y})");
            prompt.AppendLine($"当前方向: {gameState.CurrentDirection}");
            prompt.AppendLine($"蛇身长度: {gameState.SnakeBody.Count}");
            prompt.AppendLine($"当前分数: {gameState.Score}");
            prompt.AppendLine($"游戏区域: {gameState.GridWidth}x{gameState.GridHeight}");
            prompt.AppendLine("请提供简洁的策略建议（50字以内）。");
            
            return prompt.ToString();
        }

        /// <summary>
        /// 创建移动建议提示词
        /// </summary>
        private string CreateMoveAdvicePrompt(GameStateInfo gameState)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("分析贪吃蛇游戏状态，建议下一步最佳移动方向：");
            prompt.AppendLine($"蛇头: ({gameState.SnakeHead.X}, {gameState.SnakeHead.Y})");
            prompt.AppendLine($"食物: ({gameState.FoodPosition.X}, {gameState.FoodPosition.Y})");
            prompt.AppendLine($"当前方向: {gameState.CurrentDirection}");
            prompt.AppendLine("请只回答方向：Up/Down/Left/Right");
            
            return prompt.ToString();
        }

        /// <summary>
        /// 创建表现分析提示词
        /// </summary>
        private string CreatePerformanceAnalysisPrompt(int score, int gameTime, List<int> recentScores)
        {
            StringBuilder prompt = new StringBuilder();
            prompt.AppendLine("分析玩家贪吃蛇游戏表现：");
            prompt.AppendLine($"本局分数: {score}");
            prompt.AppendLine($"游戏时长: {gameTime}秒");
            prompt.AppendLine($"最近几局分数: {string.Join(", ", recentScores)}");
            prompt.AppendLine("请提供表现分析和改进建议（100字以内）。");
            
            return prompt.ToString();
        }

        /// <summary>
        /// 调用语言模型API
        /// </summary>
        private async Task<string> CallLanguageModelAsync(string prompt)
        {
            // 模拟API调用，实际项目中需要配置真实的API
            // 这里为了演示，直接抛出异常让程序使用本地逻辑
            throw new NotImplementedException("需要配置真实的API密钥");

            /*
            // 真实API调用代码示例（需要有效的API密钥）
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 150,
                temperature = 0.7
            };

            string jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);
            
            return result.choices[0].message.content.ToString().Trim();
            */
        }

        /// <summary>
        /// 从响应中解析方向
        /// </summary>
        private Direction? ParseDirectionFromResponse(string response)
        {
            response = response.ToUpper().Trim();
            
            if (response.Contains("UP")) return Direction.Up;
            if (response.Contains("DOWN")) return Direction.Down;
            if (response.Contains("LEFT")) return Direction.Left;
            if (response.Contains("RIGHT")) return Direction.Right;
            
            return null;
        }

        /// <summary>
        /// 本地智能建议（当API不可用时使用）
        /// </summary>
        private string GetLocalAdvice(GameStateInfo gameState)
        {
            var suggestions = new List<string>();

            // 分析距离食物的位置
            int deltaX = gameState.FoodPosition.X - gameState.SnakeHead.X;
            int deltaY = gameState.FoodPosition.Y - gameState.SnakeHead.Y;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                suggestions.Add(deltaX > 0 ? "建议向右移动接近食物" : "建议向左移动接近食物");
            }
            else
            {
                suggestions.Add(deltaY > 0 ? "建议向下移动接近食物" : "建议向上移动接近食物");
            }

            // 检查危险区域
            if (IsNearWall(gameState))
            {
                suggestions.Add("注意避开墙壁！");
            }

            if (IsNearSelf(gameState))
            {
                suggestions.Add("小心不要撞到自己！");
            }

            // 根据蛇的长度给出建议
            if (gameState.SnakeBody.Count > 10)
            {
                suggestions.Add("蛇身较长，移动时要更加小心规划路径");
            }

            return suggestions.Count > 0 ? string.Join(" ", suggestions) : "继续保持当前策略！";
        }

        /// <summary>
        /// 本地移动建议算法
        /// </summary>
        private Direction? GetLocalMoveAdvice(GameStateInfo gameState)
        {
            GamePoint head = gameState.SnakeHead;
            GamePoint food = gameState.FoodPosition;
            
            // 简单的寻路算法：优先朝食物方向移动，同时避免碰撞
            List<Direction> possibleMoves = new List<Direction>();
            
            // 计算到食物的距离
            int deltaX = food.X - head.X;
            int deltaY = food.Y - head.Y;
            
            // 优先考虑距离食物更近的方向
            if (Math.Abs(deltaX) >= Math.Abs(deltaY))
            {
                if (deltaX > 0) possibleMoves.Add(Direction.Right);
                if (deltaX < 0) possibleMoves.Add(Direction.Left);
                if (deltaY > 0) possibleMoves.Add(Direction.Down);
                if (deltaY < 0) possibleMoves.Add(Direction.Up);
            }
            else
            {
                if (deltaY > 0) possibleMoves.Add(Direction.Down);
                if (deltaY < 0) possibleMoves.Add(Direction.Up);
                if (deltaX > 0) possibleMoves.Add(Direction.Right);
                if (deltaX < 0) possibleMoves.Add(Direction.Left);
            }
            
            // 检查每个可能的移动是否安全
            foreach (Direction direction in possibleMoves)
            {
                if (IsSafeMove(gameState, direction))
                {
                    return direction;
                }
            }
            
            // 如果没有安全的朝向食物的移动，尝试其他方向
            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (IsSafeMove(gameState, direction))
                {
                    return direction;
                }
            }
            
            return null; // 没有安全的移动
        }

        /// <summary>
        /// 检查移动是否安全
        /// </summary>
        private bool IsSafeMove(GameStateInfo gameState, Direction direction)
        {
            GamePoint head = gameState.SnakeHead;
            GamePoint nextPosition = direction switch
            {
                Direction.Up => new GamePoint(head.X, head.Y - 1),
                Direction.Down => new GamePoint(head.X, head.Y + 1),
                Direction.Left => new GamePoint(head.X - 1, head.Y),
                Direction.Right => new GamePoint(head.X + 1, head.Y),
                _ => head
            };
            
            // 检查是否撞墙
            if (nextPosition.X < 0 || nextPosition.X >= gameState.GridWidth ||
                nextPosition.Y < 0 || nextPosition.Y >= gameState.GridHeight)
            {
                return false;
            }
            
            // 检查是否撞到自己（除了尾巴，因为移动后尾巴会消失）
            for (int i = 0; i < gameState.SnakeBody.Count - 1; i++)
            {
                if (gameState.SnakeBody[i] == nextPosition)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// 检查是否靠近墙壁
        /// </summary>
        private bool IsNearWall(GameStateInfo gameState)
        {
            GamePoint head = gameState.SnakeHead;
            return head.X <= 1 || head.X >= gameState.GridWidth - 2 ||
                   head.Y <= 1 || head.Y >= gameState.GridHeight - 2;
        }

        /// <summary>
        /// 检查是否靠近自己的身体
        /// </summary>
        private bool IsNearSelf(GameStateInfo gameState)
        {
            GamePoint head = gameState.SnakeHead;
            foreach (GamePoint bodyPart in gameState.SnakeBody.Skip(3)) // 跳过头部和紧邻的身体部分
            {
                int distance = Math.Abs(head.X - bodyPart.X) + Math.Abs(head.Y - bodyPart.Y);
                if (distance <= 2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 本地游戏技巧
        /// </summary>
        private string GetLocalGameTips()
        {
            var tips = new[]
            {
                "技巧1：尽量让蛇身形成螺旋形状，为后续移动留出空间。",
                "技巧2：不要急于吃食物，优先考虑安全的移动路径。",
                "技巧3：当蛇身较长时，可以绕着边界移动来争取时间。",
                "技巧4：学会预判：提前规划下几步的移动路径。",
                "技巧5：在角落附近要格外小心，容易被困住。"
            };
            
            Random random = new Random();
            return tips[random.Next(tips.Length)];
        }

        /// <summary>
        /// 本地表现分析
        /// </summary>
        private string GetLocalPerformanceAnalysis(int score, List<int> recentScores)
        {
            if (recentScores.Count == 0)
            {
                return $"本局得分{score}分。继续练习，熟悉游戏节奏！";
            }
            
            double averageScore = recentScores.Average();
            
            if (score > averageScore)
            {
                return $"表现不错！本局{score}分超过了平均水平{averageScore:F1}分。继续保持！";
            }
            else
            {
                return $"本局{score}分低于平均{averageScore:F1}分。建议多练习路径规划和危险预判。";
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}