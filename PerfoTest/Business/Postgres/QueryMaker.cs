using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Business.Postgres
{
    public class QueryMaker
    {
        public string GetPolygonQueryString(params PostgresStringPoint[] points)
        {
            var polygonStringFirstPart = "ST_GeomFromText('POLYGON((";
            var polygonStringMiddlePart = "";

            for (int i = 0; i < points.Length; i++)
            {
                if (i > 0)
                {
                    polygonStringMiddlePart += " ,";
                }

                polygonStringMiddlePart += points[i].FirstPart + " " + points[i].SecondPart;
            }

            var polygonStringLastPart = "))')";
            var polygonString = polygonStringFirstPart + polygonStringMiddlePart + polygonStringLastPart;
            return polygonString;
        }

        public string GetSearchWithinPolygonQueryString(string tableName, string columnName, string polygon)
        {
            var query = $"select content from {tableName} where ST_Contains(ST_GeomFromText('{polygon}" +
                $"), ST_GeomFromText(concat('Point','(',{tableName}.{columnName}[0],' ',{tableName}.{columnName}[1] , ')'))) " +
                $"limit 2000";



            return query;
        }

    }
}
