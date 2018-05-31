RD /S /Q NET2
"%PROGRAMFILES%\7-Zip\7z.exe" x Tools\NET2.zip
echo .csproj > exclude.txt
XCOPY /E /EXCLUDE:exclude.txt InitialLetters\*.* NET2\InitialLetters\

DEL exclude.txt
Tools\Replace\bin\Release\Replace.exe NET2 *.Designer.cs ((System.ComponentModel.ISupportInitialize) //((System.ComponentModel.ISupportInitialize)
CALL Version.bat
CD NET2
CALL Version.bat
CD ..
