shader_type canvas_item;
render_mode unshaded;

// Modified version of https://www.shadertoy.com/view/wtfyzf

uniform vec2 resolution = vec2(1.0, 1.0);
uniform float speed = 0.1;


// Angle => 2D rotation matrix
mat2 rot2d(float a)
{
	return mat2(vec2(cos(a), sin(a)), vec2(-sin(a), cos(a)));
}

// https://www.iquilezles.org/www/articles/fbm/fbm.htm
float terrain_height(vec2 position)
{
	float height = 0.0;
	float amplitude = 0.8;
	float n;

	// Sum up 20 layers
	for(int i = 0; i < 20; i++) 
	{
		n = sin(position.x) * cos(position.y);
		n = n * n * n; // Simple base pattern, no noise
		height += n * amplitude; // Accumulate value at current amplitude
		position *= rot2d(float(i * i)); // Rotate (phase-shift?) by a hash of the current iteration
		position *= 1.4; // Scale (increase frequency by ~1/2 an octave)
		amplitude *= 0.66; // Reduce amplitude
	}

	return height;
}

vec4 createEffect(vec2 base_uv, float time2)
{
	float nTime = time2 * speed;
	vec2 uv = (base_uv.xy * 2. - resolution.xy) / min(resolution.x, resolution.y);
	vec3 water_tint = vec3(1);
	vec3 p;
	vec3 ray_origin = vec3(-cos(nTime * 0.5) * 4.0, 1.0, nTime); // Camera position
	vec3 ray_direction = normalize(vec3(uv.x, uv.y - 0.7 + cos(nTime) * 0.5, 1.7));
	ray_direction.xy *= rot2d(sin(nTime * 0.5) * 0.5); // Camera orientation and field of view
	float dist = 0.0;
	float ray_length = 0.0;

	// Raymarching loop
	for(int i = 0; i < 75; i++)
	{
		p = ray_origin + ray_direction * ray_length; // Get current ray position

		// If we're close to the surface check p.y for a water hit
		if(dist < 0.1 && p.y < 0.0)
		{
			ray_origin.y = -ray_origin.y;
			ray_direction.y = -ray_direction.y; // Reflect ray
			water_tint = vec3(0.78, 0.85, 0.8); // Make water greener and darker
		}
		else // Not underwater - march
		{
			// Real distance is hard to calculate so use p's
			// height above the terrain, and since that's obviously
			dist = p.y-terrain_height(p.xz);
			// wrong - only march .6 of the way and hope for the best.
			ray_length += dist * 0.6;
		}
		
		// We are far away (in the sky)
		if(ray_length > 20.0)
			break;
	}

	// From https://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
	vec2 h = vec2(0.0001, 0);
	vec3 surface_normal = normalize(
			vec3(terrain_height(p.xz - h.xy) - terrain_height(p.xz + h.xy),
			2.0 * h.x,
			terrain_height(p.xz - h.yx) - terrain_height(p.xz + h.yx)));

	// Terrain, backlit
	vec3 terrain_color = vec3(0.9, 0.7, 0.6) * (surface_normal.z * 0.5 + 0.5);
	// Sunny horizon
	vec3 sky_color = mix(vec3(1.2, 1.0, 0.9), vec3(0.4,0.5,0.6), abs(ray_direction.y));
	
	// Reuse terrain_height() FBM to paint some clouds
	if(ray_length > 20.0)
	{
		vec2 cloud_uv = ray_direction.xz / ray_direction.y + nTime;
		sky_color *= 1. - min(0., terrain_height(cloud_uv) * ray_direction.y);
	}
	
	// Mountains-sky fade
	vec3 color = mix(terrain_color, sky_color, min(1.0, ray_length / 20.0));
	// Water color
	color *= water_tint;
	// Gamma correction
	return vec4(pow(color, vec3(.4545)), 1);
}

void fragment()
{
	COLOR = createEffect(UV, TIME);
}
