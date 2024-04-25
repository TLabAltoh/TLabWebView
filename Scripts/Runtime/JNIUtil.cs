using System;
using System.Runtime.InteropServices;

namespace TLab.Android.WebView
{
	public static class JNIUtil
	{

		public const string LIB_NAME = "jni-util";

		[DllImport(LIB_NAME)]
		public static extern int add(int x, int y);

		[DllImport(LIB_NAME)]
		public static extern IntPtr UpdateSurfaceFunc();

		[DllImport(LIB_NAME)]
		public static extern void UpdateSurface(int instance_ptr);

		[DllImport(LIB_NAME)]
		public static extern IntPtr GetTexturePtr(int instance_ptr);
	}
}
