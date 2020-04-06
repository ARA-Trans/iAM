lexer grammar CalculateEvaluateLexer
   ;

IDENTIFIER
   : NON_DIGIT (NON_DIGIT | DIGIT)*
   ;

NUMBER
   : MANTISSA_PART EXPONENT_PART?
   ;

LEFT_PAREN
   : '('
   ;

RIGHT_PAREN
   : ')'
   ;

TIMES
   : '*'
   ;

DIVIDED_BY
   : '/'
   ;

PLUS
   : '+'
   ;

MINUS
   : '-'
   ;

AND
   : [aA] [nN] [dD]
   ;

OR
   : [oO] [rR]
   ;

EQUAL
   : '='
   ;

NOT_EQUAL
   : '<>'
   ;

LESS_THAN
   : '<'
   ;

LESS_THAN_OR_EQUAL
   : '<='
   ;

GREATER_THAN_OR_EQUAL
   : '>='
   ;

GREATER_THAN
   : '>'
   ;

COMMA
   : ','
   ;

LEFT_BRACKET
   : '['
   ;

RIGHT_BRACKET
   : ']'
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

EMPTY_EVALUATION_LITERAL
   : '||'
   ;

EVALUATION_LITERAL_OPENING_DELIMITER
   : '|' -> mode(EVALUATION_LITERAL_MODE)
   ;

mode EVALUATION_LITERAL_MODE
   ;

EVALUATION_LITERAL_CONTENT
   : ~'|'+
   ;

EVALUATION_LITERAL_CLOSING_DELIMITER
   : '|' -> mode(DEFAULT_MODE)
   ;
