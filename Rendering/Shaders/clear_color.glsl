#[compute]
#version 450

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(rgba8, set = 0, binding = 0)
uniform restrict writeonly image2D output_image;

layout(set = 0, binding = 1) uniform sampler2D sprite_texture;

layout(push_constant, std430) uniform Parameters
{
	vec2 object_position;
	float depth;
	float padding;
} parameters;

void main()
{
	ivec2 pixel = ivec2(gl_GlobalInvocationID.xy);

	if (pixel.x >= 320 || pixel.y >= 180)
		return;

	vec4 color = vec4(0.1, 0.2, 0.4, 1.0);

	float half_size = max(1.0, 30.0 / parameters.depth);

	vec2 sprite_size = vec2(textureSize(sprite_texture, 0));

	float scale = 60.0 / parameters.depth;

	vec2 drawn_size = sprite_size * scale;

	vec2 uv =
		(vec2(pixel) - parameters.object_position) / drawn_size +
		vec2(0.5);

	if (
		uv.x >= 0.0 &&
		uv.x <= 1.0 &&
		uv.y >= 0.0 &&
		uv.y <= 1.0
	)
	{
		vec4 sprite_color = texture(sprite_texture, uv);

		if (sprite_color.a > 0.0)
			color = sprite_color;
	}

	imageStore(output_image, pixel, color);
}
