shader_type spatial;

uniform sampler2D spritesheet;
uniform int num_perspectives;

void vertex() {
	
}

vec3 project_on_plane(vec3 to_proj, vec3 plane_normal) {
	float d = dot(to_proj, plane_normal);
	vec3 proj_on_normal = plane_normal * d;
	return to_proj - proj_on_normal;
}

void fragment() {
	vec3 up = (MODEL_MATRIX * vec4(0, 1, 0, 0)).xyz;
	vec3 dir_proj = (MODEL_MATRIX * vec4(0, 0, -1, 0)).xyz;
	vec3 dir_to_cam = CAMERA_POSITION_WORLD - MODEL_MATRIX
	ALBEDO.rgb = texture(spritesheet, UV).rgb;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
