//Rounded Box - exact   (https://www.shadertoy.com/view/4llXD7 and https://www.youtube.com/watch?v=s5NGeUV2EyU)
void sdRoundedBox_float( in float2 p, in float2 b, in float4 r, out float sdf)
{
    r.xy = (p.x>0.0)?r.xy : r.zw;
    r.x  = (p.y>0.0)?r.x  : r.y;
    float2 q = abs(p)-b+r.x;
    sdf = min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
}

void sinEasing_float( in float t, out float s)
{
	s = sin((t - 0.5) * 3.14) * 0.5 + 0.5;
}