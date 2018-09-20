using Duality;
using Duality.Components.Renderers;
using Duality.Editor;
using Duality.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duality.Editor.AtlasViewer
{
    public class AtlasSpriteConverter : DataConverter
    {
        public override Type TargetType => typeof(SpriteRenderer);

        public override bool CanConvertFrom(ConvertOperation convert)
        {
            return
                convert.AllowedOperations.HasFlag(ConvertOperation.Operation.CreateObj) &&
                convert.CanPerform<AtlasSprite>();
        }

        public override bool Convert(ConvertOperation convert)
        {
            if (convert.Result.OfType<ICmpSpriteRenderer>().Any())
                return false;
            List<object> results = new List<object>();
            List<AtlasSprite> availData = convert.Perform<AtlasSprite>().ToList();
            

            foreach (var sprite in availData)
            {
                if (!sprite.Pixmap.IsAvailable)
                    continue;

                GameObject gameobj = convert.Result.OfType<GameObject>().FirstOrDefault();
                SpriteRenderer renderer = convert.Result.OfType<SpriteRenderer>().FirstOrDefault();

                if (renderer == null)
                {
                    var r = sprite.Pixmap.Res.Atlas[sprite.Index];
                    renderer = new SpriteRenderer()
                    {
                        SpriteIndex = sprite.Index,
                        Rect = new Rect(-r.W / 2, -r.H / 2, r.W, r.H)
                    };

                    var mat = this.FindMatchingResources<Pixmap, Material>(sprite.Pixmap.Res, (s, t) => t.MainTexture.Res?.BasePixmap == s).FirstOrDefault();
                    if (mat == null)
                    {
                        var tex = this.FindMatchingResources<Pixmap, Texture>(sprite.Pixmap.Res, (s, t) => t.BasePixmap == s).FirstOrDefault();
                        if (tex == null)
                        {
                            string texPath = PathHelper.GetFreePath(sprite.Pixmap.FullName, Resource.GetFileExtByType<Texture>());
                            tex = new Texture(sprite.Pixmap);
                            tex.Save(texPath);
                        }
                        string matPath = PathHelper.GetFreePath(sprite.Pixmap.FullName, Resource.GetFileExtByType<Material>());
                        mat = new Material() { MainTexture = tex };
                        mat.Save(matPath);
                    }
                    renderer.SharedMaterial = mat;
                    results.Add(renderer);
                    convert.SuggestResultName(renderer, $"{sprite.Pixmap.Name}_{sprite.Index}");
                    convert.MarkObjectHandled(sprite);
                }
            }
            convert.AddResult(results);
            return false;
        }
    }
}
