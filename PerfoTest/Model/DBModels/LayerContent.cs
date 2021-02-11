namespace LayersApi.Models.DBModels
{
    public class LayerContent
    {
        public string Id { get; set; }
        public string Geojson { get; set; }
        public string LayerId { get; set; }
        public Layer Layer { get; set; }
    }
}
