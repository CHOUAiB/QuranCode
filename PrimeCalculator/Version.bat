IF not exist "NET2" NET2.bat
Tools\Replace\bin\Release\Replace.exe NET2 Version.bat 000 631
Tools\Version\bin\Release\Version.exe .               6.19.631.4 6.19.631.4 -Tools
Tools\Touch\bin\Release\Touch.exe Build               6:19                  -Tools
Tools\Touch\bin\Release\Touch.exe Tools                            2009-07-29 7:29

Tools\Touch\bin\Release\Touch.exe .                   6:19                  -Tools
COPY PrimeCalculator.zip         ..\Backup\PrimeCalculator_6.19.631.zip
COPY PrimeCalculator.Source.zip  ..\Backup\PrimeCalculator_6.19.631.Source.zip
MOVE PrimeCalculator.zip         ..\qurancode.com\
MOVE PrimeCalculator.Source.zip  ..\qurancode.com\
