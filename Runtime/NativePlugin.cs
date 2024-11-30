using System;
using System.Runtime.InteropServices;

namespace TLab.WebView
{
	public static class NativePlugin
	{
		public const string LIB_SHARED_TEXTURE = "shared-texture";
		public const string LIB_NATIVE = "jni-libTLabWebView";

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern IntPtr DummyRenderEventFunc();

		[DllImport(LIB_SHARED_TEXTURE)]
		public static extern void DummyRenderEvent(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern IntPtr UpdateSharedTextureFunc();

		[DllImport(LIB_NATIVE)]
		public static extern void UpdateSharedTexture(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern IntPtr DisposeFunc();

		[DllImport(LIB_NATIVE)]
		public static extern void Dispose(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern void ReleaseSharedTexture(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern IntPtr GetPlatformTextureID(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern void SetUnityTextureID(int instance_ptr, long unity_texture_id);

		[DllImport(LIB_NATIVE)]
		public static extern bool ContentExists(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern void SetSurface(int instance_ptr, int surface_ptr, int width, int height);

		[DllImport(LIB_NATIVE)]
		public static extern void RemoveSurface(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern bool GetIsFragmentInitialized(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern bool GetIsFragmentDisposed(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern bool GetSharedBufferUpdateFlag(int instance_ptr);

		[DllImport(LIB_NATIVE)]
		public static extern void SetSharedBufferUpdateFlag(int instance_ptr, bool value);
	}
}
