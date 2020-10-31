using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfoTest.Model
{
    public class GetLayersRequest
    {
        public List<string> LayerIds { get; set; }
        public BoundingBox boundingBox { get; set; }
    }
}
