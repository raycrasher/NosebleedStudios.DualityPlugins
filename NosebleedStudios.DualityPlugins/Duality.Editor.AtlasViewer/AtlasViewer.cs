using Duality;
using Duality.Editor;
using Duality.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Duality.Editor.AtlasViewer
{
    public partial class AtlasViewer : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private Bitmap _bitmap;
        private List<Button> _buttonCache = new List<Button>();
        private ContentRef<Pixmap> _pixmap;
        const int BtnSize = 64 + 4;
        private int _numVisible;

        protected int NumRows => flowPanel.ClientSize.Height / BtnSize;
        protected int NumCols => flowPanel.ClientSize.Width / BtnSize;

        public int NumSprites => _pixmap.Res?.Atlas?.Count ?? 0;

        public AtlasViewer()
        {
            InitializeComponent();
            virtScroll.Scroll += OnScrollBarScroll;
            this.AllowDrop = true;
            RefreshScrollBar();
            flowPanel.MouseWheel += OnFPMouseWheel;
        }

        private void OnFPMouseWheel(object sender, MouseEventArgs e)
        {
            int value = virtScroll.Value - Math.Sign(e.Delta);
            if (value == virtScroll.Value)
                return;
            else if (value < 0)
                virtScroll.Value = 0;
            else if (value > virtScroll.Maximum)
                virtScroll.Value = virtScroll.Maximum;
            else
                virtScroll.Value = value;
            flowPanel.SuspendLayout();
            flowPanel.Invalidate(true);
            flowPanel.ResumeLayout();
        }

        private void RefreshScrollBar()
        {
            virtScroll.Minimum = 0;
            virtScroll.Maximum = NumSprites / (NumCols <= 0 ? 10 : NumCols) + NumRows + 1;
        }

        private void OnScrollBarScroll(object sender, ScrollEventArgs e)
        {
            flowPanel.SuspendLayout();
            foreach (var btn in _buttonCache)
                btn.Visible = true;
            flowPanel.Invalidate(true);
            flowPanel.ResumeLayout();
            
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshScrollBar();
            DoScroll();
            Invalidate();
        }

        protected void DoScroll()
        {
            flowPanel.SuspendLayout();
            var numControlsPerPage = NumCols * (NumRows+1);
            foreach (var btn in _buttonCache)
                btn.Visible = true;

            if (_buttonCache.Count < numControlsPerPage)
            {
                
                
                for(int i = 0; i < _buttonCache.Count; i++)
                {
                    _buttonCache[i].Invalidate();
                }
                List<Button> added = new List<Button>();
                for(int i=_buttonCache.Count; i < numControlsPerPage; i++)
                {
                    var btn = new Button() { Tag = i, Width=64, Height=64, Margin=new Padding(2) };
                    btn.Paint += OnButtonPaint;
                    _buttonCache.Add(btn);
                    added.Add(btn);
                    //flowPanel.Controls.Add(btn);
                    btn.MouseDown += (o, e) => DoSpriteDragDrop(((int)((Control)o).Tag) + (virtScroll.Value * NumCols));
                }
                flowPanel.Controls.AddRange(added.ToArray());
            }
            else if(_buttonCache.Count > numControlsPerPage) {
                for (int i = numControlsPerPage; i < _buttonCache.Count; i++)
                    _buttonCache[i].Visible = false;
            }
            _numVisible = numControlsPerPage;
            flowPanel.Invalidate(true);
            flowPanel.ResumeLayout();
            
        }

        private void DoSpriteDragDrop(int i)
        {
            if(_pixmap.IsAvailable && _pixmap.Res.Atlas!=null && _pixmap.Res.Atlas.Count > i)
                this.DoDragDrop(new AtlasSprite(_pixmap.Res, i), DragDropEffects.Link);
        }

        private void OnButtonPaint(object sender, PaintEventArgs e)
        {
            var btn = (Button)sender;
            var idx = (int)btn.Tag;
            idx += virtScroll.Value * NumCols;

            if (_pixmap.IsAvailable && _pixmap.Res.Atlas!=null && _bitmap!=null)
            {
                var atlas = _pixmap.Res.Atlas;
                if (idx >= atlas.Count)
                {
                    btn.Visible = false;
                    return;
                }
                var rect = atlas[idx];
                int x = btn.Width / 2 - (int)rect.W / 2;
                int y = btn.Height / 2 - (int)rect.H / 2;
                e.Graphics.DrawImage(_bitmap, x, y, new RectangleF(rect.X, rect.Y, rect.W, rect.H), GraphicsUnit.Pixel);
            }
            else
            {
                btn.Visible = false;
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            DataObject data = e.Data as DataObject;
            var op = new ConvertOperation(data, ConvertOperation.Operation.All);
            if (op.CanPerform<Pixmap>())
                e.Effect = e.AllowedEffect;
            else
                e.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            DataObject data = e.Data as DataObject;
            var dragObjQuery = new ConvertOperation(data, ConvertOperation.Operation.All).Perform<Pixmap>();
            if (dragObjQuery != null)
            {
                Pixmap pixmap = dragObjQuery.FirstOrDefault();
                if (_pixmap != pixmap && pixmap.Atlas != null && pixmap.Atlas.Count > 0 && pixmap.MainLayer != null && pixmap.Width > 0 && pixmap.Height > 0)
                {
                    _pixmap = pixmap;
                    virtScroll.Value = 0;
                    RefreshScrollBar();
                    RefreshDisplay(pixmap);
                }
                e.Effect = e.AllowedEffect;
            }
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _bitmap?.Dispose();
        }

        private void RefreshDisplay(Pixmap pixmap)
        {
            this.Text = "Atlas Viewer: " + pixmap.FullName;
            if (_bitmap != null)
                _bitmap.Dispose();

            _bitmap = pixmap.MainLayer.ToBitmap();
            DoScroll();
        }
    }
}
