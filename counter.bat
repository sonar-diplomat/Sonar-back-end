@echo off
setlocal enabledelayedexpansion

:: Header
echo ================================
echo  .cs data analyzer
echo ================================
echo.

:: Initialize counters
set "fileCount=0"
set "lineCount=0"
set "charCount=0"

:: Loop through all .cs files recursively
for /r %%f in (*.cs) do (
    set /a fileCount+=1

    :: Count lines (use "find" and delayed expansion safely)
    for /f %%a in ('find /v /c "" ^< "%%f"') do (
        set /a lineCount+=%%a
    )

    :: Count characters (bytes)
    for %%b in ("%%f") do (
        set /a charCount+=%%~zb
    )
)

:: Output results
echo Files: !fileCount!
echo Lines: !lineCount!
echo Characters: !charCount!
echo.
pause
