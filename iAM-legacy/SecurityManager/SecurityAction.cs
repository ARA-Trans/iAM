using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;
using System.Data;

namespace SecurityManager
{
	public class SecurityAction
	{
		private String m_actionID;
		private String m_actionIDColumnName;

		private Dictionary<String, String> m_columnData;
		private List<String> m_membershipRoleIDs;
		private List<SecurityActionGroup> m_groups;

		public SecurityAction( Dictionary<string, string> actionDescriptor, string actionIDColumnName )
		{
			m_actionIDColumnName = actionIDColumnName;
			m_actionID = actionDescriptor[m_actionIDColumnName];
			m_columnData = actionDescriptor;
			//GetColumnData( actionID );
			BuildObjectMembership();
		}

		private void BuildObjectMembership()
		{
			m_membershipRoleIDs = new List<String>();
			m_groups = new List<SecurityActionGroup>();
			String query = "SELECT ACTIONGROUP_ID FROM SEC_ACTIONGROUP_MEM WHERE " + m_actionIDColumnName + " = '" + m_actionID + "'";
			DataSet groups = DBMgr.ExecuteQuery( query );
			foreach( DataRow group in groups.Tables[0].Rows )
			{
				m_membershipRoleIDs.Add( group[0].ToString().Trim() );
			}
		}

		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is SecurityAction )
			{
				SecurityAction otherAction = ( SecurityAction )obj;
				foreach( string columnName in otherAction.m_columnData.Keys )
				{
					if( !this.m_columnData.ContainsKey( columnName ) || this.m_columnData[columnName] != otherAction.m_columnData[columnName] )
					{
						same = false;
						break;
					}
				}
			}
			else if( obj is Dictionary<string, string> )
			{
				Dictionary<string, string> descriptor = ( Dictionary<string, string> )obj;
				foreach( string columnName in descriptor.Keys )
				{
					if( !this.m_columnData.ContainsKey( columnName ) || this.m_columnData[columnName] != descriptor[columnName] )
					{
						same = false;
						break;
					}
				}
			}
			else
			{
				same = base.Equals( obj );
			}
			return same;
		}


		public String ActionID
		{
			get
			{
				return m_actionID;
			}
			set
			{
				m_actionID = value;
			}
		}
		public List<String> GroupIDs
		{
			get
			{
				return m_membershipRoleIDs;
			}
		}
		public String this[String key]
		{
			get
			{
				return m_columnData[key];
			}
		}
		public List<SecurityActionGroup> Groups
		{
			get
			{
				return m_groups;
			}
		}

		public Dictionary<string, string> Descriptor
		{
			get
			{
				return m_columnData;
			}
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}