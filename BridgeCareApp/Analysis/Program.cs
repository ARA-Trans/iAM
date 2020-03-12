using System;

namespace ARA.iAM.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            AnalysisConfiguration configuration = DataPersistence.GetConfigurationById(91);

            AnalysisResult result = configuration.Compile().Run();
        }
    }

    class AnalysisConfiguration
    {
        public CompiledAnalysis Compile()
        {
            throw new NotImplementedException();
        }
    }

    class CompiledAnalysis
    {
        public AnalysisResult Run()
        {
            throw new NotImplementedException();
        }
    }

    class AnalysisResult
    {

    }
}
