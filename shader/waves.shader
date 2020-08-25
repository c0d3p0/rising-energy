shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx, unshaded;

// Modified version of http://glslsandbox.com/e#65601.0

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effect_color : hint_color = vec4(0.2, 0.7, 2.3, 1.0);
uniform int iteration = 5;
uniform vec3 vertex_adjustment = vec3(0.05, 0.05, 0.05);
uniform float spectrum = 1.0;
uniform int effect_alpha = 0;
const float TAU = 6.28318530718;


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

vec4 calculateEffect(vec2 base_uv, float time)
{
    float finalTime = time * 5.2 + 25.0;
    vec2 uv = base_uv.xy / resolution.xy;

    vec2 p = mod(uv * TAU, TAU) - 500.0;
    vec2 i = vec2(p);
    float c = 1.0;
    float inten =  .005;

    for(int n = 0; n < iteration; n++)
	{
        float t = finalTime * (1.0 - (2.3 / float(n + 1)));
        i = p + vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
        c += 1.0 / length(vec2(p.x / (sin(i.x + t) / inten + spectrum), p.y / (cos(i.y + t) / inten)));
    }

    c /= float(iteration);
    c = 1.1-pow(c, 1.5);
    vec3 color = vec3(pow(abs(c), 9.0));
    return vec4(color * effect_color.xyz, calculateAlpha(color));
}

void vertex()
{
	vec4 effect = calculateEffect(UV.xy, TIME);
	vec4 adjustment = log2(effect) * vec4(vertex_adjustment, 1.0);
	// vec4 adjustment = abs(1.0 (1.0  - (1.0 / effect)) * deformation);
	
	VERTEX.xyz += adjustment.xyz;
}

void fragment()
{
	vec4 effect = calculateEffect(UV.xy, TIME);
    ALBEDO = effect.rgb;
	ALPHA = effect.a;
}

