// YUV to RGB conversion (fragment shader)
// Author: Max Schwarz <Max@x-quadraht.de>

// SHADER 2: Fragment shader, calculates RGB from YUV

// INPUTS
uniform sampler2D y_sampler;
uniform sampler2D u_sampler;
uniform sampler2D v_sampler;

// INPUTS FROM VERTEX SHADER
varying vec2 v_texcoord;

void main() {
	float yChannel = texture2D(y_sampler, v_texcoord).x;
	float uChannel = texture2D(u_sampler, v_texcoord).x;
	float vChannel = texture2D(v_sampler, v_texcoord).x;
	
	// This does the colorspace conversion from Y'UV to RGB as a matrix
	// multiply.  It also does the offset of the U and V channels from
	// [0,1] to [-.5,.5] as part of the transform.
	//  - adapted from google's o3d shader examples:
	//    trunk/samples_webgl/shaders/yuv2rgb-glsl.shader
	vec4 channels = vec4(yChannel, uChannel, vChannel, 1.0);
	
	mat4 conversion = mat4(
		1.0,     0.0,     1.402,  -0.701,
		1.0,    -0.344,  -0.714,   0.529,
		1.0,     1.772,   0.0,    -0.886,
		0.0,     0.0,     0.0,     0.0
	);
	vec3 rgb = (channels * conversion).xyz;
	
	gl_FragColor = vec4(rgb, 1.0);
}