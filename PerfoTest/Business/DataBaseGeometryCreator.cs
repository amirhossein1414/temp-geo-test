using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LayersApi.Business
{
    public class DataBaseGeometryCreator
    {
        public string GetGeometryString(Feature feature)
        {
            string geometryString = CreateWellKnownTextGeometry(feature);

            return $"ST_GeomFromText('{geometryString}')";
        }

        public string CreateWellKnownTextGeometry(Feature feature)
        {
            string geometryString;

            switch (feature.Geometry.Type)
            {
                case GeoJSONObjectType.Point:
                    geometryString = CreatePointObject(feature);
                    break;
                case GeoJSONObjectType.MultiPoint:
                    geometryString = CreateMultiPointObject(feature);
                    break;
                case GeoJSONObjectType.LineString:
                    geometryString = CreateLineStringObject(feature);
                    break;
                case GeoJSONObjectType.MultiLineString:
                    geometryString = CreateMultiLineStringObject(feature);
                    break;
                case GeoJSONObjectType.Polygon:
                    geometryString = CreatePolygonObject(feature);
                    break;
                case GeoJSONObjectType.MultiPolygon:
                    geometryString = CreateMultiPolygonObject(feature);
                    break;
                default:
                    throw new Exception("invalid geometry type:" + feature.Geometry.Type.ToString());
            }

            return geometryString;
        }

        public string CreateMultiPolygonObject(Feature feature)
        {
            var multiPolygonFeature = feature.Geometry as MultiPolygon;
            var multiPolygon = string.Join(",", multiPolygonFeature.Coordinates.Select(x => $"({GetConcatedLineStrings(x.Coordinates)})"));

            return $"MULTIPOLYGON({multiPolygon})";
        }

        public string CreatePolygonObject(Feature feature)
        {
            var polygonFeature = feature.Geometry as Polygon;
            var polygon = GetConcatedLineStrings(polygonFeature.Coordinates);

            return $"POLYGON({polygon})";
        }

        public string CreatePointObject(Feature feature)
        {
            var pointFeature = feature.Geometry as Point;
            var point = CreateConcatedPoints(new List<IPosition>() { pointFeature.Coordinates });

            return $"Point({point})";
        }

        public string CreateMultiPointObject(Feature feature)
        {
            var multiPointFeature = feature.Geometry as MultiPoint;
            var multiPoint = CreateConcatedPoints(multiPointFeature.Coordinates.Select(x => x.Coordinates));

            return $"MULTIPOINT({multiPoint})";
        }

        public string CreateLineStringObject(Feature feature)
        {
            var lineStringFeature = feature.Geometry as LineString;
            var lineString = CreateConcatedPoints(lineStringFeature.Coordinates);

            return $"LINESTRING({lineString})";
        }

        public string CreateMultiLineStringObject(Feature feature)
        {
            var multiLineStringFeature = feature.Geometry as MultiLineString;
            var multiLineString = GetConcatedLineStrings(multiLineStringFeature.Coordinates);

            return $"MULTILINESTRING({multiLineString})";
        }

        public string GetConcatedLineStrings(IEnumerable<LineString> lineStrings)
        {
            var result = string.Join(",", lineStrings.Select(x => $"({CreateConcatedPoints(x.Coordinates)})"));
            return result;
        }

        public string CreateConcatedPoints(IEnumerable<IPosition> points)
        {
            return string.Join(",", points.Select(x => $"{x.Longitude} {x.Latitude}"));
        }
    }
}
