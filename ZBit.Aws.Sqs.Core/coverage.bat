%USERPROFILE%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe -register:user -filter:"+[Tms*]* -[*]*.DbModel* -[*]*ProjectInstaller  -[*]Tms.WinSvc.Program" -excludebyfile:*\*Designer.cs "-target:C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\MSTest.exe" "-targetargs:/testcontainer:ZBit.Aws.Sqs.CoreTests\bin\Debug\netcoreapp1.1\ZBit.Aws.Sqs.CoreTests.dll"
pause
#.\packages\ReportGenerator.2.4.5.0\tools\ReportGenerator.exe "-reports:results.xml" "-targetdir:.\coverage"
pause