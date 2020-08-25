shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx, unshaded;

// Modified version of https://www.shadertoy.com/view/4l23zd

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float speed = 5.0;
uniform float range = 10.0;


vec4 createEffect(float time)
{
	vec4 color = effect_color;
	color.rgb += range * effect_color.rgb * (0.1 * cos(time * speed));
	// color.rgb += minimum.rgb * (0.1 * cos(time * 5.0));
	return color;
}


void fragment()
{
	vec4 effect = createEffect(TIME);
	ALBEDO = effect.rgb;
}
