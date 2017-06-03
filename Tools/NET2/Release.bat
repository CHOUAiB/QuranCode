@echo off

CALL Clean.bat
CALL Version.bat

cd Build\Release\
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 ..\..\QuranCode1433.zip *.*
cd ..\..\

CALL Version.bat
