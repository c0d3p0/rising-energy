shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;
uniform float metallic = 0.0;
uniform float specular = 0.5;
uniform float roughness : hint_range(0,1) = 1.0;
uniform float point_size : hint_range(0,128);
uniform float normal_scale : hint_range(-16,16) = 1;
uniform vec3 vertex_displacement = vec3(1.0, 1.0, 1.0);
uniform vec3 uv1_scale = vec3(1.0, 1.0, 1.0);
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;
uniform vec4 metallic_texture_channel;
uniform vec4 roughness_texture_channel;
uniform vec4 albedo : hint_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D texture_albedo : hint_albedo;
uniform sampler2D texture_metallic : hint_white;
uniform sampler2D texture_roughness : hint_white;
uniform sampler2D texture_normal : hint_normal;
uniform sampler2D texture_displacement : hint_white;


void vertex()
{
	vec4 displacement = texture(texture_displacement, UV.xy);
	VERTEX += displacement.xyz * vertex_displacement / uv1_scale;
	UV=UV*uv1_scale.xy+uv1_offset.xy;
}




void fragment()
{
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	ALBEDO = albedo.rgb * albedo_tex.rgb;
	float metallic_tex = dot(texture(texture_metallic,base_uv),metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	float roughness_tex = dot(texture(texture_roughness,base_uv),roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	SPECULAR = specular;
	NORMALMAP = texture(texture_normal,base_uv).rgb;
	NORMALMAP_DEPTH = normal_scale;
}
