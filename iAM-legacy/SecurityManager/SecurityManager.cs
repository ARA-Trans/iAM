using System;
using System.Collections.Generic;
using System.Text;
using DatabaseManager;
using System.Data;
using Crypto;

namespace SecurityManager
{
	public class SecurityManager
	{
		private List<SecurityAction> m_securityActions = null;
		private List<SecurityUser> m_securityUsers = null;
		private List<SecurityActionGroup> m_securityActionGroups = null;
		private List<SecurityUserGroup> m_securityUserGroups = null;

		private string m_userIDColumnName;
		private string m_userPasswordColumnName;
		//private string m_userPasswordEncryptionKey;
		private string m_actionIDColumnName;

		/// <summary>
		/// Create a SecurityManager object with the user name and password to be authenticated.
		/// </summary>
		public SecurityManager( String actionIDColumnName, String userIDColumnName )
		{
			m_userIDColumnName = userIDColumnName;
			m_actionIDColumnName = actionIDColumnName;
		}

		public List<SecurityUser> AllUsers
		{
			get
			{
				if( m_securityUsers == null )
				{
					LoadAccessControlList();
				}				
				return m_securityUsers;
			}
		}

		public List<SecurityUserGroup> AllUserGroups
		{
			get
			{
				//string query = "SELECT USERGROUP_NAME FROM SEC_USERGROUPS";
				//DataSet allUserGroupNameSet = DBMgr.ExecuteQuery( query );
				//List<string> userGroupNames = new List<string>();
				//foreach( DataRow userGroupNameRow in allUserGroupNameSet.Tables[0].Rows )
				//{
				//    userGroupNames.Add( userGroupNameRow["USERGROUP_NAME"].ToString());
				//}
				if( m_securityUserGroups == null )
				{
					LoadAccessControlList();
				}
				return m_securityUserGroups;
			}
		}

		public List<SecurityAction> AllActions
		{
			get
			{
				if( m_securityActions == null )
				{
					//m_securityUsers = new List<SecurityUser>();
					//m_securityActions = new List<SecurityAction>();
					LoadAccessControlList();
				}
				return m_securityActions;
			}
		}

		public List<SecurityActionGroup> AllActionGroups
		{
			get
			{
				//string query = "SELECT ACTIONGROUP_NAME FROM SEC_ACTIONGROUPS";
				//DataSet allActionGroupNameSet = DBMgr.ExecuteQuery( query );
				//List<string> actionGroupNames = new List<string>();
				//foreach( DataRow actionGroupNameRow in allActionGroupNameSet.Tables[0].Rows )
				//{
				//    actionGroupNames.Add( actionGroupNameRow["ACTIONGROUP_NAME"].ToString());
				//}

				if( m_securityActionGroups == null )
				{
					LoadAccessControlList();
				}
				return m_securityActionGroups;
			}
		}


		/// <summary>
		/// Authenticates the user name and password with the database.
		/// </summary>
		/// <returns>True if user authentication is successful, false otherwise.</returns>
		//public bool AuthenticateUser(Dictionary<String,String> authenticationInformation, string passwordColumn, string passPhrase )
		public SecurityUser AuthenticateUser(Dictionary<String,String> authenticationInformation, string passwordColumn )
		{
			//LoadAccessControlList();
			string query = "SELECT * FROM SEC_USERS WHERE ";
			SecurityUser authenticatedUser = null;

			foreach( string columnName in authenticationInformation.Keys )
			{
				if( columnName != passwordColumn )
				{
					query += columnName + " = '" + authenticationInformation[columnName] + "' AND ";
				}
			}

			query = query.Remove( query.Length - 5 );

			DataSet passwordSet = DBMgr.ExecuteQuery( query );
			bool authenticated = false;
			if( passwordSet.Tables[0].Rows.Count > 0 )
			{
				if( passwordSet.Tables[0].Rows.Count > 1 )
				{
					throw new ArgumentException( "Error validating user: Ambiguous user specification." );
				}
				else
				{
					authenticated = CryptoManager.VerifyHash( authenticationInformation[passwordColumn], "MD5", ( string )passwordSet.Tables[0].Rows[0][passwordColumn] );
				}
			}
			else
			{
#if MDSHA
				AddUser( authenticationInformation, passwordColumn, "" );
				authenticatedUser = AuthenticateUser( authenticationInformation, passwordColumn );
				authenticated = false;		//authentication has succeeded, but we don't want to bother re-running the query
#else
				throw new ArgumentException( "Error validating user: Specified user not found." );
#endif
			}

			if( authenticated )
			{
				m_userPasswordColumnName = passwordColumn;
				passwordSet.Tables[0].Columns.Remove( passwordColumn );
				authenticatedUser = GetUserFromDescriptor( BuildDescriptorFromRow( passwordSet.Tables[0].Rows[0] ) );
			}
			return authenticatedUser;
		}

		public string GetUserID( Dictionary<String, String> userDescriptor )
		{
			string userID = "";
			foreach (SecurityUser user in m_securityUsers)
			{
				bool match = true;
				foreach( String IDKey in userDescriptor.Keys )
				{
					if( user[IDKey] != userDescriptor[IDKey] )
					{
						match = false;
						break;
					}
				}
				if (match)
				{
					userID = user[m_userIDColumnName];
					break;
				}
			}

			if( userID == "" )
			{
				throw new Exception( "User descriptor produced no ID." );
			}

			return userID;
		}

		private string GetActionID( Dictionary<string, string> actionDescriptor )
		{
			string actionID = "";
			foreach( SecurityAction action in m_securityActions )
			{
				bool match = true;
				foreach( String IDKey in actionDescriptor.Keys )
				{
					if( action[IDKey] != actionDescriptor[IDKey] )
					{
						match = false;
						break;
					}
				}
				if( match )
				{
					actionID = action[m_actionIDColumnName];
					break;
				}
			}

			return actionID;
		}

		/// <summary>
		/// Loads all user/object relationships.
		/// </summary>
		private void LoadAccessControlList()
		{
			m_securityActionGroups = new List<SecurityActionGroup>();
			m_securityActions = new List<SecurityAction>();
			m_securityUserGroups = new List<SecurityUserGroup>();
			m_securityUsers = new List<SecurityUser>();

			LoadActionList();
			LoadUserList();
			LoadActionGroupList();
			LoadUserGroupList();

			LinkActions();
			LinkUsers();

		}

		private void LinkActions()
		{
			foreach( SecurityActionGroup actionGroup in m_securityActionGroups )
			{
				foreach( SecurityAction action in actionGroup.Members )
				{
					action.Groups.Add( actionGroup );
				}
			}
		}

		private void LinkUsers()
		{
			foreach( SecurityUserGroup userGroup in m_securityUserGroups )
			{
				foreach( SecurityUser user in userGroup.Members )
				{
					user.Groups.Add( userGroup );
				}
			}
		}

		private void LoadActionList()
		{
			DataSet actions = DBMgr.ExecuteQuery( "SELECT * FROM SEC_ACTIONS" );
			foreach( DataRow actionRow in actions.Tables[0].Rows )
			{
				Dictionary<string, string> actionDescriptor = BuildDescriptorFromRow( actionRow );
				m_securityActions.Add( new SecurityAction( actionDescriptor, m_actionIDColumnName ) );
			}
		}

		private void LoadUserList()
		{
			DataSet users = DBMgr.ExecuteQuery( "SELECT * FROM SEC_USERS" );
			foreach( DataRow userRow in users.Tables[0].Rows )
			{
				Dictionary<String, String> userIdentification = BuildDescriptorFromRow( userRow );
				m_securityUsers.Add( new SecurityUser( userIdentification, m_securityActions, m_userIDColumnName, m_actionIDColumnName ) );
			}
		}

		private void LoadActionGroupList()
		{
			DataSet actionGroups = DBMgr.ExecuteQuery( "SELECT ACTIONGROUP_ID, ACTIONGROUP_NAME FROM SEC_ACTIONGROUPS" );
			foreach( DataRow actionGroupRow in actionGroups.Tables[0].Rows )
			{
				string id = actionGroupRow["ACTIONGROUP_ID"].ToString();
				string name = actionGroupRow["ACTIONGROUP_NAME"].ToString();
				m_securityActionGroups.Add( new SecurityActionGroup( id, name, m_securityActions.FindAll( delegate( SecurityAction a )
				{
					return a.GroupIDs.Contains( id );
				} ) ) );
			}
		}

		private void LoadUserGroupList()
		{
			DataSet userGroups = DBMgr.ExecuteQuery( "SELECT USERGROUP_ID, USERGROUP_NAME FROM SEC_USERGROUPS" );
			foreach( DataRow userGroupRow in userGroups.Tables[0].Rows )
			{
				string id = userGroupRow["USERGROUP_ID"].ToString();
				string name = userGroupRow["USERGROUP_NAME"].ToString();
				m_securityUserGroups.Add( new SecurityUserGroup( id, name, m_securityUsers.FindAll( delegate( SecurityUser a )
				{
					return a.GroupIDs.Contains( id );
				} ) ) );
			}
		}

		private Dictionary<string, string> BuildDescriptorFromRow( DataRow row )
		{
			Dictionary<string, string> descriptor = new Dictionary<string, string>();
			foreach( DataColumn column in row.Table.Columns )
			{
				descriptor.Add( column.ColumnName, row[column].ToString().Trim() );
			}

			return descriptor;
		}

		//public bool CreateUser( Dictionary<String, String> userDescriptor )
		//{
		//    return DBMgr.ExecuteNonQuery( GenerateInsertFromDescriptor( "SEC_USERS", userDescriptor ) ) > 0;
		//}

		//public bool ModifyUserSettings( Dictionary<String, String> userDescriptor, Dictionary<String, String> settingDescriptor )
		//{
		//    String query = "UPDATE SEC_USERS SET ";
		//    foreach( String updateColumn in settingDescriptor.Keys )
		//    {
		//        query += updateColumn + " = '" + settingDescriptor[updateColumn] + "', ";
		//    }
		//    query.Remove( query.Length - 2 );
		//    query += GenerateWhereClauseFromDescriptor( settingDescriptor );

		//    return DBMgr.ExecuteNonQuery( query ) > 0;
		//}

		public String GetPermissions( Dictionary<string, string> userSpecifier, Dictionary<string, string> actionSpecifier )
		{
			String permissionLevel = "";
			SecurityUser userToCheck = GetUserFromSpecifier( userSpecifier );
			if( userToCheck != null )
			{
				SecurityAction actionToCheck = GetActionFromDescriptor( actionSpecifier );
				if( actionToCheck != null )
				{
					if( userToCheck.ActionPermission.ContainsKey( actionToCheck ) )
					{
						permissionLevel = userToCheck.ActionPermission[actionToCheck];
					}
				}
			}
					
			return permissionLevel;
		}

		private SecurityUser GetUserFromSpecifier( Dictionary<string, string> userSpecifier )
		{
			SecurityUser correctUser = null;
			foreach( SecurityUser userToCheck in m_securityUsers )
			{
				bool found = true;
				foreach( string userSpecification in userSpecifier.Keys )
				{
					if( userToCheck[userSpecification] != userSpecifier[userSpecification] )
					{
						found = false;
						break;
					}
				}
				if( found )
				{
					correctUser = userToCheck;
					break;
				}
			}

			return correctUser;
		}

		private SecurityAction GetActionFromDescriptor( Dictionary<string, string> actionDescriptor )
		{
			SecurityAction correctAction = null;
			foreach( SecurityAction actionToCheck in m_securityActions )
			{
				bool found = true;
				foreach( string actionSpecification in actionDescriptor.Keys )
				{
					if( actionToCheck[actionSpecification] != actionDescriptor[actionSpecification] )
					{
						found = false;
						break;
					}
				}
				if( found )
				{
					correctAction = actionToCheck;
					break;
				}
			}

			return correctAction;
		}


		public List<SecurityAction> GetActionMembers( Dictionary<string,string> actionGroupDescriptor )
		{
			string actionGroupID = GetActionGroupID( actionGroupDescriptor );
			List<SecurityAction> members = new List<SecurityAction>();
			foreach( SecurityAction action in m_securityActions )
			{
				if( action.GroupIDs.Contains( actionGroupID ) )
				{
					members.Add( action );
				}
			}
			return members;
		}

		private string GetActionGroupID( Dictionary<string, string> actionGroupDescriptor )
		{
			string query = "SELECT ACTIONGROUP_ID FROM SEC_ACTIONGROUPS";
			query += GenerateWhereClauseFromDescriptor( actionGroupDescriptor );
			return DBMgr.ExecuteQuery( query ).Tables[0].Rows[0]["ACTIONGROUP_ID"].ToString();
		}

		public List<SecurityUser> GetUserMembers( Dictionary<string, string> userGroupDescriptor )
		{
			string userGroupID = GetUserGroupID( userGroupDescriptor );
			List<SecurityUser> members = new List<SecurityUser>();
			foreach( SecurityUser user in m_securityUsers )
			{
				if( user.GroupIDs.Contains( userGroupID ) )
				{
					members.Add( user );
				}
			}
			return members;
		}

		private string GetUserGroupID( Dictionary<string, string> userGroupDescriptor )
		{
			string query = "SELECT USERGROUP_ID FROM SEC_USERGROUPS";
			query += GenerateWhereClauseFromDescriptor( userGroupDescriptor );
			return DBMgr.ExecuteQuery( query ).Tables[0].Rows[0]["USERGROUP_ID"].ToString();
		}

		public bool AddUserGroup( Dictionary<string, string> newUserGroupDescriptor )
		{
			bool added = false;
			if( !UserGroupExists( newUserGroupDescriptor ) )
			{
				string query = GenerateInsertFromDescriptor( "SEC_USERGROUPS", newUserGroupDescriptor );
				added = added = DBMgr.ExecuteNonQuery( query ) > 0;
				if( added )
				{
					Dictionary<string,string> completeUserGroupDescriptor = GenerateFullUserGroupDescriptor( newUserGroupDescriptor );
					m_securityUserGroups.Add( new SecurityUserGroup( completeUserGroupDescriptor ));
				}
			}
			else
			{
				throw new Exception( "Usergroup already defined." );
			}
			return added;
		}

		private Dictionary<string, string> GenerateFullUserGroupDescriptor( Dictionary<string, string> partialUserGroupDescriptor )
		{
			Dictionary<string, string> fullUserGroupDescriptor = new Dictionary<string, string>();
			string query = "SELECT * FROM SEC_USERGROUPS";

			Dictionary<string, string> tempUserDescriptor = new Dictionary<string, string>();

			foreach( string key in partialUserGroupDescriptor.Keys )
			{
				tempUserDescriptor[key] = partialUserGroupDescriptor[key];
			}

			query += GenerateWhereClauseFromDescriptor( tempUserDescriptor );
			DataSet fullUserGroupSet = DBMgr.ExecuteQuery( query );

			if( fullUserGroupSet.Tables[0].Rows.Count < 1 )
			{
				throw new Exception( "Usergroup descriptor produced no results." );
			}
			else if( fullUserGroupSet.Tables[0].Rows.Count > 1 )
			{
				throw new Exception( "Usergroup descriptor produced ambiguous results." );
			}
			else
			{
				foreach( DataColumn column in fullUserGroupSet.Tables[0].Columns )
				{
					fullUserGroupDescriptor.Add( column.ColumnName, fullUserGroupSet.Tables[0].Rows[0][column].ToString().Trim() );
				}
			}

			return fullUserGroupDescriptor;
		}

		private bool UserGroupExists( Dictionary<string, string> userGroupDescriptor )
		{
			return m_securityUserGroups.Exists( 
	delegate( SecurityUserGroup userGroup )
	{
		return userGroup.Equals( userGroupDescriptor );
	} );
		}

		public bool AddActionGroup( Dictionary<string, string> newActionGroupDescriptor )
		{
			bool added = false;
			if( !ActionGroupExists( newActionGroupDescriptor ) )
			{
				string query = GenerateInsertFromDescriptor( "SEC_ACTIONGROUPS", newActionGroupDescriptor );

				added = DBMgr.ExecuteNonQuery( query ) > 0;

				if( added )
				{
					Dictionary<string, string> completeActionGroupDescriptor = GenerateFullActionGroupDescriptor( newActionGroupDescriptor );
					m_securityActionGroups.Add( new SecurityActionGroup( completeActionGroupDescriptor ) );
				}
			}
			else
			{
				throw new Exception( "Actiongroup already defined." );
			}



			return added;
		}

		private Dictionary<string, string> GenerateFullActionGroupDescriptor( Dictionary<string, string> partialActionGroupDescriptor )
		{
			Dictionary<string, string> fullActionGroupDescriptor = new Dictionary<string, string>();
			string query = "SELECT * FROM SEC_ACTIONGROUPS";

			Dictionary<string, string> tempActionDescriptor = new Dictionary<string, string>();

			foreach( string key in partialActionGroupDescriptor.Keys )
			{
				tempActionDescriptor[key] = partialActionGroupDescriptor[key];
			}

			query += GenerateWhereClauseFromDescriptor( tempActionDescriptor );
			DataSet fullActionGroupSet = DBMgr.ExecuteQuery( query );

			if( fullActionGroupSet.Tables[0].Rows.Count < 1 )
			{
				throw new Exception( "Actiongroup descriptor produced no results." );
			}
			else if( fullActionGroupSet.Tables[0].Rows.Count > 1 )
			{
				throw new Exception( "Actiongroup descriptor produced ambiguous results." );
			}
			else
			{
				foreach( DataColumn column in fullActionGroupSet.Tables[0].Columns )
				{
					fullActionGroupDescriptor.Add( column.ColumnName, fullActionGroupSet.Tables[0].Rows[0][column].ToString().Trim() );
				}
			}

			return fullActionGroupDescriptor;
		}

		//public bool AddUser( Dictionary<string, string> newUserDescriptor, string passwordColumn, string unencryptedPassword, string passphrase )
		public bool AddUser( Dictionary<string, string> newUserDescriptor, string passwordColumn, string unencryptedPassword )
		{
			//can't use the default insert function anymore because password encryption requires special care
			//string query = GenerateInsertFromDescriptor( "SEC_USERS", newUserDescriptor );

			string encryptedPassword = CryptoManager.ComputeHash( unencryptedPassword, "MD5" );
			string insertClause = "INSERT INTO SEC_USERS (";
			string valuesClause = " VALUES ( ";
			bool first = true;
            foreach (string columnName in newUserDescriptor.Keys)
            {
                if (columnName != passwordColumn)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        insertClause += ", ";
                        valuesClause += ", ";
                    }

                    insertClause += columnName;
                    valuesClause += "'" + newUserDescriptor[columnName] + "'";
                }
            }


			insertClause += ", " + passwordColumn + ")";
			valuesClause += ", '" + encryptedPassword + "')";

			string query = insertClause + valuesClause;

			bool added = false;

			SecurityUser userToAdd = null;
			if( DBMgr.ExecuteNonQuery( query ) > 0 )
			{
				Dictionary<string, string> completeUserDescriptor = GenerateFullUserDescriptor( newUserDescriptor, passwordColumn );
				userToAdd = new SecurityUser( completeUserDescriptor, m_securityActions, m_userIDColumnName, m_actionIDColumnName );
				m_securityUsers.Add( userToAdd );
				added = true;
			}

			return added;
		}

		private Dictionary<string, string> GenerateFullUserDescriptor( Dictionary<string, string> partialUserDescriptor, string passwordColumn )
		{
			Dictionary<string, string> fullUserDescriptor = new Dictionary<string,string>();
			string query = "SELECT * FROM SEC_USERS";

			Dictionary<string, string> tempUserDescriptor = new Dictionary<string, string>();

			foreach( string key in partialUserDescriptor.Keys )
			{
				if( key != passwordColumn )
				{
					tempUserDescriptor[key] = partialUserDescriptor[key];
				}
			}

			query += GenerateWhereClauseFromDescriptor( tempUserDescriptor );
			DataSet fullUserSet = DBMgr.ExecuteQuery( query );

			if( fullUserSet.Tables[0].Rows.Count < 1 )
			{
				throw new Exception( "User descriptor produced no results." );
			}
			else if( fullUserSet.Tables[0].Rows.Count > 1 )
			{
				throw new Exception( "User descriptor produced ambiguous results." );
			}
			else
			{
				foreach( DataColumn column in fullUserSet.Tables[0].Columns )
				{
					if( column.ColumnName != passwordColumn )
					{
						fullUserDescriptor.Add( column.ColumnName, fullUserSet.Tables[0].Rows[0][column].ToString().Trim() );
					}
				}
			}

			return fullUserDescriptor;
		}

		public bool AddUserToGroup( Dictionary<string, string> userDescriptor, Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			string groupID = GetUserGroupID( groupDescriptor );

			Dictionary<string, string> userGroupMembershipDescriptor = new Dictionary<string, string>();
			userGroupMembershipDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userGroupMembershipDescriptor.Add( "USERGROUP_ID", groupID );

			string query = GenerateInsertFromDescriptor( "SEC_USERGROUP_MEM", userGroupMembershipDescriptor );
			bool added = DBMgr.ExecuteNonQuery( query ) > 0;
			if( added )
			{
				SecurityUser changedUser = GetUserFromDescriptor( userDescriptor );
				changedUser.GroupIDs.Add( groupID );
				UpdateUserAccess( userOfInterest );
			}

			return added;
		}

		private SecurityUser GetUserFromDescriptor( Dictionary<string, string> userDescriptor )
		{
			SecurityUser foundUser = null;
			//SecurityUser checkUser = new SecurityUser( userDescriptor, m_securityActions, this );
			foreach( SecurityUser user in m_securityUsers )
			{
				if( user.Equals( userDescriptor ) )
				{
					foundUser = user;
					break;
				}
			}

			return foundUser;
		}

		public bool RemoveUserFromGroup( Dictionary<string, string> userDescriptor, Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			string groupID = GetUserGroupID( groupDescriptor );
			string query = "DELETE FROM SEC_USERGROUP_MEM";

			Dictionary<string, string> userGroupMembershipDescriptor = new Dictionary<string,string>();
			userGroupMembershipDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userGroupMembershipDescriptor.Add( "USERGROUP_ID", groupID );

			query += GenerateWhereClauseFromDescriptor( userGroupMembershipDescriptor );

			bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			if( removed )
			{
				SecurityUser changedUser = GetUserFromDescriptor( userDescriptor );
				changedUser.GroupIDs.Remove( groupID );
				UpdateUserAccess( userOfInterest );
			}

			return removed;
		}

		public bool AddActionToGroup( Dictionary<string, string> actionDescriptor, Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			string groupID = GetActionGroupID( groupDescriptor );

			Dictionary<string, string> actionGroupMembershipDescriptor = new Dictionary<string, string>();
			actionGroupMembershipDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );
			actionGroupMembershipDescriptor.Add( "ACTIONGROUP_ID", groupID );

			string query = GenerateInsertFromDescriptor( "SEC_ACTIONGROUP_MEM", actionGroupMembershipDescriptor );
			bool added = DBMgr.ExecuteNonQuery( query ) > 0;
			if( added )
			{
				SecurityAction changedAction = GetActionFromDescriptor( actionDescriptor );
				changedAction.GroupIDs.Add( groupID );
				UpdateUserAccess( userOfInterest );
			}

			return added;
		}

		public bool RemoveActionFromGroup( Dictionary<string, string> actionDescriptor, Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			string groupID = GetActionGroupID( groupDescriptor );
			string query = "DELETE FROM SEC_ACTIONGROUP_MEM";
			
			Dictionary<string, string> actionGroupMembershipDescriptor = new Dictionary<string, string>();
			actionGroupMembershipDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );
			actionGroupMembershipDescriptor.Add( "ACTIONGROUP_ID", groupID );
			
			query += GenerateWhereClauseFromDescriptor( actionGroupMembershipDescriptor );

			bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			if( removed )
			{
				SecurityAction changedAction = GetActionFromDescriptor( actionDescriptor );
				changedAction.GroupIDs.Remove( groupID );
				UpdateUserAccess( userOfInterest );
			}

			return removed;
		}

		public bool SetUserGroupActionGroupPermissions( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionGroupDescriptor, string permissionLevel, SecurityUser userOfInterest )
		{
			string query = "";

			Dictionary<string, string> userGroupActionGroupPermisssionsDescriptor = new Dictionary<string, string>();
			userGroupActionGroupPermisssionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionGroupPermisssionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ) );

			if( GetUserGroupActionGroupPermission( userGroupDescriptor, actionGroupDescriptor ) != "" )
			{
				query = "UPDATE SEC_USERGROUP_ACTIONGROUP_PER SET ACCESS_LEVEL = '" + permissionLevel + "'";
				query += GenerateWhereClauseFromDescriptor( userGroupActionGroupPermisssionsDescriptor );
			}
			else
			{
				userGroupActionGroupPermisssionsDescriptor.Add( "ACCESS_LEVEL", permissionLevel );
				query = GenerateInsertFromDescriptor( "SEC_USERGROUP_ACTIONGROUP_PER", userGroupActionGroupPermisssionsDescriptor );
			}

			bool permissionsSet = (DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool RemoveUserGroupActionGroupPermissions( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionGroupDescriptor, SecurityUser userOfInterest )
		{
			string query = "DELETE FROM SEC_USERGROUP_ACTIONGROUP_PER";

			Dictionary<string, string> userGroupActionGroupPermissionsDescriptor = new Dictionary<string,string>();
			userGroupActionGroupPermissionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionGroupPermissionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ));

			query += GenerateWhereClauseFromDescriptor( userGroupActionGroupPermissionsDescriptor );

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool SetUserGroupActionPermissions( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionDescriptor, string permissionLevel, SecurityUser userOfInterest )
		{
			string query = "";

			Dictionary<string, string> userGroupActionPermissionsDescriptor = new Dictionary<string, string>();
			userGroupActionPermissionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionPermissionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );

			if( GetUserGroupActionPermission( userGroupDescriptor, actionDescriptor ) != "" )
			{
				query = "UPDATE SEC_USERGROUP_ACTION_PER SET ACCESS_LEVEL = '" + permissionLevel + "'";
				query += GenerateWhereClauseFromDescriptor( userGroupActionPermissionsDescriptor );

			}
			else
			{
				userGroupActionPermissionsDescriptor.Add( "ACCESS_LEVEL", permissionLevel );
				query = GenerateInsertFromDescriptor( "SEC_USERGROUP_ACTION_PER", userGroupActionPermissionsDescriptor );
			}

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool RemoveUserGroupActionPermissions( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionDescriptor, SecurityUser userOfInterest )
		{
			string query = "DELETE FROM SEC_USERGROUP_ACTION_PER";
			
			Dictionary<string, string> userGroupActionPermissionsDescriptor = new Dictionary<string,string>();
			userGroupActionPermissionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionPermissionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ));

			query += GenerateWhereClauseFromDescriptor( userGroupActionPermissionsDescriptor );

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool SetUserActionGroupPermissions( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionGroupDescriptor, string permissionLevel, SecurityUser userOfInterest )
		{
			string query = "";

			Dictionary<string, string> userActionGroupPermisssionsDescriptor = new Dictionary<string, string>();
			userActionGroupPermisssionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userActionGroupPermisssionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ) );
	
			if( GetUserActionGroupPermission( userDescriptor, actionGroupDescriptor ) != "" )
			{
				query = "UPDATE SEC_USER_ACTIONGROUP_PER SET ACCESS_LEVEL = '" + permissionLevel + "'";
				query += GenerateWhereClauseFromDescriptor( userActionGroupPermisssionsDescriptor );
			}
			else
			{
				userActionGroupPermisssionsDescriptor.Add( "ACCESS_LEVEL", permissionLevel );
				query = GenerateInsertFromDescriptor( "SEC_USER_ACTIONGROUP_PER", userActionGroupPermisssionsDescriptor );
			}

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool RemoveUserActionGroupPermissions( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionGroupDescriptor, SecurityUser userOfInterest )
		{
			string query = "DELETE FROM SEC_USER_ACTIONGROUP_PER";
			
			Dictionary<string, string> userGroupActionPermissionsDescriptor = new Dictionary<string,string>();
			userGroupActionPermissionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userGroupActionPermissionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ));

			query += GenerateWhereClauseFromDescriptor( userGroupActionPermissionsDescriptor );

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool SetUserActionPermissions( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionDescriptor, string permissionLevel, SecurityUser userOfInterest )
		{
			string query = "";

			Dictionary<string, string> userActionPermisssionsDescriptor = new Dictionary<string, string>();
			userActionPermisssionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userActionPermisssionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );

			if( GetUserActionPermission( userDescriptor, actionDescriptor ) != "" )
			{
				query = "UPDATE SEC_USER_ACTION_PER SET ACCESS_LEVEL = '" + permissionLevel + "'";
				query += GenerateWhereClauseFromDescriptor( userActionPermisssionsDescriptor );
			}
			else
			{
				userActionPermisssionsDescriptor.Add( "ACCESS_LEVEL", permissionLevel );
				query = GenerateInsertFromDescriptor( "SEC_USER_ACTION_PER", userActionPermisssionsDescriptor );
			}

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public bool RemoveUserActionPermissions( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionDescriptor, SecurityUser userOfInterest )
		{
			string query = "DELETE FROM SEC_USER_ACTION_PER";

			Dictionary<string, string> userActionPermissionsDescriptor = new Dictionary<string,string>();
			userActionPermissionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userActionPermissionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ));

			query += GenerateWhereClauseFromDescriptor( userActionPermissionsDescriptor );

			bool permissionsSet = ( DBMgr.ExecuteNonQuery( query ) > 0 );
			if( permissionsSet )
			{
				UpdateUserAccess( userOfInterest );
			}
			return permissionsSet;
		}

		public string GetUserGroupActionGroupPermission( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionGroupDescriptor )
		{
			string query = "SELECT ACCESS_LEVEL FROM SEC_USERGROUP_ACTIONGROUP_PER";

			Dictionary<string, string> userGroupActionGroupPermissionsDescriptor = new Dictionary<string,string>();
			userGroupActionGroupPermissionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionGroupPermissionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userGroupActionGroupPermissionsDescriptor );

			DataSet permissionSet = DBMgr.ExecuteQuery( query );
			string permissions = "";
			if( permissionSet.Tables[0].Rows.Count > 0 )
			{
				permissions = permissionSet.Tables[0].Rows[0]["ACCESS_LEVEL"].ToString();
			}
			return permissions;
		}

		public string GetUserGroupActionPermission( Dictionary<string, string> userGroupDescriptor, Dictionary<string, string> actionDescriptor )
		{
			string query = "SELECT ACCESS_LEVEL FROM SEC_USERGROUP_ACTION_PER";
			
			Dictionary<string, string> userGroupActionPermissionsDescriptor = new Dictionary<string,string>();
			userGroupActionPermissionsDescriptor.Add( "USERGROUP_ID", GetUserGroupID( userGroupDescriptor ) );
			userGroupActionPermissionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userGroupActionPermissionsDescriptor );
			
			DataSet permissionSet = DBMgr.ExecuteQuery( query );
			string permissions = "";
			if( permissionSet.Tables[0].Rows.Count > 0 )
			{
				permissions = permissionSet.Tables[0].Rows[0]["ACCESS_LEVEL"].ToString();
			}
			return permissions;
		}

		public string GetUserActionGroupPermission( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionGroupDescriptor )
		{
			string query = "SELECT ACCESS_LEVEL FROM SEC_USER_ACTIONGROUP_PER";
			
			Dictionary<string, string> userActionGroupPermissionsDescriptor = new Dictionary<string,string>();
			userActionGroupPermissionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userActionGroupPermissionsDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( actionGroupDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userActionGroupPermissionsDescriptor );
			
			DataSet permissionSet = DBMgr.ExecuteQuery( query );
			string permissions = "";
			if( permissionSet.Tables[0].Rows.Count > 0 )
			{
				permissions = permissionSet.Tables[0].Rows[0]["ACCESS_LEVEL"].ToString();
			}
			return permissions;
		}

		public string GetUserActionPermission( Dictionary<string, string> userDescriptor, Dictionary<string, string> actionDescriptor )
		{
			string query = "SELECT ACCESS_LEVEL FROM SEC_USER_ACTION_PER";

			Dictionary<string, string> userActionPermissionsDescriptor = new Dictionary<string,string>();
			userActionPermissionsDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );
			userActionPermissionsDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userActionPermissionsDescriptor );
			
			DataSet permissionSet = DBMgr.ExecuteQuery( query );
			string permissions = "";
			if( permissionSet.Tables[0].Rows.Count > 0 )
			{
				permissions = permissionSet.Tables[0].Rows[0]["ACCESS_LEVEL"].ToString();
			}
			return permissions;
		}

		public string AddAction( Dictionary<string, string> newActionDescriptor, SecurityUser userOfInterest )
		{
			bool added = false;
			string addedID = "";
			if( !ActionExists( newActionDescriptor ) )
			{
				string query = GenerateInsertFromDescriptor( "SEC_ACTIONS", newActionDescriptor );

				added = DBMgr.ExecuteNonQuery( query ) > 0;
				if( added )
				{
					Dictionary<string, string> completeActionDescriptor = GenerateFullActionDescriptor( newActionDescriptor );
					m_securityActions.Add( new SecurityAction( completeActionDescriptor, m_actionIDColumnName ) );
					UpdateUserAccess( userOfInterest );
					addedID = completeActionDescriptor[m_actionIDColumnName];
				}
			}
			else
			{
				//throw new Exception( "Action already defined." );
				//Check with Dave to see what appropriate action is.
				addedID = "-1";
			}

			return addedID;
		}

		public bool ActionExists( Dictionary<string, string> actionDescriptor )
		{
			//bool exists = false;
			//foreach( SecurityAction action in m_securityActions )
			//{
			//    if( action.Equals( actionDescriptor ) )
			//    {
			//        exists = true;
			//        break;
			//    }
			//}
			return m_securityActions.Exists(
				delegate( SecurityAction action )
				{
					return action.Equals( actionDescriptor );
				});
		}

		private void UpdateUserAccess( SecurityUser user )
		{
			if( user != null )
			{
				user.UpdateActions( m_securityActions );
			}
		}

		private Dictionary<string, string> GenerateFullActionDescriptor( Dictionary<string, string> partialActionDescriptor )
		{
			Dictionary<string, string> fullActionDescriptor = null;
			string query = "SELECT * FROM SEC_ACTIONS";
			query += GenerateWhereClauseFromDescriptor( partialActionDescriptor );

			DataSet fullActionSet = DBMgr.ExecuteQuery( query );
			if( fullActionSet.Tables[0].Rows.Count < 1 )
			{
				throw new Exception( "Action descriptor produced no results." );
			}
			else if( fullActionSet.Tables[0].Rows.Count > 1 )
			{
				throw new Exception( "Action descriptor produced ambiguous results." );
			}
			else
			{
				fullActionDescriptor = BuildDescriptorFromRow( fullActionSet.Tables[0].Rows[0] );
			}

			return fullActionDescriptor;
		}

		//public bool AddActionGroup( Dictionary<string, string> newActionGroupDescriptor )
		//{
		//    bool added = false;
		//    if( !ActionGroupExists( newActionGroupDescriptor ) )
		//    {
		//        string query = GenerateInsertFromDescriptor( "SEC_ACTIONGROUPS", newActionGroupDescriptor );

		//        added = DBMgr.ExecuteNonQuery( query ) > 0;

		//        if( added )
		//        {
		//            Dictionary<string, string> completeActionGroupDescriptor = GenerateFullActionGroupDescriptor( newActionGroupDescriptor );
		//            m_securityActions.Add( new SecurityActionGroup( completeActionGroupDescriptor ) );
		//        }
		//    }
		//    else
		//    {
		//        throw new Exception( "Actiongroup already defined." );
		//    }



		//    return added;
		//}

		private bool ActionGroupExists( Dictionary<string, string> actionGroupDescriptor )
		{
			string query = "SELECT COUNT(*) FROM SEC_ACTIONGROUPS";
			query += GenerateWhereClauseFromDescriptor( actionGroupDescriptor );

			return DBMgr.ExecuteScalar( query ) > 0;
		}

		private string GenerateWhereClauseFromDescriptor( Dictionary<string, string> descriptor )
		{
			string clause = " WHERE ";
			bool first = true;
			foreach( string columnName in descriptor.Keys )
			{
				if( first )
				{
					first = false;
				}
				else
				{
					clause += " AND ";
				}
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						clause += columnName + " = '" + descriptor[columnName] + "'";
						break;
					case "ORACLE":
						if( descriptor[columnName] != "" )
						{
							clause += columnName + " = '" + descriptor[columnName] + "'";
						}
						else
						{
							clause += columnName + " IS NULL";
						}
						break;
					default:
						throw new NotImplementedException( "TODO: implement ANSI version of GenerateWhereClauseFromDescriptor()" );
				}
			}

			return clause;
		}

		private string GenerateInsertFromDescriptor( string tableName, Dictionary<string, string> descriptor )
		{
			string insertClause = "INSERT INTO " + tableName + " (";
			string valuesClause = " VALUES ( ";
			bool first = true;
			foreach( string columnName in descriptor.Keys )
			{
				if( first )
				{
					first = false;
				}
				else
				{
					insertClause += ", ";
					valuesClause += ", ";
				}
				insertClause += columnName;
				valuesClause += "'" + descriptor[columnName] + "'";
			}
			insertClause += ")";
			valuesClause += ")";
			return insertClause + valuesClause;
		}

		public List<string> GetUserGroups( Dictionary<string, string> userDescriptor )
		{
			List<string> groupNames = new List<string>();
			string userID = GetUserID( userDescriptor );
			string innerQuery = "SELECT USERGROUP_ID FROM SEC_USERGROUP_MEM WHERE " +
				m_userIDColumnName + " = '" + userID + "'";
			string query = "SELECT USERGROUP_NAME FROM SEC_USERGROUPS WHERE USERGROUP_ID IN (" + innerQuery + ")";
			DataSet membershipSet = DBMgr.ExecuteQuery( query );
			foreach( DataRow membershipRow in membershipSet.Tables[0].Rows )
			{
				groupNames.Add( membershipRow["USERGROUP_NAME"].ToString() );
			}

			return groupNames;
		}

		public bool UpdateUserInformation( Dictionary<string, string> userDescriptor, Dictionary<string, string> newInformation )
		{
			String query = "UPDATE SEC_USERS SET ";
			foreach( String updateColumn in newInformation.Keys )
			{
				query += updateColumn + " = '" + newInformation[updateColumn] + "', ";
			}
			query = query.Remove( query.Length - 2 );
			query += " WHERE " + m_userIDColumnName + " = '" + GetUserID( userDescriptor ) + "'";

			return DBMgr.ExecuteNonQuery( query ) > 0;
		}

		public bool RemoveUser( Dictionary<string, string> userDescriptor )
		{
			string query = "DELETE FROM SEC_USERS";

			Dictionary<string, string> userIDDescriptor = new Dictionary<string, string>();
			userIDDescriptor.Add( m_userIDColumnName, GetUserID( userDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userIDDescriptor );

			bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			if( removed )
			{
				SecurityUser removedUser = GetUserFromDescriptor( userDescriptor );
				m_securityUsers.Remove( removedUser );
			}

			return removed;
		}

		public bool UpdateUserGroupInformation( Dictionary<string, string> groupDescriptor, Dictionary<string, string> newInformation )
		{
			String query = "UPDATE SEC_USERGROUPS SET ";
			foreach( String updateColumn in newInformation.Keys )
			{
				query += updateColumn + " = '" + newInformation[updateColumn] + "', ";
			}
			query = query.Remove( query.Length - 2 );
			query += " WHERE USERGROUP_ID  = '" + GetUserGroupID( groupDescriptor ) + "'";

			return DBMgr.ExecuteNonQuery( query ) > 0;
		}

		public bool RemoveUserGroup( Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			string query = "DELETE FROM SEC_USERGROUPS";

			Dictionary<string, string> userGroupIDDescriptor = new Dictionary<string, string>();
			userGroupIDDescriptor.Add( "USERGROUP_ID", GetUserGroupID( groupDescriptor ) );

			query += GenerateWhereClauseFromDescriptor( userGroupIDDescriptor );

			bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			if( removed )
			{
				UpdateUserAccess( userOfInterest );
			}

			return removed;
		}

		public bool UpdateActionGroupInformation( Dictionary<string, string> groupDescriptor, Dictionary<string, string> newInformation )
		{
			String query = "UPDATE SEC_ACTIONGROUPS SET ";
			foreach( String updateColumn in newInformation.Keys )
			{
				query += updateColumn + " = '" + newInformation[updateColumn] + "', ";
			}
			query = query.Remove( query.Length - 2 );
			query += " WHERE ACTIONGROUP_ID  = '" + GetActionGroupID( groupDescriptor ) + "'";

			return DBMgr.ExecuteNonQuery( query ) > 0;
		}

		public bool RemoveActionGroup( Dictionary<string, string> groupDescriptor, SecurityUser userOfInterest )
		{
			//string query = "DELETE FROM SEC_ACTIONGROUPS";

			//Dictionary<string, string> actionGroupIDDescriptor = new Dictionary<string, string>();
			//actionGroupIDDescriptor.Add( "ACTIONGROUP_ID", GetActionGroupID( groupDescriptor ) );

			//query += GenerateWhereClauseFromDescriptor( actionGroupIDDescriptor );

			//bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			//if( removed )
			//{
			//    UpdateUserAccess( userOfInterest );
			//}

			//return removed;

			string seekID = GetActionGroupID( groupDescriptor );
			SecurityActionGroup groupToRemove = m_securityActionGroups.Find( delegate( SecurityActionGroup g )
			{
				return g.ID == seekID;
			} );

			return RemoveActionGroup( groupToRemove, userOfInterest );
		}

		public bool RemoveActionGroup( SecurityActionGroup actionGroup, SecurityUser userOfInterest )
		{
			bool success = false;
			string testQuery = "SELECT COUNT(*) FROM SEC_ACTIONGROUPS WHERE ACTIONGROUP_ID = '" + actionGroup.ID + "'";
			string deleteQuery = "DELETE FROM SEC_ACTIONGROUPS WHERE ACTIONGROUP_ID = '" + actionGroup.ID + "'";

			int affectedRows = DBMgr.ExecuteScalar( testQuery );
			if( affectedRows > 0 )
			{
				if( affectedRows != 1 )
				{
					throw new ArgumentException( "ERROR: ambiguous ActionGroup specification.  More than one ActionGroup with ID = '" + actionGroup.ID + "' in database." );
				}
			}
			else
			{
				throw new ArgumentException( "ERROR: could not locate ActionGroup with ID = '" + actionGroup.ID + "' in database." );
			}
			if( DBMgr.ExecuteNonQuery( deleteQuery ) == 1 )
			{
				foreach( SecurityAction action in actionGroup.Members )
				{
					action.Groups.Remove( actionGroup );
				}
				m_securityActionGroups.Remove( actionGroup );

				UpdateUserAccess( userOfInterest );
				success = true;
			}
			else
			{
				throw new ArgumentException( "ERROR: When attempting to delete ActionGroup with ID = '" + actionGroup.ID + "', DELETE statement did not affect the expected number of rows (1)." );
			}

			return success;
		}

		public List<string> GetActionGroups( Dictionary<string, string> actionDescriptor )
		{
			List<string> groupNames = new List<string>();
			string actionID = GetActionID( actionDescriptor );
			string innerQuery = "SELECT ACTIONGROUP_ID FROM SEC_ACTIONGROUP_MEM WHERE " +
				m_actionIDColumnName + " = '" + actionID + "'";
			string query = "SELECT ACTIONGROUP_NAME FROM SEC_ACTIONGROUPS WHERE ACTIONGROUP_ID IN (" + innerQuery + ")";
			DataSet membershipSet = DBMgr.ExecuteQuery( query );
			foreach( DataRow membershipRow in membershipSet.Tables[0].Rows )
			{
				groupNames.Add( membershipRow["ACTIONGROUP_NAME"].ToString() );
			}

			return groupNames;
		}

		public bool UpdateActionInformation( Dictionary<string, string> actionDescriptor, Dictionary<string, string> newInformation )
		{
			String query = "UPDATE SEC_ACTIONS SET ";
			foreach( String updateColumn in newInformation.Keys )
			{
				query += updateColumn + " = '" + newInformation[updateColumn] + "', ";
			}
			query = query.Remove( query.Length - 2 );
			query += " WHERE " + m_actionIDColumnName + " = '" + GetActionID( actionDescriptor ) + "'";

			return DBMgr.ExecuteNonQuery( query ) > 0;
		}

		public bool RemoveAction( Dictionary<string, string> actionDescriptor, SecurityUser userOfInterest )
		{
			//string query = "DELETE FROM SEC_ACTIONS";

			//Dictionary<string, string> actionIDDescriptor = new Dictionary<string, string>();
			//actionIDDescriptor.Add( m_actionIDColumnName, GetActionID( actionDescriptor ) );

			//query += GenerateWhereClauseFromDescriptor( actionIDDescriptor );

			//bool removed = DBMgr.ExecuteNonQuery( query ) > 0;
			//if( removed )
			//{
			//    SecurityAction removedAction = GetActionFromDescriptor( actionDescriptor );
			//    foreach( SecurityActionGroup group in removedAction.Groups )
			//    {
			//        group.Members.Remove( removedAction );
			//    }
			//    m_securityActions.Remove( removedAction );

			//    UpdateUserAccess( userOfInterest );
			//}

			//return removed;

			SecurityAction removedAction = GetActionFromDescriptor( actionDescriptor );

			return RemoveAction( removedAction, userOfInterest );
		}

		public bool RemoveAction( SecurityAction action, SecurityUser userOfInterest )
		{
			bool success = false;
			string testQuery = "SELECT COUNT(*) FROM SEC_ACTIONS WHERE " + m_actionIDColumnName + " = '" + action.ActionID + "'";
			string deleteQuery = "DELETE FROM SEC_ACTIONS WHERE " + m_actionIDColumnName + " = '" + action.ActionID + "'";

			int affectedRows = DBMgr.ExecuteScalar( testQuery );
			if( affectedRows > 0 )
			{
				if( affectedRows != 1 )
				{
					throw new ArgumentException( "ERROR: ambiguous action specification.  More than one action with ID = '" + action.ActionID + "' in database." );
				}
			}
			else
			{
				throw new ArgumentException( "ERROR: could not locate Action with ID = '" + action.ActionID + "' in database." );
			}
			if( DBMgr.ExecuteNonQuery( deleteQuery ) == 1 )
			{
				foreach( SecurityActionGroup group in action.Groups )
				{
					group.Members.Remove( action );
				}
				m_securityActions.Remove( action );

				UpdateUserAccess( userOfInterest );
				success = true;
			}
			else
			{
				throw new ArgumentException( "ERROR: When attempting to delete action with ID = " + action.ActionID + ", DELETE statement did not affect the expected number of rows (1)." );
			}

			return success;
		}



		public List<SecurityAction> GetAllActions( Dictionary<string, string> actionDescriptor )
		{
			return AllActions.FindAll( delegate( SecurityAction a )
			{
				return a.Equals( actionDescriptor );
			} );
		}
	}
}
