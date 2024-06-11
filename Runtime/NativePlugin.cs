using System;
using System.Runtime.InteropServices;

namespace TLab.Android.WebView
{
	public static class NativePlugin
	{
		public const string LIB_SHARED_TEXTURE = "shared-texture";
		public const string LIB_JNI_UTIL = "jni-util";

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern IntPtr DummyRenderEventFunc();

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern void DummyRenderEvent(int instance_ptr);

		[DllImport(LIB_JNI_UTIL)]
		public static extern IntPtr UpdateSurfaceFunc();

		[DllImport(LIB_JNI_UTIL)]
		public static extern void UpdateSurface(int instance_ptr);

		[DllImport(LIB_JNI_UTIL)]
		public static extern IntPtr GetBindedPlatformTextureID(int instance_ptr);

		[DllImport(LIB_JNI_UTIL)]
		public static extern void SetUnityTextureID(int instance_ptr, long unity_texture_id);

		[DllImport(LIB_JNI_UTIL)]
		public static extern bool GetSharedBufferUpdateFlag(int instance_ptr);

		[DllImport(LIB_JNI_UTIL)]
		public static extern void SetHardwareBufferUpdateFlag(int instance_ptr, bool value);
	}
}
