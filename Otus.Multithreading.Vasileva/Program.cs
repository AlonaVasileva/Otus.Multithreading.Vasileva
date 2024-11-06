using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Otus.Multithreading.Vasileva
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Устанавливаем контекст лицензии EPPlus для использования бесплатно
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Получаем информацию об окружении
            string environmentInfo = $"OS: {Environment.OSVersion}, " +
                                     $"CPU: {Environment.ProcessorCount} cores, " +
                                     $"Arch: {(Environment.Is64BitOperatingSystem ? "64-bit" : "32 - bit")}, " + 
                                     $"User: {Environment.UserName}, " +  
                                     $"Machine: {Environment.MachineName}, " + 
                                     $"CLR Version: {Environment.Version}";

            // Создаем массивы разного размера
            int[] array1 = Enumerable.Range(1, 100000).ToArray();
            int[] array2 = Enumerable.Range(1, 1000000).ToArray();
            int[] array3 = Enumerable.Range(1, 10000000).ToArray();

            // Создаем список для сбора результатов
            List<List<object>> allResults = new List<List<object>>();

            // Для массива из 100 000 элементов
            List<object> row1 = new List<object> { 100000 };  // Начинаем с размера массива
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArraySequential(array1), "Sequential", row1);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array1, 2), "Parallel 2 Threads", row1);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array1, 4), "Parallel 4 Threads", row1);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array1, 8), "Parallel 8 Threads", row1);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayLinq(array1), "LINQ", row1);
            allResults.Add(row1);

            // Для массива из 1 000 000 элементов
            List<object> row2 = new List<object> { 1000000 };
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArraySequential(array2), "Sequential", row2);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array2, 2), "Parallel 2 Threads", row2);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array2, 4), "Parallel 4 Threads", row2);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array2, 8), "Parallel 8 Threads", row2);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayLinq(array2), "LINQ", row2);
            allResults.Add(row2);

            // Для массива из 10 000 000 элементов
            List<object> row3 = new List<object> { 10000000 };
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArraySequential(array3), "Sequential", row3);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array3, 2), "Parallel 2 Threads", row3);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array3, 4), "Parallel 4 Threads", row3);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayParallel(array3, 8), "Parallel 8 Threads", row3);
            TimeMeasurement.MeasureTime(() => SumCalculation.SumArrayLinq(array3), "LINQ", row3);
            allResults.Add(row3);

            // Проверяем доступность файла перед записью
            string filePath = "performance_results.xlsx";
            bool fileIsLocked = true;
            int maxRetries = 5;
            int retries = 0;

            // Пытаемся открыть файл до 5 раз
            while (fileIsLocked && retries < maxRetries)
            {
                try
                {
                    // Попытка открыть файл в режиме записи
                    using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileIsLocked = false; // Если удалось открыть файл — файл не занят
                    }
                }
                catch (IOException)
                {
                    retries++;
                    Console.WriteLine("Файл занят другим процессом. Попытка №" + retries);
                    Thread.Sleep(2000);  // Задержка 2 секунды перед повторной попыткой
                }
            }

            // Если файл так и не был доступен, выводим сообщение
            if (fileIsLocked)
            {
                Console.WriteLine("Не удалось получить доступ к файлу. Он занят другим процессом.");
            }
            else
            {
                // Если файл доступен, записываем результаты
                try
                {
                    // Создаем новый Excel пакет
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        // Если лист с таким именем уже существует, удаляем его
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name == "Results");
                        if (worksheet != null)
                        {
                            package.Workbook.Worksheets.Delete(worksheet);
                        }

                        // Добавляем новый лист
                        worksheet = package.Workbook.Worksheets.Add("Results");

                        // Записываем информацию о системе в первую строку
                        worksheet.Cells[1, 1].Value = "Environment Info";
                        worksheet.Cells[1, 2].Value = environmentInfo;

                        // Записываем заголовок таблицы
                        worksheet.Cells[2, 1].Value = "Array Size";
                        worksheet.Columns[1].Width = 20;
                        worksheet.Cells[2, 2].Value = "Sequential Time (ms)";
                        worksheet.Columns[2].Width = 20;
                        worksheet.Cells[2, 3].Value = "Parallel 2 Threads (ms)";
                        worksheet.Columns[3].Width = 25;
                        worksheet.Cells[2, 4].Value = "Parallel 4 Threads (ms)";
                        worksheet.Columns[4].Width = 25;
                        worksheet.Cells[2, 5].Value = "Parallel 8 Threads (ms)";
                        worksheet.Columns[5].Width = 25;
                        worksheet.Cells[2, 6].Value = "LINQ Time (ms)";
                        worksheet.Columns[6].Width = 20;

                        // Записываем данные
                        int row = 3;
                        foreach (var result in allResults)
                        {
                            for (int col = 0; col < result.Count; col++)
                            {
                                worksheet.Cells[row, col + 1].Value = result[col];  // Записываем в ячейки
                            }
                            row++;
                        }

                        // Сохраняем файл
                        package.Save();
                    }

                    Console.WriteLine("Результаты успешно записаны в файл Excel.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Ошибка при записи в Excel: " + ex.Message);
                }
            }
            Console.ReadKey();
        }

    }
}
