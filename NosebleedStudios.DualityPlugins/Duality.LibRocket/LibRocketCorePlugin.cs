using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duality.LibRocket
{
    public class LibRocketCorePlugin: CorePlugin
    {
        public static GuiCore GuiCore { get; private set; }

        protected override void InitPlugin()
        {
            GuiCore = GuiCore ?? new GuiCore();
            GuiCore.Initialize();
            base.InitPlugin();
        }

        protected override void OnDisposePlugin()
        {
            base.OnDisposePlugin();
            GuiCore.Shutdown();
            GuiCore = null;

        }
    }
}
