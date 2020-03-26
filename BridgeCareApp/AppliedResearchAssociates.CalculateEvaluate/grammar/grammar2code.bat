rd /s /q code
.\antlr.bat CalculateEvaluate.g4 -o code -Dlanguage=CSharp -package AppliedResearchAssociates.CalculateEvaluate
if %errorlevel%==0 exit
