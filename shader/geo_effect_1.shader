shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

// Modified version of http://glslsandbox.com/e#64524.0

uniform vec2 resolution = vec2(1, 1);
uniform vec4 color_1 : hint_color = vec4(0.5, 0.0, 0.0, 1.0);
uniform vec4 color_2 : hint_color = vec4(0.7, 0.3, 0.0, 1.0);
uniform vec4 color_3 : hint_color = vec4(0.1, 0.0, 0.0, 1.0);
uniform vec4 color_4 : hint_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform vec4 color_override : hint_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float speed = 0.2;
uniform int effect_alpha = 0;


mat2 rot(float a)
{
	a *= 3.1415926;
	return mat2(vec2(cos(a), sin(a)), vec2(-sin(a), cos(a)));
}

float map(vec3 p, float time)
{
	float t = time * speed;
	p.xy *= rot(t * 2.0 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.7 * sin(0.2 * t));
	p = abs(p) - 3.0;
	p = abs(p) - 0.5;
	p = abs(p) - 5.0 * (sin(t) * 0.5 + 0.5);
	p.xy *= rot(t * 2.0 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.7 * sin(0.2 * t));
	p = abs(p) - 0.5;
	p = abs(p) - 5.0 * (sin(t * 3.0) * 0.5 + 0.5);
	p = abs(p) - 2.0;
	float d = 0.1;    
	d = max(d, dot(p, vec3(-1, -1, -1)));
	d = max(d, dot(p, vec3(1, -1, 1)));
	d = max(d, dot(p, vec3(1, 1, -1)));
	d = max(d, dot(p, vec3(-1, 1, 1)));
	return (d * 0.577 - 0.1);
}

float map2(vec3 p, float time)
{
	float t = time * speed;
	p.xy += rot(t * 0.5 + 0.3 * sin(0.5 * t)) * vec2(3);
	p.zx += rot(t * 1.5 + 0.2 * sin(0.2 * t)) * vec2(2);
	p.xy *= rot(t * 2.0 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.7 * sin(0.2 * t));
	p = abs(p) - 3.0;
	p = abs(p) - 0.5;
	p = abs(p) - 5.0 * (sin(t) * 0.5 + 0.5);
	p.xy *= rot(t * 2.0 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.7 * sin(0.2 * t));
	p = abs(p) - 0.5;
	p = abs(p) - 5.0 * (sin(t * 3.0) * 0.5 + 0.5);
	p = abs(p) - 2.0;
	return length(cross(p, normalize(vec3(1))));
}

float map3(vec3 p, float time)
{
	float t = time * speed;
	p.xy *= rot(t * 2.0 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.7 * sin(0.2 * t));
	p = abs(p) - 3.0;
	p = abs(p) - 0.5;
	p = abs(p) - 5.0 * (sin(t) * 0.5 + 0.5);
	p.xy *= rot(t * 0.5 + 0.3 * sin(0.5 * t));
	p.zx *= rot(t * 1.5 + 0.2 * sin(0.2 * t)); 
	p = abs(p) - 0.5;
	p = abs(p) - 8.0 * (sin(t * 3.0) * 0.5 + 0.5);
	p = abs(p) - 8.0 * (sin(t * 3.0) * 0.5 + 0.5);
	p = abs(p) - 2.0;
	return length(cross(p, normalize(vec3(1))));
}

float calculateAlpha(vec3 color)
{
	float alpha;
	
	if(effect_alpha == 1)
		alpha = color.r + color.g + color.b;
	else if(effect_alpha == 2)
		alpha = color.r * color.g * color.b;
	else
		alpha = 1.0;
		
	if(alpha > 1.0)
		alpha = 1.0;

	return alpha;
}

vec4 createEffect(vec2 base_uv, float time)
{
	vec2 p = (base_uv * 2.0 - resolution.xy) / resolution.y;
	vec3 col = color_override.rgb;
	vec3 rd = normalize(vec3(p, -2));
	vec3 ro = vec3(0, 0, 30);
	float t = 0.0;
	float d;

	for(int i = 0; i < 20; i++)
	{
		vec3 i_p = ro + rd * t;
		d = map(i_p, time) * 0.3;

		if(d < 0.01)
			break;

		t += d;
	}

	col += 0.05 / d * color_1.rgb / length(p) * 0.8;

	rd = normalize(vec3(p, -1.5));
	ro = vec3(1, 3, 25);
	t = 0.0;

	for(int i = 0; i < 30; i++)
	{
		vec3 i_p = ro + rd * t;
		d = map2(i_p, time) * 0.3;

		if(d < 0.01)
			break;

		t +=d;
	}

	col += 0.02 / d * color_2.rgb / length(p) * 0.5;

	rd = normalize(vec3(p, (1.0 - dot(p, p) * 0.5) * 0.5));
	ro = vec3(1, 3, 15);
	t = 0.0;

	for(int i = 0; i < 20; i++)
	{
		vec3 i_p = ro + rd * t;
		d = map3(i_p, time) * 0.3;

		if(d < 0.01)
			break;

		t +=d;
	}

	col += color_3.rgb * exp(-d * d * 50.);
	col = pow(col, vec3(1.2));
	t = time * speed * 5.0;
	col += color_4.rgb * sin(p.y * 500.0 - t) * sin(p.x * 300.0 - t) * 0.2;
	return vec4(col, calculateAlpha(col));
}

void fragment()
{
	vec2 base_uv = UV;
	vec4 effect = createEffect(base_uv, TIME);
	ALBEDO = effect.rgb;
	ALPHA = effect.a;
}
