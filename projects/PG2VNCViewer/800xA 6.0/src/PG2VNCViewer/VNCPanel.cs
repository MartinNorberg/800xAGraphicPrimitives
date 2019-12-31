/*
Copyright (c) 2019, Stefan Berndt
Parts of this file are copied from VncSharp, so i must take its copyright too.
*/

// VncSharp - .NET VNC Client Library
// Copyright (C) 2008 David Humphrey
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AfwDynamicGraphics;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using VncSharp;
using System.Diagnostics;
using System.Drawing;
using VncSharp.Encodings;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace PG2VNCViewer
{
    public partial class PG2VNCViewer : FrameItem, ITimerUpdateable
    {
        RfbProtocol rfb;
        Framebuffer buffer;
        EncodedRectangleFactory factory;
        KeyTable keytable = new KeyTable();
        int sleeptime = 0;

        // vnc client handling - main thread
        private void RunNetThread()
        {
            while (!terminate)
            {
                if (autoconnect && host!="")
                {
                    try
                    {
                        sleeptime = 0;
                        Log.Info("Connecting to \"" + host + ":" + port + "\"...");
                        rfb = new RfbProtocol();
                        rfb.Connect(host, (int)port);
                        Log.Debug("Connected.");
                        rfb.ReadProtocolVersion();

                        // Handle possible repeater connection
                        if (rfb.ServerVersion == 0.0)
                        {
                            rfb.WriteProxyAddress();
                            // Now we are connected to the real server; read the protocol version of the 
                            // server
                            rfb.ReadProtocolVersion();
                            // Resume normal handshake and protocol
                        }

                        rfb.WriteProtocolVersion();
                        Log.Debug("Protocol Version:" + rfb.ServerVersion);

                        // Figure out which type of authentication the server uses
                        byte[] types = rfb.ReadSecurityTypes();

                        // Based on what the server sends back in the way of supported Security Types, one of
                        // two things will need to be done: either the server will reject the connection (i.e., type = 0),
                        // or a list of supported types will be sent, of which we need to choose and use one.
                        if (types.Length > 0)
                        {
                            if (types[0] == 0)
                            {
                                // The server is not able (or willing) to accept the connection.
                                // A message follows indicating why the connection was dropped.
                                throw new Exception("Connection Failed. The server rejected the connection for the following reason: " + rfb.ReadSecurityFailureReason());
                            }
                            else
                            {
                                byte securityType = GetSupportedSecurityType(types);
                                if (securityType == 0) throw new Exception("Unknown Security Type(s), The server sent one or more unknown Security Types.");

                                rfb.WriteSecurityType(securityType);

                                // Protocol 3.8 states that a SecurityResult is still sent when using NONE (see 6.2.1)
                                if (rfb.ServerVersion >= 3.799f && rfb.ServerVersion <= 3.801 && securityType == 1)
                                {
                                    if (rfb.ReadSecurityResult() > 0)
                                    {
                                        // For some reason, the server is not accepting the connection.  Get the
                                        // reason and throw an exception
                                        throw new Exception ("Unable to Connect to the Server. The Server rejected the connection for the following reason: " + rfb.ReadSecurityFailureReason());
                                    }
                                }

                                if (securityType > 1)
                                {
                                    byte[] challenge = rfb.ReadSecurityChallenge();
                                    rfb.WriteSecurityResponse(EncryptChallenge(pass, challenge));
                                    if (rfb.ReadSecurityResult() != 0)
                                    {
                                        // Authentication failed, and if the server is using Protocol version 3.8, a 
                                        // plain text message follows indicating why the error happend.  I'm not 
                                        // currently using this message, but it is read here to clean out the stream.
                                        // In earlier versions of the protocol, the server will just drop the connection.
                                        autoconnect = false;
                                        if (rfb.ServerVersion == 3.8)
                                        {
                                            throw new Exception("The server denyes the authentication. Reason: \"" + rfb.ReadSecurityFailureReason() + "\"");
                                        }
                                        throw new Exception("The server denyes the authentication.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Something is wrong, since we should have gotten at least 1 Security Type
                            throw new Exception("Protocol Error Connecting to Server. The Server didn't send any Security Types during the initial handshake.");
                        }

                        // Finish initializing protocol with host
                        rfb.WriteClientInitialisation(shared);
                        buffer = rfb.ReadServerInit();
                        rfb.WriteSetPixelFormat(buffer);	// just use the server's framebuffer format

                        if (hideRemoteCursor)
                        {
                            rfb.WriteSetEncodings(new int[] 
                            {	
                                RfbProtocol.ZRLE_ENCODING,
			                    RfbProtocol.HEXTILE_ENCODING, 
								RfbProtocol.RRE_ENCODING,
								RfbProtocol.COPYRECT_ENCODING,
								RfbProtocol.RAW_ENCODING,
                                RfbProtocol.CURSOR_PSEUDO_ENCODING
                            });
                        }
                        else
                        {
                            rfb.WriteSetEncodings(new int[] 
                            {	
                                RfbProtocol.ZRLE_ENCODING,
			                    RfbProtocol.HEXTILE_ENCODING, 
								RfbProtocol.RRE_ENCODING,
								RfbProtocol.COPYRECT_ENCODING,
								RfbProtocol.RAW_ENCODING
                            });
                        }

                        // Create an EncodedRectangleFactory so that EncodedRectangles can be built according to set pixel layout
                        factory = new EncodedRectangleFactory(rfb, buffer);
                        // Get the initial destkop from the host

                        rfb.WriteFramebufferUpdateRequest(0, 0, (ushort)buffer.Width, (ushort)buffer.Height, false);

                        while (!terminate && autoconnect)
                        {
                            switch (rfb.ReadServerMessageType())
                            {
                                case RfbProtocol.FRAMEBUFFER_UPDATE:
                                    int rectangles = rfb.ReadFramebufferUpdate();

                                    for (int i = 0; i < rectangles; ++i)
                                    {
                                        // Get the update rectangle's info
                                        Rectangle rectangle;
                                        int enc;
                                        rfb.ReadFramebufferUpdateRectHeader(out rectangle, out enc);

                                        // Build a derived EncodedRectangle type and pull-down all the pixel info
                                        EncodedRectangle er = factory.Build(rectangle, enc);

                                        er.Decode();
                                        er.Draw();
                                    }
                                    break;
                                case RfbProtocol.BELL:
                                    Log.Debug("Bell");
                                    break;
                                case RfbProtocol.SERVER_CUT_TEXT:
                                    rfb.ReadServerCutText();
                                    Log.Debug("server-cut-text");
                                    break;
                                case RfbProtocol.SET_COLOUR_MAP_ENTRIES:
                                    Log.Debug("SET_COLOUR_MAP_ENTRIES");
                                    rfb.ReadColourMapEntry();
                                    break;
                                default:
                                    throw new Exception("Unknown messagetype.");
                            }
                            invalid = true;
                            rfb.WriteFramebufferUpdateRequest(0, 0, (ushort)buffer.Width, (ushort)buffer.Height, true);
                        }
                    }
                    catch (SocketException)
                    {
                        Log.Info("Unable to connect to the server. Retry in 60s.");
                        sleeptime = 60000;
                    }
                    catch (IOException e)
                    {
                        Log.Debug("Exception in NetThread(" + e.GetType().ToString() + "): " + e.Message);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Exception in NetThread(" + e.GetType().ToString() + "): " + e.Message);
                        sleeptime = 10000;
                    }
                    try
                    {
                        rfb.Close();
                    }
                    catch (Exception) {}
                    rfb = null;
                    buffer = null;
                    factory = null;
                    invalid = true;
                    Thread.Sleep(sleeptime);
                }
            }
        }

        // mouse handling - vnc side
        private void MouseUpdate(MouseEventContext mouseEventContext, bool logging)
        {
            if (rfb != null && !viewonly)
            {
                byte b = 0;

                if (mouseEventContext.MouseEventArgs.LeftButton == MouseButtonState.Pressed) b |= 1;
                if (mouseEventContext.MouseEventArgs.MiddleButton == MouseButtonState.Pressed) b |= 2;
                if (mouseEventContext.MouseEventArgs.RightButton == MouseButtonState.Pressed) b |= 4;

                double x = mouseEventContext.X;
                double y = mouseEventContext.Y;

                x -= Position.X;
                y -= Position.Y;

                if (stretch)
                {
                    x *= buffer.Width / ClientArea.Width;
                    y *= buffer.Height / ClientArea.Height;
                }

                if (logging)
                {
                    Log.Debug("MouseUpdate x=" + (int)x + " y=" + (int)y + " btn=" + b);
                }
                MouseUpdate(b, (int)x, (int)y);
            }
        }
        private void MouseUpdate(byte btn, int x, int y)
        {
            if (rfb != null)
            {
                rfb.WritePointerEvent(btn, new System.Drawing.Point(x, y));
            }
        }

        // keyboard handling - vnc side
        bool shift = false;
        bool ctrl = false;
        bool alt = false;
        private void WriteKey(Key key, bool pressed, bool logging)
        {
            if (rfb != null && !viewonly && rfb.Reader != null)
            {
                uint keysyn = 0;
                try
                {
                    if (key == Key.RightShift) shift = pressed;
                    if (key == Key.LeftShift) shift = pressed;
                    if (key == Key.RightCtrl) ctrl = pressed;
                    if (key == Key.LeftCtrl) ctrl = pressed;
                    if (key == Key.RightAlt)
                    {
                        ctrl = pressed;
                        alt = pressed;
                    }
                    if (key == Key.LeftAlt) alt = pressed;

                    if (alt && ctrl)
                    {
                        keytable.AltCtlKeys.TryGetValue(key, out keysyn);
                    }

                    if (shift && keysyn == 0)
                    {
                        keytable.ShiftKeys.TryGetValue(key, out keysyn);
                    }

                    if (keysyn == 0)
                    {
                        keytable.NomodKeys.TryGetValue(key, out keysyn);
                    }

                    if (keysyn != 0)
                    {
                        rfb.WriteKeyEvent(keysyn, pressed);
                        if (logging)
                        {
                            Log.Debug("Key"+(pressed?"Down":"Up")+ " Key=" + key+ " Code="+keysyn);
                        }
                    }
                    else
                    {
                        Log.Info("Unhandled keymapping: " + key);
                    }
                }
                catch (Exception e)
                {
                    Log.Info("Exception in WriteKey:"+e.Message);
                }
            }
        }

        private void WriteKey(uint key, bool pressed)
        {
            if (rfb != null)
            {
                rfb.WriteKeyEvent(key, pressed);
            }
        }


        /// <summary>
        /// Examines a list of Security Types supported by a VNC Server and chooses one that the Client supports.  See 6.1.2 of the RFB Protocol document v. 3.8.
        /// </summary>
        /// <param name="types">An array of bytes representing the Security Types supported by the VNC Server.</param>
        /// <returns>A byte that represents the Security Type to be used by the Client.</returns>
        protected byte GetSupportedSecurityType(byte[] types)
        {
            // Pick the first match in the list of given types.  If you want to add support for new
            // security types, do it here:
            for (int i = 0; i < types.Length; ++i)
            {
                if (types[i] == 1  	// None
                    || types[i] == 2	// VNC Authentication
                   ) return types[i];
            }
            return 0;
        }
        
        /// <summary>
        /// Encrypts a challenge using the specified password. See RFB Protocol Document v. 3.8 section 6.2.2.
        /// </summary>
        /// <param name="password">The user's password.</param>
        /// <param name="challenge">The challenge sent by the server.</param>
        /// <returns>Returns the encrypted challenge.</returns>
        protected byte[] EncryptChallenge(string password, byte[] challenge)
        {
            byte[] key = new byte[8];

            // Key limited to 8 bytes max.
            if (password.Length >= 8)
            {
                System.Text.Encoding.ASCII.GetBytes(password, 0, 8, key, 0);
            }
            else
            {
                System.Text.Encoding.ASCII.GetBytes(password, 0, password.Length, key, 0);
            }

            // VNC uses reverse byte order in key
            for (int i = 0; i < 8; i++)
                key[i] = (byte)(((key[i] & 0x01) << 7) |
                                 ((key[i] & 0x02) << 5) |
                                 ((key[i] & 0x04) << 3) |
                                 ((key[i] & 0x08) << 1) |
                                 ((key[i] & 0x10) >> 1) |
                                 ((key[i] & 0x20) >> 3) |
                                 ((key[i] & 0x40) >> 5) |
                                 ((key[i] & 0x80) >> 7));

            // VNC uses DES, not 3DES as written in some documentation
            DES des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.ECB;

            ICryptoTransform enc = des.CreateEncryptor(key, null);

            byte[] response = new byte[16];
            enc.TransformBlock(challenge, 0, challenge.Length, response, 0);

            return response;
        }
 
    }
}
