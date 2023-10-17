@ECHO OFF

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage.opencover.xml

reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report