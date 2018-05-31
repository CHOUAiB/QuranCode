RD /S /Q NET2
"%PROGRAMFILES%\7-Zip\7z.exe" x Tools\NET2.zip
echo .csproj > exclude.txt
XCOPY /E /EXCLUDE:exclude.txt Globals\*.* NET2\Globals\
XCOPY /E /EXCLUDE:exclude.txt Utilities\*.* NET2\Utilities\
XCOPY /E /EXCLUDE:exclude.txt Composites\*.* NET2\Composites\

DEL exclude.txt
Tools\Replace\bin\Release\Replace.exe NET2 *.Designer.cs ((System.ComponentModel.ISupportInitialize) //((System.ComponentModel.ISupportInitialize)
CALL Version.bat
CD NET2
CALL Version.bat
CD ..
