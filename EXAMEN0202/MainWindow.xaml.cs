using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using EXAMEN0202.Models;
using EXAMEN0202.Services;

namespace EXAMEN0202
{
    public partial class MainWindow : Window
    {
        private decimal _lastWidth;
        private decimal _lastHeight;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetDimensions(out decimal w, out decimal h))
                return;

            MaterialType material = GetSelectedMaterial();

            decimal cost = Calculator.CalculateCost(w, h, material);

            // Сохраняем последние значения (для квитанции)
            _lastWidth = w;
            _lastHeight = h;

            SizeText.Text = $"{w:0.00}х{h:0.00}";
            MaterialText.Text = material == MaterialType.Aluminum ? "Алюминий" : "Пластик";
            CostText.Text = $"{cost:0.00} руб.";

            ReceiptButton.IsEnabled = true; // квитанцию можно делать после расчёта
        }

        private void ReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            // По заданию: сформировать чек для каждого вида материала
            try
            {
                var materials = new[] { MaterialType.Aluminum, MaterialType.Plastic };

                string[] saved = materials.Select(m =>
                {
                    decimal cost = Calculator.CalculateCost(_lastWidth, _lastHeight, m);
                    return ReceiptService.CreateAndSaveReceipt(_lastWidth, _lastHeight, m, cost);
                }).ToArray();

                MessageBox.Show(
                    "Квитанции сохранены:\n\n" + string.Join("\n", saved),
                    "Готово",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private MaterialType GetSelectedMaterial()
        {
            // Если индекс 0 — алюминий, иначе пластик
            return MaterialBox.SelectedIndex == 0 ? MaterialType.Aluminum : MaterialType.Plastic;
        }

        private bool TryGetDimensions(out decimal w, out decimal h)
        {
            w = 0;
            h = 0;

            // 1) Проверяем пустые поля (требование тестов)
            if (string.IsNullOrWhiteSpace(WidthBox.Text) || string.IsNullOrWhiteSpace(HeightBox.Text))
            {
                MessageBox.Show("Заполните оба поля размера.", "Проверка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 2) Парсим числа (поддержка “,” и “.”)
            if (!TryParseDecimal(WidthBox.Text, out w) || !TryParseDecimal(HeightBox.Text, out h))
            {
                MessageBox.Show("Введите корректные числа (пример: 15.00 или 15,00).", "Проверка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 3) Проверяем отрицательные/нулевые (требование тестов)
            if (w <= 0 || h <= 0)
            {
                MessageBox.Show("Размеры должны быть больше 0.", "Проверка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool TryParseDecimal(string input, out decimal value)
        {
            value = 0;

            // Быстрый фикс для случаев, когда пользователь ставит запятую
            string normalized = input.Trim().Replace(',', '.');

            // Пытаемся инвариантно (с точкой) — надёжно для экзамена
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }
    }
}