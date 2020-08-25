shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

// Modified version of http://glslsandbox.com/e#64420.2

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 bg_color : hint_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float triangleInnerCircleSize = 0.5;
uniform float triangleOuterCircleSize = 0.35;
uniform float effect_scale = 1.08;
// uniform int effect_alpha : hint_range(0, 2) = 2;
// uniform float alpha_tolerance = 0.2f;
uniform float line_width_1 = 0.02;
uniform float line_width_2 = 0.025;
uniform float symbol_width_1 = 0.05;
uniform float symbol_width_2 = 0.03;
uniform float symbol_circle_1_scale = 2.25;
uniform float symbol_circle_2_scale = 2.15;
uniform float symbol_circle_3_scale = 1.8;
uniform float symbol_circle_4_scale = 1.45;
uniform float symbol_circle_5_scale = 1.2;
uniform float symbol_circle_6_scale = 1.1;
uniform float symbol_circle_7_scale = 0.995;
uniform float symbol_circle_8_scale = 0.75;
uniform float speed = 0.5; // 0.3
uniform float color_multiplier = 2.0;
uniform float alpha_factor = 80.0;
uniform bool alpha_enabled = true;
const float PI = 3.1415926535897932384626433832795;
const float PIx5 = PI * 0.5;


vec2 rotate(vec2 v, float c, float s)
{
	return vec2(v.x * c - v.y * s, v.x * s + v.y * c);
}

vec2 rotateByRadius(vec2 v, float r)
{
	return rotate(v, cos(r), sin(r));
}

float boxLength(vec2 pos)
{
	vec2 q = abs(pos);
	return max(q.x, q.y);
}

float capsuleLength(vec2 pos, vec2 dir)
{
	vec2 ba = -dir;
	vec2 pa = pos + ba;
	ba *= 2.0;
	return length(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0));
} 

float triangleLength(vec2 p)
{
	p.y += 0.32;
	return max(abs(p.x * 1.8) + p.y, 1.0 - p.y * 1.8) * 0.75;
}

vec2 fracOrigin(vec2 v)
{
	return (fract(v) - 0.5) * 2.0;
}

float Bu(vec2 pos)
{
	float a = capsuleLength(pos + vec2(0.0, -0.5), vec2(1.0, 0.0));   
	float b = capsuleLength(pos + vec2(-0.3, 0.3), vec2(1.0, 1.0) * 0.707);  
	float c = length(pos + vec2(-1.3, -1.3));
	float d = length(pos + vec2(-1.8, -1.3));
	return min(min(min(a, b), c), d);
}

float Chi(vec2 pos)
{
	float a = capsuleLength(pos + vec2(0.0, -0.0), vec2(1.0, 0.0));   
	float b = capsuleLength(pos + vec2(0.0, -1.3), vec2(1.0, 0.8) * 0.4);  
	float c = capsuleLength(pos + vec2(0.0, -0.0), vec2(0.1, 1.0));  
	return min(min(a, b), c);
}

float To(vec2 pos)
{
	float a = capsuleLength(pos + vec2(0.5, 0.0), vec2(0.0, 1.0));   
	float b = capsuleLength(pos + vec2(0.0, 0.0), vec2(1.0, -0.8) * 0.4);    
	return min(a, b);
}

float Ba(vec2 pos)
{
	float a = capsuleLength(pos + vec2(0.8, 0.0), vec2(0.3, 1.0));   
	float b = capsuleLength(pos + vec2(-0.8, 0.0), vec2(-0.3, 1.0));     
	float c = length(pos + vec2(-1.3, -1.3));
	float d = length(pos + vec2(-1.8, -1.3));
	return min(min(min(a, b), c), d);
}

float Butitoba(vec2 pos, float power)
{
	float ret = 0.0 + power / Bu(pos)
			+ power / Chi(pos + vec2(-3.0, 0.0))
			+ power / To(pos + vec2(-6.0, 0.0))
			+ power / Ba(pos + vec2(-9.0, 0.0));
	return ret;
}

float smoothstepLine(float lower, float upper, float value, float width)
{
	width *= 0.5;
	return smoothstep(lower - width, lower, value) *
			(1.0 - smoothstep(upper, upper + width, value));
}

float smoothLine(float value, float target, float width)
{
	return width / abs(value - target);
}

vec2 smoothLine2(float value, float target, float width)
{
	return vec2(step(0.0, value - target), width / abs(value - target));
}

float circleTriangle(vec2 pos)
{
	float circle = length(pos * triangleInnerCircleSize);
	float triangle = triangleLength(pos * 0.3);    
	return smoothLine(circle, 1.0, line_width_2) +
			smoothLine(triangle, 1.0, line_width_2);
}

vec2 circleTriangle2(vec2 pos)
{
	float circle2 = length(pos * triangleOuterCircleSize);
	vec2 ret = smoothLine2(circle2, 1.0, line_width_2);
	ret.y += circleTriangle(pos);
	return ret;
}

float atan2(float y, in float x)
{
	return x == 0.0 ? sign(y) * PIx5 : atan(y, x);
}

vec2 polar(vec2 uv)
{
	float r = length(uv);
	float s = atan2(uv.y, uv.x) / PI;
	return vec2(r, s);
}

float ButitobaCircle(vec2 pos, float time)
{
	vec2 pp = polar(rotateByRadius(pos, -time) * symbol_circle_8_scale);
	return Butitoba(mod(rotateByRadius(pp * vec2(2.0, 32.0), PIx5),
			vec2(16.0, 4.0)) - 1.5, symbol_width_1) *
			smoothstepLine(6.0, 7.5, pp.x, 1.5);
}

float ButitobaCircle2(vec2 pos, float scale, float x, float y,
		float x2, float y2, float lower, float upper, float r)
{
	vec2 pp = polar(rotateByRadius(pos, r) * scale);
	return Butitoba(mod(rotateByRadius(pp * vec2(x, y), PIx5),
			vec2(x2, y2)) - 1.5, symbol_width_2) *
			smoothstepLine(lower, upper, pp.x, 0.2);
}

/*
float getAlpha(float alpha)
{
	float fixedAlpha = alpha >= alpha_tolerance ? alpha : 0.0;
	return fixedAlpha < 1.0 ? fixedAlpha : 1.0;
}
*/

float calculateAlpha(vec3 color)
{
	if(alpha_enabled)
	{
		float alpha = 0.0001 * alpha_factor *
			((color.r * color.g * color.b) * (color.r + color.g + color.b));
		return (alpha < 1.0 ? alpha : 1.0) * effect_color.a;
	}
	
	return 1.0;
}

/*
float calculateAlpha(vec3 color)
{
	float alpha;
	
	if(effect_alpha == 1)
		alpha = (color.r + color.g + color.b) / 3.0;
	else if(effect_alpha == 2)
		alpha = color.r * color.g * color.b;
	else
		alpha = 1.0;
	
	if(alpha < alpha_tolerance)
		alpha = 0.0;

	return min(alpha, 1.0);
}
*/

vec4 createEffect(vec2 base_uv, float time)
{
	vec2 uv = (base_uv.xy - resolution.xy * 0.5) / resolution.yy * 20.0;     
	uv *= effect_scale;
        
	uv = rotateByRadius(uv, time * speed);
    
	vec2 c2 = circleTriangle2(uv * 1.4 + vec2(0.0, 8.0));
	vec2 c3 = circleTriangle2(uv * 1.4 +
			rotateByRadius(vec2(0.0, 8.0), PI * 2.0 * 0.3333));
	vec2 c4 = circleTriangle2(uv * 1.4 +
			rotateByRadius(vec2(0.0, 8.0), PI * 2.0 * 0.6666));
    
	float mask = c2.x * c3.x * c4.x;
	float len = length(uv);
	vec3 eff = vec3(0.0);
    
	eff += (0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.0 + time * 0.5)) - 4.5)
		+ 0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.25 - time * 0.5)) - 4.5)
		+ 0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.5 + time * 0.5)) - 4.5)
		+ 0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.75 - time * 0.5)) - 4.5));

	eff += (ButitobaCircle(uv, time) +
			(ButitobaCircle2(uv, symbol_circle_7_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 5.0 + time * 0.2) +
			smoothLine(len, 10.0 + 0.25 * abs(sin(time)), line_width_1) +
			smoothLine(len, 7.75 + 0.25 * abs(cos(time)), line_width_1) +
			smoothLine(len, 2.75 + 7.75 * abs(mod(time * 0.8, 1.0)), line_width_1) +

			ButitobaCircle2(uv, symbol_circle_6_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 5.0 - time * 0.75) +
			ButitobaCircle2(uv, symbol_circle_5_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 15.0 + time * 0.275) +
			ButitobaCircle2(uv, symbol_circle_4_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 15.0 + time * 0.2418654) +
			
			smoothLine(len, 5.0, line_width_1) + smoothLine(len, 5.5, line_width_1) +

			ButitobaCircle2(uv, symbol_circle_2_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 35.0 + time * 0.34685) +
			ButitobaCircle2(uv, symbol_circle_1_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 135.0 + time * 0.114) +
			ButitobaCircle2(uv, symbol_circle_3_scale, 8.0, 64.0,
					12.0, 4.0, 7.5, 7.9, 532.0 + time * 0.54158) +
					
			0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.0 + time * 0.5)) - 4.5) +
			0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.25 - time * 0.5)) - 4.5) +
			0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.5 + time * 0.5)) - 4.5) +
			0.005 / abs(boxLength(rotateByRadius(uv, PIx5 * 0.75 - time * 0.5)) - 4.5) +
			0.1 / abs(boxLength(uv * vec2(8.0, 0.5) - vec2(0.0, 2.9)) - 1.0) +
			0.1 / abs(boxLength(rotateByRadius(uv, PI * 2.0 * 0.3333) * vec2(8.0, 0.5) - vec2(0.0, 2.9)) - 1.0) +
			0.1 / abs(boxLength(rotateByRadius(uv, PI * 2.0 * 0.6666) * vec2(8.0, 0.5) - vec2(0.0, 2.9)) - 1.0)) *
			mask + circleTriangle(uv) + c2.y + c3.y + c4.y);
	
	eff.x = eff.x > 8.0 ? 8.0 : eff.x;  
	eff.y = eff.y > 8.0 ? 8.0 : eff.y;
	eff.z = eff.z > 8.0 ? 8.0 : eff.z;
	vec3 color = bg_color.rgb + ((eff * effect_color.rgb) - (eff * bg_color.rgb));
	return vec4(color * color_multiplier, calculateAlpha(color));
}


void fragment()
{
	vec4 effect = createEffect(UV, TIME);
	ALBEDO = effect.rgb;
	ALPHA = effect.a;
}
