rd /s /q code
set classpath=.\antlr-4.8-complete.jar;%classpath%
java org.antlr.v4.Tool CalculateEvaluate.g4 -o code -Dlanguage=CSharp -package AppliedResearchAssociates.CalculateEvaluate
if %errorlevel%==0 exit
