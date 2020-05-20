namespace AppliedResearchAssociates.iAM
{
    internal static class PatternStrings
    {
        public static string DecimalPart => $@"(?:\.{NaturalNumber})";

        public static string Exponent => $@"(?:[eE]{Sign}?{NaturalNumber})";

        public static string Identifier => @"(?:[a-zA-Z_][a-zA-Z_0-9]*)";

        public static string Mantissa => $@"(?:{Mantissa1}|{Mantissa2})";

        public static string Mantissa1 => $@"(?:{NaturalNumber}{DecimalPart}?)";

        public static string Mantissa2 => $@"(?:{DecimalPart})";

        public static string NaturalNumber => @"(?:\d+)";

        public static string Number => $@"(?:{Sign}?{Mantissa}{Exponent}?)";

        public static string Sign => @"(?:\+|-)";
    }
}
