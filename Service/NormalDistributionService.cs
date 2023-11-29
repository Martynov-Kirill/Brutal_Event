using System;
using System.Collections.Generic;
using System.Linq;
using BrutalEvent.Service.Interface;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BrutalEvent.Service
{
    public class NormalDistributionService : INormalDistributionService
    {
        public float CalculateRarity(SelectableLevel currentLevel, float currentRate)
        {
            var players = RoundManager.Instance.playersManager.ClientPlayerList.Count;
            // Определяем среднюю (mu) и стандартное отклонение (sigma)
            float muCof = (float)Math.Pow(Math.PI, players);
            float mu = currentRate + players * muCof;
            float sigma = currentRate / players; 

            // Расчет редкости
            float rarity = NormalDistribution(mu, sigma, players);

            // Корректировка редкости в диапазоне от 0 до 100
            return Mathf.Clamp(rarity, 0, 1000);
        }

        public float NormalDistribution(float mu, float sigma, int players)
        {
            // Генерация значений с нормальным распределением
            float sum = 0f;
            var average = new List<float>();
            for (int i = 0; i < players; i++)
            {
                average.Add(Random.Range(0f, 1f));
                sum += average.LastOrDefault();
            }

            float normalValue = sum - (average.Sum() / average.Count);

            // Применяем масштабирование и смещение для получения нужного распределения
            return mu + sigma * normalValue;
        }
    }
}
