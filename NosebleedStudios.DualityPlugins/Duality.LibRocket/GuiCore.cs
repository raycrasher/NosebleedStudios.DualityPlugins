using Duality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibRocketNet;
using Duality.Input;
using Duality.Drawing;
using Duality.Resources;
using System.Xml.Linq;
using System.IO;

namespace Duality.LibRocket
{
    public class GuiCore
    {
        public LibRocketSystemInterface SystemInterface { get; private set; }
        public LibRocketRendererInterface RendererInterface { get; private set; }
        public Context CoreContext { get; private set; }

        public static GuiCore Instance => LibRocketCorePlugin.GuiCore;

        private int _lastRenderFrame = -1;

        internal void Initialize()
        {
            SystemInterface = new LibRocketSystemInterface();
            RendererInterface = new LibRocketRendererInterface();
            LibRocketNet.Core.SystemInterface = SystemInterface;
            LibRocketNet.Core.RenderInterface = RendererInterface;

            LibRocketNet.Core.Initialize();
            CoreContext = LibRocketNet.Core.CreateContext("DualityMainContext", new Vector2i(1000, 1000), RendererInterface);
            Core.InitDebugger(CoreContext);

            LoadFontFaces();

            DualityApp.Keyboard.KeyDown += OnKeyDown;
            DualityApp.Keyboard.KeyUp += OnKeyUp;
            DualityApp.Mouse.ButtonDown += OnButtonDown;
            DualityApp.Mouse.ButtonUp += OnButtonUp;
            DualityApp.Mouse.Move += OnMouseMove;

            //Core.ScriptEvent += OnScriptEvent;
        }

        private void LoadFontFaces()
        {
            try
            {
                if (File.Exists("librocket.xml"))
                {
                    var doc = XDocument.Load("librocket.xml");
                    doc?.Element("librocket")?.Element("fonts")?.Elements("font")?.Select(e =>
                            (path: e.Attribute("path")?.Value,
                             family: e.Attribute("family")?.Value,
                             style: e.Attribute("style")?.Value == "italic" ? LibRocketNet.FontStyle.Italic : LibRocketNet.FontStyle.Normal,
                             weight: e.Attribute("weight")?.Value == "bold" ? LibRocketNet.FontWeight.Bold : LibRocketNet.FontWeight.Normal))
                        ?.Where(e =>
                            !string.IsNullOrWhiteSpace(e.path) && File.Exists(e.path) &&
                            !string.IsNullOrWhiteSpace(e.family))
                        ?.ToList()
                        ?.ForEach(font => LibRocketNet.Core.LoadFontFace(font.path, font.family, font.style, font.weight));
                }                
            }
            catch (Exception ex)
            {
                Logs.Game.WriteError("Error loading fonts from librocket.xml");
                Logs.Game.WriteError(ex.ToString());
            }
        }

        public void Update()
        {            
            if (CoreContext != null)
            {
                foreach (var ch in DualityApp.Keyboard.CharInput)
                    CoreContext.ProcessTextInput(ch);
                CoreContext.Update();
            }            
        }

        

        public void Draw(IDrawDevice device, DrawTechnique technique)
        {
            if (CoreContext != null && _lastRenderFrame != Time.FrameCount)
            {
                RendererInterface.Device = device;
                RendererInterface.Technique = technique;

                if (CoreContext.Dimensions.X != (int)device.TargetSize.X ||
                    CoreContext.Dimensions.Y != (int)device.TargetSize.Y
                    )
                    CoreContext.Dimensions = new Vector2i((int)device.TargetSize.X, (int)device.TargetSize.Y);

                CoreContext.Render();
                _lastRenderFrame = Time.FrameCount;
            }
        }

        internal void Shutdown()
        {
            DualityApp.Keyboard.KeyDown -= OnKeyDown;
            DualityApp.Keyboard.KeyUp -= OnKeyUp;
            DualityApp.Mouse.ButtonDown -= OnButtonDown;
            DualityApp.Mouse.ButtonUp -= OnButtonUp;
            DualityApp.Mouse.Move -= OnMouseMove;
            CoreContext?.UnloadAllDocuments();
            CoreContext?.Dispose();
            CoreContext = null;
            LibRocketNet.Core.Shutdown();
        }


        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            CoreContext?.ProcessMouseMove((int)e.Pos.X, (int)e.Pos.Y, GetKeyModifiers());
        }

        private void OnButtonUp(object sender, MouseButtonEventArgs e)
        {
            CoreContext?.ProcessMouseButtonUp((int)e.Button, GetKeyModifiers());
        }

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            CoreContext?.ProcessMouseButtonDown((int)e.Button, GetKeyModifiers());
        }

        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            CoreContext?.ProcessKeyUp(TranslateSpecialKey(e.Key), GetKeyModifiers());
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F8)
                LibRocketNet.Core.DebugMode = !LibRocketNet.Core.DebugMode;

            CoreContext?.ProcessKeyDown(TranslateSpecialKey(e.Key), GetKeyModifiers());
        }

        private KeyIdentifiers TranslateSpecialKey(Key key)
        {
            switch (key)
            {
                case Key.A: return KeyIdentifiers.A;
                case Key.B: return KeyIdentifiers.B;
                case Key.C: return KeyIdentifiers.C;
                case Key.D: return KeyIdentifiers.D;
                case Key.E: return KeyIdentifiers.E;
                case Key.F: return KeyIdentifiers.F;
                case Key.G: return KeyIdentifiers.G;
                case Key.H: return KeyIdentifiers.H;
                case Key.I: return KeyIdentifiers.I;
                case Key.J: return KeyIdentifiers.J;
                case Key.K: return KeyIdentifiers.K;
                case Key.L: return KeyIdentifiers.L;
                case Key.M: return KeyIdentifiers.M;
                case Key.N: return KeyIdentifiers.N;
                case Key.O: return KeyIdentifiers.O;
                case Key.P: return KeyIdentifiers.P;
                case Key.Q: return KeyIdentifiers.Q;
                case Key.R: return KeyIdentifiers.R;
                case Key.S: return KeyIdentifiers.S;
                case Key.T: return KeyIdentifiers.T;
                case Key.U: return KeyIdentifiers.U;
                case Key.V: return KeyIdentifiers.V;
                case Key.W: return KeyIdentifiers.W;
                case Key.X: return KeyIdentifiers.X;
                case Key.Y: return KeyIdentifiers.Y;
                case Key.Z: return KeyIdentifiers.Z;
                case Key.Number0: return KeyIdentifiers.Num0;
                case Key.Number1: return KeyIdentifiers.Num1;
                case Key.Number2: return KeyIdentifiers.Num2;
                case Key.Number3: return KeyIdentifiers.Num3;
                case Key.Number4: return KeyIdentifiers.Num4;
                case Key.Number5: return KeyIdentifiers.Num5;
                case Key.Number6: return KeyIdentifiers.Num6;
                case Key.Number7: return KeyIdentifiers.Num7;
                case Key.Number8: return KeyIdentifiers.Num8;
                case Key.Number9: return KeyIdentifiers.Num9;
                case Key.Keypad0: return KeyIdentifiers.Numpad0;
                case Key.Keypad1: return KeyIdentifiers.Numpad1;
                case Key.Keypad2: return KeyIdentifiers.Numpad2;
                case Key.Keypad3: return KeyIdentifiers.Numpad3;
                case Key.Keypad4: return KeyIdentifiers.Numpad4;
                case Key.Keypad5: return KeyIdentifiers.Numpad5;
                case Key.Keypad6: return KeyIdentifiers.Numpad6;
                case Key.Keypad7: return KeyIdentifiers.Numpad7;
                case Key.Keypad8: return KeyIdentifiers.Numpad8;
                case Key.Keypad9: return KeyIdentifiers.Numpad9;
                case Key.Up: return KeyIdentifiers.Up;
                case Key.Down: return KeyIdentifiers.Down;
                case Key.Left: return KeyIdentifiers.Left;
                case Key.Right: return KeyIdentifiers.Right;

                case Key.PageUp: return KeyIdentifiers.PageUp;
                case Key.PageDown: return KeyIdentifiers.PageDown;

                case Key.Space: return KeyIdentifiers.Space;
                case Key.Enter: return KeyIdentifiers.Return;

                case Key.Home: return KeyIdentifiers.Home;
                case Key.End: return KeyIdentifiers.End;
                case Key.Insert: return KeyIdentifiers.Insert;
                case Key.Delete: return KeyIdentifiers.Delete;

                case Key.ShiftLeft: return KeyIdentifiers.LShift;
                case Key.ShiftRight: return KeyIdentifiers.RShift;
                case Key.ControlLeft: return KeyIdentifiers.LControl;
                case Key.ControlRight: return KeyIdentifiers.RControl;
                case Key.Escape: return KeyIdentifiers.Escape;

                case Key.Plus: return KeyIdentifiers.OemPlus;
                case Key.Minus: return KeyIdentifiers.OemMinus;
                case Key.KeypadAdd: return KeyIdentifiers.Add;
                case Key.KeypadSubtract: return KeyIdentifiers.Subtract;
                case Key.KeypadMultiply: return KeyIdentifiers.Multiply;
                case Key.KeypadDivide: return KeyIdentifiers.Divide;
                case Key.KeypadDecimal: return KeyIdentifiers.Decimal;

                case Key.Comma: return KeyIdentifiers.OemComma;
                case Key.Period: return KeyIdentifiers.OemPeriod;
                case Key.BackSpace: return KeyIdentifiers.Back;
                case Key.BackSlash: return KeyIdentifiers.Divide;

                case Key.F1: return KeyIdentifiers.F1;
                case Key.F2: return KeyIdentifiers.F2;
                case Key.F3: return KeyIdentifiers.F3;
                case Key.F4: return KeyIdentifiers.F4;
                case Key.F5: return KeyIdentifiers.F5;
                case Key.F6: return KeyIdentifiers.F6;
                case Key.F7: return KeyIdentifiers.F7;
                case Key.F8: return KeyIdentifiers.F8;
                case Key.F9: return KeyIdentifiers.F9;
                case Key.F10: return KeyIdentifiers.F10;
                case Key.F11: return KeyIdentifiers.F11;
                case Key.F12: return KeyIdentifiers.F12;
                case Key.F13: return KeyIdentifiers.F13;
                case Key.F14: return KeyIdentifiers.F14;
                case Key.F15: return KeyIdentifiers.F15;

                case Key.Tab: return KeyIdentifiers.Tab;


                default:
                    return KeyIdentifiers.Unknown;
            }
        }

        private KeyModifier GetKeyModifiers()
        {
            KeyModifier modifiers = 0;
            var kbState = DualityApp.Keyboard;

            if (kbState.KeyPressed(Key.ShiftLeft) ||
                kbState.KeyPressed(Key.ShiftRight))
                modifiers |= KeyModifier.Shift;

            if (kbState.KeyPressed(Key.ControlLeft) ||
                kbState.KeyPressed(Key.ControlRight))
                modifiers |= KeyModifier.Control;

            if (kbState.KeyPressed(Key.AltLeft) ||
                kbState.KeyPressed(Key.AltRight))
                modifiers |= KeyModifier.Alt;

            return modifiers;
        }
        
    }
}
