parser grammar CalculateEvaluateParser
   ;

options
{
   tokenVocab = CalculateEvaluateLexer;
}

root
   : calculation # calculationRoot
   | evaluation  # evaluationRoot
   ;

calculation
   : IDENTIFIER LEFT_PAREN arguments RIGHT_PAREN                             # invocation
   | MINUS calculation                                                       # negation
   | left = calculation operation = (TIMES | DIVIDED_BY) right = calculation # multiplicationOrDivision
   | left = calculation operation = (PLUS | MINUS) right = calculation       # additionOrSubtraction
   | NUMBER                                                                  # numberLiteral
   | IDENTIFIER                                                              # constantReference
   | parameterReference                                                      # numberParameterReference
   | LEFT_PAREN calculation RIGHT_PAREN                                      # calculationGrouping
   ;

evaluation
   : left = evaluation AND right = evaluation                    # logicalConjunction
   | left = evaluation OR right = evaluation                     # logicalDisjunction
   | parameterReference EQUAL EVALUATION_LITERAL                 # equal
   | parameterReference NOT_EQUAL EVALUATION_LITERAL             # notEqual
   | parameterReference LESS_THAN EVALUATION_LITERAL             # lessThan
   | parameterReference LESS_THAN_OR_EQUAL EVALUATION_LITERAL    # lessThanOrEqual
   | parameterReference GREATER_THAN_OR_EQUAL EVALUATION_LITERAL # greaterThanOrEqual
   | parameterReference GREATER_THAN EVALUATION_LITERAL          # greaterThan
   | LEFT_PAREN evaluation RIGHT_PAREN                           # evaluationGrouping
   ;

arguments
   : calculation (COMMA calculation)*
   ;

parameterReference
   : LEFT_BRACKET IDENTIFIER RIGHT_BRACKET
   ;
