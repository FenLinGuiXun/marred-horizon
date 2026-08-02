#[compute]
#version 450

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(rgba8, set = 0, binding = 0)
uniform restrict writeonly image2D output_image;

layout(push_constant, std430) uniform Parameters
{
	vec2 object_position;
} parameters;

void main()
{
	ivec2 pixel = ivec2(gl_GlobalInvocationID.xy);

	if (pixel.x >= 320 || pixel.y >= 180)
		return;

	vec4 color = vec4(0.1, 0.2, 0.4, 1.0);

	vec2 difference = abs(vec2(pixel) - parameters.object_position);

	if (difference.x <= 3.0 && difference.y <= 3.0)
		color = vec4(1.0, 0.8, 0.2, 1.0);

	imageStore(output_image, pixel, color);
}
