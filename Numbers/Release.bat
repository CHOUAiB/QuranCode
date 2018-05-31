@echo off

CALL Clean.bat
CD NET2
CALL Clean.bat
CD ..

"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip LICENSE
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip *.md
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip *.bat
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip *.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip *.sln
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 Numbers.Source.zip *.suo
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.Source.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.Source.zip Globals\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.Source.zip Utilities\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.Source.zip Numbers\*.*

CD NET2
CALL Version.bat
CD ..

MD Win7
MD Win10
XCOPY /E NET2\Build\Release\*.* Win7\
XCOPY /E      Build\Release\*.* Win10\
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.zip Win7\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 Numbers.zip Win10\*.*
RD /S /Q Win7
RD /S /Q Win10

CALL Version.bat
