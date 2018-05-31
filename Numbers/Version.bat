IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 631
Tools\Version\bin\Release\Version.exe .               6.19.631.4 6.19.631.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools                            2009-07-29 7:29

Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY Numbers.zip         ..\Backup\Numbers_6.19.631.zip
COPY Numbers.Source.zip  ..\Backup\Numbers_6.19.631.Source.zip
MOVE Numbers.zip         ..\qurancode.com\
MOVE Numbers.Source.zip  ..\qurancode.com\
