IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 653
Tools\Version\bin\Release\Version.exe .               6.19.653.4 6.19.653.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools                            2009-07-29 7:29
 
Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY ActiveQuran.zip        ..\Backup\ActiveQuran_6.19.653.zip
COPY ActiveQuran.Source.zip ..\Backup\ActiveQuran_6.19.653.Source.zip
MOVE ActiveQuran.zip        ..\qurancode.com\
MOVE ActiveQuran.Source.zip ..\qurancode.com\
