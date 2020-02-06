using CommandLine;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Simulation.AnalysisProcessQueueing
{
    public sealed class AnalysisProcessHandle : IDisposable
    {
        public AnalysisProcessHandle(AnalysisProcessOptions simulationOptions)
        {
            SimulationOptions = simulationOptions ?? throw new ArgumentNullException(nameof(simulationOptions));
            AnalysisTask = new Task(RunChildProcess, TaskCreationOptions.LongRunning);
        }

        public event EventHandler AnalysisStarting;

        public event EventHandler AnalysisStopped;

        public bool AnalysisIsCompleted => AnalysisTask.IsCompleted;

        public ProgramExitCode? ChildProcessExitCode
        {
            get
            {
                try
                {
                    return (ProgramExitCode?)ChildProcess?.ExitCode;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        public Exception Exception
        {
            get
            {
                var childProcessError = ChildProcessError;
                var taskException = AnalysisTask.Exception;

                if (childProcessError != null && taskException != null)
                {
                    return new AggregateException(childProcessError, taskException);
                }
                else
                {
                    return childProcessError ?? taskException;
                }
            }
        }

        public ResourceConsumptionStatistics ResourceConsumption { get; private set; }

        public AnalysisProcessOptions SimulationOptions { get; }

        public AnalysisStatus Status { get; private set; } = AnalysisStatus.New;

        public void CancelAnalysis() => StopAnalysis(StopCode.AbnormalTermination);

        public void Dispose()
        {
            StopAnalysis(StopCode.Disposal);
            ChildProcess?.Dispose();
        }

        public void StartAnalysis()
        {
            lock (Lock_AnalysisStatus)
            {
                if (Status == AnalysisStatus.New)
                {
                    Status = AnalysisStatus.Starting;
                    OnAnalysisStarting(EventArgs.Empty);

                    AnalysisTask.Start();
                }
            }
        }

        public void WaitForCompletion() => AnalysisTask.Wait();

        private readonly Task AnalysisTask;
        private readonly object Lock_AnalysisStatus = new object();
        private Process ChildProcess;
        private Exception ChildProcessError;
        private bool ChildProcessHasExited;

        private enum StopCode
        {
            NormalTermination,
            AbnormalTermination,
            Disposal,
        }

        private void KillChildProcess()
        {
            if (Status == AnalysisStatus.Started && !ChildProcessHasExited)
            {
                try
                {
                    ChildProcess.Kill();
                }
                catch (Win32Exception)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private void OnAnalysisStarting(EventArgs e) => AnalysisStarting?.Invoke(this, e);

        private void OnAnalysisStopped(EventArgs e) => AnalysisStopped?.Invoke(this, e);

        private void RunChildProcess()
        {
            try
            {
                var tempFolderPath = Path.GetTempPath();
                var workingFolderName = Path.GetRandomFileName();
                var workingFolderPath = Path.Combine(tempFolderPath, workingFolderName);
                _ = Directory.CreateDirectory(workingFolderPath);

                using (var inputPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
                {
                    SimulationOptions.PipeHandle = inputPipe.GetClientHandleAsString();

                    var arguments = Parser.Default.FormatCommandLine(SimulationOptions);

                    var startInfo = new ProcessStartInfo(typeof(Program).Assembly.Location, arguments)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = workingFolderPath,
                    };

                    lock (Lock_AnalysisStatus)
                    {
                        if (Status != AnalysisStatus.Starting)
                        {
                            return;
                        }

                        ChildProcess = new Process
                        {
                            StartInfo = startInfo,
                            EnableRaisingEvents = true,
                        };

                        ChildProcess.Exited += delegate
                        {
                            ChildProcessHasExited = true;
                        };

                        if (!ChildProcess.Start())
                        {
                            throw new InvalidOperationException("Could not start analysis process.");
                        }

                        Status = AnalysisStatus.Started;
                    }

                    inputPipe.DisposeLocalCopyOfClientHandle();

                    var formatter = new BinaryFormatter();
                    T read<T>() => (T)formatter.Deserialize(inputPipe);

                    using (var reader = new BinaryReader(inputPipe))
                    {
                        while (!ChildProcessHasExited)
                        {
                            Program.StreamContentCode contentCode;
                            try
                            {
                                contentCode = (Program.StreamContentCode)reader.ReadInt32();
                            }
                            catch (EndOfStreamException)
                            {
                                break;
                            }

                            switch (contentCode)
                            {
                                case Program.StreamContentCode.Error:
                                    ChildProcessError = read<Exception>();
                                    break;

                                case Program.StreamContentCode.ResourceConsumptionStatistics:
                                    ResourceConsumption = read<ResourceConsumptionStatistics>();
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid content code received.");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                CancelAnalysis();
                throw;
            }

            StopAnalysis(StopCode.NormalTermination);
        }

        private void StopAnalysis(StopCode stopCode)
        {
            lock (Lock_AnalysisStatus)
            {
                if (Status != AnalysisStatus.Stopped)
                {
                    if (stopCode != StopCode.NormalTermination)
                    {
                        KillChildProcess();
                    }

                    Status = AnalysisStatus.Stopped;

                    if (stopCode != StopCode.Disposal)
                    {
                        OnAnalysisStopped(EventArgs.Empty);
                    }
                }
            }
        }
    }
}
