using System;
using System.Globalization;
using System.IO;
using System.Text;
using EXAMEN0202.Models;

namespace EXAMEN0202.Services
{
    public static class ReceiptService
    {
        public static string CreateAndSaveReceipt(
            decimal width, decimal height, MaterialType material, decimal cost)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Receipts");
            Directory.CreateDirectory(folder); // создаём папку, если её нет

            string receiptNo = GenerateReceiptNumber();
            DateTime now = DateTime.Now;

            string costForFile = cost.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', '-');
            string dateForFile = now.ToString("yyyy-MM-dd");
            string fileName = $"{receiptNo}_{dateForFile}_{costForFile}.txt";

            string fullPath = MakeUniquePath(Path.Combine(folder, fileName));

            string text = BuildReceiptText(width, height, material, cost, receiptNo, now);
            File.WriteAllText(fullPath, text, Encoding.UTF8);

            return fullPath;
        }

        private static string BuildReceiptText(decimal width, decimal height, MaterialType material,
            decimal cost, string receiptNo, DateTime dt)
        {
            string materialName = material == MaterialType.Aluminum ? "Алюминий" : "Пластик";
            string sizeLine = $"{width:0.00}х{height:0.00}";

            // Шаблон “как в квитанции” из билета
            return
$@"ООО ""Уютный Дом""
Добро пожаловать
ККМ 00075411     #3969
ИНН 1087746942040
ЭКЛЗ 3851495566

Чек №{receiptNo}
{dt:dd.MM.yy HH:mm} СИС.

наименование товара
жалюзи
{sizeLine}

материал
{materialName}

Итог
={cost:0.00}
Сдача
=0
Сумма итого:
={cost:0.00}

************************
";
        }

        private static string GenerateReceiptNumber()
        {
            // Уникальность: время + хвост тиков (простое и надёжное для экзамена)
            long tail = DateTime.UtcNow.Ticks % 1_000_000;
            return $"{DateTime.Now:HHmmssfff}{tail:000000}";
        }

        private static string MakeUniquePath(string path)
        {
            if (!File.Exists(path)) return path;

            string dir = Path.GetDirectoryName(path)!;
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            // если файл уже существует — добавляем суффикс
            int i = 1;
            while (true)
            {
                string candidate = Path.Combine(dir, $"{name}_{i}{ext}");
                if (!File.Exists(candidate)) return candidate;
                i++;
            }
        }
    }
}