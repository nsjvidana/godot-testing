shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo : source_color, filter_linear_mipmap, repeat_enable;
uniform float alpha_scissor_threshold : hint_range(0.0, 1.0, 0.001);
uniform float point_size : hint_range(0.1, 128.0, 0.1);

uniform float roughness : hint_range(0.0, 1.0);
uniform sampler2D texture_metallic : hint_default_white, filter_linear_mipmap, repeat_enable;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_roughness_r, filter_linear_mipmap, repeat_enable;

uniform float specular : hint_range(0.0, 1.0, 0.01);
uniform float metallic : hint_range(0.0, 1.0, 0.01);

uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;

uniform sampler2D spritesheet: filter_nearest;
uniform int num_perspectives;
uniform int sprite_size;

varying float uv_offset_x;

vec3 project_on_plane(vec3 to_proj, vec3 plane_normal) {
	float d = dot(to_proj, plane_normal);
	vec3 proj_on_normal = plane_normal * d;
	return to_proj - proj_on_normal;
}

float signed_angle_to(vec3 from, vec3 to, vec3 axis) {
    vec3 cros = cross(from, to);
    float angle = atan(length(cros), dot(from, to));
	float angle_sign = 1.0 - 2.0 * float(dot(cros, axis) < 0.0);
	return angle * angle_sign;
}

float round_to_precision(float value, float precision) {
	return round(value * (1.0/precision)) * precision;
}

void vertex() {
	mat3 rot = mat3(MODEL_MATRIX);
	vec3 up = (rot * vec3(0, 1, 0)).xyz;
	vec3 dir_proj = (rot * vec3(0, 0, -1)).xyz;
	
	vec3 dir_to_cam = CAMERA_POSITION_WORLD - MODEL_MATRIX[3].xyz;
	vec3 dir_to_cam_proj = project_on_plane(dir_to_cam, up);
	
	float rot_disp = signed_angle_to(dir_proj, dir_to_cam_proj, -up);
	//todo: try using fma() for this
	float rot_2pi = rot_disp + TAU * float(rot_disp < 0.0);
	
	float t = round_to_precision(rot_2pi/TAU, 0.00001);
	float index = mod(round(t * float(num_perspectives)), float(num_perspectives));
	uv_offset_x = index / float(num_perspectives);
	
	//todo: calculate UV y coord offset for full sprite armature
	//	(might wanna make uv_offset_x into a single vec2)
	
	
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
			MAIN_CAM_INV_VIEW_MATRIX[0],
			MAIN_CAM_INV_VIEW_MATRIX[1],
			MAIN_CAM_INV_VIEW_MATRIX[2],
			MODEL_MATRIX[3]);
	MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
}

void fragment() {
	vec2 base_uv = UV;

	float uv_x = uv_offset_x + UV.x / float(num_perspectives);
	vec4 sprite_tex = texture(spritesheet, vec2(uv_x, UV.y));
	ALBEDO.rgb = sprite_tex.rgb;

	float metallic_tex = dot(texture(texture_metallic, base_uv), metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	SPECULAR = specular;

	vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
	float roughness_tex = dot(texture(texture_roughness, base_uv), roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	
	ALPHA = sprite_tex.a;
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
}
