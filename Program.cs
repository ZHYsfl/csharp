using System;
using System.Windows.Forms;

namespace SnakeGameAI
{
    /// <summary>
    /// 程序入口点
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用应用程序视觉样式
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 运行主窗体
            Application.Run(new MainForm());
        }
    }
}