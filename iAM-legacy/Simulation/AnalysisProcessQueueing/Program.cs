using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Simulation.AnalysisProcessQueueing
{
    internal static class Program
    {
        internal enum StreamContentCode
        {
            Error,
        }

        private static void CheckPipe(object state)
        {
            var outputPipe = (PipeStream)state;
            if (!outputPipe.IsConnected)
            {
                Environment.Exit((int)ProgramExitCode.Disconnected);
            }
        }

        private static string GetElapsedTimeLogEntry(string label, TimeSpan elapsed) => $"[{DateTime.Now.ToString("u")}] [INFO] Total elapsed time ({label}): {elapsed}";

        private static int Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            _ = Parser.Default.ParseArguments<AnalysisProcessOptions>(args).WithParsed(Simulation);

            Console.Error.WriteLine(GetElapsedTimeLogEntry("CLI program", timer.Elapsed));

            return (int)ProgramExitCode.Success;
        }

        private static void Simulation(AnalysisProcessOptions options)
        {
            using (var outputPipe = new AnonymousPipeClientStream(PipeDirection.Out, options.PipeHandle))
            using (var writer = new BinaryWriter(outputPipe))
            using (new Timer(CheckPipe, outputPipe, 0, 500))
            {
                try
                {
                    var simulation = new Simulation(options.SimulationName, options.NetworkName, options.SimulationId, options.NetworkId, options.ConnectionString);
                    simulation.CompileSimulation(options.IsApiCall);
                }
                catch (Exception e)
                {
                    report(e, StreamContentCode.Error);
                    throw;
                }

                void report<T>(T data, StreamContentCode contentCode)
                {
                    writer.Write((int)contentCode);
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(outputPipe, data);
                    writer.Flush();
                }
            }
        }
    }
}
