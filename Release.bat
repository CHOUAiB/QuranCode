@echo off

CALL Clean.bat
CD NET2
CALL Clean.bat
CD ..

"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip LICENSE
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.md
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.bat
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.txt
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.sln
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip    -mx5 QuranCode1433.Source.zip *.suo
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Tools\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Globals\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Utilities\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Model\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip DataAccess\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Server\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Client\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Research\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Common\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip QuranCode\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip PrimeCalculator\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip QuranLab\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip InitialLetters\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Composites\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip Numbers\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip WordGenerator\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.Source.zip AhlulBayt\*.*

CD NET2
CALL Version.bat
CD ..

MD Win7
MD Win10
XCOPY /E NET2\Build\Release\*.* Win7\
XCOPY /E      Build\Release\*.* Win10\
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Win7\*.*
"%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -r -mx5 QuranCode1433.zip Win10\*.*
RD /S /Q Win7
RD /S /Q Win10

CALL Version.bat
