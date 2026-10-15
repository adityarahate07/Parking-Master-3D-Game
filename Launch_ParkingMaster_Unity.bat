@echo off
echo ====================================================================
echo   Launching PARKING MASTER 3D in Unity Editor (Unity 6 - 6000.6.4f1)
echo   "Park Perfectly. Drive Carefully."
echo ====================================================================
echo Project Path: %~dp0
start "" "C:\Program Files\Unity\Hub\Editor\6000.6.4f1\Editor\Unity.exe" -projectPath "%~dp0"
