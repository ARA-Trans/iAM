using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityManager;
using DatabaseManager;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace RoadCareSecurityOperations
{
	public enum AccessLevel
	{
		None,
		Default,
		ReadOnly,
		ReadWrite,
		Create,
		CreateDestroy
	}

	public class SecOps
	{
		private RoadCareUser m_currentUser = null;
		private SecurityManager.SecurityManager securityEngine = null;

		private bool readOnlyMode = false;
		private bool lockOutMode = false;

		public SecOps()
		{
			securityEngine = new SecurityManager.SecurityManager( "ACTION_ID", "USER_ID" );
		}

		public string CurrentUserName
		{
			get
			{
				return m_currentUser.Name;
			}
		}

		public RoadCareUser CurrentUser
		{
			get
			{
				return m_currentUser;
			}
		}

		public bool LockOut
		{
			get
			{
				return lockOutMode;
			}
			set
			{
				lockOutMode = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return readOnlyMode;
			}
			set
			{
				readOnlyMode = value;
			}
		}

		private AccessLevel GetPermissions( Dictionary<String, String> actionSpecifier )
		{
			AccessLevel grantedLevel = AccessLevel.Default;
			if( !securityEngine.ActionExists( actionSpecifier ) )
			{
				securityEngine.AddAction( actionSpecifier, m_currentUser.ToSecurityUser );
				AddActionToGroup( actionSpecifier, "ALL_ACTIONS" );
			}
			String accessDescriptor = securityEngine.GetPermissions( m_currentUser.Descriptor, actionSpecifier );
			if( lockOutMode )
			{
				grantedLevel = AccessLevel.None;
			}
			else if( readOnlyMode )
			{
				switch( accessDescriptor )
				{
					case "None":
						grantedLevel = AccessLevel.None;
						break;
					case "ReadOnly":
						grantedLevel = AccessLevel.ReadOnly;
						break;
					case "ReadWrite":
						grantedLevel = AccessLevel.ReadOnly;
						break;
					case "Create":
						grantedLevel = AccessLevel.ReadOnly;
						break;
					case "CreateDestroy":
						grantedLevel = AccessLevel.ReadOnly;
						break;
				}
			}
			else
			{
				switch( accessDescriptor )
				{
					case "None":
						grantedLevel = AccessLevel.None;
						break;
					case "ReadOnly":
						grantedLevel = AccessLevel.ReadOnly;
						break;
					case "ReadWrite":
						grantedLevel = AccessLevel.ReadWrite;
						break;
					case "Create":
						grantedLevel = AccessLevel.Create;
						break;
					case "CreateDestroy":
						grantedLevel = AccessLevel.CreateDestroy;
						break;
				}
			}

			return grantedLevel;
		}

		private bool AddActionToGroup( Dictionary<string, string> actionSpecifier, string group )
		{
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", group );
			return securityEngine.AddActionToGroup( actionSpecifier, actionGroupDescriptor, m_currentUser.ToSecurityUser );
		}

		public bool IsAuthenticated
		{
			get
			{
				bool authenticated = false;
				if( m_currentUser != null && m_currentUser.IsAuthenticated )
				{
					authenticated = true;
				}
				return authenticated;
			}
		}

		public List<RoadCareUser> AllUsers
		{
			get
			{
				List<RoadCareUser> everyRoadCareUser = new List<RoadCareUser>();
				foreach( SecurityUser user in securityEngine.AllUsers )
				{
					everyRoadCareUser.Add( new RoadCareUser( user ) );
				}
				everyRoadCareUser.Sort( delegate( RoadCareUser a, RoadCareUser b )
				{
					return a.Name.CompareTo(b.Name);
				} );
				return everyRoadCareUser;
			}
		}
		public List<RoadCareUserGroup> AllUserGroups
		{
			get
			{
				List<RoadCareUserGroup> everyUserGroup = new List<RoadCareUserGroup>();
				foreach( SecurityUserGroup group in securityEngine.AllUserGroups )
				{
					everyUserGroup.Add( new RoadCareUserGroup( group ) );
				}
				everyUserGroup.Sort( delegate( RoadCareUserGroup a, RoadCareUserGroup b )
				{
					return a.Name.CompareTo( b.Name );
				} );

				return everyUserGroup;
			}
		}
		public List<RoadCareAction> AllActions
		{
			get
			{
				List<RoadCareAction> everyRoadCareAction = new List<RoadCareAction>();
				foreach( SecurityAction action in securityEngine.AllActions )
				{
					everyRoadCareAction.Add( new RoadCareAction( action ) );
				}
				everyRoadCareAction.Sort( delegate( RoadCareAction a, RoadCareAction b )
				{
					int compare = a.Descriptor["ACTION_TYPE"].CompareTo( b.Descriptor["ACTION_TYPE"] );
					if( compare == 0 )
					{
						compare = a.Descriptor["DESCRIPTION"].CompareTo( b.Descriptor["DESCRIPTION"] );
						if( compare == 0 )
						{
							if( a.Descriptor["NETWORK_"] != "" && b.Descriptor["NETWORK_"] != "" )
							{
								compare = int.Parse( a.Descriptor["NETWORK_"] ).CompareTo( int.Parse( b.Descriptor["NETWORK_"] ) );
								if( compare == 0 )
								{
									if( a.Descriptor["SIMULATION"] != "" && b.Descriptor["SIMULATION"] != "" )
									{
										compare = int.Parse( a.Descriptor["SIMULATION"] ).CompareTo( int.Parse( b.Descriptor["SIMULATION"] ) );
									}
								}
							}
						}
					}
					return compare;
				} );

				return everyRoadCareAction;
			}
		}
		public List<RoadCareActionGroup> AllActionGroups
		{
			get
			{
				List<RoadCareActionGroup> everyActionGroup = new List<RoadCareActionGroup>();
				foreach( SecurityActionGroup group in securityEngine.AllActionGroups )
				{
					everyActionGroup.Add( new RoadCareActionGroup( group ) );
				}
				everyActionGroup.Sort( delegate( RoadCareActionGroup a, RoadCareActionGroup b )
				{
					return a.Name.CompareTo( b.Name );
				} );

				return everyActionGroup;
			}
		}

		#region Users
		public void SetCurrentUser( String userName, String password )
		{
			m_currentUser = new RoadCareUser( userName, password );
			m_currentUser.IsAuthenticated = false;

			SecurityUser authenticatedUser = AuthenticateUser( m_currentUser );

			if( authenticatedUser != null )
			{
				m_currentUser = new RoadCareUser( authenticatedUser );
				m_currentUser.IsAuthenticated = true;
			}
		}

		private SecurityUser AuthenticateUser( RoadCareUser userToAuthenticate )
		{
			Dictionary<String, String> authenticationInformation = new Dictionary<string, string>();
			authenticationInformation["USER_LOGIN"] = userToAuthenticate.Name;
			authenticationInformation["PASSWORD_"] = userToAuthenticate.UnencryptedPassword;

			//userToAuthenticate.IsAuthenticated = securityEngine.AuthenticateUser( authenticationInformation, "PASSWORD", "8!Resplendent40320Romans" );
			SecurityUser authenticatedUser = securityEngine.AuthenticateUser( authenticationInformation, "PASSWORD_" );

			return authenticatedUser;
		}
		public bool AddUser( string userName, string userPassword, string emailAddress, string securityQuestion, string securityAnswer )
		{
			Dictionary<string, string> userDescriptor = new Dictionary<string, string>();
			userDescriptor.Add( "USER_LOGIN", userName );
			//Can't do this because the default insert generation throws single quotes around the encrypt statement
			//userDescriptor.Add( "PASSWORD", "EncryptByPassPhrase('8!Resplendent40320Romans', '" + userPassword + "')");
			if( emailAddress != "" )
			{
				userDescriptor.Add( "EMAIL", emailAddress );
				userDescriptor.Add( "LOWERED_EMAIL", emailAddress.ToLower() );
			}
			if( securityQuestion != "" )
			{
				userDescriptor.Add( "PASSWORD_QUESTION", securityQuestion );
				if( securityAnswer != "" )
				{
					userDescriptor.Add( "PASSWORD_ANSWER", securityAnswer );
				}
			}

			userDescriptor.Add( "IS_APPROVED", "1" );

			//return securityEngine.AddUser( userDescriptor, "PASSWORD_", userPassword ) != null;
			return securityEngine.AddUser( userDescriptor, "PASSWORD_", userPassword );
		}
		public List<string> GetUserGroups( RoadCareUser user )
		{
			return securityEngine.GetUserGroups( user.Descriptor );
		}

		public void UpdateUserInformation( RoadCareUser user, string userName, string email, string question, string answer )
		{
			Dictionary<string, string> newInformation = new Dictionary<string, string>();
			newInformation.Add( "USER_LOGIN", userName );
			newInformation.Add( "EMAIL", email );
			newInformation.Add( "LOWERED_EMAIL", email.ToLower() );
			newInformation.Add( "PASSWORD_QUESTION", question );
			newInformation.Add( "PASSWORD_ANSWER", answer );

			if( securityEngine.UpdateUserInformation( user.Descriptor, newInformation ) )
			{
				user.Descriptor["USER_LOGIN"] = userName;
				user.Descriptor["EMAIL"] = email;
				user.Descriptor["LOWERED_EMAIL"] = email.ToLower();
				user.Descriptor["PASSWORD_QUESTION"] = question;
				user.Descriptor["PASSWORD_ANSWER"] = answer;
			}
		}

		public void RemoveUser( RoadCareUser user )
		{
			securityEngine.RemoveUser( user.Descriptor );
		}

		#endregion

		#region UserGroups
		public List<RoadCareUser> GetUserMembers( string userGroupName )
		{
			List<RoadCareUser> roadCareMembers = new List<RoadCareUser>();
			Dictionary<string, string> groupDescriptor = new Dictionary<string, string>();
			groupDescriptor.Add( "USERGROUP_NAME", userGroupName );
			List<SecurityUser> securityMembers = securityEngine.GetUserMembers( groupDescriptor );

			foreach( SecurityUser member in securityMembers )
			{
				roadCareMembers.Add( new RoadCareUser( member ) );
			}

			return roadCareMembers;
		}

		public bool AddUserGroup( string newUserGroup )
		{
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", newUserGroup );

			return securityEngine.AddUserGroup( userGroupDescriptor );
		}
		public bool AddUserToGroup( RoadCareUser user, string group )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", group );
			bool added = false;
			if( m_currentUser != null )
			{
				added = securityEngine.AddUserToGroup( userDescriptor, userGroupDescriptor, m_currentUser.ToSecurityUser );
			}
			else
			{
				added = securityEngine.AddUserToGroup( userDescriptor, userGroupDescriptor, null );
			}
			return added;
		}
		public bool RemoveUserFromGroup( RoadCareUser user, string group )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", group );
			return securityEngine.RemoveUserFromGroup( userDescriptor, userGroupDescriptor, m_currentUser.ToSecurityUser );
		}

		public void UpdateUserGroupInformation( string groupName, string newGroupName )
		{
			Dictionary<string, string> groupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> newInformation = new Dictionary<string, string>();
			groupDescriptor.Add( "USERGROUP_NAME", groupName );
			newInformation.Add( "USERGROUP_NAME", newGroupName );
			securityEngine.UpdateUserGroupInformation( groupDescriptor, newInformation );
		}

		public void RemoveUserGroup( string userGroupName )
		{
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", userGroupName );
			securityEngine.RemoveUserGroup( userGroupDescriptor, m_currentUser.ToSecurityUser );
		}


		#endregion

		#region Actions
		public bool AddAction( string type, string description, string networkID, string simulationID )
		{
			Dictionary<string, string> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", type );
			if( description != "" )
			{
				actionDescriptor.Add( "DESCRIPTION", description );
			}
			if( networkID != "" )
			{
				//dsmelser
				//keyword removal
				//actionDescriptor.Add( "NETWORK", networkID );
				actionDescriptor.Add( "NETWORK_", networkID );
			}
			if( simulationID != "" )
			{
				actionDescriptor.Add( "SIMULATION", simulationID );
			}

			return securityEngine.AddAction( actionDescriptor, m_currentUser.ToSecurityUser ) != "";
		}

		public List<string> GetActionGroups( RoadCareAction action )
		{
			return securityEngine.GetActionGroups( action.Descriptor );
		}

		public void UpdateActionInformation( RoadCareAction action, string actionType, string description, string network, string simulation )
		{
			Dictionary<string, string> newInformation = new Dictionary<string, string>();
			newInformation.Add( "ACTION_TYPE", actionType );
			newInformation.Add( "DESCRIPTION", description );
			//dsmelser
			//keyword removal
			//newInformation.Add( "NETWORK", network );
			newInformation.Add( "NETWORK_", network );
			newInformation.Add( "SIMULATION", simulation );

			if( securityEngine.UpdateActionInformation( action.Descriptor, newInformation ) )
			{
				action.Descriptor["ACTION_TYPE"] = actionType;
				action.Descriptor["DESCRIPTION"] = description;
				//dsmelser
				//keyword removal
				action.Descriptor["NETWORK_"] = network;
				action.Descriptor["SIMULATION"] = simulation;
			}
		}

		public void RemoveAction( RoadCareAction action )
		{
			securityEngine.RemoveAction( action.Descriptor, m_currentUser.ToSecurityUser );
		}

		#endregion

		#region ActionGroups
		public bool AddActionGroup( string newActionGroupName )
		{
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", newActionGroupName );
			
			return securityEngine.AddActionGroup( actionGroupDescriptor );
		}

		public List<RoadCareAction> GetActionMembers( string actionGroupName )
		{
			List<RoadCareAction> roadCareMembers = new List<RoadCareAction>();
			Dictionary<string, string> groupDescriptor = new Dictionary<string, string>();
			groupDescriptor.Add( "ACTIONGROUP_NAME", actionGroupName );
			List<SecurityAction> securityMembers = securityEngine.GetActionMembers( groupDescriptor );

			foreach( SecurityAction member in securityMembers )
			{
				roadCareMembers.Add( new RoadCareAction( member ) );
			}

			return roadCareMembers;
		}

		public bool AddActionToGroup( RoadCareAction action, string group )
		{
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", group );
			return securityEngine.AddActionToGroup( actionDescriptor, actionGroupDescriptor, m_currentUser.ToSecurityUser );
		}

		public bool RemoveActionFromGroup( RoadCareAction action, string group )
		{
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", group );
			return securityEngine.RemoveActionFromGroup( actionDescriptor, actionGroupDescriptor, m_currentUser.ToSecurityUser );
		}

		public void UpdateActionGroupInformation( string groupName, string newGroupName )
		{
			Dictionary<string, string> groupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> newInformation = new Dictionary<string, string>();
			groupDescriptor.Add( "ACTIONGROUP_NAME", groupName );
			newInformation.Add( "ACTIONGROUP_NAME", newGroupName );
			securityEngine.UpdateActionGroupInformation( groupDescriptor, newInformation );
		}

		public void RemoveActionGroup( string actionGroupName )
		{
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", actionGroupName );
			securityEngine.RemoveActionGroup( actionGroupDescriptor, m_currentUser.ToSecurityUser );
		}

		public RoadCareAction GetAction( string type, string description, string network, string simulation )
		{
			RoadCareAction matchAction = null;
			if( network != "" )
			{
				if( simulation != "" )
				{
					foreach( RoadCareAction secAction in AllActions )
					{
						if( secAction.Descriptor["ACTION_TYPE"] == type )
						{
							if( secAction.Descriptor["DESCRIPTION"] == description )
							{
								if( secAction.Descriptor["NETWORK_"] == network )
								{
									if( secAction.Descriptor["SIMULATION"] == simulation )
									{
										if( matchAction == null )
										{
											matchAction = secAction;
										}
										else
										{
											throw new ArgumentException( "Ambiguous action specification." );
										}
									}
								}
							}
						}
					}
				}
				else
				{
					foreach( RoadCareAction secAction in AllActions )
					{
						if( secAction.Descriptor["ACTION_TYPE"] == type )
						{
							if( secAction.Descriptor["DESCRIPTION"] == description )
							{
								if( secAction.Descriptor["NETWORK_"] == network )
								{
									if( matchAction == null )
									{
										matchAction = secAction;
									}
									else
									{
										throw new ArgumentException( "Ambiguous action specification." );
									}
								}
							}
						}
					}
				}
			}
			else
			{
				foreach( RoadCareAction secAction in AllActions )
				{
					if( secAction.Descriptor["ACTION_TYPE"] == type )
					{
						if( secAction.Descriptor["DESCRIPTION"] == description )
						{
							if( matchAction == null )
							{
								matchAction = secAction;
							}
							else
							{
								throw new ArgumentException( "Ambiguous action specification." );
							}
						}
					}
				}
			}

			return matchAction;
		}

		#endregion

		#region Permissions

		public void SetUserGroupActionGroupPermissions( string userGroup, string actionGroup, string permissionLevel )
		{
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", userGroup );
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", actionGroup );
			if( permissionLevel != "Default" )
			{
				securityEngine.SetUserGroupActionGroupPermissions( userGroupDescriptor, actionGroupDescriptor, permissionLevel, m_currentUser.ToSecurityUser );
			}
			else
			{
				securityEngine.RemoveUserGroupActionGroupPermissions( userGroupDescriptor, actionGroupDescriptor, m_currentUser.ToSecurityUser );
			}
		}

		public void SetUserGroupActionPermissions( string userGroup, RoadCareAction action, string permissionLevel )
		{
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			userGroupDescriptor.Add( "USERGROUP_NAME", userGroup );
			if( permissionLevel != "Default" )
			{
				securityEngine.SetUserGroupActionPermissions( userGroupDescriptor, actionDescriptor, permissionLevel, m_currentUser.ToSecurityUser );
			}
			else
			{
				securityEngine.RemoveUserGroupActionPermissions( userGroupDescriptor, actionDescriptor, m_currentUser.ToSecurityUser );
			}
		}

		public void SetUserActionGroupPermissions( RoadCareUser user, string actionGroup, string permissionLevel )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", actionGroup );
			if( permissionLevel != "Default" )
			{
				securityEngine.SetUserActionGroupPermissions( userDescriptor, actionGroupDescriptor, permissionLevel, m_currentUser.ToSecurityUser );
			}
			else
			{
				securityEngine.RemoveUserActionGroupPermissions( userDescriptor, actionGroupDescriptor, m_currentUser.ToSecurityUser );
			}
		}

		public void SetUserActionPermissions( RoadCareUser user, RoadCareAction action, string permissionLevel )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			if( permissionLevel != "Default" )
			{
				securityEngine.SetUserActionPermissions( userDescriptor, actionDescriptor, permissionLevel, m_currentUser.ToSecurityUser );
			}
			else
			{
				securityEngine.RemoveUserActionPermissions( userDescriptor, actionDescriptor, m_currentUser.ToSecurityUser );
			}
		}

		public string GetUserGroupActionGroupPermissions( string userGroup, string actionGroup )
		{

			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			userGroupDescriptor.Add( "USERGROUP_NAME", userGroup );
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", actionGroup );
			string permissions = securityEngine.GetUserGroupActionGroupPermission( userGroupDescriptor, actionGroupDescriptor );
			if( permissions == "" )
			{
				permissions = "Default";
			}
			return permissions;
		}

		public string GetUserGroupActionPermissions( string userGroup, RoadCareAction action )
		{
			Dictionary<string, string> userGroupDescriptor = new Dictionary<string, string>();
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			userGroupDescriptor.Add( "USERGROUP_NAME", userGroup );
			string permissions = securityEngine.GetUserGroupActionPermission( userGroupDescriptor, actionDescriptor );
			if( permissions == "" )
			{
				permissions = "Default";
			}
			return permissions;
		}

		public string GetUserActionGroupPermissions( RoadCareUser user, string actionGroup )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> actionGroupDescriptor = new Dictionary<string, string>();
			actionGroupDescriptor.Add( "ACTIONGROUP_NAME", actionGroup );
			string permissions = securityEngine.GetUserActionGroupPermission( userDescriptor, actionGroupDescriptor );
			if( permissions == "" )
			{
				permissions = "Default";
			}
			return permissions;
		}

		public string GetUserActionPermissions( RoadCareUser user, RoadCareAction action )
		{
			Dictionary<string, string> userDescriptor = user.Descriptor;
			Dictionary<string, string> actionDescriptor = action.Descriptor;
			string permissions = securityEngine.GetUserActionPermission( userDescriptor, actionDescriptor );
			if( permissions == "" )
			{
				permissions = "Default";
			}
			return permissions;
		}
		#endregion

		public bool CanViewAsset( string assetName )
		{
			return GetPermissions( GenerateRawAssetActionDescriptor( assetName ) ) >= AccessLevel.ReadOnly;
		}

		private Dictionary<string, string> GenerateRawAssetActionDescriptor( string assetName )
		{
			Dictionary<string, string> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ASSET" );
			actionDescriptor.Add( "DESCRIPTION", assetName );

			return actionDescriptor;
		}

		public bool CanViewDynamicSegmentation( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "DYNAMIC_SEGMENTATION" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewConstructionHistoryView( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "VIEW" );
			actionDescriptor.Add( "DESCRIPTION", "CONSTRUCTION_HISTORY" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewAttributeView( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "VIEW" );
			actionDescriptor.Add( "DESCRIPTION", "ATTRIBUTE" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewSectionView( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "VIEW" );
			actionDescriptor.Add( "DESCRIPTION", "SECTION" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewAssetView( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "VIEW" );
			actionDescriptor.Add( "DESCRIPTION", "ASSET" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewGISView( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "VIEW" );
			actionDescriptor.Add( "DESCRIPTION", "GIS" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewAttributeViewAttribute( string networkID, string attributeViewAttribute )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ATTRIBUTE_VIEW" );
			actionDescriptor.Add( "DESCRIPTION", attributeViewAttribute );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}


		#region Network Definition
		public bool CanViewNetworkDefinitionData()
		{
			return GetPermissions( GenerateNetworkDefinitionActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifyNetworkDefinitionData()
		{
			return GetPermissions( GenerateNetworkDefinitionActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateNetworkDefinitionData()
		{
			return GetPermissions( GenerateNetworkDefinitionActionDescriptor() ) >= AccessLevel.Create;
		}

		public bool CanDeleteNetworkDefintionData()
		{
			return GetPermissions( GenerateNetworkDefinitionActionDescriptor() ) >= AccessLevel.CreateDestroy;
		}
		private Dictionary<string, string> GenerateNetworkDefinitionActionDescriptor()
		{
			Dictionary<string, string> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "NETWORK_DEFINITION" );

			return actionDescriptor;
		}
		#endregion

		#region Raw Attributes
		public bool CanViewRawAttributeData( string attributeName )
		{
			return GetPermissions( GenerateRawAttributeActionDescriptor( attributeName ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifyRawAttributeData( string attributeName )
		{
			Dictionary<String, String> generalActionDescriptor = new Dictionary<string, string>();
			generalActionDescriptor.Add( "ACTION_TYPE", "RAW_ATTRIBUTES" );

			bool generalPermission = GetPermissions( generalActionDescriptor ) >= AccessLevel.ReadWrite;
			return generalPermission && GetPermissions( GenerateRawAttributeActionDescriptor( attributeName ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateRawAttributeData( string attributeName )
		{
			return GetPermissions( GenerateRawAttributeActionDescriptor( attributeName ) ) >= AccessLevel.Create;
		}

		public bool CanDeleteRawAttributeData( string attributeName )
		{
			return GetPermissions( GenerateRawAttributeActionDescriptor( attributeName ) ) >= AccessLevel.CreateDestroy;
		}
		private Dictionary<string, string> GenerateRawAttributeActionDescriptor( string attributeName )
		{
			Dictionary<string, string> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ATTRIBUTE" );
			actionDescriptor.Add( "DESCRIPTION", attributeName );

			return actionDescriptor;
		}
		#endregion

		#region Segmentation Logic
		public bool CanViewSegmentationLogic( string networkID )
		{
			return GetPermissions( GenerateSegmentationLogicActionDescriptor( networkID ) ) >= AccessLevel.ReadOnly;
		}
		public bool CanModifySegmentationLogic( string networkID )
		{
			return GetPermissions( GenerateSegmentationLogicActionDescriptor( networkID ) ) >= AccessLevel.ReadWrite;
		}
		private Dictionary<string, string> GenerateSegmentationLogicActionDescriptor( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SEGMENTATION" );
			actionDescriptor.Add( "DESCRIPTION", "LOGIC" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return actionDescriptor;
		}
		#endregion

		#region Segmentation Results
		public bool CanViewSegmentationResults( string networkID )
		{
			return GetPermissions( GenerateSegmentationResultsActionDescriptor( networkID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySegmentationResults( string networkID )
		{
			return GetPermissions( GenerateSegmentationResultsActionDescriptor( networkID ) ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSegmentationResultsActionDescriptor( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SEGMENTATION" );
			actionDescriptor.Add( "DESCRIPTION", "RESULTS" );
			actionDescriptor.Add( "NETWORK_", networkID );

			return actionDescriptor;
		}
		#endregion

		public bool CanBatchModifySegmentationResults( string networkID )
		{
			return GetPermissions( GenerateSegmentationResultsBatchActionDescriptor( networkID ) ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSegmentationResultsBatchActionDescriptor( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SEGMENTATION RESULTS" );
			actionDescriptor.Add( "DESCRIPTION", "BATCH OPERATIONS" );
			actionDescriptor.Add( "NETWORK_", networkID );

			return actionDescriptor;
		}


		#region Rollup
		public bool CanViewRollup( string networkID )
		{
			return GetPermissions( GenerateRollupActionDescriptor( networkID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifyRollup( string networkID )
		{
			return GetPermissions( GenerateRollupActionDescriptor( networkID ) ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateRollupActionDescriptor( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SEGMENTATION" );
			actionDescriptor.Add( "DESCRIPTION", "ROLLUP" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return actionDescriptor;
		}
		#endregion

		#region Simulation Results/Committed
		public bool CanViewSimulationCommitted( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationCommittedActionDescriptor(networkID,simulationID) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationCommitted( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationCommittedActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}
	
		public bool CanCreateSimulationCommitted( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationCommittedActionDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		public bool CanRemoveSimulationCommitted( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationCommittedActionDescriptor( networkID, simulationID ) ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<String, String> GenerateSimulationCommittedActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "COMMITTED_" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}


		public bool CanViewSimulationResults( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationResultsActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationResults( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationResultsActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSimulationResultsActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "RESULTS" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
		#endregion

		#region Simulation Performance
		public bool CanViewSimulationPerformance( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationPerformanceActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationPerformance( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationPerformanceActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationPerformance( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationPerformanceActionDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		private Dictionary<string, string> GenerateSimulationPerformanceActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "PERFORMANCE" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
		#endregion

		#region Simulation Treatment
		public bool CanViewSimulationTreatment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationTreatmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationTreatment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationTreatmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationTreatment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationTreatmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		private Dictionary<string, string> GenerateSimulationTreatmentActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "TREATMENT" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
		#endregion

		public bool CanModifySimulationAttributesData( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAttributesDataActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSimulationAttributesDataActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "ATTRIBUTES_" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}

		public bool CanModifySecurityUsers()
		{
			return GetPermissions( GenerateSecurityUsersActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSecurityUsers()
		{
			return GetPermissions( GenerateSecurityUsersActionDescriptor() ) >= AccessLevel.Create;
		}

		public bool CanRemoveSecurityUsers()
		{
			return GetPermissions( GenerateSecurityUsersActionDescriptor() ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<string, string> GenerateSecurityUsersActionDescriptor()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "USERS" );

			return actionDescriptor;
		}

		public bool CanModifySecurityUserGroups()
		{
			return GetPermissions( GenerateSecurityUserGroupsActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSecurityUserGroups()
		{
			return GetPermissions( GenerateSecurityUserGroupsActionDescriptor() ) >= AccessLevel.Create;
		}

		public bool CanRemoveSecurityUserGroups()
		{
			return GetPermissions( GenerateSecurityUserGroupsActionDescriptor() ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<string, string> GenerateSecurityUserGroupsActionDescriptor()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "USERGROUPS" );

			return actionDescriptor;
		}

		public bool CanModifySecurityActions()
		{
			return GetPermissions( GenerateSecurityActionsActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSecurityActions()
		{
			return GetPermissions( GenerateSecurityActionsActionDescriptor() ) >= AccessLevel.Create;
		}

		public bool CanRemoveSecurityActions()
		{
			return GetPermissions( GenerateSecurityActionsActionDescriptor() ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<string, string> GenerateSecurityActionsActionDescriptor()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "ACTIONS" );

			return actionDescriptor;
		}

		public bool CanModifySecurityActionGroups()
		{
			return GetPermissions( GenerateSecurityActionGroupsActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSecurityActionGroups()
		{
			return GetPermissions( GenerateSecurityActionGroupsActionDescriptor() ) >= AccessLevel.Create;
		}

		public bool CanRemoveSecurityActionGroups()
		{
			return GetPermissions( GenerateSecurityActionGroupsActionDescriptor() ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<string, string> GenerateSecurityActionGroupsActionDescriptor()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "ACTIONGROUPS" );

			return actionDescriptor;
		}



		public bool CanModifySecurityPermissions()
		{
			return GetPermissions( GenerateSecurityPermissionsActionDescriptor() ) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSecurityPermissionsActionDescriptor()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "PERMISSIONS" );

			return actionDescriptor;
		}

		public bool CanViewUserData()
		{
			return GetPermissions( GenerateSecurityUsersActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewUserGroupData()
		{
			return GetPermissions( GenerateSecurityUserGroupsActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewActionData()
		{
			return GetPermissions( GenerateSecurityActionsActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewActionGroupData()
		{
			return GetPermissions( GenerateSecurityActionGroupsActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewPermissionsData()
		{
			return GetPermissions( GenerateSecurityPermissionsActionDescriptor() ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateRawAttribute()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "RAW_ATTRIBUTES" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public bool CanCreateRawAsset()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "RAW_ASSETS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public bool CanCreateNetworks()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "NETWORKS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public bool CanViewNetwork( string networkID )
		{
			return GetPermissions( GenerateNetworkActionDescriptor( networkID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanDeleteNetwork( string networkID )
		{
			return GetPermissions( GenerateNetworkActionDescriptor( networkID ) ) >= AccessLevel.CreateDestroy;
		}

		public bool CanCreateSubNetworks( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SUB_NETWORK" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public Dictionary<string, string> GenerateNetworkActionDescriptor( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "NETWORK" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return actionDescriptor;
		}

		public bool CanCreateAttributeViewReport( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ATTRIBUTE_VIEW_REPORT" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			//reports don't modify data, so this can be ReadOnly.
			//return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanRollupAssets( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ROLLUP" );
			actionDescriptor.Add( "DESCRIPTION", "ASSET" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.CreateDestroy;
		}

		public bool CanCreateSimulation( string networkID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATIONS" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public bool CanRunSimulation( string network, string simulation )
		{
			Dictionary<String, String> actionDescriptor = GenerateSimulationActionDescriptor( network, simulation );
			return GetPermissions( actionDescriptor ) >= AccessLevel.CreateDestroy;
		}

		public bool CanDeleteSimulation( string network, string simulation )
		{
			Dictionary<String, String> actionDescriptor = GenerateSimulationActionDescriptor( network, simulation );
			return GetPermissions( actionDescriptor ) >= AccessLevel.CreateDestroy;
		}

		private Dictionary<string, string> GenerateSimulationActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}

		#region Reports

		public bool CanCreateInputSummaryReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			//return GetPermissions( GenerateReportActionDescriptor( network, simulation, "InputSummary" ) ) >= AccessLevel.Create;
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "InputSummary" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateTotalBudgetReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			//return GetPermissions( GenerateReportActionDescriptor( network, simulation, "TotalBudget" ) ) >= AccessLevel.Create;
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "TotalBudget" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateBudgetPerDistrictReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "BudgetPerDistrict" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateBudgetPerTreatmentReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "BudgetPerTreatment" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateLaneMilesPerTreatmentReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "LaneMilesPerTreatment" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateLaneMilesPerDistrictReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "LaneMilesPerDistrict" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateBudgetPerConditionReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "BudgetPerCondition" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateLaneMilesPerConditionReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "LaneMilesPerCondition" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateTotalLaneMilesPerConditionReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "TotalLaneMilesPerCondition" ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateDetailedResultsReport( string network, string simulation )
		{
			//reports don't modify data, so this can be ReadOnly.
			return GetPermissions( GenerateReportActionDescriptor( network, simulation, "DetailedResults" ) ) >= AccessLevel.ReadOnly;
		}

		private Dictionary<string, string> GenerateReportActionDescriptor( string network, string simulation, string reportType )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "REPORT" );
			actionDescriptor.Add( "DESCRIPTION", reportType );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", network );
			actionDescriptor.Add( "NETWORK_", network );
			actionDescriptor.Add( "SIMULATION", simulation );

			return actionDescriptor;

		}

		#endregion

		#region Simulation Analysis
		public bool CanViewSimulationAnalysis( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationAnalysisData( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationAnalysis( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisActionDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		public bool CanModifySimulationAnalysisJurisdiction(string networkID, string simulationID)
		{
			return GetPermissions(GenerateSimulationAnalysisJurisdictionActionDescriptor(networkID, simulationID)) >= AccessLevel.ReadWrite;
		}

		private Dictionary<string, string> GenerateSimulationAnalysisJurisdictionActionDescriptor(string networkID, string simulationID)
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add("ACTION_TYPE", "SIMULATION ANALYSIS");
			actionDescriptor.Add("DESCRIPTION", "JURISDICTION");
			actionDescriptor.Add("NETWORK_", networkID);
			actionDescriptor.Add("SIMULATION", simulationID);

			return actionDescriptor;
		}


		private Dictionary<string, string> GenerateSimulationAnalysisActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "ANALYSIS" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
		#endregion


		#region Simulation Investment
		public bool CanViewSimulationInvestment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationInvestmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationInvestment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationInvestmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationInvestment( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationInvestmentActionDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		private Dictionary<string, string> GenerateSimulationInvestmentActionDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION" );
			actionDescriptor.Add( "DESCRIPTION", "INVESTMENT" );
			//dsmelser
			//keyword removal
			//actionDescriptor.Add( "NETWORK", networkID );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
		#endregion


		public bool CanViewCalculatedAssets()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CALCULATED_ASSETS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewPCI()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "PCI" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewAssetToAttribute()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ASSET_TO_ATTRIBUTE" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewCalculatedAttribute( string attribute )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CALCULATED_ATTRIBUTE" );
			actionDescriptor.Add( "DESCRIPTION", attribute );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateNetworkReport( string reportName )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "NETWORK_REPORT" );
			actionDescriptor.Add( "DESCRIPTION", reportName );

			//reports don't modify data, so this can be ReadOnly.
			//return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateSimulationReport( string reportName )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SIMULATION_REPORT" );
			actionDescriptor.Add( "DESCRIPTION", reportName );

			//reports don't modify data, so this can be ReadOnly.
			//return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewSimulationAnalysisPriority( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisPriorityDescriptor(networkID, simulationID) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationAnalysisPriority( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisPriorityDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationAnalysisPriority( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisPriorityDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		public bool CanRemoveSimulationAnalysisPriority( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisPriorityDescriptor( networkID, simulationID ) ) >= AccessLevel.CreateDestroy;
		}

		public Dictionary<string, string> GenerateSimulationAnalysisPriorityDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ANALYSIS" );
			actionDescriptor.Add( "DESCRIPTION", "PRIORITY" );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}
			


		public bool CanViewSimulationAnalysisTargets( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisTargetsDescriptor(networkID, simulationID)) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationAnalysisTargets( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisTargetsDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationAnalysisTargets( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisTargetsDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		public bool CanRemoveSimulationAnalysisTargets( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisTargetsDescriptor( networkID, simulationID ) ) >= AccessLevel.CreateDestroy;
		}

		public Dictionary<string, string> GenerateSimulationAnalysisTargetsDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ANALYSIS" );
			actionDescriptor.Add( "DESCRIPTION", "TARGETS" );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}

		public bool CanViewSimulationAnalysisDeficients( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisDeficientsDescriptor(networkID, simulationID) ) >= AccessLevel.ReadOnly;
		}

		public bool CanModifySimulationAnalysisDeficients( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisDeficientsDescriptor( networkID, simulationID ) ) >= AccessLevel.ReadWrite;
		}

		public bool CanCreateSimulationAnalysisDeficients( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisDeficientsDescriptor( networkID, simulationID ) ) >= AccessLevel.Create;
		}

		public bool CanRemoveSimulationAnalysisDeficients( string networkID, string simulationID )
		{
			return GetPermissions( GenerateSimulationAnalysisDeficientsDescriptor( networkID, simulationID ) ) >= AccessLevel.CreateDestroy;
		}

		public Dictionary<string, string> GenerateSimulationAnalysisDeficientsDescriptor( string networkID, string simulationID )
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "ANALYSIS" );
			actionDescriptor.Add( "DESCRIPTION", "DEFICIENTS" );
			actionDescriptor.Add( "NETWORK_", networkID );
			actionDescriptor.Add( "SIMULATION", simulationID );

			return actionDescriptor;
		}


		public bool CanViewConstructionHistory()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CONSTRUCTION" );
			actionDescriptor.Add( "DESCRIPTION", "HISTORY" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewConstructionMaterials()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CONSTRUCTION" );
			actionDescriptor.Add( "DESCRIPTION", "MATERIALS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}
	
		
		public void CheckFreshInstall()
		{
			//CheckTables();
			CheckForInitialUser();
			//CheckPrimaryKeyConstraints();
			//CheckForeignKeyConstraints();
			//CheckAttributeCapitalization();
            
		}

		private void CheckAttributeCapitalization()
		{
			List<string> attributes = GetAllAttributes();
			List<string> unCapitalAttributes = FilterOutCapitalized( attributes );
			if( unCapitalAttributes.Count > 0 )
			{
				CheckEverythingForAttributes( unCapitalAttributes );

		//private void loadBinariesToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//    BinaryLoader makeBinaries = new BinaryLoader();
		//    this.Cursor = Cursors.WaitCursor;
		//    makeBinaries.CreateDLLs();
		//    this.Cursor = Cursors.Arrow;
		//}
				
			}
		}

		private List<string> GetAllAttributes()
		{
			List<string> attributes = new List<string>();

			string attributeQuery = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_";
			DataSet attributeSet = DBMgr.ExecuteQuery( attributeQuery );

			if( attributeSet.Tables.Count > 0 )
			{
				foreach( DataRow attributeRow in attributeSet.Tables[0].Rows )
				{
					attributes.Add( attributeRow["ATTRIBUTE_"].ToString() );
				}
			}

			return attributes;
		}

		private List<string> FilterOutCapitalized( List<string> attributes )
		{
			List<string> unCapitalized = new List<string>();

			foreach( string attribute in attributes )
			{
				if( attribute != attribute.ToUpper() )
				{
					unCapitalized.Add( attribute );
				}
			}

			return unCapitalized;
		}

		private void CheckEverythingForAttributes( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			updateStatements.AddRange( CapitalizeAttributesTableAndAttributeTables( unCapitalAttributes ) );
			updateStatements.AddRange( CapitalizeAttributesCalculatedTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeBenefitCostTables( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeCommitConsequencesTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeConsequencesTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeCostsTable( unCapitalAttributes ));
			updateStatements.AddRange( CaptializeCriteriaSegmentTable( unCapitalAttributes ));
			updateStatements.AddRange( CaptializeDeficientsTable( unCapitalAttributes ));
			updateStatements.AddRange( CaptializeFeasibilityTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeOptionsTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizePerformanceTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizePrioritizedNeedTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizePriorityTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeReportTables( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeRollupControlTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeSecurityActionsTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeSegmentControlTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeSimulationsTableAndSimulationTables( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeTargetDeficientTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeTargetsTable( unCapitalAttributes ));
			updateStatements.AddRange( CapitalizeTreatmentConsequencesTable( unCapitalAttributes ));

			DBMgr.ExecuteBatchNonQuery( updateStatements );
		}

		private List<string> CapitalizeAttributesTableAndAttributeTables( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();
			//1. Fix Attribute tables
			foreach( string attribute in unCapitalAttributes )
			{
				if( DBMgr.IsTableInDatabase( attribute ) )
				{
					DBMgr.RenameTable( attribute, attribute.ToUpper() );

				}
			}

			//2. Fix the Attributes_ table
			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "ATTRIBUTES_", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeAttributesCalculatedTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "EQUATION" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "ATTRIBUTES_CALCULATED", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeBenefitCostTables( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> benefitCostTables = GetTablesContaining( "BENEFITCOST_" );
			foreach( string benefitCostTable in benefitCostTables )
			{
				updateStatements.AddRange( CapitalizeBenefitCostTable( benefitCostTable, unCapitalAttributes ) );
			}

			return updateStatements;
		}

		private List<string> CapitalizeBenefitCostTable( string benefitCostTable, List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "RLHASH" );
			columnsToCheck.Add( "CHANGEHASH" );

			updateStatements.AddRange( FixTableColumnCapitalization( benefitCostTable, columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
			
		}

		private List<string> CapitalizeCommitConsequencesTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "COMMIT_CONSEQUENCES", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeConsequencesTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "CONSEQUENCES", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeCostsTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "COST_" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "COSTS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CaptializeCriteriaSegmentTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "FAMILY_EXPRESSION" );

			updateStatements.AddRange( FixTableColumnCapitalization( "CRITERIA_SEGMENT", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CaptializeDeficientsTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "DEFICIENTS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CaptializeFeasibilityTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "FEASIBILITY", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeOptionsTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "OPTION_VALUE" );

			updateStatements.AddRange( FixTableColumnCapitalization( "OPTIONS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizePerformanceTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "CRITERIA" );
			columnsToCheck.Add( "EQUATION" );

			updateStatements.AddRange( FixTableColumnCapitalization( "PERFORMANCE", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizePrioritizedNeedTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "PRIORITIZEDNEED", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizePriorityTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "PRIORITY", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeReportTables( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();
			List<string> reportTables = GetTablesContaining( "REPORT_" );

			foreach( string reportTable in reportTables )
			{
				updateStatements.AddRange( CapitalizeReportTable( reportTable, unCapitalAttributes ) );
			}

			return updateStatements;
		}

		private List<string> CapitalizeReportTable( string reportTable, List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "RLHASH" );
			columnsToCheck.Add( "CHANGEHASH" );

			updateStatements.AddRange( FixTableColumnCapitalization( reportTable, columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeRollupControlTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "ROLLUP_CONTROL", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeSecurityActionsTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "DESCRIPTION" );

			updateStatements.AddRange( FixTableColumnCapitalization( "SEC_ACTIONS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeSegmentControlTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "SEGMENT_CONTROL", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeSimulationsTableAndSimulationTables( List<string> unCapitalAttributes )
		{
			//1. Capitalize the simulation tables
			List<string> simulationTables = GetTablesContaining( "SIMULATION_" );
			foreach( string simulationTable in simulationTables )
			{
				CapitalizeTableColumns( simulationTable );
			}

			//2. Capitalize the SIMULATIONS table
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "JURISDICTION" );
			columnsToCheck.Add( "WEIGHTING" );
			columnsToCheck.Add( "SIMULATION_VARIABLES" );

			updateStatements.AddRange( FixTableColumnCapitalization( "SIMULATIONS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private void CapitalizeTableColumns( string table )
		{
			if( DBMgr.IsTableInDatabase( table ) )
			{
				List<string> tableColumns = DBMgr.GetTableColumns( table );
				foreach( string tableColumn in tableColumns )
				{
					if( tableColumn != tableColumn.ToUpper() )
					{
						DBMgr.RenameColumn( table, tableColumn, tableColumn.ToUpper() );
					}
				}
			}
		}

		private List<string> CapitalizeTargetDeficientTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "TARGET_DEFICIENT", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeTargetsTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );
			columnsToCheck.Add( "CRITERIA" );

			updateStatements.AddRange( FixTableColumnCapitalization( "TARGETS", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> CapitalizeTreatmentConsequencesTable( List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			List<string> columnsToCheck = new List<string>();
			columnsToCheck.Add( "ATTRIBUTE_" );

			updateStatements.AddRange( FixTableColumnCapitalization( "TREATMENT_CONSEQUENCES", columnsToCheck, unCapitalAttributes ) );

			return updateStatements;
		}

		private List<string> FixTableColumnCapitalization( string tableName, List<string> columnsToCheck, List<string> unCapitalAttributes )
		{
			List<string> updateStatements = new List<string>();

			string query = BuildCapitalizationQuery( tableName, columnsToCheck );

			DataSet capitalizationData = DBMgr.ExecuteQuery( query );
			if( capitalizationData.Tables.Count > 0 )
			{
				foreach( DataRow capitalizationRow in capitalizationData.Tables[0].Rows )
				{
					string updateStatement = CreateCapitalizationUpdateStatement( capitalizationRow, tableName, columnsToCheck, unCapitalAttributes );
					if( !String.IsNullOrEmpty( updateStatement ) )
					{
						updateStatements.Add( updateStatement );
					}
				}
			}

			return updateStatements;			

		}

		private string BuildCapitalizationQuery( string tableName, List<string> columnsToCheck )
		{
			string selectClause = "SELECT DISTINCT ";
			string fromClause = " FROM " + tableName;

			foreach( string column in columnsToCheck )
			{
				selectClause += column + ", ";
			}
			selectClause = selectClause.Substring( 0, selectClause.Length - 2 );

			string query = selectClause + fromClause;

			return query;
		}

		private string CreateCapitalizationUpdateStatement( DataRow capitalizationRow, string tableName, List<string> columnsToCheck, List<string> unCapitalAttributes )
		{
			bool updateNeeded = false;

			string updateStatement = "";
			string updateClause = "UPDATE " + tableName;
			string setClause = " SET ";
			string whereClause = " WHERE ";

			foreach( string column in columnsToCheck )
			{
				bool columnUpdateNeeded = false;
				string checkAgainst = capitalizationRow[column].ToString();
				string finalString = checkAgainst;
				foreach( string attributeToCheck in unCapitalAttributes )
				{
					if( checkAgainst.Contains( attributeToCheck ) )
					{
						finalString = finalString.Replace( attributeToCheck, attributeToCheck.ToUpper() );
						columnUpdateNeeded = true;
						updateNeeded = true;
					}
				}
				if( columnUpdateNeeded )
				{
					setClause += column + "='" + finalString + "', ";
					whereClause += column + "='" + checkAgainst + "' AND ";
				}
			}

			if( updateNeeded )
			{
				setClause = setClause.Substring( 0, setClause.Length - 2 );
				whereClause = whereClause.Substring( 0, whereClause.Length - 5 );
				updateStatement = updateClause + setClause + whereClause;
			}

			return updateStatement;

			
		}



		private List<string> GetTablesContaining( string partialTableName )
		{
			List<string> filterTables = new List<string>();

			List<string> allTables = DBMgr.GetDatabaseTables();
			foreach( string table in allTables )
			{
				if( table.ToUpper().Contains( partialTableName ) )
				{
					filterTables.Add( table );
				}
			}

			return filterTables;
		}



		/// <summary>
		/// Create any missing RoadCare tables (and correct existing ones?)
		/// </summary>
		private void CheckTables()
		{
			//TODO: Add other checks
			CheckAttributesTable();
			CheckNetworkDefinitionTable();
			CheckAttributesCalculated();
			CheckDynamicSegmentationTable();
			CheckMultiUserTable();
            CheckPriorityTable();
			CheckOptionsTable();
			CheckPCIDistressTable();
			CheckMultilineGeometriesDefTable();
			CheckAttributesTable();
			CheckConnectionParametersTable();
		}


		private void CheckConnectionParametersTable()
		{
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					if (!DBMgr.IsTableInDatabase("CONNECTION_PARAMETERS"))
					{
						DBMgr.ExecuteNonQuery("CREATE TABLE [CONNECTION_PARAMETERS]( [CONNECTION_NAME] [varchar](200) NOT NULL, [CONNECTION_ID] [int] NULL, [SERVER] [varchar](200) NULL, [PROVIDER] [varchar](200) NULL, [NATIVE_] [bit] NULL, [DATABASE_NAME] [varchar](200) NULL, [SERVICE_NAME] [varchar](200) NULL, [SID] [varchar](200) NULL, [PORT] [varchar](50) NULL, [USERID] [varchar](200) NULL, [PASSWORD] [varchar](200) NULL, [INTEGRATED_SEC] [bit] NULL, [VIEW_STATEMENT] [varchar] (max) NULL, [IDENTIFIER] [varchar](200) CONSTRAINT [PK_CONNECTION_NAME] PRIMARY KEY ( [CONNECTION_NAME] ASC ))");
					}
					break;
				case "ORACLE":
					//TODO: This should be checking for the connectionparameters table... DBMgr.ExecuteNonQuery("CREATE TABLE \"MULTIUSER_LOCK\"   ( \"LOCKID\" NUMBER NOT NULL ENABLE, \"NETWORKID\" NUMBER NOT NULL ENABLE,  \"SIMULATIONID\" NUMBER,  \"USERID\" VARCHAR2(256 BYTE) NOT NULL ENABLE,  \"LOCK_START\" TIMESTAMP (6) NOT NULL ENABLE  )");
					break;
				default:
					throw new NotImplementedException("TODO: Implement ANSI version of CheckMultiUserTable()");
					
			}
		}

		private void CheckNetworkDefinitionTable()
		{
			//TODO: Finish check
			
			//this is to try to fix sequence issues
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "ORACLE":
					try
					{
						int maxID = DBMgr.ExecuteScalar( "SELECT MAX(ID_) FROM NETWORK_DEFINITION" );
						for( int currID = DBMgr.ExecuteScalar( "SELECT NETWORK_DEFINITION_ID_SEQ.NEXTVAL FROM DUAL" );
							currID <= maxID;
							currID = DBMgr.ExecuteScalar( "SELECT NETWORK_DEFINITION_ID_SEQ.NEXTVAL FROM DUAL" ) )
						{
						}
					}
					catch
					{
						//if there's an error here, that's pretty bad but we don't want to crash the program
					}
					break;
			}

		}

		private void CheckAttributesTable()
		{
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "ORACLE":
					if (DBMgr.IsTableInDatabase("ATTRIBUTES_"))
					{
						List<string> attributesColumnNames = DBMgr.GetTableColumns("ATTRIBUTES_");
						if (!attributesColumnNames.Contains("SID_"))
						{
							DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_ ADD SID_ VARCHAR2(200 CHAR) NULL");
						}
						if (!attributesColumnNames.Contains("INTEGRATED_SECURITY"))
						{
							DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_ ADD INTEGRATED_SECURITY NUMBER NULL");
						}
						if (!attributesColumnNames.Contains("PORT"))
						{
							DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_ ADD PORT NUMBER NULL");
						}
						if (!attributesColumnNames.Contains("SERVICE_NAME"))
						{
							DBMgr.ExecuteNonQuery("ALTER TABLE ATTRIBUTES_ ADD SERVICE_NAME VARCHAR2(200 CHAR) NULL");
						}
					}
					else
					{
						throw new NotImplementedException("Implement functionality for automatically creating the ATTRIBUTES_ table.");
					}
					break;
				case "MSSQL":
					if (DBMgr.IsTableInDatabase("ATTRIBUTES_"))
					{
						List<string> attributesColumnNames = DBMgr.GetTableColumns("ATTRIBUTES_");
						Dictionary<string, string> columnsToAdd = new Dictionary<string, string>();
						if (!attributesColumnNames.Contains("SID_"))
						{
							columnsToAdd.Add("SID_", "varchar(200)");
						}
						if (!attributesColumnNames.Contains("INTEGRATED_SECURITY"))
						{
							columnsToAdd.Add("INTEGRATED_SECURITY", "bit");
						}
						if (!attributesColumnNames.Contains("PORT"))
						{
							columnsToAdd.Add("PORT", "int");
						}
						if (!attributesColumnNames.Contains("SERVICE_NAME"))
						{
							columnsToAdd.Add("SERVICE_NAME", "varchar(200)");
						}
						DBMgr.AddTableColumns("ATTRIBUTES_", columnsToAdd);
					}
					else
					{
						throw new NotImplementedException("Implement funtionality for automatically creating the ATTRIBUTES_ table. ");
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void CheckMultilineGeometriesDefTable()
		{
			// Oracle only allows 30 characters in its table names.  Guess how long this one is. (Its 32)
			// Dave says we should do something, with columns...Not sure what he is going on about.
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "ORACLE":
					if (DBMgr.IsTableInDatabase("MULTILINE_GEOMETRIES_DEFINITIO"))
					{
						DBMgr.RenameTable("MULTILINE_GEOMETRIES_DEFINITIO", "MULTILINE_GEOMETRIES_DEF");
					}
					if (!DBMgr.IsTableInDatabase("MULTILINE_GEOMETRIES_DEF"))
					{
						//DBMgr.ExecuteNonQuery(@"CREATE TABLE ""MULTILINE_GEOMETRIES_DEF""     (	""MULTILINE_GEOM_ID"" NUMBER(10,0) NOT NULL ENABLE,  	""FACILITY_ID"" NUMBER(10,0) NOT NULL ENABLE,  	""LINESTRING"" CLOB,  	""BEGIN_SEGMENT"" FLOAT(126),  	""END_SEGMENT"" FLOAT(126),  	""NOTES"" VARCHAR2(4000 CHAR),  	 CONSTRAINT ""PK_MULTILINE_GEOMETRIES_DEFINI"" PRIMARY KEY (""MULTILINE_GEOM_ID"")    )  CREATE OR REPLACE TRIGGER ""MULTILINE_GEOMETRIES_DEFINIT_2"" BEFORE INSERT OR UPDATE ON MULTILINE_GEOMETRIES_DEF FOR EACH ROW DECLARE  v_newVal NUMBER(12) := 0; v_incval NUMBER(12) := 0; BEGIN   IF INSERTING AND :new.MULTILINE_GEOM_ID IS NULL THEN     SELECT  MULTILINE_GEOMETRIES_DEFINIT_1.NEXTVAL INTO v_newVal FROM DUAL;     IF v_newVal = 1 THEN        SELECT max(MULTILINE_GEOM_ID) INTO v_newVal FROM MULTILINE_GEOMETRIES_DEFINITIO;       v_newVal := v_newVal + 1;       LOOP            EXIT WHEN v_incval>=v_newVal;            SELECT MULTILINE_GEOMETRIES_DEFINIT_1.nextval INTO v_incval FROM dual;       END LOOP;     END IF;    sqlserver_utilities.identity := v_newVal;     :new.MULTILINE_GEOM_ID := v_newVal;   END IF; END; / ALTER TRIGGER ""MULTILINE_GEOMETRIES_DEFINIT_2"" ENABLE;");
					}
					break;
				case "MSSQL":
					if (DBMgr.IsTableInDatabase("MULTILINE_GEOMETRIES_DEFINITION"))
					{
						DBMgr.RenameTable("MULTILINE_GEOMETRIES_DEFINITION", "MULTILINE_GEOMETRIES_DEF");
					}
					if (!DBMgr.IsTableInDatabase("MULTILINE_GEOMETRIES_DEF"))
					{
						DBMgr.ExecuteNonQuery(@"CREATE TABLE  [MULTILINE_GEOMETRIES_DEF]( 	[MULTILINE_GEOM_ID] [int] IDENTITY(1,1) NOT NULL, 	[FACILITY_ID] [int] NOT NULL, 	[LINESTRING] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL, 	[BEGIN_SEGMENT] [float] NULL, 	[END_SEGMENT] [float] NULL, 	[NOTES] [varchar](4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,  CONSTRAINT [PK_MULTILINE_GEOMETRIES_DEF] PRIMARY KEY CLUSTERED ( [MULTILINE_GEOM_ID] ASC)) ");
					}
					break;
				default:
					throw new NotImplementedException();
			}	
		}

		private void CheckPCIDistressTable()
		{
			if (!DBMgr.IsTableInDatabase("PCI_DISTRESS"))
			{
				//throw new NotImplementedException("TODO: implement new table creation for PCI_DISTRESS()");
			}
			else
			{
				List<string> pciDistressCols = DBMgr.GetTableColumns("PCI_DISTRESS");
				if (!pciDistressCols.Contains("METRIC_CONVERSION"))
				{
					string alterTable = "ALTER TABLE PCI_DISTRESS ADD METRIC_CONVERSION float";
					DBMgr.ExecuteNonQuery(alterTable);

					// Now add the PCI metric conversion values to the table?
				}

                if (!pciDistressCols.Contains("ATTRIBUTE_"))
                {
                    string alterTable = "ALTER TABLE PCI_DISTRESS ADD ATTRIBUTE_ VARCHAR(200) NULL";
                    DBMgr.ExecuteNonQuery(alterTable);

                }
			}
		}

		private void CheckOptionsTable()
		{
			if (!DBMgr.IsTableInDatabase("OPTIONS"))
			{
				throw new NotImplementedException("TODO: implement new table creation for PRIORITY()");
			}
			else
			{
				string query = "SELECT OPTION_NAME, OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'PCI_UNITS'";
				DataSet optionRows = DBMgr.ExecuteQuery(query);
				if (optionRows.Tables[0].Rows.Count != 1)
				{
					string insert = "INSERT INTO OPTIONS VALUES ('PCI_UNITS', 'US')";
					DBMgr.ExecuteNonQuery(insert);
				}
			}
		}

        private void CheckPriorityTable()
        {
            if (!DBMgr.IsTableInDatabase("PRIORITY"))
            {
                throw new NotImplementedException("TODO: implement new table creation for PRIORITY()");
            }
            List<String> listColumns = DBMgr.GetTableColumns("PRIORITY");
            if(!listColumns.Contains("YEARS"))
            {

                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        DBMgr.ExecuteNonQuery("ALTER TABLE PRIORITY ADD YEARS INT NULL");
                        break;
                    case "ORACLE":
                        DBMgr.ExecuteNonQuery("ALTER TABLE PRIORITY ADD (YEARS NUMBER NULL)");
                        break;
                    default:
                        throw new NotImplementedException("TODO: Implement ANSI DYNAMIC_SEGMENTATION_ID creation.");
                        
                }

            }
        }

		private void CheckAttributesCalculated()
		{
			if( !DBMgr.IsTableInDatabase( "ATTRIBUTES_CALCULATED" ))
			{
				throw new NotImplementedException( "TODO: implement ATTRIBUTES_CALCULATED table creation for CheckAttributesCalculated()" );
			}
			else
			{
				//todo: check table columns
				try
				{
					DBMgr.GetCurrentAutoIncrement("ATTRIBUTES_CALCULATED");
				}
				catch	//I think there's an error here with the sequence allowing ridiculously big numbers
				{
					DBMgr.ExecuteNonQuery( "DROP SEQUENCE ATTRIBUTES_CALCULATED_ID_SEQ" );
					DBMgr.ExecuteNonQuery( "CREATE SEQUENCE ATTRIBUTES_CALCULATED_ID_SEQ MINVALUE 1 MAXVALUE 2000000000 INCREMENT BY 1 START WITH 1 NOCACHE" );
					DBMgr.ExecuteNonQuery( "UPDATE ATTRIBUTES_CALCULATED SET ID_ = ATTRIBUTES_CALCULATED_ID_SEQ.NEXTVAL" );
				}
			}
		}

		private void CheckDynamicSegmentationTable()
		{
			if( !DBMgr.IsTableInDatabase( "DYNAMIC_SEGMENTATION" ) )
			{
				throw new NotImplementedException( "TODO: implement new table creation for CheckDynamicSegmentationTable()" );
			}
			else
			{
				bool networkid = false;
				bool routes = false;
				bool begin_station = false;
				bool end_station = false;
				bool direction = false;
				bool breakcause = false;
				bool dynamic_segmentation_id = false;

				DataSet columnSet;

				//TODO: develop/use classes to check column types, quantifiers, attributes (nullability, identity, primary/foreign key constraints, etc...)
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						columnSet = DBMgr.ExecuteQuery( "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DYNAMIC_SEGMENTATION'" );
						break;
					case "ORACLE":
						columnSet = DBMgr.ExecuteQuery( "SELECT COLUMN_NAME FROM USER_TAB_COLS WHERE TABLE_NAME = 'DYNAMIC_SEGMENTATION'" );
						break;
					default:
						throw new NotImplementedException( "TODO: implement ANSI version of CheckDynamicSegmentationTable()" );
				}

				foreach( DataRow columnRow in columnSet.Tables[0].Rows )
				{
					switch( columnRow["COLUMN_NAME"].ToString() )
					{
						case "NETWORKID":
							networkid = true;
							break;
						case "ROUTES":
							routes = true;
							break;
						case "BEGIN_STATION":
							begin_station = true;
							break;
						case "END_STATION":
							end_station = true;
							break;
						case "DIRECTION":
							direction = true;
							break;
						case "BREAKCAUSE":
							breakcause = true;
							break;
						case "DYNAMIC_SEGMENTATION_ID":
							dynamic_segmentation_id = true;
							break;
						default:
							//todo: maybe include handling of unexpected columns?
							break;
					}
				}

				if( !networkid )
				{
					throw new NotImplementedException( "TODO: Implement NETWORKID column creation for CheckDynamicSegmentationTable()" );
				}
				if( !routes )
				{
					throw new NotImplementedException( "TODO: Implement ROUTES column creation for CheckDynamicSegmentationTable()" );
				}
				if( !begin_station )
				{
					throw new NotImplementedException( "TODO: Implement BEGIN_STATION column creation for CheckDynamicSegmentationTable()" );
				}
				if( !end_station )
				{
					throw new NotImplementedException( "TODO: Implement END_STATION column creation for CheckDynamicSegmentationTable()" );
				}
				if( !direction )
				{
					throw new NotImplementedException( "TODO: Implement DIRECTION column creation for CheckDynamicSegmentationTable()" );
				}
				if( !breakcause )
				{
					throw new NotImplementedException( "TODO: Implement BREAKCAUSE column creation for CheckDynamicSegmentationTable()" );
				}
				if( !dynamic_segmentation_id )
				{
					DBMgr.ExecuteNonQuery( "DELETE FROM DYNAMIC_SEGMENTATION" );
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							DBMgr.ExecuteNonQuery( "ALTER TABLE DYNAMIC_SEGMENTATION ADD DYNAMIC_SEGMENTATION_ID INT NOT NULL IDENTITY" );
							DBMgr.ExecuteNonQuery( "ALTER TABLE DYNAMIC_SEGMENTATION ADD PRIMARY KEY (DYNAMIC_SEGMENTATION_ID)" );
							break;
						case "ORACLE":
							DBMgr.ExecuteNonQuery( "ALTER TABLE DYNAMIC_SEGMENTATION ADD (DYNAMIC_SEGMENTATION_ID NUMBER NOT NULL)" );
							DBMgr.ExecuteNonQuery( "ALTER TABLE DYNAMIC_SEGMENTATION ADD CONSTRAINT PKI_DYNAMIC_SEGMENTATION PRIMARY KEY (DYNAMIC_SEGMENTATION_ID)" );

							if( DBMgr.ExecuteScalar( "SELECT COUNT(*) FROM user_sequences WHERE sequence_name = 'DYNAMIC_SEGMENTATION_ID_SEQ'" ) == 0 )
							{
								DBMgr.ExecuteNonQuery( "CREATE SEQUENCE DYNAMIC_SEGMENTATION_ID_SEQ INCREMENT BY 1 START WITH 1 NOCACHE" );
							}

							DBMgr.ExecuteNonQuery( "create or replace TRIGGER DYNAMIC_SEGMENTATION_ID_TRG BEFORE INSERT OR UPDATE ON DYNAMIC_SEGMENTATION " +
								"FOR EACH ROW " +
								"DECLARE " +
								"v_newVal NUMBER(12) := 0; " +
								"v_incval NUMBER(12) := 0; " +
								"BEGIN " +
								"  IF INSERTING AND :new.DYNAMIC_SEGMENTATION_ID IS NULL THEN " +
								"    SELECT  DYNAMIC_SEGMENTATION_ID_SEQ.NEXTVAL INTO v_newVal FROM DUAL; " +
								"    IF v_newVal = 1 THEN  " +
								"      SELECT max(DYNAMIC_SEGMENTATION_ID) INTO v_newVal FROM DYNAMIC_SEGMENTATION; " +
								"      v_newVal := v_newVal + 1; " +
								"      LOOP " +
								"           EXIT WHEN v_incval>=v_newVal; " +
								"           SELECT DYNAMIC_SEGMENTATION_ID_SEQ.nextval INTO v_incval FROM dual; " +
								"      END LOOP; " +
								"    END IF; " +
								"   sqlserver_utilities.identity := v_newVal;  " +
								"   :new.DYNAMIC_SEGMENTATION_ID := v_newVal; " +
								"  END IF; " +
								"END;" );


							//break;
							DBMgr.ExecuteNonQuery( "ALTER TRIGGER DYNAMIC_SEGMENTATION_ID_TRG ENABLE" );
							break;
						default:
							throw new NotImplementedException( "TODO: Implement ANSI DYNAMIC_SEGMENTATION_ID creation." );
							
					}
				}
			}
		}

		private void CheckMultiUserTable()
		{
			if (!DBMgr.IsTableInDatabase("MULTIUSER_LOCK"))
			{
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.ExecuteNonQuery("CREATE TABLE [MULTIUSER_LOCK]( [LOCKID] [int] NOT NULL, [NETWORKID] [int] NOT NULL, [SIMULATIONID] [int] NULL, [USERID] [varchar](256) NOT NULL, [LOCK_START] [datetime] NOT NULL CONSTRAINT [PK_MULTIUSER_LOCK] PRIMARY KEY ( [LOCKID] ASC ))");
						//throw new NotImplementedException( "TODO: Implement SQL Server version of CheckMultiUserTable()" );
						break;
					case "ORACLE":
						DBMgr.ExecuteNonQuery("CREATE TABLE \"MULTIUSER_LOCK\"   ( \"LOCKID\" NUMBER NOT NULL ENABLE, \"NETWORKID\" NUMBER NOT NULL ENABLE,  \"SIMULATIONID\" NUMBER,  \"USERID\" VARCHAR2(256 BYTE) NOT NULL ENABLE,  \"LOCK_START\" TIMESTAMP (6) NOT NULL ENABLE  )");
						break;
					default:
						throw new NotImplementedException("TODO: Implement ANSI version of CheckMultiUserTable()");

				}
			}
			else
			{
				//we start out with missing columns having all the table columns and then we remove the ones we find
				List<string> missingColumns = new List<string>();
				missingColumns.Add( "LOCKID" );
				missingColumns.Add( "NETWORKID" );
				missingColumns.Add( "SIMULATIONID" );
				missingColumns.Add( "USERID" );
				missingColumns.Add( "LOCK_START" );
				missingColumns.Add( "NETWORKREAD" );
				missingColumns.Add( "SIMULATIONREAD" );
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DataSet columnsData = DBMgr.ExecuteQuery( "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MULTIUSER_LOCK'" );
						foreach( DataRow columnNameRow in columnsData.Tables[0].Rows )
						{
							missingColumns.Remove( columnNameRow["COLUMN_NAME"].ToString().ToUpper() );
						}
						if( missingColumns.Count > 0 )
						{
							string alterTableClause = "ALTER TABLE MULTIUSER_LOCK";
							string addClause = " ADD ";
							foreach( string missingColumn in missingColumns )
							{
								switch( missingColumn )
								{
									case "LOCKID":
										addClause += missingColumn + " INT NOT NULL,";
										break;
									case "NETWORKID":
									case "SIMULATIONID":
									case "SIMULATIONREAD":
									case "NETWORKREAD":
										addClause += missingColumn + " INT,";
										//addClause += "ADD " + missingColumn + " INT,";
										break;
									case "USERID":
										addClause += missingColumn + " VARCHAR(256),";
										break;
									case "LOCK_START":
										addClause += missingColumn + " DATETIME,"; //WARNING: DO NOT USE TIMESTAMP FOR SQL SERVER
										break;
								}
							}
					addClause = addClause.Substring( 0, addClause.Length - 1 );
							//addClause += ")";
							string alterTableStatement = alterTableClause + addClause;
							DBMgr.ExecuteNonQuery( alterTableStatement );
						}
						break;
					case "ORACLE":
						columnsData = DBMgr.ExecuteQuery("SELECT COLUMN_NAME FROM USER_TAB_COLS WHERE TABLE_NAME = 'MULTIUSER_LOCK'");
						foreach (DataRow columnNameRow in columnsData.Tables[0].Rows)
						{
							missingColumns.Remove(columnNameRow["COLUMN_NAME"].ToString().ToUpper());
						}
						if (missingColumns.Count > 0)
						{
							string alterTableClause = "ALTER TABLE MULTIUSER_LOCK";
							string addClause = " ADD (";
							foreach (string missingColumn in missingColumns)
							{
								switch (missingColumn)
								{
									case "LOCKID":
										addClause += missingColumn + " NUMBER NOT NULL,";
										break;
									case "NETWORKID":
									case "SIMULATIONID":
									case "SIMULATIONREAD":
									case "NETWORKREAD":
										addClause += missingColumn + " NUMBER,";
										break;
									case "USERID":
										addClause += missingColumn + " VARCHAR2(256),";
										break;
									case "LOCK_START":
										addClause += missingColumn + " TIMESTAMP,"; //WARNING: DO NOT USE TIMESTAMP FOR SQL SERVER
										break;
								}
							}
							addClause = addClause.Substring(0, addClause.Length - 1);
							addClause += ")";
							string alterTableStatement = alterTableClause + addClause;
							DBMgr.ExecuteNonQuery(alterTableStatement);
						}
						break;
					default:
						throw new NotImplementedException("TODO: Implement ANSI version of CheckMultiUserTable()");
						
				}

				//TODO: add else to check for proper columns if table exists

			}
		}

		/// <summary>
		/// The automated Oracle transition fails to properly set up primary key constraints
		/// </summary>
		private void CheckPrimaryKeyConstraints()
		{
			List<string> constraintFixCommands = new List<string>();
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					//for now we'll leave MSSQL alone...
					break;
				case "ORACLE":

					string constraintQuery = "SELECT TABLE_NAME, CONSTRAINT_NAME FROM user_constraints WHERE STATUS != 'ENABLED' AND constraint_type = 'P' ORDER BY TABLE_NAME";
					DataSet constraints = DBMgr.ExecuteQuery( constraintQuery );

					foreach( DataRow constraint in constraints.Tables[0].Rows )
					{
						string tableName = constraint["TABLE_NAME"].ToString();
						string constraintName = constraint["CONSTRAINT_NAME"].ToString();

						constraintFixCommands.Add( "ALTER TABLE " + tableName + " ENABLE CONSTRAINT " + constraintName );
					}
					break;
				default:
					throw new NotImplementedException( "TODO: implement ANSI version of CheckPrimaryKeyConstraints()" );
					
			}
			DBMgr.ExecuteBatchNonQuery( constraintFixCommands );
		}

		private void CheckForInitialUser()
		{
			if( securityEngine.AllUsers.Count == 0 )
			{
#if MDSHA
				if( AddUser( Environment.UserName, "", "", "Is this a fresh installation?", "yes" ) )
				//email can't just be the empty string because Oracle treats '' as NULL
#else
				if( AddUser( "install", "install", "default@ara.com", "Is this a fresh installation?", "yes" ) )
#endif
				{
					if( !securityEngine.AllUserGroups.Exists( delegate( SecurityUserGroup g )
					{
						return g.Name == "Administrators";
					} ) )
					{
						if( AddUserGroup( "Administrators" ) )
						{
							if( !AddUserToGroup( new RoadCareUser( securityEngine.AllUsers[0] ), "Administrators" ) )
							{
								throw new Exception( "Unable to add first user to existing Administrators group." );
							}
						}
						else
						{
							throw new Exception( "Unable to create Administrators usergroup for fresh RoadCare installation." );
						}
					}
					else
					{
						if( !AddUserToGroup( new RoadCareUser( securityEngine.AllUsers[0] ), "Administrators" ) )
						{
							throw new Exception( "Unable to add first user to existing Administrators group." );
						}
					}
				}
				else
				{
					throw new Exception( "Unable to create the initial user." );
				}
			}
		}

		/// <summary>
		/// The automated Oracle transition fails to properly set up foreign key constraints
		/// </summary>
		private void CheckForeignKeyConstraints()
		{
			List<string> constraintFixCommands = new List<string>();
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					//for now we'll leave MSSQL alone...
					break;
				case "ORACLE":
					string constraintQuery = "SELECT uc.TABLE_NAME, uc.CONSTRAINT_NAME, ucc1.COLUMN_NAME as FK, ucc2.TABLE_NAME as REFERTAB, ucc2.COLUMN_NAME as REFERCOL FROM user_constraints uc LEFT JOIN user_cons_columns ucc1 ON uc.CONSTRAINT_NAME = ucc1.constraint_name LEFT JOIN user_cons_columns ucc2 ON uc.R_CONSTRAINT_NAME = ucc2.constraint_name WHERE uc.CONSTRAINT_TYPE = 'R' AND (DELETE_RULE != 'CASCADE' OR STATUS = 'DISABLED') ORDER BY uc.TABLE_NAME";
					DataSet constraints = DBMgr.ExecuteQuery( constraintQuery );

					foreach( DataRow constraint in constraints.Tables[0].Rows )
					{
						string tableName = constraint["TABLE_NAME"].ToString();
						string constraintName = constraint["CONSTRAINT_NAME"].ToString();
						string foreignKey = constraint["FK"].ToString();
						string referenceTable = constraint["REFERTAB"].ToString();
						string referenceColumn = constraint["REFERCOL"].ToString();

						constraintFixCommands.Add( "ALTER TABLE " + tableName + " DROP CONSTRAINT " + constraintName );
						constraintFixCommands.Add( "DELETE FROM " + tableName + " WHERE " + foreignKey + " NOT IN (SELECT DISTINCT " + referenceColumn + " AS " + foreignKey + " FROM " + referenceTable + ")" );
						constraintFixCommands.Add( "ALTER TABLE " + tableName + " ADD CONSTRAINT " + constraintName + " FOREIGN KEY (" + foreignKey + ") REFERENCES " + referenceTable + " (" + referenceColumn + ") ON DELETE CASCADE" );

					}
					break;
				default:
					throw new NotImplementedException( "TODO: implement ANSI version of CheckForeignKeyConstraints()" );
					
			}
			DBMgr.ExecuteBatchNonQuery( constraintFixCommands );
		}

		public void RemoveNetworkActions( string networkID )
		{
			List<SecurityActionGroup> relevantGroups = new List<SecurityActionGroup>();
			List<SecurityAction> toRemove = new List<SecurityAction>();
			

			foreach( SecurityAction action in securityEngine.AllActions )
			{
				if( action["NETWORK_"] == networkID )
				{
					//collect all the groups that may possibly now be empty
					foreach( SecurityActionGroup group in action.Groups )
					{
						if( !relevantGroups.Contains( group ) )
						{
							relevantGroups.Add( group );
						}
					}
					toRemove.Add( action );
				}
			}

			foreach( SecurityAction action in toRemove )
			{
				//remove the actions
				securityEngine.RemoveAction( action, m_currentUser.ToSecurityUser );
			}


			foreach (SecurityActionGroup actionGroup in securityEngine.AllActionGroups)
			{
				if (actionGroup.Members.Count == 0)
				{
					securityEngine.RemoveActionGroup(actionGroup, m_currentUser.ToSecurityUser);
				}
			}
		}


		public bool CanViewCalculatedFields()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CALCULATED_FIELDS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanCreateCalculatedFields()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "CALCULATED_FIELDS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.Create;
		}

		public void RemoveSimulationActions( string network, string simulation )
		{
			List<RoadCareAction> actionsToRemove = new List<RoadCareAction>();
			foreach( RoadCareAction checkAction in AllActions )
			{
				if( checkAction.Descriptor["NETWORK_"] == network && checkAction.Descriptor["SIMULATION"] == simulation )
				{
					actionsToRemove.Add( checkAction );
				}
			}

			foreach( RoadCareAction actionToRemove in actionsToRemove )
			{
				RemoveAction( actionToRemove );
			}
		}

		public bool CanLoadBinaries()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "BINARIES" );
			actionDescriptor.Add( "DESCRIPTION", "LOAD" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.CreateDestroy;
		}

		public bool CanViewSecurityNode()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "SECURITY" );
			actionDescriptor.Add( "DESCRIPTION", "ROOT" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewRawAttributeNode()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "RAW_ATTRIBUTES" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewAssetNode()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "RAW_ASSETS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public bool CanViewNetworkNode()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "NETWORKS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadOnly;
		}

		public void CopyNetworkPoliciesFromTo( string fromNetwork, string toNetwork )
		{
			Dictionary<string, string> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "NETWORK_", fromNetwork );

			List<SecurityAction> relevantActions = securityEngine.GetAllActions( actionDescriptor );
			Dictionary<string, string> actionIDMapping = new Dictionary<string, string>();
			List<string> releventGroups = new List<string>();
			
			foreach( SecurityAction relevantAction in relevantActions )
			{
				//create the new action using the old action's descriptor
				Dictionary<string,string> newActionDescriptor = new Dictionary<string,string>();
				foreach( string key in relevantAction.Descriptor.Keys )
				{
					newActionDescriptor.Add( key, relevantAction.Descriptor[key] );
				}
				newActionDescriptor["NETWORK_"] = toNetwork;

				//newActionDescriptor.Remove( "ACTION_ID" );
				//string addedActionId = securityEngine.AddAction( newActionDescriptor, m_currentUser.ToSecurityUser );
				newActionDescriptor.Remove( "ACTION_ID" );
				string addedActionId = securityEngine.AddAction( newActionDescriptor, m_currentUser.ToSecurityUser );
				actionIDMapping.Add( relevantAction.ActionID, addedActionId );

				
				//copy new action into the previous action's groups
				foreach( string groupID in relevantAction.GroupIDs )
				{
					DBMgr.ExecuteNonQuery( "INSERT INTO SEC_ACTIONGROUP_MEM (ACTIONGROUP_ID, ACTION_ID) VALUES (" + groupID + "," + addedActionId + ")" );
					if( !releventGroups.Contains( groupID ) )
					{
						releventGroups.Add( groupID );
					}
				}
			}

			List<string> insertCommands = new List<string>();

			//COPY USER/ACTION
			DataSet userActionSet = DBMgr.ExecuteQuery( "SELECT * FROM SEC_USER_ACTION_PER WHERE ACTION_ID IN (SELECT ACTION_ID FROM SEC_ACTIONS WHERE NETWORK_ = '"+fromNetwork+"')" );
			foreach( DataRow userActionRow in userActionSet.Tables[0].Rows )
			{
				insertCommands.Add( "INSERT INTO SEC_USER_ACTION_PER (USER_ID, ACTION_ID, ACCESS_LEVEL) VALUES (" + userActionRow["USER_ID"].ToString() + "," + actionIDMapping[userActionRow["ACTION_ID"].ToString()] + "," + userActionRow["ACCESS_LEVEL"].ToString() + ")" );
			}

			//COPY USERGROUP/ACTION
			DataSet usergroupActionSet = DBMgr.ExecuteQuery( "SELECT * FROM SEC_USERGROUP_ACTION_PER WHERE ACTION_ID IN (SELECT ACTION_ID FROM SEC_ACTIONS WHERE NETWORK_ = '" + fromNetwork + "')" );
			foreach( DataRow userActionRow in userActionSet.Tables[0].Rows )
			{
				insertCommands.Add( "INSERT INTO SEC_USERGROUP_ACTION_PER (USERGROUP_ID, ACTION_ID, ACCESS_LEVEL) VALUES (" + userActionRow["USERGROUP_ID"].ToString() + "," + actionIDMapping[userActionRow["ACTION_ID"].ToString()] + "," + userActionRow["ACCESS_LEVEL"].ToString() + ")" );
			}

			DBMgr.ExecuteBatchNonQuery( insertCommands );

			//since we copy the newly created actions into the existing groups (rather than trying to guess at an appropriate new group)
			//we don't have to worry about dealing with actiongroup permissions...all the previous actiongroup permissions will still work
		}


		public bool CanModifyOptions()
		{
			Dictionary<String, String> actionDescriptor = new Dictionary<string, string>();
			actionDescriptor.Add( "ACTION_TYPE", "OPTIONS" );

			return GetPermissions( actionDescriptor ) >= AccessLevel.ReadWrite;
		}

	}
}
