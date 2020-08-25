shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

// Modified version of http://glslsandbox.com/e#65253.1

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(0.2, 0.7, 2.3, 1.0);
uniform vec4 bg_color : hint_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float max_amplitude = 4.0;
uniform float thickness = 0.1;
uniform float alpha_factor = 1;
uniform bool alpha_enabled = false;
uniform float line_amount = 10.0;
const float PI = 3.141592654;


float calculateAlpha(vec3 color)
{
	if(alpha_enabled)
	{
		float alpha = alpha_factor * (color.r + color.g + color.b) *
				(color.r * color.g * color.b);
		return (alpha < 1.0 ? alpha : 1.0) * effect_color.a;
	}

	return 1.0;
}

vec3 mainImage(vec2 base_uv, float time)
{
	float t = time * 8.0;
	float ot1 = max_amplitude;
	float ot3 = 2.0;
	float ot5 = 0.1;
	float ot7 = 0.01;
	float ot9 = 0.025;
	float Q = 5000.0;
	vec2 uv = base_uv / resolution.xy;
	float amnt;
	float nd;
	float ip;
	float alpha;
	vec4 cbuff = bg_color;

	for(float i= 0.0; i < line_amount; i++)
	{
		ip = i - 2.0;
		nd = 2.0 / 4.0 * ot1 * sin(uv.x * 2.0 * 3.14 + ip * 0.4 + t * 0.05) / 2.0;

		nd += 1.0 / 4.0 * ot3 * sin(3.0 * uv.x * 2.0 * 3.14 + ip * 0.4) / 2.0;

		nd += 1.0 / 4.0 * ot5 * sin(5.0 * uv.x * 2.0 * 3.14 + ip * 0.4) / 2.0;

		nd += 1.0/4.0 * ot7 * sin(7.0 * uv.x * 2.0 * 3.14 + ip * 0.4) / 2.0;

		nd /= 5.0;
		nd += 0.5;
		amnt = thickness / abs(nd - uv.y) * 0.01;
		amnt = smoothstep(0.01, 0.5 + 10.0 * uv.y, amnt) * 5.5;
		alpha = (10.0 - i) / 5.0;
		cbuff += vec4(amnt * alpha * 0.3, amnt * 0.3 * alpha , amnt * uv.y * alpha, 0);
	}
	
	return vec3(cbuff[0] * effect_color.r, cbuff[1] * effect_color.g,
			cbuff[2] * effect_color.b);
}

vec4 createEffect(vec2 base_uv, float time)
{
	vec3 color = mainImage(base_uv.xy * 0.5, time);
	color += mainImage(base_uv.xy, time);
	color += mainImage(base_uv.xy * 2., time);
	color += mainImage(base_uv.xy * 4., time);
	color += mainImage(base_uv.xy * 8., time);
	return vec4(color, calculateAlpha(color));
}
	
void fragment()
{
	vec4 effect = createEffect(UV.xy, TIME);
	ALBEDO = effect.rgb;
	ALPHA = effect.a;
}