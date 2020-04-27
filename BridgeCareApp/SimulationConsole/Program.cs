using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulation;

namespace SimulationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = args[0];
            var networkId = args[1];
            var simulationId = args[2];


            var simulation = new Simulation.Simulation("", "",simulationId,networkId, connectionString);

            simulation.CompileSimulation(false);


        }
    }
}
