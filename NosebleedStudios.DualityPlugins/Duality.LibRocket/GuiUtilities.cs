using Duality.Resources;
using LibRocketNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duality.LibRocket
{
    public static class GuiUtilities
    {
        public static void ClearChildren(this Element elem)
        {
            elem.InnerRml = "";
        }
        public static string GetImageElementRmlFromAtlas(ContentRef<Texture> texture, int index, string additionalProps=null)
        {
            if (!texture.IsAvailable)
                return "<img/>";
            var atlas = texture.Res.BasePixmap.Res?.Atlas;
            Rect rect;
            if (atlas == null || index < 0 || index >= atlas.Count)
                rect = new Rect(0, 0, texture.Res.Size.X, texture.Res.Size.Y);
            else
                rect = atlas[index];

            rect = texture.Res.LookupAtlas(index);
            rect.Pos *= texture.Res.Size;
            rect.Size *= texture.Res.Size;


            return $@"<img src=""/{texture.Path}"" coords=""{rect.LeftX}px, {rect.TopY}px, {rect.RightX}px, {rect.BottomY}px"" {additionalProps} />";
        }

        public static string GetImageElementRml(ContentRef<Texture> texture, string additionalProps = null)
        {
            if (!texture.IsAvailable)
                return "<img/>";
            return $@"<img src=""/{texture.Path}"" style=""width:{texture.Res.Size.X}px; height:{texture.Res.Size.Y}px;"" {additionalProps} />";
        }
    }
}
