//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from CalculateEvaluateLexer.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace AppliedResearchAssociates.CalculateEvaluate {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public partial class CalculateEvaluateLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		EVALUATION_LITERAL=1, WHITESPACE=2, AND=3, OR=4, LEFT_PAREN=5, RIGHT_PAREN=6, 
		TIMES=7, DIVIDED_BY=8, PLUS=9, MINUS=10, EQUAL=11, NOT_EQUAL=12, LESS_THAN=13, 
		LESS_THAN_OR_EQUAL=14, GREATER_THAN_OR_EQUAL=15, GREATER_THAN=16, COMMA=17, 
		LEFT_BRACKET=18, RIGHT_BRACKET=19, IDENTIFIER=20, NUMBER=21, EVALUATION_LITERAL_1_OPENING_DELIMITER=22, 
		EVALUATION_LITERAL_2_OPENING_DELIMITER=23;
	public const int
		EVALUATION_LITERAL_1_MODE=1, EVALUATION_LITERAL_2_MODE=2;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE", "EVALUATION_LITERAL_1_MODE", "EVALUATION_LITERAL_2_MODE"
	};

	public static readonly string[] ruleNames = {
		"WHITESPACE", "AND", "OR", "LEFT_PAREN", "RIGHT_PAREN", "TIMES", "DIVIDED_BY", 
		"PLUS", "MINUS", "EQUAL", "NOT_EQUAL", "LESS_THAN", "LESS_THAN_OR_EQUAL", 
		"GREATER_THAN_OR_EQUAL", "GREATER_THAN", "COMMA", "LEFT_BRACKET", "RIGHT_BRACKET", 
		"IDENTIFIER", "NUMBER", "LETTER", "DIGIT", "MANTISSA_PART", "EXPONENT_PART", 
		"DECIMAL_PART", "NATURAL_NUMBER", "EVALUATION_LITERAL_1_OPENING_DELIMITER", 
		"EVALUATION_LITERAL_2_OPENING_DELIMITER", "EVALUATION_LITERAL_1_CLOSING_DELIMITER", 
		"EVALUATION_LITERAL_1_CONTENT", "EVALUATION_LITERAL_2_CLOSING_DELIMITER", 
		"EVALUATION_LITERAL_2_CONTENT"
	};


	public CalculateEvaluateLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CalculateEvaluateLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, "'('", "')'", "'*'", "'/'", "'+'", "'-'", 
		"'='", "'<>'", "'<'", "'<='", "'>='", "'>'", "','", "'['", "']'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "EVALUATION_LITERAL", "WHITESPACE", "AND", "OR", "LEFT_PAREN", "RIGHT_PAREN", 
		"TIMES", "DIVIDED_BY", "PLUS", "MINUS", "EQUAL", "NOT_EQUAL", "LESS_THAN", 
		"LESS_THAN_OR_EQUAL", "GREATER_THAN_OR_EQUAL", "GREATER_THAN", "COMMA", 
		"LEFT_BRACKET", "RIGHT_BRACKET", "IDENTIFIER", "NUMBER", "EVALUATION_LITERAL_1_OPENING_DELIMITER", 
		"EVALUATION_LITERAL_2_OPENING_DELIMITER"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "CalculateEvaluateLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static CalculateEvaluateLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x19', '\xB5', '\b', '\x1', '\b', '\x1', '\b', '\x1', 
		'\x4', '\x2', '\t', '\x2', '\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', 
		'\x4', '\x4', '\x5', '\t', '\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', 
		'\t', '\a', '\x4', '\b', '\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', 
		'\t', '\n', '\x4', '\v', '\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', 
		'\t', '\r', '\x4', '\xE', '\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', 
		'\x10', '\t', '\x10', '\x4', '\x11', '\t', '\x11', '\x4', '\x12', '\t', 
		'\x12', '\x4', '\x13', '\t', '\x13', '\x4', '\x14', '\t', '\x14', '\x4', 
		'\x15', '\t', '\x15', '\x4', '\x16', '\t', '\x16', '\x4', '\x17', '\t', 
		'\x17', '\x4', '\x18', '\t', '\x18', '\x4', '\x19', '\t', '\x19', '\x4', 
		'\x1A', '\t', '\x1A', '\x4', '\x1B', '\t', '\x1B', '\x4', '\x1C', '\t', 
		'\x1C', '\x4', '\x1D', '\t', '\x1D', '\x4', '\x1E', '\t', '\x1E', '\x4', 
		'\x1F', '\t', '\x1F', '\x4', ' ', '\t', ' ', '\x4', '!', '\t', '!', '\x3', 
		'\x2', '\x6', '\x2', 'G', '\n', '\x2', '\r', '\x2', '\xE', '\x2', 'H', 
		'\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', 
		'\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', 
		'\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', 
		'\x3', '\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', 
		'\r', '\x3', '\r', '\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', 
		'\x3', '\xF', '\x3', '\xF', '\x3', '\x10', '\x3', '\x10', '\x3', '\x11', 
		'\x3', '\x11', '\x3', '\x12', '\x3', '\x12', '\x3', '\x13', '\x3', '\x13', 
		'\x3', '\x14', '\x3', '\x14', '\x3', '\x14', '\a', '\x14', 'x', '\n', 
		'\x14', '\f', '\x14', '\xE', '\x14', '{', '\v', '\x14', '\x3', '\x15', 
		'\x3', '\x15', '\x5', '\x15', '\x7F', '\n', '\x15', '\x3', '\x16', '\x3', 
		'\x16', '\x3', '\x17', '\x3', '\x17', '\x3', '\x18', '\x3', '\x18', '\x5', 
		'\x18', '\x87', '\n', '\x18', '\x3', '\x18', '\x5', '\x18', '\x8A', '\n', 
		'\x18', '\x3', '\x19', '\x3', '\x19', '\x5', '\x19', '\x8E', '\n', '\x19', 
		'\x3', '\x19', '\x3', '\x19', '\x3', '\x1A', '\x3', '\x1A', '\x3', '\x1A', 
		'\x3', '\x1B', '\x6', '\x1B', '\x96', '\n', '\x1B', '\r', '\x1B', '\xE', 
		'\x1B', '\x97', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', '\x3', '\x1C', 
		'\x3', '\x1C', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1D', '\x3', '\x1D', 
		'\x3', '\x1D', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1E', '\x3', '\x1E', 
		'\x3', '\x1E', '\x3', '\x1F', '\x3', '\x1F', '\x3', '\x1F', '\x3', '\x1F', 
		'\x3', ' ', '\x3', ' ', '\x3', ' ', '\x3', ' ', '\x3', ' ', '\x3', '!', 
		'\x3', '!', '\x3', '!', '\x3', '!', '\x2', '\x2', '\"', '\x5', '\x4', 
		'\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', '\t', '\x11', 
		'\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', '\xE', '\x1B', 
		'\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', '\x13', '%', 
		'\x14', '\'', '\x15', ')', '\x16', '+', '\x17', '-', '\x2', '/', '\x2', 
		'\x31', '\x2', '\x33', '\x2', '\x35', '\x2', '\x37', '\x2', '\x39', '\x18', 
		';', '\x19', '=', '\x2', '?', '\x2', '\x41', '\x2', '\x43', '\x2', '\x5', 
		'\x2', '\x3', '\x4', '\f', '\x5', '\x2', '\v', '\f', '\xF', '\xF', '\"', 
		'\"', '\x4', '\x2', '\x43', '\x43', '\x63', '\x63', '\x4', '\x2', 'P', 
		'P', 'p', 'p', '\x4', '\x2', '\x46', '\x46', '\x66', '\x66', '\x4', '\x2', 
		'Q', 'Q', 'q', 'q', '\x4', '\x2', 'T', 'T', 't', 't', '\x5', '\x2', '\x43', 
		'\\', '\x61', '\x61', '\x63', '|', '\x3', '\x2', '\x32', ';', '\x4', '\x2', 
		'G', 'G', 'g', 'g', '\x4', '\x2', '-', '-', '/', '/', '\x2', '\xB4', '\x2', 
		'\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x11', '\x3', '\x2', '\x2', '\x2', '\x2', '\x13', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x15', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x17', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1D', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1F', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '!', '\x3', '\x2', '\x2', '\x2', '\x2', '#', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '%', '\x3', '\x2', '\x2', '\x2', '\x2', '\'', '\x3', '\x2', 
		'\x2', '\x2', '\x2', ')', '\x3', '\x2', '\x2', '\x2', '\x2', '+', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\x39', '\x3', '\x2', '\x2', '\x2', '\x2', 
		';', '\x3', '\x2', '\x2', '\x2', '\x3', '=', '\x3', '\x2', '\x2', '\x2', 
		'\x3', '?', '\x3', '\x2', '\x2', '\x2', '\x4', '\x41', '\x3', '\x2', '\x2', 
		'\x2', '\x4', '\x43', '\x3', '\x2', '\x2', '\x2', '\x5', '\x46', '\x3', 
		'\x2', '\x2', '\x2', '\a', 'L', '\x3', '\x2', '\x2', '\x2', '\t', 'P', 
		'\x3', '\x2', '\x2', '\x2', '\v', 'S', '\x3', '\x2', '\x2', '\x2', '\r', 
		'U', '\x3', '\x2', '\x2', '\x2', '\xF', 'W', '\x3', '\x2', '\x2', '\x2', 
		'\x11', 'Y', '\x3', '\x2', '\x2', '\x2', '\x13', '[', '\x3', '\x2', '\x2', 
		'\x2', '\x15', ']', '\x3', '\x2', '\x2', '\x2', '\x17', '_', '\x3', '\x2', 
		'\x2', '\x2', '\x19', '\x61', '\x3', '\x2', '\x2', '\x2', '\x1B', '\x64', 
		'\x3', '\x2', '\x2', '\x2', '\x1D', '\x66', '\x3', '\x2', '\x2', '\x2', 
		'\x1F', 'i', '\x3', '\x2', '\x2', '\x2', '!', 'l', '\x3', '\x2', '\x2', 
		'\x2', '#', 'n', '\x3', '\x2', '\x2', '\x2', '%', 'p', '\x3', '\x2', '\x2', 
		'\x2', '\'', 'r', '\x3', '\x2', '\x2', '\x2', ')', 't', '\x3', '\x2', 
		'\x2', '\x2', '+', '|', '\x3', '\x2', '\x2', '\x2', '-', '\x80', '\x3', 
		'\x2', '\x2', '\x2', '/', '\x82', '\x3', '\x2', '\x2', '\x2', '\x31', 
		'\x89', '\x3', '\x2', '\x2', '\x2', '\x33', '\x8B', '\x3', '\x2', '\x2', 
		'\x2', '\x35', '\x91', '\x3', '\x2', '\x2', '\x2', '\x37', '\x95', '\x3', 
		'\x2', '\x2', '\x2', '\x39', '\x99', '\x3', '\x2', '\x2', '\x2', ';', 
		'\x9E', '\x3', '\x2', '\x2', '\x2', '=', '\xA3', '\x3', '\x2', '\x2', 
		'\x2', '?', '\xA8', '\x3', '\x2', '\x2', '\x2', '\x41', '\xAC', '\x3', 
		'\x2', '\x2', '\x2', '\x43', '\xB1', '\x3', '\x2', '\x2', '\x2', '\x45', 
		'G', '\t', '\x2', '\x2', '\x2', '\x46', '\x45', '\x3', '\x2', '\x2', '\x2', 
		'G', 'H', '\x3', '\x2', '\x2', '\x2', 'H', '\x46', '\x3', '\x2', '\x2', 
		'\x2', 'H', 'I', '\x3', '\x2', '\x2', '\x2', 'I', 'J', '\x3', '\x2', '\x2', 
		'\x2', 'J', 'K', '\b', '\x2', '\x2', '\x2', 'K', '\x6', '\x3', '\x2', 
		'\x2', '\x2', 'L', 'M', '\t', '\x3', '\x2', '\x2', 'M', 'N', '\t', '\x4', 
		'\x2', '\x2', 'N', 'O', '\t', '\x5', '\x2', '\x2', 'O', '\b', '\x3', '\x2', 
		'\x2', '\x2', 'P', 'Q', '\t', '\x6', '\x2', '\x2', 'Q', 'R', '\t', '\a', 
		'\x2', '\x2', 'R', '\n', '\x3', '\x2', '\x2', '\x2', 'S', 'T', '\a', '*', 
		'\x2', '\x2', 'T', '\f', '\x3', '\x2', '\x2', '\x2', 'U', 'V', '\a', '+', 
		'\x2', '\x2', 'V', '\xE', '\x3', '\x2', '\x2', '\x2', 'W', 'X', '\a', 
		',', '\x2', '\x2', 'X', '\x10', '\x3', '\x2', '\x2', '\x2', 'Y', 'Z', 
		'\a', '\x31', '\x2', '\x2', 'Z', '\x12', '\x3', '\x2', '\x2', '\x2', '[', 
		'\\', '\a', '-', '\x2', '\x2', '\\', '\x14', '\x3', '\x2', '\x2', '\x2', 
		']', '^', '\a', '/', '\x2', '\x2', '^', '\x16', '\x3', '\x2', '\x2', '\x2', 
		'_', '`', '\a', '?', '\x2', '\x2', '`', '\x18', '\x3', '\x2', '\x2', '\x2', 
		'\x61', '\x62', '\a', '>', '\x2', '\x2', '\x62', '\x63', '\a', '@', '\x2', 
		'\x2', '\x63', '\x1A', '\x3', '\x2', '\x2', '\x2', '\x64', '\x65', '\a', 
		'>', '\x2', '\x2', '\x65', '\x1C', '\x3', '\x2', '\x2', '\x2', '\x66', 
		'g', '\a', '>', '\x2', '\x2', 'g', 'h', '\a', '?', '\x2', '\x2', 'h', 
		'\x1E', '\x3', '\x2', '\x2', '\x2', 'i', 'j', '\a', '@', '\x2', '\x2', 
		'j', 'k', '\a', '?', '\x2', '\x2', 'k', ' ', '\x3', '\x2', '\x2', '\x2', 
		'l', 'm', '\a', '@', '\x2', '\x2', 'm', '\"', '\x3', '\x2', '\x2', '\x2', 
		'n', 'o', '\a', '.', '\x2', '\x2', 'o', '$', '\x3', '\x2', '\x2', '\x2', 
		'p', 'q', '\a', ']', '\x2', '\x2', 'q', '&', '\x3', '\x2', '\x2', '\x2', 
		'r', 's', '\a', '_', '\x2', '\x2', 's', '(', '\x3', '\x2', '\x2', '\x2', 
		't', 'y', '\x5', '-', '\x16', '\x2', 'u', 'x', '\x5', '-', '\x16', '\x2', 
		'v', 'x', '\x5', '/', '\x17', '\x2', 'w', 'u', '\x3', '\x2', '\x2', '\x2', 
		'w', 'v', '\x3', '\x2', '\x2', '\x2', 'x', '{', '\x3', '\x2', '\x2', '\x2', 
		'y', 'w', '\x3', '\x2', '\x2', '\x2', 'y', 'z', '\x3', '\x2', '\x2', '\x2', 
		'z', '*', '\x3', '\x2', '\x2', '\x2', '{', 'y', '\x3', '\x2', '\x2', '\x2', 
		'|', '~', '\x5', '\x31', '\x18', '\x2', '}', '\x7F', '\x5', '\x33', '\x19', 
		'\x2', '~', '}', '\x3', '\x2', '\x2', '\x2', '~', '\x7F', '\x3', '\x2', 
		'\x2', '\x2', '\x7F', ',', '\x3', '\x2', '\x2', '\x2', '\x80', '\x81', 
		'\t', '\b', '\x2', '\x2', '\x81', '.', '\x3', '\x2', '\x2', '\x2', '\x82', 
		'\x83', '\t', '\t', '\x2', '\x2', '\x83', '\x30', '\x3', '\x2', '\x2', 
		'\x2', '\x84', '\x86', '\x5', '\x37', '\x1B', '\x2', '\x85', '\x87', '\x5', 
		'\x35', '\x1A', '\x2', '\x86', '\x85', '\x3', '\x2', '\x2', '\x2', '\x86', 
		'\x87', '\x3', '\x2', '\x2', '\x2', '\x87', '\x8A', '\x3', '\x2', '\x2', 
		'\x2', '\x88', '\x8A', '\x5', '\x35', '\x1A', '\x2', '\x89', '\x84', '\x3', 
		'\x2', '\x2', '\x2', '\x89', '\x88', '\x3', '\x2', '\x2', '\x2', '\x8A', 
		'\x32', '\x3', '\x2', '\x2', '\x2', '\x8B', '\x8D', '\t', '\n', '\x2', 
		'\x2', '\x8C', '\x8E', '\t', '\v', '\x2', '\x2', '\x8D', '\x8C', '\x3', 
		'\x2', '\x2', '\x2', '\x8D', '\x8E', '\x3', '\x2', '\x2', '\x2', '\x8E', 
		'\x8F', '\x3', '\x2', '\x2', '\x2', '\x8F', '\x90', '\x5', '\x37', '\x1B', 
		'\x2', '\x90', '\x34', '\x3', '\x2', '\x2', '\x2', '\x91', '\x92', '\a', 
		'\x30', '\x2', '\x2', '\x92', '\x93', '\x5', '\x37', '\x1B', '\x2', '\x93', 
		'\x36', '\x3', '\x2', '\x2', '\x2', '\x94', '\x96', '\x5', '/', '\x17', 
		'\x2', '\x95', '\x94', '\x3', '\x2', '\x2', '\x2', '\x96', '\x97', '\x3', 
		'\x2', '\x2', '\x2', '\x97', '\x95', '\x3', '\x2', '\x2', '\x2', '\x97', 
		'\x98', '\x3', '\x2', '\x2', '\x2', '\x98', '\x38', '\x3', '\x2', '\x2', 
		'\x2', '\x99', '\x9A', '\a', ')', '\x2', '\x2', '\x9A', '\x9B', '\x3', 
		'\x2', '\x2', '\x2', '\x9B', '\x9C', '\b', '\x1C', '\x3', '\x2', '\x9C', 
		'\x9D', '\b', '\x1C', '\x4', '\x2', '\x9D', ':', '\x3', '\x2', '\x2', 
		'\x2', '\x9E', '\x9F', '\a', '~', '\x2', '\x2', '\x9F', '\xA0', '\x3', 
		'\x2', '\x2', '\x2', '\xA0', '\xA1', '\b', '\x1D', '\x3', '\x2', '\xA1', 
		'\xA2', '\b', '\x1D', '\x5', '\x2', '\xA2', '<', '\x3', '\x2', '\x2', 
		'\x2', '\xA3', '\xA4', '\a', ')', '\x2', '\x2', '\xA4', '\xA5', '\x3', 
		'\x2', '\x2', '\x2', '\xA5', '\xA6', '\b', '\x1E', '\x6', '\x2', '\xA6', 
		'\xA7', '\b', '\x1E', '\a', '\x2', '\xA7', '>', '\x3', '\x2', '\x2', '\x2', 
		'\xA8', '\xA9', '\v', '\x2', '\x2', '\x2', '\xA9', '\xAA', '\x3', '\x2', 
		'\x2', '\x2', '\xAA', '\xAB', '\b', '\x1F', '\x3', '\x2', '\xAB', '@', 
		'\x3', '\x2', '\x2', '\x2', '\xAC', '\xAD', '\a', '~', '\x2', '\x2', '\xAD', 
		'\xAE', '\x3', '\x2', '\x2', '\x2', '\xAE', '\xAF', '\b', ' ', '\x6', 
		'\x2', '\xAF', '\xB0', '\b', ' ', '\a', '\x2', '\xB0', '\x42', '\x3', 
		'\x2', '\x2', '\x2', '\xB1', '\xB2', '\v', '\x2', '\x2', '\x2', '\xB2', 
		'\xB3', '\x3', '\x2', '\x2', '\x2', '\xB3', '\xB4', '\b', '!', '\x3', 
		'\x2', '\xB4', '\x44', '\x3', '\x2', '\x2', '\x2', '\r', '\x2', '\x3', 
		'\x4', 'H', 'w', 'y', '~', '\x86', '\x89', '\x8D', '\x97', '\b', '\x2', 
		'\x3', '\x2', '\x5', '\x2', '\x2', '\a', '\x3', '\x2', '\a', '\x4', '\x2', 
		'\x6', '\x2', '\x2', '\t', '\x3', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace AppliedResearchAssociates.CalculateEvaluate
