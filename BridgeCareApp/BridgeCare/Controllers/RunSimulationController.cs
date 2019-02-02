using BridgeCare.Models;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Windows.Input;

namespace BridgeCare.Controllers
{
    public class RunSimulationController : ApiController
    {
        // GET: api/RunSimulation
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RunSimulation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/RunSimulation
        public void Post([FromBody]SimulationModel data)
        {
            try
            {
                DBMgr.NativeConnectionParameters = new ConnectionParameters
                ("data source=40.121.5.125;initial catalog=BridgeCare;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework",
                false, "MSSQL");
                var testData = new Simulation.Simulation(data.SimulationName, data.NetworkName,
                    data.SimulationId.ToString(), data.NetworkId.ToString());
                var simulationThread = new Thread(new ThreadStart(testData.CompileSimulation));
                simulationThread.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // PUT: api/RunSimulation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RunSimulation/5
        public void Delete(int id)
        {
        }
    }
}
