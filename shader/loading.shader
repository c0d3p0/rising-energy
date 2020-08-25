shader_type canvas_item;
render_mode unshaded;

// Modified version of http://glslsandbox.com/e#55335.0

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(0.6, 0.08, 0.03, 1.0);
uniform int effect_alpha = 0;
uniform float size = 0.3;
uniform float speed = 3.0;
uniform float thickness = 1.9;
const float PI = 3.1415926536;
const int N = 120;


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
	vec2 position = (base_uv.xy / resolution.xy);
	vec2 center = position * 2.0 - 1.0;
	center.x *= resolution.x / resolution.y;
	float c = 0.0;
	float r = size;
	float o;

	for(int i = 0; i < N; i++)
	{
		vec2 xy;
		o = float(i) / float(N) * 2.0 * PI;
		xy.x = r * cos(o);
		xy.y = r * sin(o);
		xy += center;
		c += pow(300000.0, (1.0 - length(xy) * 3.0 / thickness) *
				(0.99 + 0.3 * fract(float(-i) / float(N) - (time * speed) * 0.5))) /
				3000000.0;
	}
	
	vec3 color = sqrt(c * effect_color.rgb);
	return vec4(color, calculateAlpha(color));
}

void fragment()
{
	COLOR = createEffect(UV, TIME);
}
