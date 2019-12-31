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
using System.Windows.Input;
using AfwDynamicGraphics;

namespace PG2VNCViewer
{
    public class KeyTable
    {
        public KeyTable()
        {
            init();
        }

        public Dictionary<Key, uint> NomodKeys;
        public Dictionary<Key, uint> ShiftKeys;
        public Dictionary<Key, uint> AltCtlKeys;

        public void init()
        {
            NomodKeys = new Dictionary<Key, uint>()
            { 
                {Key.A, 0x61},     // The A key.
	            {Key.B, 0x62},     // The B key.
	            {Key.C, 0x63},     // The C key.
	            {Key.D, 0x64},     // The D key.
	            {Key.E, 0x65},     // The E key.
	            {Key.F, 0x66},     // The F key.
	            {Key.G, 0x67},     // The G key.
	            {Key.H, 0x68},     // The H key.
	            {Key.I, 0x69},     // The I key.
	            {Key.J, 0x6a},     // The J key.
	            {Key.K, 0x6b},     // The K key.
	            {Key.L, 0x6c},     // The L key.
	            {Key.M, 0x6d},     // The M key.
	            {Key.N, 0x6e},     // The N key.
	            {Key.O, 0x6f},     // The O key.
	            {Key.P, 0x70},     // The P key.
	            {Key.Q, 0x71},     // The Q key.
	            {Key.R, 0x72},     // The R key.
	            {Key.S, 0x73},     // The S key.
	            {Key.T, 0x74},     // The T key.
	            {Key.U, 0x75},     // The U key.
	            {Key.V, 0x76},     // The V key.
	            {Key.W, 0x77},     // The W key.
	            {Key.X, 0x78},     // The X key.
	            {Key.Y, 0x79},     // The Y key.
	            {Key.Z, 0x7a},     // The Z key.
	            {Key.D0, 0x30},     // The 0 (zero) key.
	            {Key.D1, 0x31},     // The 1 (one) key.
	            {Key.D2, 0x32},     // The 2 key.
	            {Key.D3, 0x33},     // The 3 key.
	            {Key.D4, 0x34},     // The 4 key.
	            {Key.D5, 0x35},     // The 5 key.
	            {Key.D6, 0x36},     // The 6 key.
	            {Key.D7, 0x37},     // The 7 key.
	            {Key.D8, 0x38},     // The 8 key.
	            {Key.D9, 0x39},     // The 9 key.
	            {Key.F1, 0xFFBE},     // The F1 key.
	            {Key.F2, 0xFFBF},     // The F2 key.
	            {Key.F3, 0xFFC0},     // The F3 key.
	            {Key.F4, 0xFFC1},     // The F4 key.
	            {Key.F5, 0xFFC2},     // The F5 key.
	            {Key.F6, 0xFFC3},     // The F6 key.
	            {Key.F7, 0xFFC4},     // The F7 key.
	            {Key.F8, 0xFFC5},     // The F8 key.
	            {Key.F9, 0xFFC6},     // The F9 key.
	            {Key.F10, 0xFFC7},     // The F10 key.
	            {Key.F11, 0xFFC8},     // The F11 key.
	            {Key.F12, 0xFFC9},     // The F12 key.
	            {Key.F13, 0xFFCA},     // The F13 key.
	            {Key.F14, 0xFFCB},     // The F14 key.
	            {Key.F15, 0xFFCC},     // The F15 key.
	            {Key.F16, 0xFFCD},     // The F16 key.
	            {Key.F17, 0xFFCE},     // The F17 key.
	            {Key.F18, 0xFFCF},     // The F18 key.
	            {Key.F19, 0xFFD0},     // The F19 key.
	            {Key.F20, 0xFFD1},     // The F20 key.
	            {Key.F21, 0xFFD2},     // The F21 key.
	            {Key.F22, 0xFFD3},     // The F22 key.
	            {Key.F23, 0xFFD4},     // The F23 key.
	            {Key.F24, 0xFFD5},     // The F24 key.
	            {Key.NumPad0, 0xFFB0},     // The 0 key on the numeric keypad.
	            {Key.NumPad1, 0xFFB1},     // The 1 key on the numeric keypad.
	            {Key.NumPad2, 0xFFB2},     // The 2 key on the numeric keypad.
	            {Key.NumPad3, 0xFFB3},     // The 3 key on the numeric keypad.
	            {Key.NumPad4, 0xFFB4},     // The 4 key on the numeric keypad.
	            {Key.NumPad5, 0xFFB5},     // The 5 key on the numeric keypad.
	            {Key.NumPad6, 0xFFB6},     // The 6 key on the numeric keypad.
	            {Key.NumPad7, 0xFFB7},     // The 7 key on the numeric keypad.
	            {Key.NumPad8, 0xFFB8},     // The 8 key on the numeric keypad.
	            {Key.NumPad9, 0xFFB9},     // The 9 key on the numeric keypad.
	            {Key.LeftAlt, 0xFFE9},     // The left ALT key.
	            {Key.System, 0xFFE3},     // The left ALT key.
	            {Key.LeftCtrl, 0xFFE3},     // The left CTRL key.
	            {Key.LeftShift, 0xFFE1},     // The left Shift key.
	            {Key.LWin, 0xFFEB},     // The left Windows logo key (Microsoft Natural Keyboard).
	            {Key.RightAlt, 0xFFEA},     // The right ALT key.
	            {Key.RightCtrl, 0xFFE4},     // The right CTRL key.
	            {Key.RightShift, 0xFFE2},     // The right Shift key.
	            {Key.RWin, 0xFFEC},     // The right Windows logo key (Microsoft Natural Keyboard).
                {Key.Add, 0xFFAB},        // The Add key.
	            {Key.Back, 0xFF08},     // The Backspace key.
	            {Key.Cancel, 0xFF6B},     // The Cancel key.
	            {Key.CapsLock, 0xFFE6},     // The Caps Lock key.
	            {Key.Clear, 0xFF0B},     // The Clear key.
	            {Key.Delete, 0xFFFF},     // The Delete key.
	            {Key.Divide, 0xFFAF},     // The Divide key.
	            {Key.Down, 0xFF54},     // The Down Arrow key.
	            {Key.End, 0xFF57},     // The End key.
	            {Key.Enter, 0xFF0D},     // The Enter key.
	            {Key.Decimal, 0xFFAE},     // The Decimal key.
	            {Key.Escape, 0xFF1B},     // The ESC key.
	            {Key.Home, 0xFF50},     // The Home key.
	            {Key.Insert, 0xFF63},     // The Insert key.
	            {Key.Left, 0xFF51},     // The Left Arrow key.
	            {Key.Multiply, 0xFFAA},     // The Multiply key.
	            {Key.Next, 0xFF56},     // The Page Down key.
	            {Key.NumLock, 0xFF7F},     // The Num Lock key.
	            {Key.PrintScreen, 0xFF61},     // The Print Screen key.
	            {Key.Prior, 0xFF55},     // The Page Up key.
	            {Key.Right, 0xFF53},     // The Right Arrow key.
	            {Key.Scroll, 0xFF14},     // The Scroll Lock key.
	            {Key.Space, 0x20},     // The Spacebar key.
	            {Key.Subtract, 0xFFAD},     // The Subtract key.
	            {Key.Tab, 0xFF09},     // The Tab key.
	            {Key.Up, 0xFF52},     // The Up Arrow key.
	            {Key.OemComma, 0x2c},     // The OEM Comma key.
	            {Key.OemPeriod, 0x2e},     // The OEM Period key.
	            {Key.OemMinus, 0x2d},     // The OEM Minus key.
	            {Key.OemQuestion, 0x23},     // The OEM Question key.
	            {Key.OemQuotes, 0xe4},     // The OEM Quotes key.
	            {Key.Oem3, 0xf6},     // The OEM 3 key.
	            {Key.OemPlus, 0x2b},     // The OEM Addition key.
	            {Key.Oem1, 0xfc},     // The OEM 1 key.
	            {Key.Oem6, 0xfe51},     // The OEM 6 key.
	            {Key.OemOpenBrackets, 0xdf},     // The OEM Open Brackets key.
	            {Key.Oem5, 0xfe52}     // The OEM 5 key.
            };

            ShiftKeys = new Dictionary<Key, uint>()
            { 
                {Key.A, 0x41},     // The A key.
	            {Key.B, 0x42},     // The B key.
	            {Key.C, 0x43},     // The C key.
	            {Key.D, 0x44},     // The D key.
	            {Key.E, 0x45},     // The E key.
	            {Key.F, 0x46},     // The F key.
	            {Key.G, 0x47},     // The G key.
	            {Key.H, 0x48},     // The H key.
	            {Key.I, 0x49},     // The I key.
	            {Key.J, 0x4a},     // The J key.
	            {Key.K, 0x4b},     // The K key.
	            {Key.L, 0x4c},     // The L key.
	            {Key.M, 0x4d},     // The M key.
	            {Key.N, 0x4e},     // The N key.
	            {Key.O, 0x4f},     // The O key.
	            {Key.P, 0x50},     // The P key.
	            {Key.Q, 0x51},     // The Q key.
	            {Key.R, 0x52},     // The R key.
	            {Key.S, 0x53},     // The S key.
	            {Key.T, 0x54},     // The T key.
	            {Key.U, 0x55},     // The U key.
	            {Key.V, 0x56},     // The V key.
	            {Key.W, 0x57},     // The W key.
	            {Key.X, 0x58},     // The X key.
	            {Key.Y, 0x59},     // The Y key.
	            {Key.Z, 0x5a},     // The Z key.
	            {Key.D0, 0x3d},     // =
	            {Key.D1, 0x21},     // !
	            {Key.D2, 0x22},     // "
	            {Key.D3, 0xa7},     // §
	            {Key.D4, 0x24},     // $
	            {Key.D5, 0x25},     // %
	            {Key.D6, 0x26},     // &
	            {Key.D7, 0x2f},     // /
	            {Key.D8, 0x28},     // (
	            {Key.D9, 0x29},     // )
	            {Key.NumPad0, 0xFF9E},     // The 0 key on the numeric keypad.
	            {Key.NumPad1, 0xFF9C},     // The 1 key on the numeric keypad.
	            {Key.NumPad2, 0xFF99},     // The 2 key on the numeric keypad.
	            {Key.NumPad3, 0xFF9B},     // The 3 key on the numeric keypad.
	            {Key.NumPad4, 0xFF96},     // The 4 key on the numeric keypad.
	            {Key.NumPad5, 0xFF9D},     // The 5 key on the numeric keypad.
	            {Key.NumPad6, 0xFF98},     // The 6 key on the numeric keypad.
	            {Key.NumPad7, 0xFF95},     // The 7 key on the numeric keypad.
	            {Key.NumPad8, 0xFF97},     // The 8 key on the numeric keypad.
	            {Key.NumPad9, 0xFF9A},     // The 9 key on the numeric keypad.
	            {Key.OemComma, 0x3b},     // ;
	            {Key.OemPeriod, 0x3a},     // :
	            {Key.OemMinus, 0x5f},     // _
	            {Key.OemQuestion, 0x27},     // '
	            {Key.OemQuotes, 0xc4},     // Ä
	            {Key.Oem3, 0xd6},     // Ö
	            {Key.OemPlus, 0x2a},     // *
	            {Key.Oem1, 0xdc},     // Ü
	            {Key.Oem6, 0xfe50},     // `
	            {Key.OemOpenBrackets, 0x3f},     // ?
	            {Key.Oem5, 0xb0}     // °
            };

            AltCtlKeys = new Dictionary<Key, uint>()
            { 
	            {Key.Q, 0x40},     // @
	            {Key.E, 0x20ac},     // €
	            {Key.OemPlus, 0x7e},     // ~
	            {Key.T, 0x54},     // The T key.
	            {Key.U, 0x55},     // The U key.
	            {Key.V, 0x56},     // The V key.
	            {Key.W, 0x57},     // The W key.
	            {Key.X, 0x58},     // The X key.
	            {Key.Y, 0x59},     // The Y key.
	            {Key.Z, 0x5a},     // The Z key.
	            {Key.D0, 0x7d},     // }
	            {Key.D2, 0xb2},     // ²
	            {Key.D3, 0xfe03},     // ³
	            {Key.D7, 0x7b},     // {
	            {Key.D8, 0x5b},     // [
	            {Key.D9, 0x5d},     // ]
	            {Key.OemBackslash, 0x7c}     // |
            };
        }

        public void parseConvertion(string List, Logger Log)
        {
            string[] allConverts = List.ToLower().Split(',');
            foreach (string s in allConverts)
            {
                string[] c = s.Split('=');
                if (c.Length != 2)
                {
                    Log.Error("Keyconvert: malformed set: \"" + s + "\" !");
                    continue;
                }

                Dictionary<Key, uint> keyList = NomodKeys;
                string keyName = c[0];

                uint keyValue = 0;
                try
                {
                    keyValue = uint.Parse(c[1]);
                }
                catch (Exception)
                {
                    Log.Error("Keyconvert: malformed set value: \"" + c[1] + "\" !");
                    continue;
                }

                if (c[0].Contains("+"))
                {
                    string[] d = c[0].ToLower().Split('+');
                    if (d[0] == "shift")
                    {
                        keyList = ShiftKeys;
                        keyName = d[1];
                    }
                    if (d[1] == "shift")
                    {
                        keyList = ShiftKeys;
                        keyName = d[0];
                    }
                    if (d[0] == "alt" || d[0] == "strg")
                    {
                        keyList = AltCtlKeys;
                        keyName = d[1];
                    }
                    if (d[1] == "alt" || d[1] == "strg")
                    {
                        keyList = AltCtlKeys;
                        keyName = d[0];
                    }
                }

                bool keyFound = false;

                foreach (Key k in Enum.GetValues(typeof(Key)))
                {
                    if (k.ToString().ToLower() == keyName)
                    {
                        keyFound = true;
                        if (keyList.ContainsKey(k))
                        {
                            Log.Debug("Replaced keyconvert \"" + k.ToString() + "\" from " + keyList[k] + " to " + keyValue + ".");
                            keyList.Remove(k);
                        }
                        keyList.Add(k, keyValue);
                        break;
                    }
                }
                if (!keyFound)
                {
                    Log.Error("Keyconvert: Key \"" + keyName + "\" not found !");
                }
            }

        }
    }
}
