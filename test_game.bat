@echo off
echo 正在测试贪吃蛇游戏...
echo.
echo 1. 构建项目...
dotnet build
if %errorlevel% neq 0 (
    echo 构建失败！
    pause
    exit /b 1
)
echo 构建成功！
echo.
echo 2. 运行游戏...
echo 游戏将在新窗口中启动
echo 请测试以下功能：
echo - 使用方向键控制蛇的移动
echo - 点击开始/暂停/重启按钮
echo - 测试AI助手功能（需要配置API密钥）
echo - 查看得分和游戏状态
echo.
dotnet run
echo.
echo 测试完成！
pause