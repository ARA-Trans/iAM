using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspWebApi.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }
}