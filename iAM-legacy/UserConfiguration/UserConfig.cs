using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Drawing;

namespace UserConfiguration
{
    public class User
    {
        private String m_strUserID;
        private String m_strPassword;
        private String m_strDatabase;

        private Hashtable m_htColors;
        private Hashtable m_htValues;

        public User(String UserID, String strPassword, String strDatabase, Hashtable htColors, Hashtable htValues)
        {
            m_strUserID = UserID;
            m_strPassword = strPassword;
            m_strDatabase = strDatabase;
            m_htColors = htColors;
            m_htValues = htValues;
        }

        public void CreateDefaultColors(String strAttribute)
        {
            // Green, Blue, Yellow, Magenta, Red, Black
            //String strColor;
            //foreach(Color c in System.Drawing.)
            //{
            //    strColor = c.ToString().ToUpper();
            //    if (!strColor.Contains("WHITE"))
            //    {
            //        m_htColors.Add(strAttribute, c);
            //    }
            //}
        }

        public String UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public String Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public String DatabaseName
        {
            get { return m_strDatabase; }
            set { m_strDatabase = value; }
        }

        public Hashtable Colors
        {
            get { return m_htColors; }
            set { m_htColors = value; }
        }

        public Hashtable Values
        {
            get { return m_htValues; }
            set { m_htValues = value; }
        }
    }

    public static class UserConfig
    {
        private static Hashtable m_htUsers = new Hashtable();

        public static void AddNewUser(User user)
        {
            m_htUsers.Add(user.UserID, user);
        }

        public static void RemoveUser(User user)
        {
            m_htUsers.Remove(user.UserID);
        }

        public static User GetUser(String strUserName)
        {
            return (User)m_htUsers[strUserName];
        }
    }
}
