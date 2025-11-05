shader_type canvas_item;

uniform vec4 color_bottom = vec4(1.0, 0.5, 0.0, 1.0); // orange
uniform vec4 color_top    = vec4(1.0, 1.0, 1.0, 1.0); // white (fully visible)
uniform float wave_intensity = 0.05;  // horizontal flicker
uniform float wave_speed = 5.0;       // speed of flicker

void fragment() {
    // Particle-local UV coordinates
    vec2 uv = UV;

    // Horizontal wave flicker
    uv.x += sin(uv.y * 10.0 + TIME * wave_speed) * wave_intensity;

    // Gradient color (bottom → top)
    vec4 grad_color = mix(color_bottom, color_top, uv.y);

    // Sample particle texture
    vec4 tex = texture(TEXTURE, uv);

    // Apply gradient to texture
    COLOR = grad_color * tex;
}