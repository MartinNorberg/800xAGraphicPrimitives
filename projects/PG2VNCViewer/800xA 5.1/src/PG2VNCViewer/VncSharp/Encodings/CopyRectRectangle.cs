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
using System.Drawing;
using System.Drawing.Imaging;

namespace VncSharp.Encodings
{
	/// <summary>
	/// Implementation of CopyRect encoding, as well as drawing support. See RFB Protocol document v. 3.8 section 6.5.2.
	/// </summary>
	public sealed class CopyRectRectangle : EncodedRectangle 
	{
		public CopyRectRectangle(RfbProtocol rfb, Framebuffer framebuffer, Rectangle rectangle)
			: base(rfb, framebuffer, rectangle, RfbProtocol.COPYRECT_ENCODING) 
		{
		}

		// CopyRect Source Point (x,y) from which to copy pixels in Draw
		Point source;

		/// <summary>
		/// Decodes a CopyRect encoded rectangle.
		/// </summary>
		public override void Decode()
		{
			// Read the source point from which to begin copying pixels
			source = new Point();
			source.X = (int) rfb.ReadUInt16();
			source.Y = (int) rfb.ReadUInt16();
		}

		public override void Draw()
		{
            int ptr = source.Y * framebuffer.Width + source.X;
            int offset = framebuffer.Width - rectangle.Width;

            int idx = 0;
            for (int y = 0; y < rectangle.Height; ++y)
            {
                for (int x = 0; x < rectangle.Width; ++x)
                {
                    framebuffer[idx++] = framebuffer.screen[ptr++];
                }
                ptr += offset;
            }
            base.Draw();
		}
	}
}