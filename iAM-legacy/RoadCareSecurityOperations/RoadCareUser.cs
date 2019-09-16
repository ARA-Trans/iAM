using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurityManager;

namespace RoadCareSecurityOperations
{
	public class RoadCareUser
	{
		private Dictionary<String,String> specifier;
		private bool m_authenticated;
		private string m_unencryptedPassword = null;
		private SecurityUser m_fundamentalUser = null;
		
		//private SecurityManager.SecurityManager m_securityEngine;

		public RoadCareUser(String userName, String password)
		{
			specifier = new Dictionary<string,string>();
			specifier.Add( "USER_LOGIN", userName );
			m_unencryptedPassword = password;
			///specifier.Add( "UNENCRYPTED_PASSWORD", password );
		}

		public RoadCareUser( SecurityUser secUser )
		{
			specifier = new Dictionary<string, string>();
			specifier.Add( "USER_LOGIN", secUser["USER_LOGIN"] );
			//specifier.Add( "PASSWORD", secUser["PASSWORD"] );
			specifier.Add( "EMAIL", secUser["EMAIL"] );
			specifier.Add( "LOWERED_EMAIL", secUser["LOWERED_EMAIL"] );
			specifier.Add( "PASSWORD_QUESTION", secUser["PASSWORD_QUESTION"] );
			specifier.Add( "PASSWORD_ANSWER", secUser["PASSWORD_ANSWER"] );
			m_fundamentalUser = secUser;
		}

		public override string ToString()
		{
			return specifier["USER_LOGIN"];
		}

		public override bool Equals( object obj )
		{
			bool same = true;
			if( obj is RoadCareUser )
			{
				RoadCareUser otherUser = ( RoadCareUser )obj;
				foreach( string key in this.specifier.Keys )
				{
					if( otherUser.specifier.ContainsKey( key ) )
					{
						if( otherUser.specifier[key] != this.specifier[key] )
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

		public String Name
		{
			get
			{
				return specifier["USER_LOGIN"];
			}
		}

		public String Password
		{
			get
			{
				return specifier["PASSWORD"];
			}
		}

		public String UnencryptedPassword
		{
			get
			{
				return m_unencryptedPassword;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_authenticated;
			}
			set
			{
				m_authenticated = value;
			}
		}

		public Dictionary<String, String> Descriptor
		{
			get
			{
				return specifier;
			}
		}

		public SecurityUser ToSecurityUser
		{
			get
			{
				return m_fundamentalUser;
			}
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

	}
}
