﻿using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class AttributeDatum<T>
    {
        public string BeginStation { get; }

        public DateTime Date { get; }

        public string Direction { get; }

        public string EndStation { get; }

        public string Route { get; }

        public T Value { get; }
    }
}