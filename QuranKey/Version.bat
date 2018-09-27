IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 741
Tools\Version\bin\Release\Version.exe .               6.19.741.4 6.19.741.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools                            2009-07-29 7:29
 
Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY QuranKey.zip                  ..\Backup\QuranKey_6.19.741.zip
COPY QuranKey.Source.zip           ..\Backup\QuranKey_6.19.741.Source.zip
MOVE QuranKey.zip                  ..\Backup\
MOVE QuranKey.Source.zip           ..\Backup\
