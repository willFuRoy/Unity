@echo off
setlocal EnableDelayedExpansion
rem 正在搜索...  
for /f "delims=" %%i in ('dir /b /a-d /s "..\Assets\HF\*.lua"') do (
	rem 为 a 赋值
	set a=%%i
	rem 重命名为txt
	set b=!a:~0,-3!txt
	rem echo !a!
	rem echo !b!
	luac53 -o !b! !a!
)
rem 搜索完毕  
pause  