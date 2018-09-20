using Duality;
using Duality.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duality.Drawing;
using Duality.Resources;

namespace Duality.LibRocket
{
    [Editor.EditorHintCategory("LibRocket")]
    public class GuiController : Renderer, ICmpUpdatable
    {
        public override float BoundRadius => float.PositiveInfinity;
        public ContentRef<DrawTechnique> Technique { get; set; }

        public override void Draw(IDrawDevice device)
        {
            LibRocketCorePlugin.GuiCore?.Draw(device, (DrawTechnique)Technique);
        }

        void ICmpUpdatable.OnUpdate()
        {
            LibRocketCorePlugin.GuiCore?.Update();
        }
    }
}
