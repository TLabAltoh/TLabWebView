using System;
using System.Runtime.InteropServices;

namespace TLab.Android.WebView
{
	public static class NativePlugin
	{
		public const string LIB_SHARED_TEXTURE = "shared-texture";
		public const string LIB_JNI_LIB_WEBVIEW = "jni-libwebview";

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern IntPtr DummyRenderEventFunc();

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern void DummyRenderEvent(int instance_ptr);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern IntPtr UpdateSurfaceFunc();

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern void UpdateSurface(int instance_ptr);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern IntPtr GetPlatformTextureID(int instance_ptr);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern void SetUnityTextureID(int instance_ptr, long unity_texture_id);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern bool ContentExists(int instance_ptr);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern bool GetSharedBufferUpdateFlag(int instance_ptr);

		[DllImport(LIB_JNI_LIB_WEBVIEW)]
		public static extern void SetHardwareBufferUpdateFlag(int instance_ptr, bool value);
	}
}
