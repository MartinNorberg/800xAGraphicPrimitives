@echo off
net session >nul 2>&1
if not %errorLevel% == 0 (
        echo Run as Administrator reqired!
	goto EXIT
)

copy "%~dp0PG2VNCViewer.dll" "c:\Program Files (x86)\ABB 800xA\Base\bin\GraphicPrimitives\"
regedit /s /i "%~dp0signature_bypass_install.reg"
echo Installation Successful!

:EXIT
pause
exit