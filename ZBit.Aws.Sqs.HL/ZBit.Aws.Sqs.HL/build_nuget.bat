C:\Users\gzinger\Downloads\DevTools\nuget pack ZBit.Aws.Sqs.HL.csproj -Prop Configuration=Release -Symbols
pause
C:\Users\gzinger\Downloads\DevTools\nuget push ZBit.Aws.Sqs.HL.1.1.0.0.nupkg -Source nuget.org
pause
#nuget push ZBit.Aws.Sqs.HL.1.1.0.0.symbols.nupkg
