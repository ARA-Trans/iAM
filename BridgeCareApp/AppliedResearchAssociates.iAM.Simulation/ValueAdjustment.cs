using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class ValueAdjustment
    {
        // Similar to Equation, but with special parsing and restricted syntax. Can handle 6
        // operations on an input value: (1) add percentage ("+40%" -> value *= 1.4); (2) subtract
        // percentage ("-40%" => value *= 0.6); (3) set percentage ("40%" -> value *= 0.4); (4) add
        // value ("+4" -> value += 4); (5) subtract value ("-4" -> value -= 4); (6) set value ("4"
        // -> value = 4). A regex to deconstruct and partially validate this syntax could look something
        // like @"^\s*((?:+|-)?)(.+?)(%?)\s*$", where group 2 (the lazy one-or-more of any
        // character) could be validated by passing it into a numeric parsing method.

        public string Expression { get; }

        public double Adjust(double value) => throw new NotImplementedException();
    }
}
