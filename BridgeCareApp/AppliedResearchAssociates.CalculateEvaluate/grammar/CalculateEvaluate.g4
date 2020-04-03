grammar CalculateEvaluate
   ;

root
   : calc # calculation
   | eval # evaluation
   ;

calc
   : ID '(' args ')'              # invocation
   | '-' calc                     # negation
   | left = calc '*' right = calc # multiplication
   | left = calc '/' right = calc # division
   | left = calc '+' right = calc # addition
   | left = calc '-' right = calc # subtraction
   | '[' ID ']'                   # parameterReference
   | ID                           # constantReference
   | (NUMBER | '|' NUMBER '|')    # numericLiteral
   | '(' calc ')'                 # calculationGrouping
   ;

args
   : calc (',' calc)*
   ;

eval
   : left = eval AND right = eval # and
   | left = eval OR right = eval  # or
   | numCmp        # numberComparison
   | strCmp        # stringComparison
   | '(' eval ')'  # evaluationGrouping
   ;

numCmp
   : left = calc '=' right = calc  # equal
   | left = calc '<>' right = calc # notEqual
   | left = calc '<' right = calc  # lessThan
   | left = calc '<=' right = calc # lessThanOrEqual
   | left = calc '>=' right = calc # greaterThanOrEqual
   | left = calc '>' right = calc  # greaterThan
   ;

strCmp
   : left = str '=' right = str  # equal
   | left = str '<>' right = str # notEqual
   ;

dateCmp
   : left = calc '=' right = calc  # equal
   | left = calc '<>' right = calc # notEqual
   | left = calc '<' right = calc  # lessThan
   | left = calc '<=' right = calc # lessThanOrEqual
   | left = calc '>=' right = calc # greaterThanOrEqual
   | left = calc '>' right = calc  # greaterThan
   ;

str
   :
   ;

date
   :
   ;

eqOp
   : calc
   | str
   | date
   ;

cmpOp
   : calc
   | date
   ;

ID
   : NON_DIGIT (NON_DIGIT | DIGIT)*
   ;

NUMBER
   : MANTISSA_PART EXPONENT_PART?
   ;

AND
   : [aA] [nN] [dD]
   ;

OR
   : [oO] [rR]
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
