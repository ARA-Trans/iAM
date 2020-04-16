lexer grammar CalculateEvaluateLexer
   ;

tokens
{
   EVALUATION_LITERAL
}

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

TYPE_ANNOTATION
   : TEXT_TYPE_ANNOTATION
   | TIMESTAMP_TYPE_ANNOTATION
   ;

TEXT_TYPE_ANNOTATION
   : '@'
   ;

TIMESTAMP_TYPE_ANNOTATION
   : '$'
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

EVALUATION_LITERAL_1_OPENING_DELIMITER
   : '\'' -> more, pushMode(EVALUATION_LITERAL_1_MODE)
   ;

EVALUATION_LITERAL_2_OPENING_DELIMITER
   : '|' -> more, pushMode(EVALUATION_LITERAL_2_MODE)
   ;

mode EVALUATION_LITERAL_1_MODE
   ;

EVALUATION_LITERAL_1_CLOSING_DELIMITER
   : '\'' -> popMode, type(EVALUATION_LITERAL)
   ;

EVALUATION_LITERAL_1_CONTENT
   : . -> more
   ;

mode EVALUATION_LITERAL_2_MODE
   ;

EVALUATION_LITERAL_2_CLOSING_DELIMITER
   : '|' -> popMode, type(EVALUATION_LITERAL)
   ;

EVALUATION_LITERAL_2_CONTENT
   : . -> more
   ;
