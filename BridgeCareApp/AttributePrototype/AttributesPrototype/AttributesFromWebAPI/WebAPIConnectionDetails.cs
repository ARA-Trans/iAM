using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.AttributesFromWebAPI
{
    class WebAPIConnectionDetails : IConnectionDetails
    {
        public string Id { get; set; }

        public string Bearer { get; set; }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Server { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DataSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public WebAPIConnectionDetails(string id, string bearer)
        {
            Id = id;
            Bearer = bearer;
        }
        // If we want some functionality related to connections. That will go here
    }
}
