@echo off
:START

del /F /S /Q *.ini

del /F /Q Translations\*.txt
rd /S /Q Bookmarks
rd /S /Q History
rd /S /Q Statistics

:END
