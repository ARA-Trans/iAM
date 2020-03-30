grammar CalculateEvaluate;

calc
   : calc ('*' | '/') calc
   | calc ('+' | '-') calc
   | ID '(' args? ')'
   | '[' ID ']'
   | NUMBER
   ;

args : calc (',' calc)* ;

ID : NON_DIGIT (NON_DIGIT | DIGIT)* ;

NUMBER : '-'? NATURAL_NUMBER ('.' DIGIT+)? ([eE] ('-' | '+')? NATURAL_NUMBER)? ;

fragment NATURAL_NUMBER
   : '0'
   | [1-9] DIGIT*
   ;

fragment DIGIT : [0-9] ;
fragment NON_DIGIT : [a-zA-Z_] ;

WS : [ \t\r\n]+ -> skip ;
