shader_type spatial;

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
}

void fragment() {
	float uv_x = uv_offset_x + UV.x / float(num_perspectives);
	vec4 color = texture(spritesheet, vec2(uv_x, UV.y));
	ALBEDO.rgb = color.rgb;
	ALPHA = color.a;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
