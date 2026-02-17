using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Gameplay.Views.DamageFlyout
{
    public sealed class RendererParticleSystem
    {
        private readonly SymbolsTextureData _textureData = new SymbolsTextureData();

        private readonly Vector2[] _texCords = new Vector2[24];
        private readonly List<Vector4> _customData = new List<Vector4>();
        private readonly List<ParticleSystemVertexStream> _streams = new();

        private ParticleSystem.EmitParams _emitParams = new();

        private ParticleSystem _currentParticleSystem;
        private ParticleSystemRenderer _particleSystemRenderer;

        // Устанавливаем рабочую систему частиц
        public void SetParticleSystem(ParticleSystem particle)
        {
            _currentParticleSystem = particle;
            _particleSystemRenderer = _currentParticleSystem.GetComponent<ParticleSystemRenderer>();
        }

        /// <summary>
        /// Спавнит частицу для отображения текста урона или другого сообщения.
        /// Использует кастомные данные для передачи информации о символах в шейдер.
        /// </summary>
        /// <param name="position">Позиция появления частицы</param>
        /// <param name="message">Сообщение, которое нужно отобразить (максимум 23 символа)</param>
        /// <param name="color">Цвет отображаемого текста</param>
        /// <param name="startSize">Начальный масштаб текста (опционально)</param>
        /// <param name="startLifeTime">Время жизни частицы в секундах</param>
        public void SpawnParticle(Vector3 position, string message, Color color, float startSize,
                                  float startLifeTime = 1.5f)
        {
            Array.Clear(_texCords, 0, _texCords.Length);

            int messageLength;


            messageLength = Mathf.Min(23, message.Length);

            for (var i = 0; i < messageLength; i++)
            {
                _texCords[i] = _textureData.GetTextTextureCoordinates(message[i]);
            }


            // Передаем длину сообщения в последний элемент
            _texCords[^1] = new Vector2(messageLength, 0);

            var custom1Data = CreateCustomData(_texCords);
            var custom2Data = CreateCustomData(_texCords, 12);

            CheckSettings();

            //Устанавливаем startSize3D по X, чтобы символы не растягивались и не сжималисm при изменении длины
            // сообщения
            _emitParams.startColor = color;
            _emitParams.position = position;
            _emitParams.applyShapeToPosition = true;
            _emitParams.startSize3D = new Vector3(messageLength, 1f, 1f);
            _emitParams.startLifetime = startLifeTime;
            _emitParams.startSize3D = Vector3.one * startSize;

            _currentParticleSystem.Emit(_emitParams, 1);

            _customData.Clear();
            //Получаем поток ParticleSystemCustomData.Custom1 из ParticleSystem
            _currentParticleSystem.GetCustomParticleData(_customData, ParticleSystemCustomData.Custom1);
            //Меняем данные последнего элемента, т.е. той частицы, которую мы только что создали
            _customData[^1] = custom1Data;
            //Возвращаем данные в ParticleSystem
            _currentParticleSystem.SetCustomParticleData(_customData, ParticleSystemCustomData.Custom1);
            _currentParticleSystem.GetCustomParticleData(_customData, ParticleSystemCustomData.Custom2);

            _customData[^1] = custom2Data;
            _currentParticleSystem.SetCustomParticleData(_customData, ParticleSystemCustomData.Custom2);
        }

        // Метод создан для проверки и настройки активных вершинных потоков у рендера частиц.
        // Убеждается, что потоки UV2, Custom1 и Custom2 присутствуют,
        // чтобы кастомные данные, передаваемые из скрипта, корректно воспринимались шейдером.
        private void CheckSettings()
        {
            _streams.Clear();
            _particleSystemRenderer.GetActiveVertexStreams(_streams);

            //Добавляем лишний поток Vector2(UV2, SizeXY, etc.), чтобы координаты в скрипте соответствовали координатам
            // в шейдере
            if (!_streams.Contains(ParticleSystemVertexStream.UV2))
            {
                _streams.Add(ParticleSystemVertexStream.UV2);
            }

            if (!_streams.Contains(ParticleSystemVertexStream.Custom1XYZW))
            {
                _streams.Add(ParticleSystemVertexStream.Custom1XYZW);
            }

            if (!_streams.Contains(ParticleSystemVertexStream.Custom2XYZW))
            {
                _streams.Add(ParticleSystemVertexStream.Custom2XYZW);
            }

            _particleSystemRenderer.SetActiveVertexStreams(_streams);
        }

        private float PackFloat(Vector2[] vector)
        {
            if (vector == null || vector.Length == 0) return 0.0f;

            // Используем целые числа для избежания проблем с точностью float
            int result = 0;

            if (vector.Length > 0)
            {
                // Добавляем clamp для защиты от некорректных значений
                int x1 = Mathf.Clamp(Mathf.RoundToInt(vector[0].x), 0, 9);
                int y1 = Mathf.Clamp(Mathf.RoundToInt(vector[0].y), 0, 9);
                result += x1 * 1000000 + y1 * 100000;
            }

            if (vector.Length > 1)
            {
                int x2 = Mathf.Clamp(Mathf.RoundToInt(vector[1].x), 0, 9);
                int y2 = Mathf.Clamp(Mathf.RoundToInt(vector[1].y), 0, 9);
                result += x2 * 10000 + y2 * 1000;
            }

            if (vector.Length > 2)
            {
                int x3 = Mathf.Clamp(Mathf.RoundToInt(vector[2].x), 0, 9);
                int y3 = Mathf.Clamp(Mathf.RoundToInt(vector[2].y), 0, 9);
                result += x3 * 100 + y3 * 10;
            }

            // Добавляем проверку на overflow
            if (result is >= 0 and <= 9999999)
                return result;

            return 0.0f;
        }

        // Функция создания Vector4 для передачи в кастомный поток ParticleSystem (Custom1 или Custom2).
        // Каждый компонент Vector4 содержит упакованные координаты трёх символов с помощью PackFloat.
        // Используется для передачи до 12 символов в одном кастомном потоке.
        // Параметр offset позволяет задать смещение при чтении texCoords.
        private Vector4 CreateCustomData(Vector2[] texCoords, int offset = 0)
        {
            var data = Vector4.zero;
            for (var i = 0; i < 4; i++)
            {
                var vector = new Vector2[3];
                for (var j = 0; j < 3; j++)
                {
                    var ind = i * 3 + j + offset;
                    if (texCoords.Length > ind)
                        vector[j] = texCoords[ind];
                    else
                    {
                        data[i] = PackFloat(vector);
                        break;
                    }
                }

                data[i] = PackFloat(vector);
            }

            if (offset == 12 && texCoords.Length > 23)
            {
                // Передаем длину в w компоненте второго custom data
                data.w = texCoords[23].x; // длина сообщения
            }

            return data;
        }
    }
}