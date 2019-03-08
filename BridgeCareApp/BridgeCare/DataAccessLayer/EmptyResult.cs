using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.DataAccessLayer
{
    public class EmptyResult
    {
        public static IQueryable<T> AsQueryable<T>()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        public static List<T> AsList<T>()
        {
            return Enumerable.Empty<T>().ToList();
        }
    }
}