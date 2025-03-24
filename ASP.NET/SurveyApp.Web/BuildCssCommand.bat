
@echo off
echo Building Tailwind CSS...
cd %~dp0
npx tailwindcss -i ./wwwroot/css/tailwind_custom.css -o ./wwwroot/css/tailwind.min.css --minify
echo CSS build completed!
pause
