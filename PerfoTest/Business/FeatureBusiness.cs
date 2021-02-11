using System;
using System.Collections.Generic;
using GeoJSON.Net.Feature;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Feature = LayersApi.Models.DBModels.Feature;
using LayersApi.Business;

namespace PerfoTest.Business
{
    public static class FeatureBusiness
    {
        public static List<Feature> CreateFeatures(FeatureCollection featureCollection, string layerId)
        {
            var features = new List<Feature>();
            var geometryCreator = new DataBaseGeometryCreator();
            var jsonReader = new WKTReader();
            var getometryStringCreator = new DataBaseGeometryCreator();

            featureCollection?.Features.ForEach(feature =>
            {
                var jsonFeature = JsonConvert.SerializeObject(feature);
                var wktGeometry = getometryStringCreator.CreateWellKnownTextGeometry(feature);

                var newFeature = new Feature()
                {
                    GeometryType = (short)feature.Geometry.Type,
                    GeojsonArea = jsonFeature,
                    Id = Guid.NewGuid().ToString(),
                    Area = jsonReader.Read(wktGeometry),
                    Layer = new LayersApi.Models.DBModels.Layer() { Id = layerId }
                };

                features.Add(newFeature);
            });

            return features;
        }
    }
}
