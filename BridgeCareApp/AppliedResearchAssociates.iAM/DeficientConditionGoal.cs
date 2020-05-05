using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public sealed class DeficientConditionGoal : ConditionGoal
    {
        public double AllowedDeficientPercentage { get; }

        public double DeficientLevel { get; }

        public bool IsMet(ICollection<double> actualLevels)
        {
            Func<double, bool> levelIsDeficient;
            if (Attribute.IsDecreasingWithDeterioration)
            {
                levelIsDeficient = LevelIsLessThanLimit;
            }
            else
            {
                levelIsDeficient = LevelIsGreaterThanLimit;
            }

            var numberOfDeficientLevels = actualLevels.Count(levelIsDeficient);
            var actualDeficientPercentage = (double)numberOfDeficientLevels / actualLevels.Count * 100;
            return actualDeficientPercentage <= AllowedDeficientPercentage;
        }

        private bool LevelIsGreaterThanLimit(double level) => level > DeficientLevel;

        private bool LevelIsLessThanLimit(double level) => level < DeficientLevel;
    }
}
