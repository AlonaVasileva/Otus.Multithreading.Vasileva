using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Otus.Multithreading.Vasileva
{
    internal class TimeMeasurement
    {
        public static void MeasureTime(Action action, string methodName, List<object> rowResults)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();

            rowResults.Add(stopwatch.ElapsedMilliseconds);
        }
    }
}
