IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 523
Tools\Version\bin\Release\Version.exe .               6.19.523.4 6.19.523.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools    2009-07-29 7:29
COPY QuranCode.zip                          QuranCode_6.19.523.NET4.zip
COPY QuranCode.Source.zip                   QuranCode_6.19.523.Source.zip
Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
MOVE                                   NET2\QuranCode_6.19.523.NET2.zip     .
