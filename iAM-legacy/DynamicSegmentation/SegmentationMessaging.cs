using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicSegmentation
{
	public static class SegmentationMessaging
	{
		static private List<String> m_strListProgress = new List<string>();
		static private bool m_bCancel = false;

		static public List<String> GetProgressList()
		{
			lock (m_strListProgress)
			{
				return m_strListProgress;
			}
		}

		static public void SetProgressList(List<String> list)
		{
			lock (m_strListProgress)
			{
				foreach (String str in list)
				{
					m_strListProgress.Add(str);

				}
			}
		}

		static public void ClearProgressList()
		{
			lock (m_strListProgress)
			{
				m_strListProgress.Clear();
			}
		}

		static public void SetCancel(bool bCancel)
		{
			m_bCancel = bCancel;
		}

		static public bool GetCancel()
		{
			return m_bCancel;
		}

		static public void AddMessage(String strMessage)
		{
			lock (m_strListProgress)
			{
				m_strListProgress.Add(strMessage);
			}

		}
	}
}
