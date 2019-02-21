namespace BridgeCare.Models
{
    public class SimulationDataModel
    {
        public int SectionId { get; set; }

        public int Year { get; set; }

        public string Deck { get; set; }

        public string Super { get; set; }

        public string Sub { get; set; }

        public string Culv { get; set; }

        public string DeckD { get; set; }

        public string SuperD { get; set; }

        public string SubD { get; set; }

        public string CulvD { get; set; }

        public string MinC { get; set; }

        public string SD { get; set; }

        // Below will be fetched from REPORT_x_y table
        public string Project { get; set; }

        public string Cost { get; set; }
    }
}