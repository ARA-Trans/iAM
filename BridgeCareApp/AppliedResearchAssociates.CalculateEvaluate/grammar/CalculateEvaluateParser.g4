parser grammar CalculateEvaluateParser
   ;

options
{
   tokenVocab = CalculateEvaluateLexer;
}

root
   : calculation EOF # calculationRoot
   | evaluation EOF  # evaluationRoot
   ;

calculation
   : IDENTIFIER LEFT_PAREN arguments RIGHT_PAREN                             # invocation
   | MINUS calculation                                                       # negation
   | left = calculation operation = (TIMES | DIVIDED_BY) right = calculation # multiplicationOrDivision
   | left = calculation operation = (PLUS | MINUS) right = calculation       # additionOrSubtraction
   | NUMBER                                                                  # numberLiteral
   | IDENTIFIER                                                              # numberReference
   | LEFT_BRACKET IDENTIFIER RIGHT_BRACKET                                   # numberParameterReference
   | LEFT_PAREN calculation RIGHT_PAREN                                      # calculationGrouping
   ;

arguments
   : calculation (COMMA calculation)*
   ;

evaluation
   : left = evaluation AND right = evaluation                   # logicalConjunction
   | left = evaluation OR right = evaluation                    # logicalDisjunction
   | parameterReference EQUAL comparisonOperand                 # equal
   | parameterReference NOT_EQUAL comparisonOperand             # notEqual
   | parameterReference LESS_THAN comparisonOperand             # lessThan
   | parameterReference LESS_THAN_OR_EQUAL comparisonOperand    # lessThanOrEqual
   | parameterReference GREATER_THAN_OR_EQUAL comparisonOperand # greaterThanOrEqual
   | parameterReference GREATER_THAN comparisonOperand          # greaterThan
   | LEFT_PAREN evaluation RIGHT_PAREN                          # evaluationGrouping
   ;

parameterReference
   : IDENTIFIER
   | LEFT_BRACKET IDENTIFIER RIGHT_BRACKET
   ;

comparisonOperand
   : parameterReference
   | literal
   | NUMBER
   ;

literal
   : LITERAL_OPENING_DELIMITER_1 content = LITERAL_CONTENT_1? LITERAL_CLOSING_DELIMITER_1
   | LITERAL_OPENING_DELIMITER_2 content = LITERAL_CONTENT_2? LITERAL_CLOSING_DELIMITER_2
   ;
