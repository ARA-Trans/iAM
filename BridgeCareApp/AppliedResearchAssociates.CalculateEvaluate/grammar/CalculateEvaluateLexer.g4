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

fragment DECIMAL_PART
   : '.' NATURAL_NUMBER
   ;

fragment NATURAL_NUMBER
   : DIGIT+
   ;

// Evaluation literal

LITERAL_OPENING_DELIMITER_1
   : LITERAL_DELIMITER_1 -> pushMode(LITERAL_MODE_1)
   ;

LITERAL_OPENING_DELIMITER_2
   : LITERAL_DELIMITER_2 -> pushMode(LITERAL_MODE_2)
   ;

fragment LITERAL_DELIMITER_1
   : '|'
   ;

fragment LITERAL_DELIMITER_2
   : '\''
   ;

mode LITERAL_MODE_1
   ;

LITERAL_CLOSING_DELIMITER_1
   : LITERAL_DELIMITER_1 -> popMode
   ;

LITERAL_CONTENT_1
   : ~'|'+
   ;

mode LITERAL_MODE_2
   ;

LITERAL_CLOSING_DELIMITER_2
   : LITERAL_DELIMITER_2 -> popMode
   ;

LITERAL_CONTENT_2
   : ~'\''+
   ;
