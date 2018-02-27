@echo off

CALL Clean.bat

"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode.Source.zip *.md
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode.Source.zip *.bat
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode.Source.zip *.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode.Source.zip *.sln
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode.Source.zip *.suo
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Globals\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Utilities\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Model\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip DataAccess\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Server\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Client\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Research\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip QuranCode\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip PrimeCalculator\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip QuranLab\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip InitialLetters\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Composites\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode.Source.zip Numbers\*.*

CD Build\Release\
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 ..\..\QuranCode.zip *.*
CD ..\..\

CALL Version.bat
