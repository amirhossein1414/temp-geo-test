using Microsoft.SqlServer.Types;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Model
{
    public class PostgresItem
    {
        public string id;
        public string title;
        public NpgsqlPoint /*NpgsqlPoint*/ /*DbGeography*/ geodata;
        public string content;
    }
}
