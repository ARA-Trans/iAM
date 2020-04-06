grammar CalculateEvaluate
   ;

// "Calculate" grammar

root
   : calculation # calculationRoot
   | evaluation  # evaluationRoot
   ;

calculation
   : IDENTIFIER '(' arguments ')'               # invocation
   | '-' calculation                            # negation
   | left = calculation '*' right = calculation # multiplication
   | left = calculation '/' right = calculation # division
   | left = calculation '+' right = calculation # addition
   | left = calculation '-' right = calculation # subtraction
   | NUMBER                                     # numberLiteral
   | IDENTIFIER                                 # constantReference
   | parameterReference                         # numberParameterReference
   | '(' calculation ')'                        # calculationGrouping
   ;

arguments
   : calculation (',' calculation)*
   ;

NUMBER
   : MANTISSA_PART EXPONENT_PART?
   ;

fragment MANTISSA_PART
   : NATURAL_NUMBER DECIMAL_PART?
   | DECIMAL_PART
   ;

fragment DECIMAL_PART
   : '.' DIGIT+
   ;

fragment EXPONENT_PART
   : [eE] ('-' | '+')? NATURAL_NUMBER
   ;

fragment NATURAL_NUMBER
   : '0'
   | [1-9] DIGIT*
   ;

// "Evaluate" sub-grammar

evaluation
   : left = evaluation AND right = evaluation  # logicalConjunction
   | left = evaluation OR right = evaluation   # logicalDisjunction
   | parameterReference '=' evaluationLiteral  # equal
   | parameterReference '<>' evaluationLiteral # notEqual
   | parameterReference '<' evaluationLiteral  # lessThan
   | parameterReference '<=' evaluationLiteral # lessThanOrEqual
   | parameterReference '>=' evaluationLiteral # greaterThanOrEqual
   | parameterReference '>' evaluationLiteral  # greaterThan
   | '(' evaluation ')'                        # evaluationGrouping
   ;

evaluationLiteral
   : '|' content = ~'|'* '|'
   ;

AND
   : [aA] [nN] [dD]
   ;

OR
   : [oO] [rR]
   ;

// Common "Calculate" and "Evaluate" rules

parameterReference
   : '[' IDENTIFIER ']'
   ;

IDENTIFIER
   : NON_DIGIT (NON_DIGIT | DIGIT)*
   ;

WHITESPACE
   : [ \t\r\n]+ -> channel(HIDDEN)
   ;

fragment DIGIT
   : [0-9]
   ;

fragment NON_DIGIT
   : [a-zA-Z_]
   ;
