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
   : IDENTIFIER LEFT_PAREN arguments RIGHT_PAREN       # invocation
   | MINUS calculation                                 # negation
   | left = calculation TIMES right = calculation      # multiplication
   | left = calculation DIVIDED_BY right = calculation # division
   | left = calculation PLUS right = calculation       # addition
   | left = calculation MINUS right = calculation      # subtraction
   | NUMBER                                            # numberLiteral
   | IDENTIFIER                                        # constantReference
   | parameterReference                                # numberParameterReference
   | LEFT_PAREN calculation RIGHT_PAREN                # calculationGrouping
   ;

evaluation
   : left = evaluation AND right = evaluation                   # logicalConjunction
   | left = evaluation OR right = evaluation                    # logicalDisjunction
   | parameterReference EQUAL evaluationLiteral                 # equal
   | parameterReference NOT_EQUAL evaluationLiteral             # notEqual
   | parameterReference LESS_THAN evaluationLiteral             # lessThan
   | parameterReference LESS_THAN_OR_EQUAL evaluationLiteral    # lessThanOrEqual
   | parameterReference GREATER_THAN_OR_EQUAL evaluationLiteral # greaterThanOrEqual
   | parameterReference GREATER_THAN evaluationLiteral          # greaterThan
   | LEFT_PAREN evaluation RIGHT_PAREN                          # evaluationGrouping
   ;

arguments
   : calculation (COMMA calculation)*
   ;

parameterReference
   : LEFT_BRACKET IDENTIFIER RIGHT_BRACKET
   ;

evaluationLiteral
   : EMPTY_EVALUATION_LITERAL
   | EVALUATION_LITERAL_OPENING_DELIMITER EVALUATION_LITERAL_CONTENT EVALUATION_LITERAL_CLOSING_DELIMITER
   ;
