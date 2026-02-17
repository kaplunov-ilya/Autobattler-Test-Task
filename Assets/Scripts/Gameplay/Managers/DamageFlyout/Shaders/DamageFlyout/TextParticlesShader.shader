Shader "Custom/StableTextParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cols ("Columns Count", Int) = 10
        _Rows ("Rows Count", Int) = 10
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "PreviewType"="Plane" "Queue" = "Transparent"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 customData1 : TEXCOORD1;
                float4 customData2 : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 customData1 : TEXCOORD1;
                float4 customData2 : TEXCOORD2;
                float textLength : TEXCOORD3;
            };

            sampler2D _MainTex;
            uint _Cols;
            uint _Rows;

            v2f vert(appdata v)
            {
                v2f o;

                // Извлекаем и валидируем длину текста
                float textLength = v.customData2.w;
                
                // Проверка на валидность данных с более жесткими границами
                if (textLength < 1.0 || textLength > 23.0 || isnan(textLength) || isinf(textLength))
                {
                    textLength = 1.0; // Безопасное значение по умолчанию
                }

                o.textLength = textLength;

                // масштабирование UV
                o.uv.x = v.uv.x * (textLength / (float)_Cols);
                o.uv.y = v.uv.y / (float)_Rows;
                o.uv.zw = v.uv.zw;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.customData1 = v.customData1;
                o.customData2 = v.customData2;

                return o;
            }

            // функция для распаковки координат с защитой от overflow
            float2 UnpackCoordinates(float packedData, uint localIndex)
            {
                // преобразование с защитой от overflow
                int data = (int)clamp(packedData, 0.0, 9999999.0);
                
                float2 coords = float2(0.0, 0.0);
                
                if (localIndex == 0)
                {
                    coords.x = (float)((data / 1000000) % 10);
                    coords.y = (float)((data / 100000) % 10);
                }
                else if (localIndex == 1)
                {
                    coords.x = (float)((data / 10000) % 10);
                    coords.y = (float)((data / 1000) % 10);
                }
                else // localIndex == 2
                {
                    coords.x = (float)((data / 100) % 10);
                    coords.y = (float)((data / 10) % 10);
                }
                
                return coords;
            }

            fixed4 frag(v2f v) : SV_Target
            {
                float2 uv = v.uv.xy;
                float textLength = v.textLength;
                
                // Проверка валидности UV с небольшим допуском
                if (uv.x < -0.001 || uv.x > 1.001 || uv.y < -0.001 || uv.y > 1.001)
                {
                    discard;
                }
                
                // вычисление индекса символа
                float symbolIndexFloat = uv.x * (float)_Cols;
                uint symbolIndex = (uint)clamp(floor(symbolIndexFloat + 0.0001), 0, _Cols - 1);
                
                // Проверяем границы с небольшим допуском
                if (symbolIndex >= (uint)textLength)
                {
                    discard;
                }

                // Определяем, в каком Vector4 хранятся данные
                uint dataIndex = symbolIndex / 3;
                uint localIndex = symbolIndex % 3;
                
                // Получаем упакованные данные
                float packedData = 0.0;
                
                if (dataIndex < 4)
                {
                    packedData = v.customData1[dataIndex];
                }
                else if (dataIndex < 8)
                {
                    packedData = v.customData2[dataIndex - 4];
                }
                else
                {
                    // Если данные вне допустимого диапазона, отбрасываем
                    discard;
                }
                
                // Проверяем валидность упакованных данных
                if (isnan(packedData) || isinf(packedData) || packedData < 0.0)
                {
                    discard;
                }
                
                // Распаковываем координаты
                float2 symbolCoords = UnpackCoordinates(packedData, localIndex);
                
                // Проверяем валидность координат
                if (symbolCoords.x < 0.0 || symbolCoords.x >= (float)_Cols || 
                    symbolCoords.y < 0.0 || symbolCoords.y >= (float)_Rows)
                {
                    discard;
                }
                
                // Вычисляем финальные UV координаты в атласе
                float invCols = 1.0 / (float)_Cols;
                float invRows = 1.0 / (float)_Rows;
                
                // вычисления frac с защитой от граничных значений
                float scaledX = symbolIndexFloat;
                float scaledY = uv.y * (float)_Rows;
                
                float fracX = scaledX - floor(scaledX);
                float fracY = scaledY - floor(scaledY);
                
                // Защита от граничных значений
                fracX = clamp(fracX, 0.0, 0.999);
                fracY = clamp(fracY, 0.0, 0.999);
                
                float2 finalUV;
                finalUV.x = fracX * invCols + symbolCoords.x * invCols;
                finalUV.y = fracY * invRows + symbolCoords.y * invRows;
                
                // Дополнительная проверка финальных UV
                finalUV = clamp(finalUV, 0.0, 1.0);
                
                // Финальная выборка из текстуры
                fixed4 col = tex2D(_MainTex, finalUV) * v.color;
                
                // Альфа-тест
                clip(col.a - 0.01);
                
                return col;
            }
            ENDCG
        }
    }
}