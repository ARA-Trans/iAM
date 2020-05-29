using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute
    {
        public Attribute(string name)
        {
            if (Name == null || !NamePattern.IsMatch(Name))
            {
                throw new MalformedInputException($"Invalid name. The valid pattern is \"{PatternStrings.Identifier}\".");
            }

            Name = name;
        }

        public static Regex NamePattern { get; } = new Regex($@"(?>\A{PatternStrings.Identifier}\z)");

        public string Name { get; }
    }

    public class Attribute<T> : Attribute
    {
        public Attribute(string name) : base(name)
        {
        }

        // [REVIEW] Remove this member in favor of separately defining one or more
        // "AttributeDataProvider"-like types, which would use this type (as metadata) to retrieve
        // the corresponding data with respect to system constraints, e.g. not enough memory to hold
        // all data at once.
        public IEnumerable<AttributeDatum<T>> Data => throw new NotImplementedException();

        public T DefaultValue { get; set; }
    }
}
