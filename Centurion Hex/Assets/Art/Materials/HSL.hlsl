void RGBtoHSL_float(float3 In, out float H, out float S, out float L) {
    // Find min and max RGB components
    float minRGB = min(min(In.r, In.g), In.b);
    float maxRGB = max(max(In.r, In.g), In.b);
    float delta = maxRGB - minRGB;

    // Calculate Lightness
    L = (maxRGB + minRGB) / 2.0;

    H = 0.0; // Hue
    S = 0.0; // Saturation

    if (delta != 0) {
        // Calculate Saturation
        S = (L < 0.5) ? (delta / (maxRGB + minRGB)) : (delta / (2.0 - maxRGB - minRGB));

        // Calculate Hue
        if (In.r == maxRGB) {
            H = ((In.g - In.b) / 6) / delta;
        } else if (In.g == maxRGB) {
            H = .33333 + ((In.b - In.r) / 6) / delta;
        } else if (In.b == maxRGB) {
            H = .66666 + ((In.r - In.g) / 6) / delta;
        }
    }
    // Normalize Hue to [0, 1]
    if (H < 0) {
        H += 1;
    } else if (H > 1)        H -= 1;}void HSLAtoRGBA_float(float H, float S, float L, float A, out float4 RGBA){    if (S == 0.0) {
        // If saturation is zero, the color is a shade of grey
        RGBA = float4(L, L, L, A);
    } else {
        float q = L < 0.5 ? L * (1.0 + S) : L + S - L * S;
        float p = 2.0 * L - q;
        float3 trgb = float3(H + 1.0 / 3.0, H, H - 1.0 / 3.0);

        for (int i = 0; i < 3; i++) {
            if (trgb[i] < 0.0) trgb[i] += 1.0;
            if (trgb[i] > 1.0) trgb[i] -= 1.0;

            if (trgb[i] < 1.0 / 6.0)
                RGBA[i] = p + (q - p) * 6.0 * trgb[i];
            else if (trgb[i] < 0.5)
                RGBA[i] = q;
            else if (trgb[i] < 2.0 / 3.0)
                RGBA[i] = p + (q - p) * (2.0 / 3.0 - trgb[i]) * 6.0;
            else
                RGBA[i] = p;
        }
        RGBA.a = A;
    }}
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
void RGBtoHSL2_float(float3 In, out float H, out float S, out float L) {
    float3 hsv = RGBToHSV(In);
    H = hsv.x;
    S = hsv.y;
    L = hsv.z;
}
void HSLAtoRGBA2_float(float H, float S, float L, float A, out float4 RGBA){    RGBA.rgb = HSVToRGB(float3 (H, S, L));    RGBA.a = A;}