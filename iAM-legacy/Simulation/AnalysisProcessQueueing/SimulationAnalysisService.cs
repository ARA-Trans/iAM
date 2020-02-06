using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Simulation.AnalysisProcessQueueing
{
    public sealed class SimulationAnalysisService : IDisposable
    {
        public SimulationAnalysisService(Func<int> getConcurrencyLevel = null)
        {
            GetConcurrencyLevel = getConcurrencyLevel ?? (() => Environment.ProcessorCount);
            HandleScanner = new Timer(ScanHandles_Locked, null, 0, 500);
        }

        public IReadOnlyDictionary<int, AnalysisProcessHandle> AllHandles => _AllHandles;

        public int AddPendingAnalysis(AnalysisProcessHandle handle)
        {
            var key = handle.SimulationOptions.SimulationId;
            if (!_AllHandles.TryAdd(key, handle))
            {
                throw new ArgumentException("An analysis process for the same simulation is already pending or completed.", nameof(handle));
            }

            PendingHandles.Enqueue(handle);
            return key;
        }

        public void Dispose()
        {
            HandleScanner?.Dispose();

            foreach (var key in _AllHandles.Keys)
            {
                RemoveAnalysis(key);
            }
        }

        public void RemoveAnalysis(int key)
        {
            if (_AllHandles.TryRemove(key, out var handle))
            {
                handle.Dispose();
            }
        }

        private readonly ConcurrentDictionary<int, AnalysisProcessHandle> _AllHandles = new ConcurrentDictionary<int, AnalysisProcessHandle>();
        private readonly Func<int> GetConcurrencyLevel;
        private readonly Timer HandleScanner;
        private readonly object HandleScanningLock = new object();
        private readonly ConcurrentDictionary<int, ResourceConsumptionStatistics> ResourceConsumptionPerHandle = new ConcurrentDictionary<int, ResourceConsumptionStatistics>();
        private readonly ConcurrentQueue<AnalysisProcessHandle> PendingHandles = new ConcurrentQueue<AnalysisProcessHandle>();

        private void ScanHandles()
        {
            foreach (var kv in _AllHandles)
            {
                if (kv.Value.ResourceConsumption != null)
                {
                    ResourceConsumptionPerHandle[kv.Key] = kv.Value.ResourceConsumption;
                }
            }

            var concurrentHandles = _AllHandles.Values.Where(handle => handle.Status == AnalysisStatus.Starting || handle.Status == AnalysisStatus.Started).ToList();
            var maximumConcurrency = GetConcurrencyLevel().Clip(1, Environment.ProcessorCount);

            while (concurrentHandles.Count < maximumConcurrency)
            {
                if (concurrentHandles.Count != 0)
                {
                    if (ResourceConsumptionPerHandle.Count == 0)
                    {
                        break;
                    }

                    const double SAFETY_FACTOR = 2;

                    var safeApproximationOfMaximumPhysicalMemoryPerSimulation = ResourceConsumptionPerHandle.Values.Max(resourceConsumption => resourceConsumption.PeakWorkingSet) * SAFETY_FACTOR;
                    var safeApproximationOfMaximumVirtualMemoryPerSimulation = ResourceConsumptionPerHandle.Values.Max(resourceConsumption => resourceConsumption.PeakVirtualMemorySize) * SAFETY_FACTOR;

                    var computerInfo = new ComputerInfo();
                    if (safeApproximationOfMaximumPhysicalMemoryPerSimulation >= computerInfo.AvailablePhysicalMemory ||
                        safeApproximationOfMaximumVirtualMemoryPerSimulation >= computerInfo.AvailableVirtualMemory)
                    {
                        break;
                    }
                }

                if (PendingHandles.TryDequeue(out var handle))
                {
                    if (!_AllHandles.Values.Contains(handle))
                    {
                        continue;
                    }

                    handle.StartAnalysis();
                    if (!handle.AnalysisIsCompleted)
                    {
                        concurrentHandles.Add(handle);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void ScanHandles_Locked(object state) => Static.TryLock(HandleScanningLock, ScanHandles);
    }
}
