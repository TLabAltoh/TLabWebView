package com.self.viewtoglrendering;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.SurfaceTexture;
import android.opengl.GLES11Ext;
import android.opengl.GLES20;
import android.opengl.GLSurfaceView;
import android.opengl.GLUtils;
import android.util.Log;
import android.view.Surface;

import javax.microedition.khronos.egl.EGLConfig;
import javax.microedition.khronos.opengles.GL10;

public class ViewToGLRenderer implements GLSurfaceView.Renderer {

    private static final String TAG = ViewToGLRenderer.class.getSimpleName();

    // ------------------------------------------------------------------------------------------------
    // Default texture resolution
    //

    private static final int DEFAULT_TEXTURE_WIDTH = 512;
    private static final int DEFAULT_TEXTURE_HEIGHT = 512;

    private int mTextureWidth = DEFAULT_TEXTURE_WIDTH;
    private int mTextureHeight = DEFAULT_TEXTURE_HEIGHT;

    // ------------------------------------------------------------------------------------------------
    // Surface and surface texture
    //

    private SurfaceTexture mSurfaceTexture;
    private Surface mSurface;

    private int[] mGlSurfaceTexture = new int[1];
    private Canvas mSurfaceCanvas;

    // ------------------------------------------------------------------------------------------------
    // Texture capture
    //

    private TextureCapture textureCapture;
    private byte[] mCaptureData;

    // ------------------------------------------------------------------------------------------------
    // Surface
    //

    public void createSurface(int width, int height) {
        releaseSurface();
        if (mGlSurfaceTexture[0] > 0) {
            //attach the texture to a surface.
            //It's a clue class for rendering an android view to gl level
            mSurfaceTexture = new SurfaceTexture(mGlSurfaceTexture[0]);
            mSurfaceTexture.setDefaultBufferSize(mTextureWidth, mTextureHeight);
            mSurface = new Surface(mSurfaceTexture);
            if (textureCapture != null) {
                textureCapture.onInputSizeChanged(mTextureWidth, mTextureHeight);
            }
        }
    }

    @Override
    public void onSurfaceCreated(GL10 gl, EGLConfig config) {
        final String extensions = GLES20.glGetString(GLES20.GL_EXTENSIONS);
        Log.d(TAG, extensions);
        if (textureCapture != null) {
            textureCapture.init();
        }
        Log.i("libWebView", "libwebview---onSurfaceCreated: function called");
    }

    public void releaseSurface() {
        if(mSurface != null){
            mSurface.release();
        }
        if(mSurfaceTexture != null){
            mSurfaceTexture.release();
        }
        mSurface = null;
        mSurfaceTexture = null;
    }

    @Override
    public void onSurfaceChanged(GL10 gl, int width, int height) {
        createSurface(mTextureWidth, mTextureHeight);
    }

    // ------------------------------------------------------------------------------------------------
    // Surface texture create
    //

    public void createTexture() {
        // Generate the texture to where android view will be rendered
        GLES20.glActiveTexture(GLES20.GL_TEXTURE0);
        GLES20.glGenTextures(1, mGlSurfaceTexture, 0);
        checkGlError("Texture generate");

        GLES20.glBindTexture(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, mGlSurfaceTexture[0]);
        checkGlError("Texture bind");

        GLES20.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES20.GL_TEXTURE_MIN_FILTER,GLES20.GL_LINEAR);
        GLES20.glTexParameterf(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES20.GL_TEXTURE_MAG_FILTER, GLES20.GL_LINEAR);
        GLES20.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES20.GL_TEXTURE_WRAP_S, GLES20.GL_CLAMP_TO_EDGE);
        GLES20.glTexParameteri(GLES11Ext.GL_TEXTURE_EXTERNAL_OES, GLES20.GL_TEXTURE_WRAP_T, GLES20.GL_CLAMP_TO_EDGE);

        GLES20.glBindTexture(GLES20.GL_TEXTURE_2D, 0);
    }

    // ------------------------------------------------------------------------------------------------
    // Draw frame (Call from texture capture)
    //

    @Override
    public void onDrawFrame(GL10 gl) {
        synchronized (this) {
            Log.i("libwebview", "libwebview---onDrawFrame: Surface image update start");
            // update texture
            mSurfaceTexture.updateTexImage();
            if (textureCapture != null) {
                textureCapture.onDrawFrame(getGLSurfaceTexture(), true);
                mCaptureData = textureCapture.getGLFboBuffer();

                Log.i("libwebview", "libwebview---onDrawFrame: Surface image updated");
            }
        }
    }

    public Canvas onDrawViewBegin() {
        mSurfaceCanvas = null;
        if (mSurface != null) {
            try {
                // mSurfaceCanvas = mSurface.lockCanvas(null);
                mSurfaceCanvas = mSurface.lockHardwareCanvas();
            }catch (Exception e){
                Log.e(TAG, "error while rendering view to gl: " + e);
            }
        }
        return mSurfaceCanvas;
    }

    public void onDrawViewEnd() {
        if(mSurfaceCanvas != null) {
            mSurface.unlockCanvasAndPost(mSurfaceCanvas);
        }
        mSurfaceCanvas = null;
    }

    public void checkGlError(String op) {
        int error;
        while ((error = GLES20.glGetError()) != GLES20.GL_NO_ERROR) {
            Log.e(TAG, op + ": glError " + GLUtils.getEGLErrorString(error));
        }
    }

    // ------------------------------------------------------------------------------------------------
    // Get surface texture
    //

    private int getGLSurfaceTexture(){
        return mGlSurfaceTexture[0];
    }

    // ------------------------------------------------------------------------------------------------
    // Get texture data
    //

    public byte[] getTexturePixels() {
        return mCaptureData;
    }

    // ------------------------------------------------------------------------------------------------
    // Texture resolution
    //

    public int getTextureWidth() {
        return mTextureWidth;
    }

    public void setTextureWidth(int textureWidth) {
        mTextureWidth = textureWidth;
    }

    public int getTextureHeight() {
        return mTextureHeight;
    }

    public void setTextureHeight(int textureHeight) {
        mTextureHeight = textureHeight;
    }

    // ------------------------------------------------------------------------------------------------
    // Create texture capture
    //

    public void createTextureCapture(Context context, int vs, int fs, int width, int height) {
        textureCapture = new TextureCapture();
        textureCapture.flipY();
        textureCapture.loadSamplerShaderProg(context, vs, fs);
    }
}