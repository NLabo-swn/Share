Shader "Custom/BoxFrame"
{
    Properties
    {
        // 塗りつぶし色（オブジェクト内部の色）
        _FillColor ("Fill Color", Color) = (1,1,1,1)
        // 枠線（稜線）色
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)
        // 枠線の太さ（オブジェクト空間の単位で扱う想定）
        // 注意: オブジェクトにスケールがある場合はスケールに応じて調整が必要
        _Thickness ("Edge Thickness (object units)", Float) = 0.02
        // ボックスの半寸法（オブジェクト空間）: x,y,z に半分のサイズを設定する
        _HalfExtents ("Half Extents (object)", Vector) = (0.5,0.5,0.5,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // マテリアルプロパティ（シェーダ内で使用する変数）
            fixed4 _FillColor;
            fixed4 _EdgeColor;
            float _Thickness;
            float3 _HalfExtents;

            // 頂点入力: メッシュのローカル頂点位置を使用
            struct appdata
            {
                float4 vertex : POSITION; // オブジェクト（ローカル）空間の頂点座標
            };

            // 頂点→フラグメントの受け渡し用
            struct v2f
            {
                float4 pos : SV_POSITION;   // クリップ空間位置（描画用）
                float3 localPos : TEXCOORD0; // オブジェクト空間の頂点位置をそのまま渡す
            };

            // 頂点シェーダ
            v2f vert (appdata v)
            {
                v2f o;
                // オブジェクト空間の頂点位置をそのままフラグメントへ渡す。
                // これによりシェーダの基準がオブジェクト内部となり、
                // オブジェクトを移動しても枠線がオブジェクトに追従する。
                o.localPos = v.vertex.xyz;

                // Unity の組み込み関数で頂点をクリップ空間に変換して描画位置を出力
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // フラグメントシェーダ
            fixed4 frag (v2f IN) : SV_Target
            {
                // オブジェクトの原点 (0,0,0) を箱の中心として扱うため、
                // ローカル位置の絶対値で各軸の距離を計算する。
                float3 local = IN.localPos;
                float3 d = abs(local); // 中心から各軸方向への正の距離

                // 各面（±X, ±Y, ±Z）から内側へ残っている距離を計算
                // 例えば faceDist.x が小さいほど ±X 面に近い
                float3 faceDist = _HalfExtents - d;

                // 各軸について枠線のしきい値（_Thickness）より近ければ近接とみなす
                // 近接は 1/0 フラグとして扱う
                int nearX = faceDist.x < _Thickness ? 1 : 0;
                int nearY = faceDist.y < _Thickness ? 1 : 0;
                int nearZ = faceDist.z < _Thickness ? 1 : 0;

                // 2面以上に近ければ稜線（エッジ）領域と判断する
                int nearCount = nearX + nearY + nearZ;

                if (nearCount >= 2)
                {
                    // 稜線（2面以上が交差するライン）をエッジ色で描画
                    return _EdgeColor;
                }
                else
                {
                    // それ以外は内部の塗り色
                    return _FillColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}