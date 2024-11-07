dotnet clean 
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet restore 
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet build --no-restore 
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./coverage
reportgenerator -reports:coverage/**/coverage.cobertura.xml -targetdir:./coverage/report -reporttypes:Html
start coverage/report/index.html