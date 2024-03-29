﻿namespace BridgeCare.Models
{
    public class YearsData
    {
        public int Year { get; set; }

        public string Deck { get; set; }

        public string Super { get; set; }

        public string Sub { get; set; }

        public string Culv { get; set; }        

        public string DeckD { get; set; }

        public string SuperD { get; set; }

        public string SubD { get; set; }

        public string CulvD { get; set; }

        public double MinC { get; set; }

        public string SD { get; set; }

        public string PoorOnOffRate { get; set; }

        // Below will be fetched from REPORT_x_y table
        public string Project { get; set; }

        public double Cost { get; set; }

        //Below data is fetched from inner join of Section_{networkId} and Report_{networkId}_{simulationId} tables
        public string Budget { get; set; }
        public string ProjectPick { get; set; }
        public string Treatment { get; set; }
        public int ProjectPickType { get; set; }
    }
}
