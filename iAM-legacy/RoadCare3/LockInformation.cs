using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
	public class LockInformation
	{
		private string lockID = "";
		private string userID = "";
		private string networkID = "";
		private string simulationID = "";
		private DateTime startTime = DateTime.MinValue;
		private bool networkReadable;
		private bool simulationReadable;

		public LockInformation()
		{
		}

		public LockInformation( DataRow lockRow )
		{
			lockID = lockRow["LOCKID"].ToString();
			userID = lockRow["USERID"].ToString();
			networkID = lockRow["NETWORKID"].ToString();
			simulationID = lockRow["SIMULATIONID"].ToString();
			startTime = DateTime.Parse( lockRow["LOCK_START"].ToString() );
			networkReadable = lockRow["NETWORKREAD"].ToString() == "1";
			simulationReadable = lockRow["SIMULATIONREAD"].ToString() == "1";
		}

		public bool NetworkReadable
		{
			get
			{
				return networkReadable;
			}
		}

		public bool SimulationReadable
		{
			get
			{
				return simulationReadable;
			}
		}
		

		public bool Locked
		{
			get
			{
				return lockID != "";
			}
		}

		public DateTime Start
		{
			get
			{
				return startTime;
			}
		}

		public string LockOwner
		{
			get
			{
				return userID;
			}
		}

		internal void UnlockNetwork()
		{
			if( lockID != "" )
			{
				DBOp.RemoveLock( lockID );
			}
			else
			{
				throw new Exception( "ERROR: Cannot attempt unlock on uninitialized LockInformation object." );
			}
		}
	}
}
