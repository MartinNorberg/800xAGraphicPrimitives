/*
Copyright (c) 2019, Stefan Berndt

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or 
   other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using AfwDynamicGraphics;
using AfwExpressionHandling;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Globalization;
using AfwDynamicGraphics.Media;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Media.Imaging;
using System.IO;

namespace PG2VNCViewer
{
    [PrimitiveItemAttribute("{CCCDCDCC-DD43-4e95-823A-577466EB2BF0}", "Custom:VNCViewer", "VNCViewer", "Custom Controls", "Display other computer's screen by using VNC")]
    public partial class PG2VNCViewer : FrameItem, ITimerUpdateable
    {
        // additional properties
        private static readonly PropertyDesc[] myprops = new PropertyDesc[]
        {
            new PropertyDesc("Hostname", StringType.Singleton, 17, "Name or IP of the remote host", "Connection"),
            new PropertyDesc("Hostport", IntegerType.Singleton, 18, "Port number of the remote host", "Connection"),
            new PropertyDesc("Password", StringType.Singleton, 19, "Password (required)", "Connection"),
            new PropertyDesc("Viewonly", BooleanType.Singleton, 20, "No interaction with remote if true", "Connection"),
            new PropertyDesc("Stretchdraw", BooleanType.Singleton, 21, "Stretch the view to fit remote screen size", "Appearance"),
            new PropertyDesc("Connect", BooleanType.Singleton, 22, "Connect to server if true, Disconnect if false", "Connection"),
            new PropertyDesc("Loglevel", IntegerType.Singleton, 23, "Display Messages (-1=clear, 0=none, 1=error, 2=info, 3=debug)", "Appearance"),
            new PropertyDesc("Shared", BooleanType.Singleton, 24, "Allow other clients to connect to this screen too. (Ignored on some VNC servers)", "Connection"),
            new PropertyDesc("Hideremotemouse", BooleanType.Singleton, 25, "Hide remote mouse cursor for better performance.", "Appearance"),
            new PropertyDesc("Keyconvert", StringType.Singleton, 26, "Convertion Table between local and remote keyboard.", "Connection"),

            new PropertyDesc("KeySim1Trigger", BooleanType.Singleton, 27, "Trigger for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("KeySim1Keycodes", IntegerArrayType.Singleton, 28, "Keycode(s) for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("KeySim2Trigger", BooleanType.Singleton, 29, "Trigger for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("KeySim2Keycodes", IntegerArrayType.Singleton, 30, "Keycode(s) for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("KeySim3Trigger", BooleanType.Singleton, 31, "Trigger for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("KeySim3Keycodes", IntegerArrayType.Singleton, 32, "Keycode(s) for Keyboard event simulation.", "Simulation"),
            new PropertyDesc("MouseSim1Trigger", BooleanType.Singleton, 33, "Trigger for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim1X", IntegerType.Singleton, 34, "X-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim1Y", IntegerType.Singleton, 35, "Y-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim1Button", IntegerType.Singleton, 36, "Button for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim2Trigger", BooleanType.Singleton, 37, "Trigger for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim2X", IntegerType.Singleton, 38, "X-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim2Y", IntegerType.Singleton, 39, "Y-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim2Button", IntegerType.Singleton, 40, "Button for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim3Trigger", BooleanType.Singleton, 41, "Trigger for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim3X", IntegerType.Singleton, 42, "X-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim3Y", IntegerType.Singleton, 43, "Y-Position for Mouse event simulation.", "Simulation"),
            new PropertyDesc("MouseSim3Button", IntegerType.Singleton, 44, "Button for Mouse event simulation.", "Simulation"),
        };

        // all properties
        private static readonly PropertyDesc[] allprops = new PropertyDesc[PG2VNCViewer.myprops.Length + FrameItem.FiGetNumberOfProps(true, true, true, true, false)];

        // instance variables
        private IElementView view;
        private DrawingVisual visual;
        private WriteableBitmap image = null;
        private WPFFont font;
        private bool hasKeyFocus = false;
        private bool hasMouseFocus = false;
        private bool invalid = true;
        private bool terminate = false;

        // property variables
        private string host = "";
        private long port = 5900;
        private string pass = "";
        private bool viewonly = false;
        private bool stretch = false;
        private bool autoconnect = false;
        private bool connect = false;
        private bool shared = false;
        private bool hideRemoteCursor = false;
        private string keyconvert = "";
        private bool KeySim1Trigger = false;
        private long[] KeySim1Keycodes = new long[0];
        private bool KeySim2Trigger = false;
        private long[] KeySim2Keycodes = new long[0];
        private bool KeySim3Trigger = false;
        private long[] KeySim3Keycodes = new long[0];
        private bool MouseSim1Trigger = false;
        private long MouseSim1X = 0;
        private long MouseSim1Y = 0;
        private long MouseSim1Button = 0;
        private bool MouseSim2Trigger = false;
        private long MouseSim2X = 0;
        private long MouseSim2Y = 0;
        private long MouseSim2Button = 0;
        private bool MouseSim3Trigger = false;
        private long MouseSim3X = 0;
        private long MouseSim3Y = 0;
        private long MouseSim3Button = 0;

        // error logging
        private Logger Log = new Logger();

        // static constructor
        static PG2VNCViewer()
        {
            int n = 0;
            FrameItem.FiFillInPropertyDescriptions(allprops, ref n, true, true, true, true, false);
            foreach (PropertyDesc propertyDesc in myprops)
            {
                allprops[n++] = propertyDesc;
            }
        }

        // another one constructor
        public PG2VNCViewer()
            : base(true, true, true)
        {
        }

        // constructor for runtime instance
        public PG2VNCViewer(PG2VNCViewer other, GraphicItemVisual otherVisual)
            : base(other, otherVisual)
        {
            Log.loglevel = other.Log.loglevel;
            host = other.host;
            port = other.port;
            pass = other.pass;
            viewonly = other.viewonly;
            stretch = other.stretch;
            autoconnect = other.autoconnect;
            connect = other.connect;
            hideRemoteCursor = other.hideRemoteCursor;
            keyconvert = other.keyconvert;
            if (keyconvert != "")
            {
                keytable.parseConvertion(keyconvert, Log);
            }
            KeySim1Keycodes = other.KeySim1Keycodes;
            KeySim2Keycodes = other.KeySim2Keycodes;
            KeySim3Keycodes = other.KeySim3Keycodes;
            MouseSim1X = other.MouseSim1X;
            MouseSim1Y = other.MouseSim1Y;
            MouseSim1Button = other.MouseSim1Button;
            MouseSim2X = other.MouseSim2X;
            MouseSim2Y = other.MouseSim2Y;
            MouseSim2Button = other.MouseSim2Button;
            MouseSim3X = other.MouseSim3X;
            MouseSim3Y = other.MouseSim3Y;
            MouseSim3Button = other.MouseSim3Button;
        }

        // returns a new instance
        public override GraphicItem GetRunTimeInstance(IElementView elementView, GraphicItemVisual visual)
        {
            return new PG2VNCViewer(this, visual);
        }

        protected override Size GetDefaultSize()
        {
            return new Size(640.0, 480.0);
        }

        protected override PropertyDesc[] GetPropertyDescriptions()
        {
            return allprops;
        }

        // called if property is changed
        protected override void TransferValue(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation)
        {
            switch (propertyIndex)
            {
                case 17:    // property "Hostname"
                    accessor.TransferString(writeOperation, accessIndex, ref host);
                    if (writeOperation) Log.Debug("Set Host=" + host);
                    break;
                case 18:    // property "Hostport"
                    accessor.TransferInteger(writeOperation, accessIndex, ref port);
                    if (writeOperation) Log.Debug("Set Port=" + port);
                    break;
                case 19:    // property "Password"
                    accessor.TransferString(writeOperation, accessIndex, ref pass);
                    if (writeOperation) Log.Debug("Set Password");
                    break;
                case 20:    // property "Viewonly"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref viewonly);
                    if (writeOperation) Log.Debug("Set Viewonly=" + viewonly);
                    break;
                case 21:    // property "Stretchdraw"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref stretch);
                    if (writeOperation) Log.Debug("Set Stretchdraw=" + stretch);
                    break;
                case 22:    // property "Autoconnect"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref autoconnect);
                    if (writeOperation) Log.Debug("Set Autoconnect=" + autoconnect);
                    if (!autoconnect && rfb != null) rfb.Close();
                    break;
                case 23:    // property "Loglevel"
                    long lvl = Log.loglevel;
                    accessor.TransferInteger(writeOperation, accessIndex, ref Log.loglevel);
                    if (writeOperation)
                    {
                        Log.Debug("Set Loglevel=" + Log.loglevel);
                        switch (Log.loglevel)
                        {
                            case -1:
                                lock (Log.loglist)
                                {
                                    Log.loglist.Clear();
                                    Log.invalid = true;
                                }
                                Log.loglevel = lvl;
                                break;
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                break;
                            default:
                                lvl = Log.loglevel;
                                Log.loglevel = 1;
                                Log.Error("Wrong Loglevel: " + lvl + ". Valid values are -1, 0, 1, 2, 3");
                                break;
                        }
                    }
                    break;
                case 24:    // property "Shared"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref shared);
                    if (writeOperation) Log.Debug("Set Shared=" + shared);
                    break;
                case 25:    // property "Hideremotemouse"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref hideRemoteCursor);
                    if (writeOperation) Log.Debug("Set Hideremotemouse=" + hideRemoteCursor);
                    break;
                case 26:    // property "Keyconvert"
                    accessor.TransferString(writeOperation, accessIndex, ref keyconvert);
                    if (writeOperation)
                    {
                        Log.Debug("Set Keyconvert=\"" + keyconvert + "\"");
                        keytable.init();
                        if (keyconvert != "")
                        {
                            keytable.parseConvertion(keyconvert, Log);
                        }
                    }
                    break;
                case 27:    // property "KeySim1Trigger"
                    simKey(accessor, accessIndex, propertyIndex, writeOperation, KeySim1Keycodes, ref KeySim1Trigger);
                    break;
                case 28:    // property "KeySim1Keycodes"
                    object o = KeySim1Keycodes;
                    accessor.TransferObject(writeOperation, accessIndex, ref o);
                    KeySim1Keycodes = (long[])o;
                    if (writeOperation) Log.Debug("Set KeySim1Keycodes");
                    break;
                case 29:    // property "KeySim2Trigger"
                    simKey(accessor, accessIndex, propertyIndex, writeOperation, KeySim2Keycodes, ref KeySim2Trigger);
                    break;
                case 30:    // property "KeySim2Keycodes"
                    o = KeySim2Keycodes;
                    accessor.TransferObject(writeOperation, accessIndex, ref o);
                    KeySim2Keycodes = (long[])o;
                    if (writeOperation) Log.Debug("Set KeySim2Keycodes");
                    break;
                case 31:    // property "KeySim3Trigger"
                    simKey(accessor, accessIndex, propertyIndex, writeOperation, KeySim3Keycodes, ref KeySim3Trigger);
                    break;
                case 32:    // property "KeySim3Keycodes"
                    o = KeySim3Keycodes;
                    accessor.TransferObject(writeOperation, accessIndex, ref o);
                    if (writeOperation) Log.Debug("Set KeySim3Keycodes");
                    KeySim3Keycodes = (long[])o;
                    break;
                case 33:    // property "MouseSim1Trigger"
                    simMouse(accessor, accessIndex, propertyIndex, writeOperation, ref MouseSim1Trigger, MouseSim1X, MouseSim1Y, MouseSim1Button);
                    break;
                case 34:    // property "MouseSim1X"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim1X);
                    if (writeOperation) Log.Debug("Set MouseSim1X=" + MouseSim1X);
                    break;
                case 35:    // property "MouseSim1Y"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim1Y);
                    if (writeOperation) Log.Debug("Set MouseSim1Y=" + MouseSim1Y);
                    break;
                case 36:    // property "MouseSim1Button"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim1Button);
                    if (writeOperation) Log.Debug("Set MouseSim1Button=" + MouseSim1Button);
                    break;
                case 37:    // property "MouseSim2Trigger"
                    simMouse(accessor, accessIndex, propertyIndex, writeOperation, ref MouseSim2Trigger, MouseSim2X, MouseSim2Y, MouseSim2Button);
                    break;
                case 38:    // property "MouseSim2X"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim2X);
                    if (writeOperation) Log.Debug("Set MouseSim2X=" + MouseSim2X);
                    break;
                case 39:    // property "MouseSim2Y"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim2Y);
                    if (writeOperation) Log.Debug("Set MouseSim2Y=" + MouseSim2Y);
                    break;
                case 40:    // property "MouseSim2Button"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim2Button);
                    if (writeOperation) Log.Debug("Set MouseSim2Button=" + MouseSim2Button);
                    break;
                case 41:    // property "MouseSim3Trigger"
                    simMouse(accessor, accessIndex, propertyIndex, writeOperation, ref MouseSim3Trigger, MouseSim3X, MouseSim3Y, MouseSim3Button);
                    break;
                case 42:    // property "MouseSim3X"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim3X);
                    if (writeOperation) Log.Debug("Set MouseSim3X=" + MouseSim3X);
                    break;
                case 43:    // property "MouseSim3Y"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim3Y);
                    if (writeOperation) Log.Debug("Set MouseSim3Y=" + MouseSim3Y);
                    break;
                case 44:    // property "MouseSim3Button"
                    accessor.TransferInteger(writeOperation, accessIndex, ref MouseSim3Button);
                    if (writeOperation) Log.Debug("Set MouseSim3Button=" + MouseSim3Button);
                    break;
                default:    // anything inherited
                    base.TransferValue(accessor, accessIndex, propertyIndex, writeOperation);
                    break;
            }
        }

        // called for init
        protected override void InitVisual(IElementView elementView, ulong noValueEffects)
        {
            Log.Debug("InitVisual");
            view = elementView;
            visual = new GraphicItemVisual(this);
            ItemDrawingVisual.Children.Add(visual);
            view.Viewer.PreviewKeyDown += new KeyEventHandler(Viewer_PreviewKeyDown);
            view.Viewer.PreviewKeyUp += new KeyEventHandler(Viewer_PreviewKeyUp);
            view.Viewer.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(Viewer_LostKeyboardFocus);
            view.Viewer.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Viewer_GotKeyboardFocus);
            font = new LogicalFont("Arial", 18.0, AfwDynamicGraphics.Media.FontStyle.Regular).GetFont(view);
            hasKeyFocus = view.Viewer.IsKeyboardFocusWithin;
            new Thread(RunNetThread).Start();
            DrawItem();
        }

        // called for exit
        public override void PrepareForRemoval(IElementView elementV, int itemIndex)
        {
            terminate = true;
            if (rfb != null) rfb.Close();
            base.PrepareForRemoval(elementV, itemIndex);
        }

        // called if update required
        protected override void UpdateVisual(IElementView elementView, ulong updateReason, ulong noValueEffects)
        {
            DrawItem();
        }

        // called cyclically
        public void OnTimerUpdate(IElementView elementView)
        {
            if (invalid)
            {
                DrawItem();
            }
        }

        // draw all
        private void DrawItem()
        {
            if (view == null) return;
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                // draw frame
                if (hasKeyFocus && hasMouseFocus && !viewonly)
                {
                    DrawFrame(drawingContext, view, true);
                }

                // draw vnc screen
                if (buffer != null && buffer.screen != null)
                {
                    int wi = Math.Min(buffer.Width, (int)ClientArea.Width);
                    int hi = Math.Min(buffer.Height, (int)ClientArea.Height);
                    int wa = (int)ClientArea.Width;
                    int ha = (int)ClientArea.Height;
                    if (stretch)
                    {
                        wi = buffer.Width;
                        wa = buffer.Width;
                        hi = buffer.Height;
                        ha = buffer.Height;
                    }
                    if (image == null || image.Height != ha || image.Width != wa)
                    {
                        image = new WriteableBitmap(wa, ha, (double)96, (double)96, PixelFormats.Bgr32, null);
                    }
                    image.WritePixels(new Int32Rect(0, 0, wi, hi), buffer.screen, buffer.Width * 4, 0);
                    drawingContext.DrawImage(image, ClientArea);
                }
                else
                {
                    drawingContext.DrawRectangle(fillColor.GetBrush(view), new System.Windows.Media.Pen(Brushes.Black, 1), ClientArea);
                }

                // draw log messages
                if (Log.loglevel > 0)
                {
                    lock (Log.loglist)
                    {
                        for (int i=0; i<Log.loglist.Count; i++)
                        {
                            FormattedText fText = new FormattedText(Log.loglist[Log.loglist.Count-i-1], CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, font.Typeface, 12, Brushes.Red);
                            drawingContext.DrawText(fText, new Point(5, i*15));
                        }
                    }
                }
            }
            invalid = false;
        }

        // mouse handling - 800xA side
        public override bool IsMouseEventConsumer(IElementView elementView)
        {
            return true;
        }

        public override void Enter(MouseEventContext mouseEventContext, IElementView elementView)
        {
            Log.Debug("Mouse Enter");
            hasMouseFocus = true;
            invalid = true;
            base.Enter(mouseEventContext, elementView);
        }

        public override void Leave(MouseEventContext mouseEventContext, IElementView elementView)
        {
            Log.Debug("Mouse Leave");
            hasMouseFocus = false;
            invalid = true;
            base.Leave(mouseEventContext, elementView);
        }

        public override void MouseDown(MouseEventContext mouseEventContext, IElementView elementView)
        {
            MouseUpdate(mouseEventContext, true);
            base.MouseDown(mouseEventContext, elementView);
        }

        public override void MouseMove(MouseEventContext mouseEventContext, IElementView elementView)
        {
            MouseUpdate(mouseEventContext, false);
            base.MouseMove(mouseEventContext, elementView);
        }

        public override void MouseUp(MouseEventContext mouseEventContext, IElementView elementView)
        {
            MouseUpdate(mouseEventContext, true);
            base.MouseUp(mouseEventContext, elementView);
        }

        public override void MouseWheel(MouseEventContext mouseEventContext, IElementView elementView)
        {
            base.MouseWheel(mouseEventContext, elementView);
        }

        private void simMouse(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation, ref bool mouseSimTrigger, long mouseSimX, long mouseSimY, long mouseSimButton)
        {
            bool old = mouseSimTrigger;
            accessor.TransferBoolean(writeOperation, accessIndex, ref mouseSimTrigger);
            if (writeOperation)
            {
                if (old != mouseSimTrigger && rfb != null)
                {
                    long btn = mouseSimTrigger ? (mouseSimButton & 7) : 0;
                    Log.Debug("Simulated mouse event sent: x=" + mouseSimX + " y=" + mouseSimY + " Btn=" + btn);
                    MouseUpdate((byte)btn, (int)mouseSimX, (int)mouseSimY);
                }
            }
        }

        // keyboard handling - 800xA side
        void Viewer_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Log.Debug("Got Keyboard Focus");
            hasKeyFocus = true;
            invalid = true;
        }

        void Viewer_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Log.Debug("Lost Keyboard Focus");
            hasKeyFocus = false;
            invalid = true;
        }

        void Viewer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (hasKeyFocus && hasMouseFocus && !viewonly)
            {
                WriteKey(e.Key, false, true);
            }
        }

        void Viewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (hasKeyFocus && hasMouseFocus && !viewonly)
            {
                WriteKey(e.Key, true, true);
            }
        }

        void simKey(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation, long[] keySimKeycodes, ref bool keySimTrigger)
        {
            bool old = keySimTrigger;
            accessor.TransferBoolean(writeOperation, accessIndex, ref keySimTrigger);
            if (writeOperation)
            {
                if (keySimKeycodes.Length > 0 && old != keySimTrigger)
                {
                    foreach (long key in keySimKeycodes)
                    {
                        WriteKey((uint)key, keySimTrigger);
                        Log.Debug("Simulated key " + (keySimTrigger ? "press" : "release") + " sent: Code=" + key);
                    }
                }
            }
        }

    }
}
