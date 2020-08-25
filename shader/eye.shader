shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx, unshaded;

// Modified version of https://www.shadertoy.com/view/wsGXRR

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 eye_color_1 : hint_color = vec4(0.1, 0.7, 1.0, 1.0);
uniform vec4 eye_color_2 : hint_color = vec4(0.0, 1.0, 0.6, 1.0);
uniform vec4 eye_color_3 : hint_color = vec4(0.4, 0.9, 0.9, 1.0);
uniform vec4 eye_color_4 : hint_color = vec4(0.8, 0.5, 0.14, 1.0);
uniform vec4 eye_color_5 : hint_color = vec4(0.04, 0.22, 0.34, 1.0);
uniform vec4 eye_bg : hint_color = vec4(.95,.9, .88, 1.0);
uniform float eye_scale = 1.0;
uniform float speed = 3.0;
const float PI = 3.1415926;


float Hash(vec2 p, float scale)
{
	return fract(sin(dot(mod(p, scale), vec2(27.16898, 38.90563))) * 5151.5473453);
}

float Noise(vec2 p, float scale)
{
	vec2 f;
	p *= scale;
	f = fract(p);
	p = floor(p);
	f = f * f * (3.0 - 2.0 * f);
	return mix(mix(Hash(p, scale),
			Hash(p + vec2(1.0, 0.0), scale), f.x),
			mix(Hash(p + vec2(0.0, 1.0), scale),
			Hash(p + vec2(1.0, 1.0), scale), f.x), f.y);
}

float fBm(vec2 p)
{
	float f = 0.0;
	float scale = 10.0;
	p = mod(p, scale);
	float amp = 0.6;

	for(int i = 0; i < 5; i++)
	{
		f += Noise(p, scale) * amp;
		amp *= .5;
		scale *= 2.;
	}

	return min(f, 1.0);
}

// https://www.iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm

float sdPie(vec2 p, vec2 c, float r)
{
	p.x = abs(p.x);
	float l = length(p) - r;
	float m = length(p - c * clamp(dot(p, c), 0.0, r));
	return max(l, m * sign(c.y * p.x - c.x * p.y));
}

float sdArc(vec2 p, vec2 sca, vec2 scb, float ra, float rb)
{
	p *= mat2(vec2(sca.x, sca.y), vec2(-sca.y, sca.x));
	p.x = abs(p.x);
	float k = (scb.y * p.x > scb.x * p.y) ? dot(p.xy, scb) : length(p.xy);
	return sqrt(dot(p, p) + ra * ra - 2.0 * ra * k) - rb;
}

vec4 over(vec4 a, vec4 b)
{
	return mix(a, b, 1.0 - a.w);
}

vec4 createEffect(vec2 base_uv, float time)
{
	vec2 uv = (base_uv - resolution.xy * 0.5) / resolution.y * 2.0;
	uv.x *= 2.0;
	float l = length(uv / 0.3 / eye_scale);
	vec4 col = eye_color_5;

	if(l < 0.86)
	{
		vec3 main = vec3(0);
		float a = atan(uv.y, uv.x);
		float anim = sin(time * PI * 0.3) * 0.04;
		main += eye_color_1.xyz * smoothstep(0.0, 0.99,
				fBm(vec2(a / PI * 09. + sin(l * 29.0 + a * 11.0) *
				0.06, l * (2.00 + anim * 2.0)))) * 0.9;
		main += eye_color_2.xyz * smoothstep(0.5, 0.70,
				fBm(vec2(a / PI * 13.0 + sin(l * 29.0 + a * 11.0) *
				0.10, l * (1.50+anim))))*.3;
		main += eye_color_3.xyz * smoothstep(0.2, 0.96,
				fBm(vec2(a / PI * 11.0 + sin(l * 11.0 + a * 17.0) * 0.11,
				l * (1.50 + anim)))) * 0.3;
		float ta = PI * 0.3;
		float tb = PI - 2.5;
		float bulge = smoothstep(0.3, 0.0, abs(l - 0.5));
		float reflection = smoothstep(0.07, 0.0, sdArc(uv, vec2(sin(ta), cos(ta)),
				vec2(sin(tb), cos(tb)), 0.52, 0.01));
		col = over(vec4(main * 0.8 + bulge * 0.2 + reflection * 0.2,
				smoothstep(0.14, 0.001, l - 0.69)), col); // iris
		col = over(vec4(eye_color_4.xyz, smoothstep(0.0, 0.97,
				fBm(vec2(a / PI * 2.0, l + anim)) *
				(sin(PI + min(l, PI * 0.25) * PI * 4.0))) * 0.7), col); // iris brown blotch
		col = over(vec4(vec3(0.0), smoothstep(0.16, 0.02 - anim, l - 0.21)), col); // pupil black
        // col = over(vec4(vec3(0.96, 0.97, 0.99),
		//		smoothstep(0.024, 0.0, length(uv - vec2(-0.06, 0.13)) -
		//		0.04 - anim * 0.02) * 0.9), col); // pupil reflection
		// col = over(vec4(vec3(.96, .97, .99)*.2, smoothstep(.01, .0, length(uv-vec2(.07,-.14))-.01)), col); // pupil reflection tiny
	}

	return over(vec4(eye_bg.xyz, smoothstep(-0.04, 0.01, l - 0.83)), col); // eye skin;
}

void fragment()
{
	vec4 effect = createEffect(UV.xy, TIME * speed);
	ALBEDO = effect.rgb;
}