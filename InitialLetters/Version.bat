IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 631
Tools\Version\bin\Release\Version.exe .               6.19.631.4 6.19.631.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools                            2009-07-29 7:29

Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY InitialLetters.zip         ..\Backup\InitialLetters_6.19.631.zip
COPY InitialLetters.Source.zip  ..\Backup\InitialLetters_6.19.631.Source.zip
MOVE InitialLetters.zip         ..\qurancode.com\
MOVE InitialLetters.Source.zip  ..\qurancode.com\
