@echo off
:START

del /F /S /Q *.config

del /F /Q Translations\*.txt
rd /S /Q Bookmarks
rd /S /Q History
rd /S /Q Drawings
rd /S /Q Statistics
rd /S /Q Research
rd /S /Q Composites

:END
