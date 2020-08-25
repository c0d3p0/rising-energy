shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

// Modified version of https://www.shadertoy.com/view/wslBWs

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 bg_color : hint_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(1.0, 1.0, 1.0, 1.0);
// uniform int effect_alpha = 0;
uniform float speed = 1.0;
uniform float rotation_factor = 1.0;
uniform bool alpha_enabled = true;
const float PI = 3.1415926535897932384626433832795028;
const float TAU = 6.2831853071;



// reference for animation curves: https://easings.net/
float easeInOutCirc(float x)
{
	return x < 0.5
		? (1.0 - sqrt(1.0 - pow(2.0 * x, 2.0))) / 2.0
		: (sqrt(1.0 - pow(-2.0 * x + 2.0, 2.0)) + 1.0) / 2.0;
}

mat2 rot(float a)
{
	return mat2(vec2(cos(a), sin(a)), vec2(-sin(a), cos(a)));
}

float aaStep(float thre, float val)
{
	return smoothstep(-0.7, 0.7, (val - thre) /
			min(0.05, fwidth(val - thre)));
}

float dt(float time)
{
	return easeInOutCirc(fract(time * speed / 2.0));
}

float anim(float time)
{
	return ((TAU / 4.0) * dt(time)) * rotation_factor;
}

void moda(inout vec2 p, float rep)
{
	float per = TAU / rep;
	float a = atan(p.y, p.x);
	a = mod(a - per * 0.5, per) - per * 0.5;
	p = vec2(cos(a), sin(a)) * length(p);
}

float circle(vec2 p, float r)
{
	return aaStep(r, length(p));
}

float circleEmpty(vec2 p, float size, float thick)
{
	return 1.0 - (circle(p, size) + (1.0 - circle(p, size * thick)));
}

float square(vec2 p, float r)
{
	p = abs(p);
	return aaStep(r, max(p.x, p.y));
}

float squareEmpty(vec2 p, float size)
{
	return 1.0 - (square(p, size) + (1.0 - square(p, size * 0.95)));
}

float moon(vec2 p)
{
	float rad = 0.3;
	return 1.0 - (circle(p, rad) + (1.0 - circle(p + vec2(rad * 0.3, 0.0), rad * 0.75)));
}

vec2 sigles(vec2 p, float spread, float time)
{
	p *= rot(anim(time));
	moda(p, 4.0);
	p.x -= spread;
	float mask = circle(p, 0.37);
	return vec2(circleEmpty(p, 0.34, 0.8) +
			moon(abs(abs(p * 3.5) - 0.1) - 0.3), mask);
}

float artefact(vec2 p, float time)
{
	float img = 0.0;
	float s = 0.2;
	vec2 pp = p;

	for (int i = 0; i < 5; i++)
	{
		float ratio = float(i) / 5.0;
		pp *= rot(TAU / 10.0);
		pp *= rot(-anim(time)); 
		s += 0.15;
		img += squareEmpty(pp, s);
	}

	float spread = 1.45;
	vec2 si = sigles(p, spread * 1.05, time);
    return img + moon(p) +
        (circleEmpty(p, spread, 0.98) +
		circleEmpty(p, spread * 1.1, 0.98)) *
		si.y + si.x;
}

/*
float calculateAlpha(vec3 color)
{
	float alpha;
	
	if(effect_alpha == 1)
		alpha = color.r + color.g + color.b;
	else if(effect_alpha == 2)
		alpha = color.r * color.g * color.b;
	else if (effect_alpha == 3)
		alpha = 0.0;
	else
		alpha = 1.0;

	return alpha < 1.0 ? alpha : 1.0;
}
*/

float calculateAlpha(vec3 color)
{
	if(alpha_enabled)
	{
		float alpha = color.r + color.g + color.b;
		return (alpha < 1.0 ? alpha : 1.0) * effect_color.a;
	}
	
	return 1.0;
}

vec4 createEffect(vec2 base_uv, float time)
{
	vec2 uv = (2.0 * base_uv - resolution.xy) / resolution.y;
	vec3 effect = vec3(artefact(uv * 2.0, time));
	vec3 color = bg_color.rgb +
			((effect * effect_color.rgb) - (effect * bg_color.rgb)); 
	return vec4(color, calculateAlpha(color));
}

void fragment()
{
	vec4 effect = createEffect(UV, TIME);
	ALBEDO = effect.rgb;
	ALPHA = effect.a;
}
