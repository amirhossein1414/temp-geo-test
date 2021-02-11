using System.Collections.Generic;

namespace LayersApi.Models.DBModels
{
    public class Layer
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public LayerContent LayerContent { get; set; }
        public List<Feature> Features { get; set; }
    }
}
