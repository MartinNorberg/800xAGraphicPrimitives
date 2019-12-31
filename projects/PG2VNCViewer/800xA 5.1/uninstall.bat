@echo off
net session >nul 2>&1
if not %errorLevel% == 0 (
        echo Run as Administrator reqired!
	goto EXIT
)

del "c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\GraphicPrimitives\PG2VNCViewer.dll"
regedit /s /i "%~dp0signature_bypass_remove.reg"
echo Uninstallation Successful!

:EXIT
pause
exit