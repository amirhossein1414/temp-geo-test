
using NetTopologySuite.Geometries;

namespace LayersApi.Models.DBModels
{
    public class Feature
    {
        public string Id { get; set; }
        public string GeojsonArea { get; set; }
        public Geometry Area { get; set; }
        public short GeometryType { get; set; }
        public Layer Layer { get; set; }
    }
}
