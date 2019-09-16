using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityManager;

namespace RoadCareSecurityOperations
{
	public class RoadCareAction
	{
		//private object m_basisObject;
		private Dictionary<string,string> specifier;

		public RoadCareAction(String actionType, String actionDescription )
		{
			specifier = new Dictionary<string,string>();
			specifier.Add( "ACTION_TYPE", actionType );
			specifier.Add( "DESCRIPTION", actionDescription );
		}

		public RoadCareAction( SecurityAction secAction )
		{
			specifier = new Dictionary<string, string>();
			specifier.Add( "ACTION_TYPE", secAction["ACTION_TYPE"] );
			specifier.Add( "DESCRIPTION", secAction["DESCRIPTION"] );
			specifier.Add( "NETWORK_", secAction["NETWORK_"] );
			specifier.Add( "SIMULATION", secAction["SIMULATION"] );
		}

		public override string ToString()
		{
			string textDescriptor = "";
			foreach( string columnName in specifier.Keys )
			{
				textDescriptor += specifier[columnName] + " ";
			}
			return textDescriptor;
		}

		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is RoadCareAction )
			{
				RoadCareAction otherAction = ( RoadCareAction )obj;
				foreach( string key in this.specifier.Keys )
				{
					if( otherAction.specifier.ContainsKey( key ) )
					{
						if( otherAction.specifier[key] != this.specifier[key] )
						{
							same = false;
							break;
						}
					}
					else
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

		public Dictionary<string, string> Descriptor
		{
			get
			{
				return specifier;
			}
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}
