shader_type spatial;
// Prevent the quad from being affected by lighting and fog. This also improves performance.
render_mode unshaded, fog_disabled;

uniform float downsample_factor = 0.0;
uniform sampler2D depth_texture : source_color, hint_depth_texture;
uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

void vertex() {
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	float depth = texture(depth_texture, SCREEN_UV).x;
	vec3 ndc = vec3(SCREEN_UV * 2.0 - 1.0, depth);
	vec4 view = INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	view.xyz /= view.w;
	float linear_depth = -view.z;

	vec4 world = INV_VIEW_MATRIX * INV_PROJECTION_MATRIX * vec4(ndc, 1.0);
	vec3 world_position = world.xyz / world.w;

	// Visualize linear depth
	//ALBEDO.rgb = vec3(fract(linear_depth));
	
	vec2 UV_new = SCREEN_UV - mod(SCREEN_UV, 1.0/VIEWPORT_SIZE * downsample_factor);
	vec2 resolution = VIEWPORT_SIZE;
	
	vec4 color = texture(screen_texture, UV_new).rgba;
	ALBEDO.rgb = color.rgb;
	ALPHA = color.a;

	// Visualize world coordinates
	//ALBEDO.rgb = fract(world_position).xyz;
}