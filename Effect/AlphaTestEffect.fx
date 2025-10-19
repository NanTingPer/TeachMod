sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float alpha;

//sampleColor -> ��COLOR0ȡ��ɫ
//coords -> ��ȡ��ǰ���Ʋ��ʵ�����
float4 PixelShaderF(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 currentPixel = tex2D(uImage0, coords);
    if (currentPixel.a > alpha)
    {
        return currentPixel;
    }
    else
    {
        return float4(0, 0, 0, 0);
    }
}

technique Technique1
{
    //0
    pass AlphaPass
    {
        PixelShader = compile ps_2_0 PixelShaderF();
    }
}