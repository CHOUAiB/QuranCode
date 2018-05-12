SET NET2.0=C:\Windows\Microsoft.NET\Framework\v2.0.50727
SET NET3.5=C:\Windows\Microsoft.NET\Framework\v3.5

CALL ClearAll.bat

cd Evaluator
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /reference:%NET2.0%\System.XML.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\Evaluator.dll /target:library Properties\AssemblyInfo.cs Context.cs Expression.cs Function.cs Functions.cs Parser.cs ParseTree.cs ParseTreeEvaluator.cs Scanner.cs Variables.cs
cd ..

cd Controls
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /reference:%NET2.0%\System.Drawing.dll /reference:%NET2.0%\System.Windows.Forms.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\Controls.dll /target:library Properties\AssemblyInfo.cs RichTextBoxEx.cs ListBoxEx.cs PictureBoxEx.cs
cd ..

cd FontBuilder
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /reference:%NET2.0%\System.Drawing.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\FontBuilder.dll /target:library Properties\AssemblyInfo.cs FontBuilder.cs
cd ..

cd MP3Player
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\MP3Player.dll /target:library Properties\AssemblyInfo.cs MP3Player.cs
cd ..

cd Touch
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\Touch.exe /target:exe /win32icon:App.ico Properties\AssemblyInfo.cs Touch.cs Program.cs
cd ..

cd Version
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\Version.exe /target:exe /win32icon:App.ico Properties\AssemblyInfo.cs Program.cs
cd ..

cd WAVMaker
md bin\Release
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:%NET2.0%\System.dll /debug:pdbonly /filealign:512 /optimize+ /out:bin\Release\WAVMaker.dll /target:library Properties\AssemblyInfo.cs WAVFile.cs WAVFileExceptions.cs WAVFormat.cs
cd ..
