// YUV to RGB conversion
// Author: Max Schwarz <Max@x-quadraht.de>

// SHADER 1: Vertex shader, calculates the current position

// output to fragment shader
varying vec2 v_texcoord;

void main()
{
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	v_texcoord = gl_MultiTexCoord0.st;
}