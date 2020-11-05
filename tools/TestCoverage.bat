cd ..
dotnet test /p:CollectCoverage=true /p:CoverletOutput="./TestResults/"  /p:CoverletOutputFormat=\"json,cobertura\" /maxcpucount:1
reportgenerator -reports:./*.Tests/TestResults/coverage.cobertura.xml -targetdir:./TestResults/CoverageOutput/ -reporttypes:Html -title:"MSF USB Project"
pause