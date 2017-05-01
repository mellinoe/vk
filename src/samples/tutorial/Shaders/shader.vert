#version 450 core
#extension GL_ARB_separate_shader_objects : enable

layout(binding = 0) uniform UniformBufferObject {
    mat4 model;
	mat4 view;
	mat4 projection;
};

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec3 in_color;
layout(location = 2) in vec2 in_texCoord;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out vec3 out_fragColor;
layout(location = 1) out vec2 out_texCoord;


void main() {
    gl_Position = projection * view * model * vec4(in_position, 0.0, 1.0);
    out_fragColor = in_color;
	out_texCoord = in_texCoord;
}
