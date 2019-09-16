using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace DataObjects
{
    public class LRSObjectSegmentationResult:LRSObject
    {
        private String m_strReason;
        private bool m_bDelete = false;

        public String Reason
        {
            set { m_strReason = value; }
            get { return m_strReason; }
        }

        public bool Delete
        {
            set { m_bDelete = value;}
            get { return m_bDelete; }
        }
        

        public double Delta
        {
            get { return this.EndStation - this.BeginStation; }
        }

        public LRSObjectSegmentationResult(DataGridViewRow row)
        {
			Route = row.Cells["ROUTES"].Value.ToString();
			if( double.TryParse( row.Cells["BEGIN_STATION"].Value.ToString(), out m_dBeginStation ) )
			{
				if( double.TryParse( row.Cells["END_STATION"].Value.ToString(), out m_dEndStation ) )
				{
					//EndStation = double.Parse(row.Cells["END_STATION"].Value.ToString());
					Direction = row.Cells["DIRECTION"].Value.ToString();
					Reason = row.Cells["REASON"].Value.ToString();
					//this.Route = row.Cells[0].Value.ToString();
					//this.BeginStation = double.Parse(row.Cells[1].Value.ToString());
					//this.EndStation = double.Parse(row.Cells[2].Value.ToString());
					//this.Direction = row.Cells[3].Value.ToString();
					//this.Reason = row.Cells[4].Value.ToString();
				}
				else
				{
					throw new Exception( "Error: could not parse END_STATION entry [" + row.Cells["END_STATION"].Value.ToString() + "]" );
				}

			}
			else
			{
				throw new Exception( "Error: could not parse BEGIN_STATION entry [" + row.Cells["BEGIN_STATION"].Value.ToString() + "]" );
			}
		}


        public LRSObjectSegmentationResult(DataReader dr)
		{
			//this.Route = dr["ROUTES"].ToString();
			//this.BeginStation = double.Parse( dr["BEGIN_STATION"].ToString() );
			//this.EndStation = double.Parse( dr["END_STATION"].ToString() );
			//this.Direction = dr["DIRECTION"].ToString();
			//this.Reason = dr["REASON"].ToString();

			if( double.TryParse( dr["BEGIN_STATION"].ToString(), out m_dBeginStation ) )
			{
				if( double.TryParse( dr["END_STATION"].ToString(), out m_dEndStation ) )
				{
					Direction = dr["DIRECTION"].ToString();
					Reason = dr["BREAKCAUSE"].ToString();
					Route = dr["ROUTES"].ToString();
				}
				else
				{
					throw new Exception( "Error: could not parse END_STATION entry [" + dr["END_STATION"].ToString() + "]" );
				}
			}
			else
			{
				throw new Exception( "Error: could not parse BEGIN_STATION entry [" + dr["BEGIN_STATION"].ToString() + "]" );
			}

        }
    }
}
