shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

// Modified version of http://glslsandbox.com/e#59532.1

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(1.0, 0.5, 0.5, 1.0);
uniform float alpha_tolerance = 0.7;
uniform float speed = 2.0;
uniform int ring_amount = 3;
uniform float size = 1.9f;
uniform float ring_thickness = 1.0;
uniform float border_thickness = 1.0;
varying float startTime;



float getAlpha(float alpha)
{
	float fixedAlpha = alpha >= alpha_tolerance ? alpha : 0.0;
	return fixedAlpha < 1.0 ? fixedAlpha : 1.0;
}


vec4 createEffect(vec2 base_uv, float time)
{
	vec2 p = (base_uv.xy * 2.0 - resolution) / min(resolution.x, resolution.y) / size;

	// float c0 = 0.02 / abs(1.0 - length(p) - 0.7);
	float c = 0.0;

	for(int i = 0; i < ring_amount; i++)
	{
		float t = speed * (time / 3.0 + (float(i) / float(ring_amount)));
		float w = (ring_thickness - fract(t)) * 0.0125;
		c += ((w / abs(length(p) - (fract(t) * 0.2 + 0.3))) * fract(-t));
	}
	
	vec4 color = min(c / (border_thickness + 1.0), 1.0) * effect_color;
	color.a = getAlpha(c) * effect_color.a;
	// color.a = calculateFade(time);
	return color;
}

void vertex()
{
	startTime = TIME;
}

void fragment()
{
	if(effect_color.a > 0.0)
	{
		vec4 effect = createEffect(UV, TIME);
		ALBEDO = effect.rgb;
		ALPHA = effect.a;
	}
	else
		ALPHA = effect_color.a;
}
