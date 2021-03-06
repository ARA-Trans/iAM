﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simulation
{
    public sealed class SimulationQueue : IDisposable
    {
        public SimulationQueue(int maximumConcurrency) => MaximumConcurrency = maximumConcurrency;

        private static SimulationQueue mainSimulationQueueInstance = null;
        public static SimulationQueue MainSimulationQueue
        {
            get
            {
                lock (typeof(SimulationQueue))
                {
                    if (mainSimulationQueueInstance == null)
                    {
                        mainSimulationQueueInstance = new SimulationQueue(1);
                    }
                    return mainSimulationQueueInstance;
                }
            }
        }

        public int MaximumConcurrency
        {
            get => _MaximumConcurrency;
            set
            {
                lock (Lock_MaximumConcurrency)
                {
                    _MaximumConcurrency = Math.Max(1, value);
                    while (Consumers.Count < _MaximumConcurrency)
                    {
                        var consumer = new Task(Consume, Consumers.Count);
                        if (!Consumers.TryAdd(Consumers.Count, consumer))
                        {
                            throw new InvalidOperationException("Consumer already exists.");
                        }

                        consumer.Start();
                    }
                }
            }
        }

        public void Dispose() => Queue.CompleteAdding();

        public Task Enqueue(SimulationParameters simulationParameters, CancellationToken cancellationToken = default)
        {
            var task = new Task(Simulation, simulationParameters, cancellationToken);
            Queue.Add(task);
            return task;
        }

        // Allow generic tasks to be enqueued, if it is unsafe to run a simulation while those tasks are running
        public Task Enqueue(Task nonSimulationTask)
        {
            Queue.Add(nonSimulationTask);
            return nonSimulationTask;
        }

        public Task Enqueue(Action nonSimulationAction, CancellationToken cancellationToken = default)
        {
            var task = new Task(nonSimulationAction, cancellationToken);
            Queue.Add(task);
            return task;
        }

        private readonly ConcurrentDictionary<int, Task> Consumers = new ConcurrentDictionary<int, Task>();

        private readonly object Lock_MaximumConcurrency = new object();

        private readonly BlockingCollection<Task> Queue = new BlockingCollection<Task>();

        private int _MaximumConcurrency;

        private static void Simulation(object state)
        {
            var parameters = (SimulationParameters)state;
            var simulation = new Simulation(parameters.SimulationName, parameters.NetworkName, parameters.SimulationId, parameters.NetworkId, parameters.ConnectionString);
            simulation.CompileSimulation(parameters.IsApiCall);
        }

        private void Consume(object state)
        {
            var consumerIndex = (int)state;
            foreach (var task in Queue.GetConsumingEnumerable())
            {
                try
                {
                    if (!task.IsCanceled)
                    {
                        task.RunSynchronously();
                    }
                }
                catch (InvalidOperationException)
                {
                }

                if (consumerIndex >= MaximumConcurrency)
                {
                    if (!Consumers.TryRemove(consumerIndex, out var _))
                    {
                        throw new InvalidOperationException("Consumer does not exist.");
                    }

                    break;
                }
            }
        }
    }
}
