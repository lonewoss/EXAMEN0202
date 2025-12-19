using System;
using EXAMEN0202.Models;

namespace EXAMEN0202.Services
{
    public static class Calculator
    {
        // Тарифы из задания (руб/м²)
        public const decimal AluminumRate = 15.50m;
        public const decimal PlasticRate = 9.90m;

        // Простой расчёт: S = W * H, Цена = S * Тариф
        public static decimal CalculateCost(decimal width, decimal height, MaterialType material)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Размеры должны быть больше 0.");

            decimal area = width * height;
            decimal rate = material == MaterialType.Aluminum ? AluminumRate : PlasticRate;
            decimal cost = area * rate;

            // Округление до копеек
            return Math.Round(cost, 2, MidpointRounding.AwayFromZero);
        }
    }
}