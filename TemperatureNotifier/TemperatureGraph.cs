using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureNotifier
{
    public static class TemperatureGraph
    {
        public static void Visualize(string dataPath)
        {
            var procInfo = new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = $"main.py {dataPath}",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(procInfo);
        }

        public static void Visualize()
        {
            var procInfo = new ProcessStartInfo()
            {
                FileName = "python.exe",
                Arguments = "main.py",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(procInfo);
        }
    }
}
