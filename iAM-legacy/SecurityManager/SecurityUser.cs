using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;

namespace SecurityManager
{
	public class SecurityUser
	{
		private string m_userID;

		private Dictionary<String, String> m_columnData;
		private Dictionary<SecurityAction, String> m_accessData;
		private List<String> m_membershipRoleIDs;
		private List<SecurityUserGroup> m_groups;
		
		private string m_userIDColumnName;
		private string m_actionIDColumnName;

		public String this[String key]
		{
			get
			{
				return m_columnData[key];
			}
		}

		public Dictionary<SecurityAction, String> ActionPermission
		{
			get
			{
				return m_accessData;
			}
		}

		public List<String> GroupIDs
		{
			get
			{
				return m_membershipRoleIDs;
			}
		}

		public SecurityUser( Dictionary<String, String> userIdentification, List<SecurityAction> securityActions, string userIDColumnName, string actionIDColumnName )
		{
			m_userIDColumnName = userIDColumnName;
			m_actionIDColumnName = actionIDColumnName;
			m_columnData = userIdentification;
			m_userID = m_columnData[m_userIDColumnName];
			BuildUserMembership();
			GetAccessData( securityActions );
		}

		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is SecurityUser )
			{
				SecurityUser otherUser = ( SecurityUser )obj;
				foreach( string columnName in otherUser.m_columnData.Keys )
				{
					if( !this.m_columnData.ContainsKey( columnName ) || this.m_columnData[columnName] != otherUser.m_columnData[columnName] )
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

		private void BuildUserMembership()
		{
			m_membershipRoleIDs = new List<String>();
			m_groups = new List<SecurityUserGroup>();
			//string userID = m_parentManager.GetUserID( m_userID );
			String query = "SELECT USERGROUP_ID FROM SEC_USERGROUP_MEM WHERE "
				+ m_userIDColumnName + " = '" + m_userID + "'";

			DataSet groups = DBMgr.ExecuteQuery(query);
			foreach (DataRow group in groups.Tables[0].Rows)
			{
				m_membershipRoleIDs.Add(group[0].ToString().Trim());
			}
		}

		private void GetAccessData(List<SecurityAction> securityActions)
		{
			m_accessData = new Dictionary<SecurityAction, string>();

				BuildUserGroupActionGroupPermissions( securityActions );
				BuildUserGroupActionPermissions( securityActions );
				BuildUserActionGroupPermissions( securityActions );
				BuildUserActionPermissions( securityActions );
		}

		private void BuildUserGroupActionGroupPermissions( List<SecurityAction> securityActions )
		{
			if( securityActions.Count > 0 && m_membershipRoleIDs.Count > 0 )
			{
				String query = "SELECT agm.action_id, ugagp.access_level FROM SEC_USERGROUP_ACTIONGROUP_PER ugagp LEFT OUTER JOIN sec_actiongroup_mem agm ON ugagp.actiongroup_id = agm.actiongroup_id WHERE ugagp.usergroup_id IN (";
				foreach( String userGroupID in m_membershipRoleIDs )
				{
					query += "'" + userGroupID + "', ";
				}
				query = query.Remove( query.Length - 2 );
				query += ")";
				DataSet accessData = DBMgr.ExecuteQuery( query );
				foreach( DataRow actionRow in accessData.Tables[0].Rows )
				{
					SecurityAction match = securityActions.Find(
						delegate( SecurityAction a )
						{
							return a.ActionID == actionRow[m_actionIDColumnName].ToString();
						} );
					if( match != null )
					{
						m_accessData[match] = actionRow["ACCESS_LEVEL"].ToString();
					}
				}
			}
		}

		private void BuildUserGroupActionPermissions(List<SecurityAction> securityActions)
		{
			if( securityActions.Count > 0 && m_membershipRoleIDs.Count > 0 )
			{
				String query = "SELECT ugagp." + m_actionIDColumnName + ", ugagp.ACCESS_LEVEL FROM SEC_USERGROUP_ACTION_PER ugagp WHERE ugagp.usergroup_id IN (";
				foreach( String userGroupID in m_membershipRoleIDs )
				{
					query += "'" + userGroupID + "', ";
				}
				query = query.Remove( query.Length - 2 );
				query += ")";
				DataSet accessData = DBMgr.ExecuteQuery( query );
				foreach( DataRow actionRow in accessData.Tables[0].Rows )
				{
					SecurityAction match = securityActions.Find(
						delegate( SecurityAction a )
						{
							return a.ActionID == actionRow[m_actionIDColumnName].ToString();
						} );
					if( match != null )
					{
						m_accessData[match] = actionRow["ACCESS_LEVEL"].ToString();
					}
				}
			}
		}

		private void BuildUserActionGroupPermissions( List<SecurityAction> securityActions )
		{
			String query = "SELECT agm." + m_actionIDColumnName + ", ugap.access_level FROM SEC_USER_ACTIONGROUP_PER ugap LEFT OUTER JOIN sec_actiongroup_mem agm ON ugap.actiongroup_id = agm.actiongroup_id WHERE ugap." + m_userIDColumnName + " = '" + m_userID + "'";
			DataSet accessData = DBMgr.ExecuteQuery(query);
			foreach( DataRow actionRow in accessData.Tables[0].Rows )
			{
				SecurityAction match = securityActions.Find(
					delegate( SecurityAction a )
					{
						return a.ActionID == actionRow[m_actionIDColumnName].ToString();
					} );
				if( match != null )
				{
					m_accessData[match] = actionRow["ACCESS_LEVEL"].ToString();
				}
			}
		}

		private void BuildUserActionPermissions( List<SecurityAction> securityActions )
		{
			String query = "SELECT uap." + m_actionIDColumnName + ", uap.access_level FROM SEC_USER_ACTION_PER uap WHERE uap." + m_userIDColumnName + " = '" + m_userID + "'";
			DataSet accessData = DBMgr.ExecuteQuery(query);
			foreach( DataRow actionRow in accessData.Tables[0].Rows )
			{
				SecurityAction match = securityActions.Find(
					delegate( SecurityAction a )
					{
						return a.ActionID == actionRow[m_actionIDColumnName].ToString();
					} );
				if( match != null )
				{
					m_accessData[match] = actionRow["ACCESS_LEVEL"].ToString();
				}
			}
		}

		public void UpdateActions( List<SecurityAction> securityActions )
		{
			GetAccessData( securityActions );
		}

		public List<SecurityUserGroup> Groups
		{
			get
			{
				return m_groups;
			}
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}
