SET NET2.0=C:\Windows\Microsoft.NET\Framework\v2.0.50727
SET NET3.5=C:\Windows\Microsoft.NET\Framework\v3.5

cd Tools
CALL BuildAll.bat
cd ..

rd /S /Q Build
md Build\Release

cd Globals
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\Globals.dll /target:library Properties\AssemblyInfo.cs Globals.cs
cd ..

cd Utilities
xcopy /E /Q Numbers\*.* ..\Build\Release\Numbers\*.*
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\Globals.dll /reference:%NET2.0%\System.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\Utilities.dll /target:library Properties\AssemblyInfo.cs Combinatorics\Combinations.cs Combinatorics\GenerateOption.cs Combinatorics\IMetaCollection.cs Combinatorics\Permutations.cs Combinatorics\SmallPrimeUtility.cs Combinatorics\Variations.cs Numbers.cs Downloader.cs Evaluator.cs ExtensionMethods.cs Radix.cs PublicStorage.cs Subsets.cs
cd ..

cd Model
xcopy /E /Q Data\*.* ..\Build\Release\Data\*.*
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\Globals.dll /reference:%NET2.0%\System.dll /reference:..\Build\Release\Utilities.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\Model.dll /target:library Properties\AssemblyInfo.cs Book.cs Bookmark.cs Chapter.cs Sentence.cs SimplificationRule.cs NumberQuery.cs SearchHistoryItem.cs SelectionHistoryItem.cs SimplificationSystem.cs NumerologySystem.cs Letter.cs Statistic.cs TranslationInfo.cs RecitationInfo.cs Selection.cs Phrase.cs Half.cs Quarter.cs Bowing.cs Group.cs Part.cs Station.cs Page.cs Verse.cs Enums.cs Word.cs
cd ..

cd DataAccess
xcopy /E /Q Audio\*.* ..\Build\Release\Audio\*.*
xcopy /E /Q Data\*.* ..\Build\Release\Data\*.*
xcopy /E /Q Translations\*.* ..\Build\Release\Translations\*.*
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\Globals.dll /reference:..\Build\Release\Model.dll /reference:%NET2.0%\System.dll /reference:..\Build\Release\Utilities.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\DataAccess.dll /target:library Properties\AssemblyInfo.cs DataAccess.cs
cd ..

cd Server
xcopy /E /Q Data\*.* ..\Build\Release\Data\*.*
xcopy /E /Q Help\*.* ..\Build\Release\Help\*.*
xcopy    /Q Rules\*.* ..\Build\Release\Rules\*.*
xcopy /E /Q Values\*.* ..\Build\Release\Values\*.*
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\DataAccess.dll /reference:..\Build\Release\Globals.dll /reference:..\Build\Release\Model.dll /reference:%NET2.0%\System.dll /reference:..\Build\Release\Utilities.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\Server.dll /target:library Properties\AssemblyInfo.cs Server.cs
cd ..

cd Client
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\Globals.dll /reference:..\Build\Release\Model.dll /reference:..\Build\Release\Server.dll /reference:%NET2.0%\System.dll /reference:..\Build\Release\Utilities.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\Client.dll /target:library Properties\AssemblyInfo.cs Client.cs
cd ..

cd QuranCode
copy Clean.bat ..\Build\Release
copy Readme.txt ..\Build\Release
xcopy /E /Q Images\*.* ..\Build\Release\Images\*.*
xcopy /E /Q Fonts\*.* ..\Build\Release\Fonts\*.*
..\Tools\SDK\resgen /compile AboutBox.resx MainForm.resx SplashForm.resx
%NET3.5%\Csc.exe /noconfig /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE;RELEASE /reference:..\Build\Release\Client.dll /reference:..\Tools\Controls\bin\Release\Controls.dll /reference:..\Tools\FontBuilder\bin\Release\FontBuilder.dll /reference:..\Build\Release\Globals.dll /reference:..\Build\Release\Model.dll /reference:..\Tools\MP3Player\bin\Release\MP3Player.dll /reference:%NET2.0%\System.dll /reference:%NET2.0%\System.Drawing.dll /reference:%NET2.0%\System.Windows.Forms.dll /reference:..\Build\Release\Utilities.dll /debug:pdbonly /filealign:512 /optimize+ /out:..\Build\Release\QuranCode.exe /resource:AboutBox.resources /resource:MainForm.resources /resource:SplashForm.resources /target:winexe Properties\AssemblyInfo.cs /win32icon:App.ico AboutBox.cs AboutBox.Designer.cs Program.cs MainForm.cs MainForm.Designer.cs SplashForm.cs SplashForm.Designer.cs
copy ..\Tools\Controls\bin\Release\Controls.dll ..\Build\Release
copy ..\Tools\FontBuilder\bin\Release\FontBuilder.dll ..\Build\Release
copy ..\Tools\MP3Player\bin\Release\MP3Player.dll ..\Build\Release
cd ..

CALL Clean.bat
