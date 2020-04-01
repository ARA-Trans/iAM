grammar CalculateEvaluate
   ;

root
   : calc # Calculation
   | eval # Evaluation
   ;

calc
   : ID '(' args? ')'             # Invocation
   | '-' calc                     # Negation
   | left = calc '*' right = calc # Multiplication
   | left = calc '/' right = calc # Division
   | left = calc '+' right = calc # Addition
   | left = calc '-' right = calc # Subtraction
   | '[' ID ']'                   # ParameterReference
   | ID                           # ConstantReference
   | NUMBER                       # NumericLiteral
   | '(' calc ')'                 # Grouping
   ;

// seems to have odd syntax with pipes delimiting both strings and numbers. define this before
// implementing calc, since we probably want to use visitor but should suss everything out first.
eval
   :
   ;

args
   : calc (',' calc)*
   ;

ID
   : NON_DIGIT (NON_DIGIT | DIGIT)*
   ;

NUMBER
   : MANTISSA_PART EXPONENT_PART?
   ;

WS
   : [ \t\r\n]+ -> skip
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

fragment DIGIT
   : [0-9]
   ;

fragment NON_DIGIT
   : [a-zA-Z_]
   ;
