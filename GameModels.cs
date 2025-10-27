using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeGameAI
{
    /// <summary>
    /// 游戏方向枚举
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        Ready,      // 准备开始
        Playing,    // 游戏中
        Paused,     // 暂停
        GameOver    // 游戏结束
    }

    /// <summary>
    /// 游戏位置点
    /// </summary>
    public struct GamePoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public GamePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is GamePoint point)
            {
                return X == point.X && Y == point.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(GamePoint left, GamePoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GamePoint left, GamePoint right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// 蛇类
    /// </summary>
    public class Snake
    {
        private List<GamePoint> body;
        private Direction currentDirection;
        private Direction nextDirection;

        public Snake(GamePoint startPosition)
        {
            body = new List<GamePoint> { startPosition };
            currentDirection = Direction.Right;
            nextDirection = Direction.Right;
        }

        /// <summary>
        /// 蛇的身体
        /// </summary>
        public List<GamePoint> Body => body;

        /// <summary>
        /// 蛇头位置
        /// </summary>
        public GamePoint Head => body[0];

        /// <summary>
        /// 当前方向
        /// </summary>
        public Direction CurrentDirection => currentDirection;

        /// <summary>
        /// 设置下一个移动方向
        /// </summary>
        public void SetDirection(Direction direction)
        {
            // 防止蛇反向移动
            if (IsOppositeDirection(direction, currentDirection))
                return;

            nextDirection = direction;
        }

        /// <summary>
        /// 移动蛇
        /// </summary>
        public void Move(bool grow = false)
        {
            currentDirection = nextDirection;
            GamePoint newHead = GetNextHeadPosition();
            
            body.Insert(0, newHead);
            
            if (!grow)
            {
                body.RemoveAt(body.Count - 1);
            }
        }

        /// <summary>
        /// 获取下一个头部位置
        /// </summary>
        private GamePoint GetNextHeadPosition()
        {
            GamePoint head = Head;
            return currentDirection switch
            {
                Direction.Up => new GamePoint(head.X, head.Y - 1),
                Direction.Down => new GamePoint(head.X, head.Y + 1),
                Direction.Left => new GamePoint(head.X - 1, head.Y),
                Direction.Right => new GamePoint(head.X + 1, head.Y),
                _ => head
            };
        }

        /// <summary>
        /// 检查是否撞到自己
        /// </summary>
        public bool CheckSelfCollision()
        {
            GamePoint head = Head;
            for (int i = 1; i < body.Count; i++)
            {
                if (body[i] == head)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查方向是否相反
        /// </summary>
        private bool IsOppositeDirection(Direction dir1, Direction dir2)
        {
            return (dir1 == Direction.Up && dir2 == Direction.Down) ||
                   (dir1 == Direction.Down && dir2 == Direction.Up) ||
                   (dir1 == Direction.Left && dir2 == Direction.Right) ||
                   (dir1 == Direction.Right && dir2 == Direction.Left);
        }
    }

    /// <summary>
    /// 食物类
    /// </summary>
    public class Food
    {
        private Random random;

        public Food()
        {
            random = new Random();
            Position = new GamePoint(0, 0);
        }

        /// <summary>
        /// 食物位置
        /// </summary>
        public GamePoint Position { get; private set; }

        /// <summary>
        /// 生成新的食物位置
        /// </summary>
        public void GenerateNewPosition(int gridWidth, int gridHeight, List<GamePoint> snakeBody)
        {
            GamePoint newPosition;
            do
            {
                newPosition = new GamePoint(
                    random.Next(0, gridWidth),
                    random.Next(0, gridHeight)
                );
            } while (snakeBody.Contains(newPosition));

            Position = newPosition;
        }
    }
}