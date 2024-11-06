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
    internal class TimeMeasurement
    {
        // Собираем результаты выполнения в список
        public static void MeasureTime(Action action, string methodName, List<object> rowResults)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            rowResults.Add(stopwatch.ElapsedMilliseconds);  // Добавляем время выполнения в список
        }
    }
}
