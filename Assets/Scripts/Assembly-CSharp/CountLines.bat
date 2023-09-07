@echo off
setlocal enabledelayedexpansion
set "totalLines=0"

for /f %%i in ('dir /b *.cs') do (
    set "file=%%i"
    for /f %%j in ('find /v /c "" ^< "!file!"') do (
        set /a "totalLines+=%%j"
    )
)

echo Total number of lines in .cs files: %totalLines%
endlocal
