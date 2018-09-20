using Duality;
using Duality.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duality.Editor.AtlasViewer
{
    public class AtlasSprite
    {
        public AtlasSprite(Pixmap map, int index)
        {
            Pixmap = map;
            Index = index;
        }
        public AtlasSprite() { }

        public int Index { get; set; }
        public ContentRef<Pixmap> Pixmap { get; set; }
        public bool IsSelected { get; set; }
    }
}
