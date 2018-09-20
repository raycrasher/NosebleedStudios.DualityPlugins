using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duality;
using Duality.Drawing;
using Duality.Resources;
using LibRocketNet;

namespace Duality.LibRocket
{
    public class LibRocketRendererInterface : LibRocketNet.RenderInterface
    {
        private bool _scissorEnabled = false;

        IDrawDevice _device;
        Dictionary<IntPtr, (ContentRef<Texture> tex, bool isGenerated)> _textures = new Dictionary<IntPtr, (ContentRef<Texture> tex, bool isGenerated)>();
        private Rect _scissorRegion;

        VertexC1P3T2[] _vtxBuffer = new VertexC1P3T2[2048];

        public float ZIndex { get; set; }

        //Dictionary<IntPtr, Geometry> _geometries = new Dictionary<IntPtr, Geometry>();
        Dictionary<IntPtr, DrawBatch> _geometries = new Dictionary<IntPtr, DrawBatch>();

        public DrawTechnique Technique { get; set; }

        public IDrawDevice Device
        {
            get => _device;
            set => _device = value;
        }

        public LibRocketRendererInterface()
        {
            _textures[IntPtr.Zero] = (null, true);
            _geometries[IntPtr.Zero] = null;

            Logs.Game.Write("LibRocket RenderInterface created.");
        }

        protected override void EnableScissorRegion(bool enable)
        {
            _scissorEnabled = enable;
        }

        protected override IntPtr CompileGeometry(Vertex[] vertices, int[] indices, IntPtr texture)
        {   
            var buffer = new VertexBuffer();
            var texinfo = _textures[texture];

            if (texinfo.tex == null)
            {
                texinfo.tex = new Texture(new Pixmap(new PixelData(1, 1, ColorRgba.White)));
                _textures[texture] = texinfo;
            }

            buffer.LoadVertexData(GetVertices(vertices, texinfo), 0, vertices.Length);
            buffer.LoadIndexData(indices.Select(s=>(ushort)s).ToArray(), 0, indices.Length);
            var batch = new DrawBatch(buffer, null, VertexMode.Triangles, new BatchInfo(Technique, ColorRgba.White, texinfo.tex));
            var idx = (IntPtr)batch.GetHashCode();
            _geometries[idx] = batch;
            return idx;
        }

        protected override void RenderCompiledGeometry(IntPtr geometry, Vector2f translation)
        {
            if (_device == null || Technique == null)
                return;
            var batch = _geometries[geometry];
            SetClipRect(batch.Material);
            if (batch.Material.Technique != Technique)
                batch.Material.Technique = Technique;
            batch.Material.SetValue("translation", new Duality.Vector2(translation.X, translation.Y));
            _device.AddBatch(batch);
            //_device.AddVertices(batchInfo, VertexMode.Triangles, batch.Vertices);
        }

        protected override void ReleaseCompiledGeometry(IntPtr geometry)
        {
            _geometries.Remove(geometry);
        }

        protected override void ReleaseTexture(IntPtr texture)
        {
            _textures.Remove(texture);
        }

        protected override bool GenerateTexture(ref IntPtr texture_handle, byte[] source, Vector2i size)
        {            
            var pixels = new ColorRgba[size.X * size.Y];

            int i = 0, j = 0;
            while (i < source.Length)
            {
                pixels[j].R = source[i++];
                pixels[j].G = source[i++];
                pixels[j].B = source[i++];
                pixels[j].A = source[i++];
                j++;
            }

            var pixdata = new PixelData(size.X, size.Y, pixels);
            var pixMap = new Pixmap(pixdata);
            var texture = new Texture(pixMap, TextureSizeMode.Default, TextureMagFilter.Linear, TextureMinFilter.Linear, format: TexturePixelFormat.Rgba);
            texture.AnisotropicFilter = true;
            texture_handle = new IntPtr(texture.GetHashCode());
            _textures[texture_handle] = (texture,true);
            return true;
        }

        private VertexC1P3T2[] GetVertices(Vertex[] vertices, (ContentRef<Texture> tex, bool isGenerated) texinfo)
        {
            var newVerts = new VertexC1P3T2[vertices.Length];
            float pixelOffset = MathF.RoundToInt(_device.TargetSize.X) != (MathF.RoundToInt(_device.TargetSize.X) / 2) * 2 ? 0.5f : 0f;
            var texSize = texinfo.tex.Res?.Size ?? Vector2.One;
            for (int i = 0; i < vertices.Length; i++)
            {
                var vtx = vertices[i];
                newVerts[i] = new VertexC1P3T2
                {
                    Color = new ColorRgba(vtx.Color.R, vtx.Color.G, vtx.Color.B, vtx.Color.A),
                    Pos = new Vector3(MathF.Round(vtx.Position.X) + pixelOffset, MathF.Round(vtx.Position.Y) + pixelOffset, ZIndex),
                    TexCoord = new Vector2(vtx.TexCoords.X, vtx.TexCoords.Y)
                };
                if (!texinfo.isGenerated)
                {
                    newVerts[i].TexCoord = newVerts[i].TexCoord / texSize;
                }
            }

            return newVerts;
        }

        private VertexC1P3T2[] GetVertices(Vertex[] vertices, int[] indices, (ContentRef<Texture> tex, bool isGenerated) texinfo)
        {
            if (vertices.Length > _vtxBuffer.Length)
                _vtxBuffer = new VertexC1P3T2[indices.Length];
            float pixelOffset = MathF.RoundToInt(_device.TargetSize.X) != (MathF.RoundToInt(_device.TargetSize.X) / 2) * 2 ? 0.5f : 0f;
            var texSize = texinfo.tex.Res?.Size ?? Vector2.One;
            for (int i = 0; i < indices.Length; i++)
            {
                var vtx = vertices[indices[i]];
                var vtx2 = new VertexC1P3T2
                {
                    Color = new ColorRgba(vtx.Color.R, vtx.Color.G, vtx.Color.B, vtx.Color.A),
                    //Color = ColorRgba.Red,
                    Pos = new Vector3(MathF.Round(vtx.Position.X) + pixelOffset, MathF.Round(vtx.Position.Y) + pixelOffset, ZIndex),
                    TexCoord = new Vector2(vtx.TexCoords.X, vtx.TexCoords.Y)
                };
                if (!texinfo.isGenerated)
                {
                    vtx2.TexCoord = vtx2.TexCoord / texSize;
                }
                _vtxBuffer[i] = vtx2;
            }

            return _vtxBuffer;
        }

        protected override bool LoadTexture(ref IntPtr texture_handle, ref Vector2i texture_dimensions, string source)
        {
            var tex = ContentProvider.RequestContent<Texture>(source);
            if (!tex.IsAvailable)
                return false;
            var hash = (IntPtr)tex.Res.GetHashCode();
            _textures[hash] = (tex,false);
            texture_handle = hash;
            return true;
        }

        protected override void SetScissorRegion(int x, int y, int width, int height)
        {
            _scissorRegion = new Rect(x, y, width, height);
        }

        private void SetClipRect(BatchInfo batch)
        {
            var size = _device.TargetSize;
            var box = _scissorEnabled ? _scissorRegion : new Rect(0, 0, size.X, size.Y);
            //box.W /= 2;
            //box.H /= 2;

            var bl = new Vector3(box.LeftX, box.TopY, 0);
            var ur = new Vector3(box.RightX, box.BottomY, 0);
            batch.SetValue("targetSize", new Vector2(size.X,size.Y));
            batch.SetValue("clipRect", new Duality.Vector4(bl.X, bl.Y, ur.X, ur.Y));
            batch.SetValue("debug", _scissorEnabled ? 1.0f : 0.0f);

            /*
            if (_scissorEnabled)
            {
                var size = _device.TargetSize;

                var bl = new Vector3(_scissorRegion.LeftX, size.Y - _scissorRegion.BottomY, 0);
                var ur = new Vector3(_scissorRegion.RightX, size.Y - _scissorRegion.TopY, 0);

                batch.SetValue("clipRect", new Duality.Vector4(bl.X, bl.Y, ur.X, ur.Y));
            }
            else
            {
                var size = _device.TargetSize;
                batch.SetValue("clipRect", new Duality.Vector4(0, 0, size.X, size.Y));
            }
            */
        }

        protected override void RenderGeometry(Vertex[] vertices, int[] indices, IntPtr texture, Vector2f translation)
        {
            if (_device == null || Technique == null)
                return;

            var batchInfo = _device.RentMaterial();
            batchInfo.Technique = Technique;
            batchInfo.MainColor = ColorRgba.White;
            var texinfo = _textures[texture];
            if (texinfo.tex == null)
            {
                texinfo.tex = new Texture(new Pixmap(new PixelData(1, 1, ColorRgba.White)));
                _textures[texture] = texinfo;
            }
            
            batchInfo.MainTexture = texinfo.tex;
            batchInfo.SetValue("translation", new Duality.Vector2(translation.X, translation.Y));
            SetClipRect(batchInfo);
            _device.AddVertices(batchInfo, VertexMode.Triangles, GetVertices(vertices, indices, texinfo), indices.Length);
        }

        
    }
}
