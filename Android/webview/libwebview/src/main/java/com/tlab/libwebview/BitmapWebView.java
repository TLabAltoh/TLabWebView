package com.tlab.libwebview;

import android.content.Context;
import android.util.AttributeSet;
import android.webkit.WebView;

public class BitmapWebView extends WebView {
    public BitmapWebView(Context context) {
        super(context);
    }

    public BitmapWebView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public BitmapWebView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public boolean performClick(){
        super.performClick();
        return false;
    }
}

