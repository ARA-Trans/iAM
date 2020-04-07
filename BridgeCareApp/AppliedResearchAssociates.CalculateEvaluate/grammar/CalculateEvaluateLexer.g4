lexer grammar CalculateEvaluateLexer
   ;

WHITESPACE
   : [ \t\r\n]+ -> channel(HIDDEN)
   ;

// Keywords

AND
   : [aA] [nN] [dD]
   ;

OR
   : [oO] [rR]
   ;

// Fixed literals

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

// Custom literals

IDENTIFIER
   : LETTER (LETTER | DIGIT)*
   ;

NUMBER
   : MANTISSA_PART EXPONENT_PART?
   ;

fragment LETTER
   : [a-zA-Z_]
   ;

fragment DIGIT
   : [0-9]
   ;

fragment MANTISSA_PART
   : NATURAL_NUMBER DECIMAL_PART?
   | DECIMAL_PART
   ;

fragment EXPONENT_PART
   : [eE] ('-' | '+')? NATURAL_NUMBER
   ;

fragment NATURAL_NUMBER
   : '0'
   | [1-9] DIGIT*
   ;

fragment DECIMAL_PART
   : '.' DIGIT+
   ;

// Evaluation literal

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
