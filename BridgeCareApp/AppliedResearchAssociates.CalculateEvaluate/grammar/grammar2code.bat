rd /s /q code
java -cp .\antlr-4.8-complete.jar;%classpath% org.antlr.v4.Tool CalculateEvaluate.g4 -o code -Dlanguage=CSharp -package AppliedResearchAssociates.CalculateEvaluate
if %errorlevel%==0 exit
