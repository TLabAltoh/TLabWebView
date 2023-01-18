precision mediump float;

varying mediump vec2 textureCoordinate;

uniform sampler2D inputImageTexture;

void main() {
    vec4 col = texture2D(inputImageTexture, textureCoordinate);
    col = vec4(col.z, col.y, col.x, col.w);
    gl_FragColor = col;
}
