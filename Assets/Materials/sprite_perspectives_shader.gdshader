shader_type spatial;
render_mode blend_mix, depth_draw_opaque, diffuse_burley, specular_schlick_ggx;

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

uniform sampler2D spritesheet: filter_nearest;
uniform int num_perspectives;
uniform int num_parts;

varying float uv_offset_y;
varying float uv_offset_x;
varying vec2 uv_offset;

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

mat3 axis_angle_mat3(vec3 axis, float angle) {
	float c = cos(angle);
	float s = sin(angle);
	
	float t = 1.0 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;
	
	return mat3(
        vec3(t * x * x + c,      t * x * y - s * z,  t * x * z + s * y),
        vec3(t * x * y + s * z,  t * y * y + c,      t * y * z - s * x),
        vec3(t * x * z - s * y,  t * y * z + s * x,  t * z * z + c)
    );
}

void vertex() {
	vec3 local_y = MODEL_MATRIX[1].xyz;
	vec3 dir_proj = -MODEL_MATRIX[2].xyz;
	
	vec3 dir_to_cam = CAMERA_POSITION_WORLD - MODEL_MATRIX[3].xyz;
	vec3 dir_to_cam_proj = project_on_plane(dir_to_cam, local_y);
	
	float rot_disp = signed_angle_to(dir_proj, dir_to_cam_proj, -local_y);
		//add extra perspective's angle since the spritesheet's perspectives
		//dont loop back to the 0-degree perspective to conserve memory usage
		//rot_disp += (TAU/float(num_perspectives)/2.0) * length(MODEL_MATRIX[1]);
		rot_disp += (TAU/float(num_perspectives)/2.0);
	//rot_2pi = rot_disp + TAU * float(rot_disp < 0.0);
	float rot_2pi = fma(TAU, float(rot_disp < 0.0), rot_disp);
	
	float t = round_to_precision(rot_2pi/TAU, 0.00001);
	float index = mod(floor(t * float(num_perspectives)), float(num_perspectives));
	uv_offset_y = index / float(num_perspectives);
	
	uv_offset_x = float(INSTANCE_ID) / float(num_parts);
	uv_offset = vec2(uv_offset_x, uv_offset_y);
	
	//applying scale
	vec3 scale = vec3(length(MODEL_MATRIX[0]), length(MODEL_MATRIX[1]), length(MODEL_MATRIX[2]));
	vec3 right = normalize(MODEL_MATRIX[0].xyz);
	vec3 up = normalize(MODEL_MATRIX[1].xyz);
	vec3 fwd = normalize(MODEL_MATRIX[1].xyz);
	VERTEX *= scale;
	
	//billboard along local y axis
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
			vec4(normalize(cross(up, MAIN_CAM_INV_VIEW_MATRIX[2].xyz)), 0.0),
			vec4(up, 0.0),
			vec4(normalize(cross(MAIN_CAM_INV_VIEW_MATRIX[0].xyz, up)), 0.0),
			MODEL_MATRIX[3]);
	MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
}

void fragment() {
	vec2 base_uv = UV;
	
	float uv_y = uv_offset_y + UV.y / float(num_perspectives);
	float uv_x = uv_offset_x + UV.x / float(num_parts);
	vec4 sprite_tex = texture(spritesheet, vec2(uv_x, uv_y));
	ALBEDO.rgb = albedo.rgb * sprite_tex.rgb;

	float metallic_tex = dot(texture(texture_metallic, base_uv), metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	SPECULAR = specular;

	vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
	float roughness_tex = dot(texture(texture_roughness, base_uv), roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	
	ALPHA = sprite_tex.a;
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
}
