IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 563
Tools\Version\bin\Release\Version.exe .               6.19.563.4 6.19.563.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools    2009-07-29 7:29
MOVE                              NET2\QuranCode1433.2.zip                  .
Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY QuranCode1433.zip        ..\Backup\QuranCode1433_6.19.563.4.zip
COPY QuranCode1433.Source.zip ..\Backup\QuranCode1433_6.19.563.Source.zip
COPY QuranCode1433.2.zip      ..\Backup\QuranCode1433_6.19.563.2.zip
