package com.self.viewtoglrendering;

import android.content.Context;
import android.graphics.Canvas;
import android.util.AttributeSet;
import android.widget.LinearLayout;

public class GLLinearLayout extends LinearLayout implements GLRenderable {

    private ViewToGLRenderer mViewToGLRenderer;

    // default constructors

    public GLLinearLayout(Context context) {
        super(context);
    }

    public GLLinearLayout(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public GLLinearLayout(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    // drawing magic
    @Override
    public void draw(Canvas canvas) {
        if (mViewToGLRenderer == null) return;

        Canvas glAttachedCanvas = mViewToGLRenderer.onDrawViewBegin();
        if (glAttachedCanvas != null) {
            //prescale canvas to make sure content fits
            glAttachedCanvas.scale(1, 1);
            //draw the view to provided canvas
            super.draw(glAttachedCanvas);
        }
        // notify the canvas is updated
        mViewToGLRenderer.onDrawViewEnd();
    }

    public void setViewToGLRenderer(ViewToGLRenderer viewToGLRenderer) {
        mViewToGLRenderer = viewToGLRenderer;
    }
}
