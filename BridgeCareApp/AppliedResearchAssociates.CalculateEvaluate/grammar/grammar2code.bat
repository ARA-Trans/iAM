rd /s /q code
call .\antlr.bat CalculateEvaluateLexer.g4 -o code -Dlanguage=CSharp -package AppliedResearchAssociates.CalculateEvaluate
call .\antlr.bat CalculateEvaluateParser.g4 -o code -Dlanguage=CSharp -package AppliedResearchAssociates.CalculateEvaluate -listener -visitor
