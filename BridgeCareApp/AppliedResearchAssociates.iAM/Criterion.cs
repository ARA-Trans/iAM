using System;
using System.Collections.Concurrent;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion()
        {
            WeakReference<FinalActor<Criterion>> key = null;
            key = this.WithFinalAction(actor => AllInstances.TryRemove(key, out _)).GetWeakReference();
            _ = AllInstances.TryAdd(key, null);
        }

        public CalculateEvaluateCompiler Compiler { get; set; }

        public static void SetCompilerForAllInstances(CalculateEvaluateCompiler compiler)
        {
            foreach (var (key, _) in AllInstances)
            {
                if (key.TryGetTarget(out var target))
                {
                    target.Value.Compiler = compiler;
                }
            }
        }

        public bool? Evaluate(CalculateEvaluateArgument argument)
        {
            if (string.IsNullOrWhiteSpace(Expression))
            {
                return null;
            }

            EnsureCompiled();
            return Evaluator(argument);
        }

        protected override void Compile()
        {
            try
            {
                Evaluator = Compiler.GetEvaluator(Expression);
            }
            catch (CalculateEvaluateException e)
            {
                throw ExpressionCouldNotBeCompiled(e);
            }
        }

        private static readonly ConcurrentDictionary<WeakReference<FinalActor<Criterion>>, object> AllInstances = new ConcurrentDictionary<WeakReference<FinalActor<Criterion>>, object>();

        private Evaluator Evaluator;
    }
}
