using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using JaceFunc =
System.Func<System.Collections.Generic.Dictionary<string, double>, double>;

namespace Reports.MDSHA.LmDmVmtCondition
{
    /// <summary>
    ///     Provides generic wrapper for indexes' state and behavior.
    /// </summary>
    /// <remarks>
    ///     This is necessary to handle copying of indexes without knowledge
    ///     of the concrete type. See remarks for Index.
    /// </remarks>
    public interface IIndex
    {
        string IndexName { get; set; }

        bool Enabled { get; set; }

        IIndex Copy();
    }

    /// <summary>
    ///     Provides some common data for custom indexes for this report.
    /// </summary>
    /// <remarks>
    ///     The type parameterization and funny type constraints are to
    ///     allow the ConcreteCopy method to be modularized across
    ///     sub-classes and still return the specific concrete types.
    /// </remarks>
    public abstract class Index<T> : IIndex where T : Index<T>, new()
    {
        public Index()
        {
            this.IndexName = string.Empty;
        }

        public string IndexName { get; set; }

        public bool Enabled { get; set; }

        public IIndex Copy()
        {
            return this.ConcreteCopy();
        }

        public virtual T ConcreteCopy()
        {
            return new T()
            {
                IndexName = this.IndexName,
                Enabled = this.Enabled,
            };
        }
    }

    /// <summary>
    ///     Encapsulates data for persisting custom indexes associated
    ///     1-to-1 with RoadCare attributes for a given simulation.
    /// </summary>
    public class AttributeIndex : Index<AttributeIndex>
    {
        public AttributeIndex()
        {
            this.SourceAttribute = string.Empty;
            this.Levels = new Level[5];
        }

        public string SourceAttribute { get; set; }

        public Level[] Levels { get; set; }

        public override AttributeIndex ConcreteCopy()
        {
            var copy = base.ConcreteCopy();
            copy.SourceAttribute = this.SourceAttribute;
            copy.Levels = this.Levels.ToArray();
            return copy;
        }
    }

    /// <summary>
    ///     Provides a way to compare equality of (and hash) AttributeIndex
    ///     objects.
    /// </summary>
    public class AttributeIndexEqualityComparer
        : IEqualityComparer<AttributeIndex>
    {
        public bool Equals(AttributeIndex o1, AttributeIndex o2)
        {
            return string.Equals(o1.SourceAttribute, o2.SourceAttribute);
        }

        public int GetHashCode(AttributeIndex o)
        {
            return o.SourceAttribute.GetHashCode();
        }
    }

    /// <summary>
    ///     Encapsulates some relevant information associated with a
    ///     condition level in RoadCare.
    /// </summary>
    /// <remarks>
    ///     This is a struct rather than a class because it's very small,
    ///     fully public, has only immutable types as members, it saves some
    ///     manual copying when replicating AttributeIndex objects, and
    ///     changes to the members could affect other indexes if instances
    ///     were somehow shared among AttributeIndex objects.
    /// </remarks>
    public struct Level
    {
        [JsonProperty("Expression")]
        private string BackingExpression;

        public double? Bound { get; set; }

        [JsonIgnore]
        public string Expression
        {
            get
            {
                return this.BackingExpression ?? string.Empty;
            }
            set
            {
                this.BackingExpression = value;
            }
        }

        [JsonIgnore]
        public Func<double, double> Compute { get; set; }
    }

    /// <summary>
    ///     Encapsulates a "derived" (free-form) index.
    /// </summary>
    public class DerivedIndex : Index<DerivedIndex>
    {
        public DerivedIndex()
        {
            this.Expression = string.Empty;
            this.SourceIndexes = new HashSet<string>();
        }

        public string Expression { get; set; }

        public HashSet<string> SourceIndexes { get; set; }

        [JsonIgnore]
        public JaceFunc Compute { get; set; }

        public override DerivedIndex ConcreteCopy()
        {
            var copy = base.ConcreteCopy();
            copy.Expression = this.Expression;
            copy.SourceIndexes = new HashSet<string>(this.SourceIndexes);
            copy.Compute = this.Compute;
            return copy;
        }

        public bool DependsOn(AttributeIndex ai)
        {
            return this.SourceIndexes.Contains(ai.IndexName);
        }
    }
}
