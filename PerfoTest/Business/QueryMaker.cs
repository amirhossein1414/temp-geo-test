using LayersApi.Models.DBModels;
using PerfoTest.Model;

namespace LayersApi.Business
{
    public static class QueryMaker
    {
        public static string GetPolygonQueryString(BoundingBox boundingBox)
        {
            var polygonStringFirstPart = "ST_GeomFromText('POLYGON((";
            var polygonStringMiddlePart = "";

            for (int i = 0; i < boundingBox?.Coordinates?.Count; i++)
            {
                if (i > 0)
                {
                    polygonStringMiddlePart += " ,";
                }

                polygonStringMiddlePart += boundingBox?.Coordinates[i].Lng + " " + boundingBox?.Coordinates[i].Lat;
            }

            var polygonStringLastPart = "))')";
            var polygonString = polygonStringFirstPart + polygonStringMiddlePart + polygonStringLastPart;

            return polygonString;
        }

        public static string GetSearchWithinPolygonQueryString(GetLayersRequest request, string boundingBoxString, string limit)
        {
            string layerIdsCriteria = CreateLayerIdsCriteria(request);

            var query = $"select \"{nameof(Feature.GeojsonArea)}\" from public.\"{nameof(Feature)}\" tb where ST_Intersects({boundingBoxString}" +
                $", tb.\"{nameof(Feature.Area)}\") {layerIdsCriteria}" +
                $"limit {limit}";

            return query;
        }

        private static string CreateLayerIdsCriteria(GetLayersRequest request)
        {
            var idCluse = "";
            if (request?.LayerIds?.Count > 0)
            {
                idCluse += "and \"LayerId\" in (";
                var isFirst = true;
                request.LayerIds.ForEach(id =>
                {
                    if (isFirst)
                    {
                        idCluse += $"'{id}'";
                        isFirst = false;
                    }
                    else
                    {
                        idCluse += $",'{id}'";
                    }
                });
                idCluse += ")";
            }

            return idCluse;
        }
    }
}
