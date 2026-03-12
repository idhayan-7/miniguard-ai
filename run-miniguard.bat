@echo off
echo ============================================
echo  MiniGuard AI - Starting...
echo ============================================
echo.
echo Starting .NET API on http://localhost:5000
echo Starting Angular on  http://localhost:4200
echo.
start "MiniGuard API" cmd /k "cd /d %~dp0backend\MiniGuard.API && dotnet run"
timeout /t 3 /nobreak >nul
start "MiniGuard Web" cmd /k "cd /d %~dp0frontend\MiniGuard.Web && ng serve"
echo.
echo Both services starting. Open http://localhost:4200 in your browser.
echo.
pause
