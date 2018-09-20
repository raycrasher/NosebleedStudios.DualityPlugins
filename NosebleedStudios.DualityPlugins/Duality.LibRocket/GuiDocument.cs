using Duality;
using Duality.Components;
using Duality.Drawing;
using Duality.Editor;
using LibRocketNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duality.LibRocket
{
    [EditorHintCategory("LibRocket")]
    public class GuiDocument : Component, ICmpInitializable, ICmpUpdatable
    {
        [DontSerialize]
        private ElementDocument _document;
        private string _filename;
        [DontSerialize]
        private bool _needsReload=false;

        public string Filename {
            get => _filename;
            set
            {                
                if (_filename != value)
                {
                    _filename = value;
                    _needsReload = true;
                }
            }
        }

        public bool EditorReload
        {
            get => false;
            set
            {
                if(value == true)
                {
                    _needsReload = true;
                }
            }
        }
        
        [EditorHintFlags(MemberFlags.Invisible)]
        public ElementDocument Document { get => _document; }

        public void Reload()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                return;
            if (File.Exists(Filename)) {
                var isShown = _document?.IsVisible ?? true;
                if(_document != null)
                {
                    _document.Close();
                    _document.Dispose();
                }
                _document = LibRocketCorePlugin.GuiCore?.CoreContext?.LoadDocument(Filename);
                _document?.Show();
            }
        }

        void ICmpInitializable.OnActivate()
        {
            if(_document==null) Reload();
            _document?.Show();
        }

        void ICmpInitializable.OnDeactivate()
        {
            _document?.Hide();
        }

        void ICmpUpdatable.OnUpdate()
        {
            if (_needsReload)
            {
                Reload();
                _needsReload = false;
            }
        }
    }
}
