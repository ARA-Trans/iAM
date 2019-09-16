using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;

namespace SecurityManager
{
	public class SecurityActionGroup
	{
		string name;
		string id;
		List<SecurityAction> members;

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

		public List<SecurityAction> Members
		{
			get
			{
				return members;
			}
		}

		public SecurityActionGroup( string newID, string newName )
		{
			name = newName;
			id = newID;
			members = new List<SecurityAction>();
		}
		public SecurityActionGroup( string newID, string newName, List<SecurityAction> newMembers )
		{
			name = newName;
			id = newID;
			members = newMembers;
		}
		public SecurityActionGroup( Dictionary<string, string> actionGroupDescriptor )
		{
			name = actionGroupDescriptor["ACTIONGROUP_NAME"];
			id = actionGroupDescriptor["ACTIONGROUP_ID"];
			members = new List<SecurityAction>();
		}


		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is SecurityActionGroup )
			{
				SecurityActionGroup otherGroup = ( SecurityActionGroup )obj;
				same = otherGroup.ID == id;
			}
			else if( obj is Dictionary<string, string> )
			{
				Dictionary<string, string> descriptor = ( Dictionary<string, string> )obj;
				foreach( string columnName in descriptor.Keys )
				{
					switch( columnName )
					{
						case "ACTIONGROUP_ID":
							if( id != descriptor[columnName] )
							{
								same = false;
								break;
							}
							break;
						case "ACTIONGROUP_NAME":
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
