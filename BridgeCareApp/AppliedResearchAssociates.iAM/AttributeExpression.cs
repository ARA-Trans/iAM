using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class AttributeExpression : CompilableExpression
    {
        public static Regex AttributePattern { get; } = new Regex($@"(?>(\[{PatternStrings.Identifier}]))", RegexOptions.Compiled);

        public Explorer Explorer { get; set; }

        public override string Expression
        {
            get => string.Concat(ExpressionFragments);
            set
            {
                if (UpdateExpression(value))
                {
                    var attributePerName = Explorer.AllAttributes.ToDictionary(attribute => attribute.Name);

                    ExpressionFragments.Clear();
                    ExpressionFragments.AddRange(AttributePattern.Split(value).Select(fragment =>
                    {
                        if (AttributePattern.IsMatch(fragment))
                        {
                            var identifier = fragment.Substring(1, fragment.Length - 2);
                            return attributePerName.TryGetValue(identifier, out var attribute)
                                ? new ExpressionFragment(attribute)
                                : new ExpressionFragment(fragment);
                        }
                        else
                        {
                            return new ExpressionFragment(fragment);
                        }
                    }));
                }
            }
        }

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Explorer == null)
                {
                    results.Add(ValidationStatus.Error, "Explorer is unset.", this, nameof(Explorer));
                }

                return results;
            }
        }

        public static void SetExplorerForAllInstances(Explorer explorer)
        {
            foreach (var (key, _) in AllInstances)
            {
                if (key.TryGetTarget(out var target))
                {
                    target.Value.Explorer = explorer;
                }
            }
        }

        protected AttributeExpression()
        {
            WeakReference<FinalActor<AttributeExpression>> key = null;
            key = this.WithFinalAction(actor => AllInstances.TryRemove(key, out _)).GetWeakReference();
            _ = AllInstances.TryAdd(key, null);
        }

        private static readonly ConcurrentDictionary<WeakReference<FinalActor<AttributeExpression>>, object> AllInstances = new ConcurrentDictionary<WeakReference<FinalActor<AttributeExpression>>, object>();

        private readonly List<ExpressionFragment> ExpressionFragments = new List<ExpressionFragment>();

        private sealed class ExpressionFragment
        {
            public ExpressionFragment(Choice<string, Attribute> fragment) => Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));

            public Choice<string, Attribute> Fragment { get; }

            public override string ToString() => Fragment.Reduce(Static.Identity, attribute => $"[{attribute.Name}]");
        }
    }
}
