using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AttributeDatum<T>
    {
        public string BeginStation { get; set; }

        public DateTime Date { get; set; }

        public string Direction { get; set; }

        public string EndStation { get; set; }

        public string Route { get; set; }

        public T Value { get; set; }
    }
}
