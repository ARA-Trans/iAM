using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DatabaseManager;
using Reports.Lexington;
using Reports.MDSHA;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    using Cis = Reports.MDSHA.ConfigurableInputSummary;
    using Fytsp = Reports.MDSHA.FyTargetsSuggestedProjects;
    using Ldvc = Reports.MDSHA.LmDmVmtCondition;
    using Blfy = Reports.MDSHA.BudgetLmPerFcPerYr;
    using Blfc = Reports.MDSHA.BudgetLmPerFcPerCond;
    using Icf = Reports.MDSHA.IriConditionPerFc;
    using AllSec = Reports.MDSHA.AllSectionsShopDmYrVmt;

    public partial class FormSpecialReportConfig : Form
    {
        private const string GenerationCompleteMessageTemplate =
            "{0}Report generation for \"{1}\" completed." +
            " Elapsed time during generation: {2}";

        private static readonly Sim NullSim =
            new Sim { Name = "--- Unspecified ( for Network Reports ) ---" };

        private readonly Net[] Nets;

        private readonly Sim[] Sims;

        private readonly string[] NetRpts;

        private readonly string[] SimRpts;

        private readonly Stopwatch Timer;

        private bool WorkCompleted;

        private string WorkingReportName;

        public FormSpecialReportConfig()
        {
            InitializeComponent();

            Nets = DBMgr
                .ExecuteQuery(
                    "SELECT networkid, network_name" +
                    " FROM networks ORDER BY network_name")
                .Tables[0]
                .AsEnumerable()
                .Select(dr => new Net
                {
                    Id = dr[0].ToString(),
                    Name = dr[1].ToString()
                })
                .ToArray();

            Sims = DBMgr
                .ExecuteQuery(
                    "SELECT simulationid, simulation, networkid" +
                    " FROM simulations ORDER BY simulation")
                .Tables[0]
                .AsEnumerable()
                .Select(dr => new Sim
                {
                    Id = dr[0].ToString(),
                    Name = dr[1].ToString(),
                    Net = dr[2].ToString()
                })
                .ToArray();

            NetRpts = DBOp.GetNetworkReportNames().ToArray();

            SimRpts =
                DBOp.GetSimulationReportNames().Concat(new[]
                {
                    Cis.Report.GenericTitle,
                    Fytsp.Report.GenericTitle,
                    Ldvc.Report.GenericTitle,
                    Blfy.Report.GenericTitle,
                    Blfc.Report.GenericTitle,
                    Icf.Report.GenericTitle,
                    AllSec.Report.GenericTitle,
                    BenefitCostRatioReport.GenericTitle,
                })
                .ToArray();

            Array.Sort(NetRpts);
            Array.Sort(SimRpts);

            cbNets.DisplayMember = "Name";
            cbSims.DisplayMember = "Name";

            if (Nets.Length != 0)
            {
                cbNets.Items.AddRange(Nets);
                cbNets.SelectedIndex = 0;
            }
            else
            {
                cbNets.SelectedIndex = -1;
            }

            fbdSimpleGen.Description =
                "Select the folder in which you would like the report file" +
                " to be generated.";

            Timer = new Stopwatch();
        }

        private static bool TryGenerate(ReportBase r)
        {
            bool completed = false;
            try
            {
                r.Generate();
                completed = true;
            }
            catch (Exception e)
            {
                Global.WriteOutput(e.Message);
            }
            return completed;
        }

        private void cbNets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var net = (Net) cbNets.SelectedItem;
            if (cbNets.Enabled)
            {
                var netId = net.Id;
                var netSims =
                    Sims.Where(s => Equals(s.Net, netId)).ToArray();

                cbSims.Items.Clear();
                cbSims.Items.Add(NullSim);
                if (netSims.Length != 0)
                {
                    cbSims.Items.AddRange(netSims);
                }
                cbSims.SelectedIndex = 0;
            }
            else
            {
                cbSims.SelectedIndex = -1;
            }
        }

        private void cbSims_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sim = (Sim) cbSims.SelectedItem;
            if (cbSims.Enabled)
            {
                var reps = sim.Id == null ? NetRpts : SimRpts;

                cbReps.Items.Clear();
                if (reps.Length != 0)
                {
                    cbReps.Items.AddRange(reps);
                    cbReps.SelectedIndex = 0;
                }
                else
                {
                    cbReps.SelectedIndex = -1;
                }
            }
            else
            {
                cbReps.SelectedIndex = -1;
            }
        }

        private void btnGenRep_Click(object sender, EventArgs e)
        {
            var net = (Net) cbNets.SelectedItem;
            var sim = (Sim) cbSims.SelectedItem;
            WorkingReportName = (string) cbReps.SelectedItem;

            if (net != null && sim != null && WorkingReportName != null)
            {
                var netId = net.Id;
                var netName = net.Name;

                var simId = string.Empty;
                var simName = string.Empty;
                if (sim.Id != null)
                {
                    simId = sim.Id;
                    simName = sim.Name;
                }

                StartProgress();

                var bw = new BackgroundWorker();
                bw.RunWorkerCompleted +=
                    (sender_, e_) => FinishProgress();

                AddReportGenerationWork(
                    bw,
                    netId,
                    netName,
                    simId,
                    simName);

                Timer.Restart();

                bw.RunWorkerAsync();
            }
        }

        private void AddReportGenerationWork(
            BackgroundWorker bw,
            string networkId,
            string networkName,
            string simulationId,
            string simulationName)
        {
            WorkCompleted = false;

            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += (sender, e) =>
                WorkCompleted = (bool) e.UserState;

            try
            {
                // Configurable Input Summary
                if (WorkingReportName == Cis.Report.GenericTitle)
                {
                    var r = new Cis.Report(simulationId, simulationName);
                    var f = new FormConfigurableInputSummary(r);

                    f.ShowDialog();
                    if (f.IsReportReady)
                    {
                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // FY Targets & Suggested Projects
                else if (WorkingReportName == Fytsp.Report.GenericTitle)
                {
                    var b = new Fytsp.Report.Builder(
                        networkId,
                        simulationId,
                        simulationName);

                    var f = new FormFyTargetsSuggestedProjects(b);

                    f.ShowDialog();
                    if (f.IsReportReady)
                    {
                        var r = f.Report;
                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // LM DM VMT Condition (Dual Report)
                else if (WorkingReportName == Ldvc.Report.GenericTitle)
                {
                    var r = new Ldvc.Report(
                        networkId,
                        networkName,
                        simulationId,
                        simulationName);

                    var f = new FormLmDmVmtConditionDualReport(r);

                    f.ShowDialog();
                    if (f.IsReportReady)
                    {
                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // Budget & LM per FC per Year
                else if (WorkingReportName == Blfy.Report.GenericTitle)
                {
                    var result = fbdSimpleGen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var r = new Blfy.Report(
                            networkId,
                            simulationId,
                            simulationName);

                        r.OutputDirectory =
                            new DirectoryInfo(fbdSimpleGen.SelectedPath);

                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // Budget & LM per FC per Condition
                else if (WorkingReportName == Blfc.Report.GenericTitle)
                {
                    var result = fbdSimpleGen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var r = new Blfc.Report(
                            networkId,
                            simulationId,
                            simulationName);

                        r.OutputDirectory =
                            new DirectoryInfo(fbdSimpleGen.SelectedPath);

                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // IRI Condition per FC
                else if (WorkingReportName == Icf.Report.GenericTitle)
                {
                    var result = fbdSimpleGen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var r = new Icf.Report(
                            networkId,
                            simulationId,
                            simulationName);

                        r.OutputDirectory =
                            new DirectoryInfo(fbdSimpleGen.SelectedPath);

                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // All Sections
                else if (WorkingReportName == AllSec.Report.GenericTitle)
                {
                    var result = fbdSimpleGen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var r = new AllSec.Report(
                            networkId,
                            simulationId,
                            simulationName);

                        r.OutputDirectory =
                            new DirectoryInfo(fbdSimpleGen.SelectedPath);

                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                // Benefit/Cost Ratio
                else if (WorkingReportName == BenefitCostRatioReport.GenericTitle)
                {
                    var result = fbdSimpleGen.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var r = new BenefitCostRatioReport(
                            networkId,
                            simulationId,
                            simulationName);

                        r.OutputDirectory =
                            new DirectoryInfo(fbdSimpleGen.SelectedPath);

                        bw.DoWork += (sender, e) =>
                            bw.ReportProgress(100, TryGenerate(r));
                    }
                }
                else
                {
                    // Reports that existed prior to Spring/Summer 2014
                    bw.DoWork += (sender, e) =>
                        SolutionExplorer.GenerateReport(
                            WorkingReportName,
                            networkId,
                            simulationId,
                            networkName,
                            simulationName);
                }
            }
            catch (ReportGenerationException e)
            {
                Global.WriteOutput(e.Message);
            }
        }

        private void StartProgress()
        {
            Enabled = false;
            progressBar.MarqueeAnimationSpeed = 100;
        }

        private void FinishProgress()
        {
            var elapsed = Timer.Elapsed;

            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Value = 0;
            progressBar.Refresh();

            if (WorkCompleted)
            {
                var message = string.Format(
                    GenerationCompleteMessageTemplate,
                    Global.GetTimeStampPrefix(),
                    WorkingReportName,
                    elapsed);

                Global.WriteOutput(message);
            }

            Enabled = true;
        }

        private class Net
        {
            public string Id { get; set; }
                
            public string Name { get; set; }
        }

        private class Sim
        {
            public string Id { get; set; }
            
            public string Name { get; set; }
            
            public string Net { get; set; }
        }
    }
}
