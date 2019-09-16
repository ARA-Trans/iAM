using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManager;
using System.Data;

namespace SecurityManager
{
	public class SecurityUserGroup
	{
		string name;
		string id;
		List<SecurityUser> members;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string ID
		{
			get
			{
				return id;
			}
		}

		public List<SecurityUser> Members
		{
			get
			{
				return members;
			}
		}

		public SecurityUserGroup( string newID, string newName )
		{
			name = newName;
			id = newID;
			members = new List<SecurityUser>();
		}
		public SecurityUserGroup( string newID, string newName, List<SecurityUser> newMembers )
		{
			name = newName;
			id = newID;
			members = newMembers;
		}
		public SecurityUserGroup( Dictionary<string, string> userGroupDescriptor )
		{
			name = userGroupDescriptor["USERGROUP_NAME"];
			id = userGroupDescriptor["USERGROUP_ID"];
			members = new List<SecurityUser>();
		}

		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is SecurityUserGroup )
			{
				SecurityUserGroup otherGroup = ( SecurityUserGroup )obj;
				same = otherGroup.ID == id;
			}
			else if( obj is Dictionary<string, string> )
			{
				Dictionary<string, string> descriptor = ( Dictionary<string, string> )obj;
				foreach( string columnName in descriptor.Keys )
				{
					switch( columnName )
					{
						case "USERGROUP_ID":
							if( id != descriptor[columnName] )
							{
								same = false;
								break;
							}
							break;
						case "USERGROUP_NAME":
							if( name != descriptor[columnName] )
							{
								same = false;
								break;
							}
							break;
						default:
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}
