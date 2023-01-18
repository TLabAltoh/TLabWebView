#extension GL_OES_EGL_image_external : require

precision mediump float;

uniform samplerExternalOES inputImageTexture;

varying vec2 textureCoordinate;

void main() {
    vec4 col = texture2D(inputImageTexture, textureCoordinate);
    col = vec4(col.w, col.x, col.y, col.z);
    gl_FragColor = col;
}
