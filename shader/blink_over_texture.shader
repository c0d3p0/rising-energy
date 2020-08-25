shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_disabled,diffuse_burley,specular_schlick_ggx;
uniform vec4 albedo : hint_color;
uniform sampler2D texture_albedo : hint_albedo;
uniform float specular;
uniform float metallic;
uniform float alpha_scissor_threshold;
uniform float roughness : hint_range(0,1);
uniform float point_size : hint_range(0,128);
uniform sampler2D texture_metallic : hint_white;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_normal : hint_normal;
uniform float normal_scale : hint_range(-16,16);
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;

uniform float speed = 5.0;
uniform float range = 1.5;
uniform bool blink = false;
uniform vec4 blink_color : hint_color = vec4(0.1, 0.2, 0.9, 1);


vec4 createEffect(vec2 base_uv, float time)
{
    vec4 tex = texture(texture_albedo, base_uv);

    if (tex.a > 0.0 && blink)
        tex.rgb += max(blink_color.rgb * (range * cos(time * speed)), 0.0);
    
    return tex;
}
	
void vertex()
{
	UV = UV * uv1_scale.xy + uv1_offset.xy;
}

void fragment()
{
	vec2 base_uv = UV;
	vec4 albedo_tex = createEffect(base_uv, TIME);
	ALBEDO = albedo.rgb * albedo_tex.rgb;
	float metallic_tex = dot(texture(texture_metallic,base_uv),metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	ROUGHNESS = roughness;
	SPECULAR = specular;
	NORMALMAP = texture(texture_normal,base_uv).rgb;
	NORMALMAP_DEPTH = normal_scale;
	ALPHA = albedo.a * albedo_tex.a;
	ALPHA_SCISSOR=alpha_scissor_threshold;
}
