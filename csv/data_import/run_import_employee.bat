@echo off
chcp 65001 >nul
echo ========================================
echo Script import employee từ CSV
echo ========================================
echo.

REM Kiểm tra Python
python --version >nul 2>&1
if errorlevel 1 (
    echo Lỗi: Python chưa được cài đặt hoặc chưa có trong PATH
    pause
    exit /b 1
)

REM Kiểm tra pyodbc
python -c "import pyodbc" >nul 2>&1
if errorlevel 1 (
    echo Đang cài đặt pyodbc...
    pip install pyodbc
    if errorlevel 1 (
        echo Lỗi: Không thể cài đặt pyodbc
        pause
        exit /b 1
    )
)

echo Đang chạy script import...
cd /d "%~dp0"
python import_employee.py

if errorlevel 1 (
    echo.
    echo Có lỗi xảy ra!
    pause
    exit /b 1
)

echo.
echo Hoàn thành!
pause
