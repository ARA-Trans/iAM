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
   | IDENTIFIER                                                              # constantReference
   | calculationParameterReference                                           # numberParameterReference
   | LEFT_PAREN calculation RIGHT_PAREN                                      # calculationGrouping
   ;

evaluation
   : left = evaluation AND right = evaluation                              # logicalConjunction
   | left = evaluation OR right = evaluation                               # logicalDisjunction
   | evaluationParameterReference EQUAL EVALUATION_LITERAL                 # equal
   | evaluationParameterReference NOT_EQUAL EVALUATION_LITERAL             # notEqual
   | evaluationParameterReference LESS_THAN EVALUATION_LITERAL             # lessThan
   | evaluationParameterReference LESS_THAN_OR_EQUAL EVALUATION_LITERAL    # lessThanOrEqual
   | evaluationParameterReference GREATER_THAN_OR_EQUAL EVALUATION_LITERAL # greaterThanOrEqual
   | evaluationParameterReference GREATER_THAN EVALUATION_LITERAL          # greaterThan
   | LEFT_PAREN evaluation RIGHT_PAREN                                     # evaluationGrouping
   ;

arguments
   : calculation (COMMA calculation)*
   ;

calculationParameterReference
   : LEFT_BRACKET IDENTIFIER RIGHT_BRACKET
   ;

evaluationParameterReference
   : LEFT_BRACKET TYPE_ANNOTATION? IDENTIFIER RIGHT_BRACKET
   ;
