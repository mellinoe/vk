#version 450 core
#extension GL_ARB_separate_shader_objects : enable

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec3 in_color;

out gl_PerVertex {
    vec4 gl_Position;
};

layout(location = 0) out vec3 out_fragColor;


void main() {
    gl_Position = vec4(in_position, 0.0, 1.0);
    out_fragColor = in_color;
}
