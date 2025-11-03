sampler uImage0 : register(s0); // 原图像
// sampler bloom : register(s1);

// 需要手动传入
// x => 1 / width
// y => 1 / height
// z => width
// w => height
// float4 uImageTexelSize; 
// float luminanceThreshold;
// float bulurSize;

struct V2fLight
{
    float4 pos : SV_POSITION; // 顶点位置
    float2 uv : TEXCOORD0; // 纹理坐标
};

// 获取亮度的顶点着色器
V2fLight VertexLight(float4 pos : POSITION, float2 uv : TEXCOORD0)
{
    V2fLight o;
    o.uv = uv;
    o.pos = pos; // 正常应该要进行矩阵变换
    return o;
}

// 计算亮度的方法
float CountLight(float4 color)
{
    return 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
}

// 较亮区域
float4 PixelLight(V2fLight i) : COLOR0//SV_Target0 // 将输出存储起来
{
    float4 color = tex2D(uImage0, i.uv); // 获取纹理颜色
    float val = clamp(CountLight(color) - 0.2, 0.0, 1.0); //luminanceThreshold
    return color * val;
}

// 模糊 顶点输出结构体
struct V2fBlur
{
    float4 pos : SV_POSITION; // 顶点位置
    float2 uv[5] : TEXCOORD0; // 5个点的材质位置
};

// V2fBlur VertexBlurVertical(float4 pos : POSITION, float2 ctuv : TEXCOORD0)
// {
//     V2fBlur o;
//     o.pos = pos; // 可能要进行变换
//     float2 uv = ctuv;

//     // 上一个像素与下一个像素
//     o.uv[0] = uv;
//     o.uv[1] = uv + float2(0.0, uImageTexelSize.y * 1.0) * bulurSize; // 上
//     o.uv[2] = uv - float2(0.0, uImageTexelSize.y * 1.0) * bulurSize; // 下
//     o.uv[3] = uv + float2(0.0, uImageTexelSize.y * 2.0) * bulurSize; // 上
//     o.uv[4] = uv - float2(0.0, uImageTexelSize.y * 2.0) * bulurSize; // 下
//     return o;
// }

// V2fBlur VertexBlurHorizontal(float4 pos : POSITION, float2 ctuv : TEXCOORD0)
// {
//     V2fBlur o;
//     o.pos = pos; // 可能要进行变换
//     float2 uv = ctuv;

//     // 左右偏移
//     o.uv[0] = uv;
//     o.uv[1] = uv + float2(uImageTexelSize.x * 1.0, 0.0) * bulurSize; // 右偏1个像素
//     o.uv[2] = uv - float2(uImageTexelSize.x * 1.0, 0.0) * bulurSize; // 左偏1个像素
//     o.uv[3] = uv + float2(uImageTexelSize.x * 2.0, 0.0) * bulurSize; // 右偏2个像素
//     o.uv[4] = uv - float2(uImageTexelSize.x * 2.0, 0.0) * bulurSize; // 左偏2个像素
//     return o;
// }

// // 进行高斯模糊
// float4 FragBlur(V2fBlur vb) : SV_TARGET0
// {
//     float weight[3] = { 0.4026, 0.2442, 0.0545 }; // 高斯核权重
//     float3 oneColor = tex2D(uImage0, vb.uv[0]).rgb; // 获取中心点颜色

//     // 全部应用高斯核
//     float3 sum = oneColor * weight[0]; // 应用高斯核
//     for (int i = 1; i < 3; i++)
//     {
//         sum += tex2D(uImage0, vb.uv[2 * i - 1]).rgb * weight[i];
//         sum += tex2D(uImage0, vb.uv[2 * i]).rgb * weight[i];
//     }

//     return float4(sum, 1.0);
// }

// // Bloom
// struct V2Bloom
// {
//     float4 pos : SV_POSITION;
//     float4 uv : TEXCOORD0;
// };

// // 当前渲染到的顶点坐标，当前渲染到的材质坐标
// V2Bloom VertexBloom(float4 pos : POSITION, float4 uv : TEXCOORD0)
// {
//     V2Bloom v2;
//     v2.pos = pos;
//     v2.uv.xy = uv;
//     v2.uv.zw = uv;
//     return v2;
// }

// float4 PixelBloom(V2Bloom v2) : COLOR0 // 渲染到屏幕
// {
//     return tex2D(uImage0, v2.uv.xy) + tex2D(bloom, v2.uv.zw);
// }

technique Technique1
{
    pass LightPass
    {
        VertexShader = compile vs_2_0 VertexLight();
        PixelShader = compile ps_2_0 PixelLight();
    }

    // pass BlurVerticalPass
    // {
    //     VertexShader = compile vs_2_0 VertexBlurVertical();
    //     PixelShader = compile ps_2_0 FragBlur();
    // }

    // pass BlurHorizontalPass
    // {
    //     VertexShader = compile vs_2_0 VertexBlurHorizontal();
    //     PixelShader = compile ps_2_0 FragBlur();
    // }

    // pass BloomPass
    // {
    //     VertexShader = compile vs_2_0 VertexBloom();
    //     PixelShader = compile ps_2_0 PixelBloom();
    // }
}