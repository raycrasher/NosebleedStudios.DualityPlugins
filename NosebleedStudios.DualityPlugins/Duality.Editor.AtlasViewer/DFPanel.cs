using System.Windows.Forms;

namespace Duality.Editor.AtlasViewer
{
    public class DFPanel : FlowLayoutPanel
    {
        protected override bool DoubleBuffered { get => true; set { } }
    }
}
