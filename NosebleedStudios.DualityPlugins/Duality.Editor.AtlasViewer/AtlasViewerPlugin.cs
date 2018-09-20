using AdamsLair.WinForms.ItemModels;
using Duality.Editor.Forms;
using Duality.Editor.Properties;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Duality.Editor.AtlasViewer
{
    public class AtlasViewerPlugin : EditorPlugin
    {
        private AtlasViewer _atlasViewer;
        private bool _isLoading;

        public override string Id => "AtlasViewer";

        public static AtlasViewerPlugin Instance { get; private set; }

        protected override void InitPlugin(MainForm main)
        {
            base.InitPlugin(main);
            Instance = this;
            InitMenuItems(main);
        }

        private void InitMenuItems(MainForm main)
        {
            MenuModelItem viewItem = main.MainMenu.RequestItem(GeneralRes.MenuName_View);
            viewItem.AddItem(new MenuModelItem
            {
                Name = "Sprite Atlas Viewer",
                ActionHandler = (o, e) => RequestAtlasViewer()
            });
        }

        protected override IDockContent DeserializeDockContent(Type dockContentType)
        {
            _isLoading = true;
            IDockContent result;
            if (dockContentType == typeof(AtlasViewer))
                result = RequestAtlasViewer();
            else
                result = base.DeserializeDockContent(dockContentType);
            _isLoading = false;
            return result;
        }


        public IDockContent RequestAtlasViewer()
        {
            if (_atlasViewer == null || _atlasViewer.IsDisposed)
            {
                _atlasViewer = new AtlasViewer();
                _atlasViewer.FormClosed += delegate (object sender, FormClosedEventArgs e) { _atlasViewer = null; };
            }

            if (!_isLoading)
            {
                _atlasViewer.Show(DualityEditorApp.MainForm.MainDockPanel);
                if (_atlasViewer.Pane != null)
                {
                    _atlasViewer.Pane.Activate();
                    _atlasViewer.Focus();
                }
            }

            return _atlasViewer;
        }
    }
}
