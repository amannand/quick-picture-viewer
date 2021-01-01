﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace quick_picture_viewer
{
	public static class DdsWrapper
	{
		private static GCHandle TmpGcHandle;

		public static Error CurrentError = 0;
		public enum Error
		{
			NoError = 0,
			MemoryError = 1,
			UnableToOpen = 2
		}

		public static Bitmap ParseDdsOrTga(string path)
		{
			try
			{
				using (var image = Pfim.Pfim.FromFile(path))
				{
					PixelFormat format;

					switch (image.Format)
					{
						case Pfim.ImageFormat.Rgba32:
							format = PixelFormat.Format32bppArgb;
							break;
						case Pfim.ImageFormat.Rgb24:
							format = PixelFormat.Format24bppRgb;
							break;
						case Pfim.ImageFormat.Rgba16:
							format = PixelFormat.Format16bppArgb1555;
							break;
						case Pfim.ImageFormat.Rgb8:
							format = PixelFormat.Format8bppIndexed;
							break;
						default:
							throw new NotImplementedException();
					}

					try
					{
						if (TmpGcHandle != null && TmpGcHandle.IsAllocated)
						{
							TmpGcHandle.Free();
						}

						TmpGcHandle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
						var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);

						CurrentError = Error.NoError;
						return new Bitmap(image.Width, image.Height, image.Stride, format, data);
					}
					catch
					{
						CurrentError = Error.MemoryError;
						return null;
					}
				}
			}
			catch
			{
				CurrentError = Error.UnableToOpen;
				return null;
			}
		}
	}
}