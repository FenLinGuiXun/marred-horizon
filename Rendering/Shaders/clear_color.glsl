#[compute]
#version 450

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(rgba8, set = 0, binding = 0) uniform restrict writeonly image2D output_image;

void main()
{
	ivec2 pixel = ivec2(gl_GlobalInvocationID.xy);

	if (pixel.x >= 320 || pixel.y >= 180)
		return;

	imageStore(output_image, pixel, vec4(0.1, 0.2, 0.4, 1.0));
}