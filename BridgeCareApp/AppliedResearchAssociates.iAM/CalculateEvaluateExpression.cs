using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public abstract class CalculateEvaluateExpression : CompilableExpression
    {
        private static readonly ConcurrentDictionary<WeakReference<FinalActor<CalculateEvaluateExpression>>, object> AllInstances = new ConcurrentDictionary<WeakReference<FinalActor<CalculateEvaluateExpression>>, object>();

        protected CalculateEvaluateExpression()
        {
            WeakReference<FinalActor<CalculateEvaluateExpression>> key = null;
            key = (this).WithFinalAction(actor => AllInstances.TryRemove(key, out _)).GetWeakReference();
            _ = AllInstances.TryAdd(key, null);
        }

        public Explorer Explorer { get; set; }

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

        public override string Expression
        {
            get => string.Concat(ExpressionFragments);
            set
            {
                if (UpdateExpression(value))
                {
                    // TODO: update fragments. use Regex.Split with capture in delimiter pattern.
                }
            }
        }

        private readonly List<ExpressionFragment> ExpressionFragments = new List<ExpressionFragment>();

        private sealed class ExpressionFragment
        {
            public ExpressionFragment(Choice<string, Attribute> fragment) => Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));

            public Choice<string, Attribute> Fragment { get; }

            public override string ToString() => Fragment.Reduce(Static.Identity, attribute => $"[{attribute.Name}]");
        }
    }
}
