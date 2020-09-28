using System.Collections.Generic;

namespace PerfoTest.Business.Postgres
{
    public static class QueryMaker
    {
        public static string GetPolygonQueryString(List<PostgresStringPoint> points)
        {
            var polygonStringFirstPart = "ST_GeomFromText('POLYGON((";
            var polygonStringMiddlePart = "";

            for (int i = 0; i < points.Count; i++)
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

        public static string GetSearchWithinPolygonQueryString(string tableName,
            string columnName,
            string polygon,
            string limit)
        {
            var query = $"select id,content from {tableName} where ST_Contains({polygon}" +
                $", ST_GeomFromText(concat('Point','(',{tableName}.{columnName}[0],' ',{tableName}.{columnName}[1] , ')'))) " +
                $"limit {limit}";



            return query;
        }

    }
}
