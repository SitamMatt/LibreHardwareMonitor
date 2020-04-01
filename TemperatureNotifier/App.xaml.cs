using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Timer = System.Timers.Timer;

namespace TemperatureNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void SwitchStartup(object sender, EventArgs args)
        {
            if (StartupManager.IsStartupEnabled())
            {
                StartupManager.DisableStartup();
                ToastFacade.Show("Temperature Notifier", "Startup has been sucessfully disabled");
            }
            else
            {
                StartupManager.EnableStartup();
                ToastFacade.Show("Temperature Notifier", "Startup has been sucessfully enabled");
            }
            ((MenuItem)sender).Checked = StartupManager.IsStartupEnabled();
        }
        StreamWriter streamWriter;

        private void Application_Startup(object sender, StartupEventArgs args)
        {
            string[] MeasuredProperties =
            {
                "Temperature/CPU Package",
                "Load/CPU Total",
                "Temperature/GPU Core",
                "Load/GPU Core",
            };

            var trays = new Dictionary<string, TemperatureTray>();
            TrayBuilder builder1 = new TrayBuilder("CPU Package")
                .WithColor(Color.Red)
                .AddContextMenu(
                    new MenuItem("Uruchom przy starcie", new EventHandler(SwitchStartup)) { Checked = StartupManager.IsStartupEnabled()},
                    new MenuItem("Zamknij", new EventHandler((s,a) => Current.Shutdown()))
                 );
            trays["CPU Package"] = builder1.GetTray();
            TrayBuilder builder2 = new TrayBuilder("GPU Core")
                .WithColor(Color.Red)
                .AddContextMenu(
                    new MenuItem("Uruchom przy starcie", new EventHandler(SwitchStartup)) { Checked = StartupManager.IsStartupEnabled() },
                    new MenuItem("Zamknij", new EventHandler((s, a) => Current.Shutdown()))
                 );
            trays["GPU Core"] = builder2.GetTray();

            trays["GPU Core"].Show();
            trays["CPU Package"].Show();

            streamWriter = File.AppendText(@"D:\measurement.log");

            Measure measure = new Measure(MeasuredProperties);
            Timer timer = new Timer() { Enabled = true,  Interval = 30 * 1000 }; // 30 seconds
            timer.Elapsed += delegate (object s, ElapsedEventArgs e)
            {
                var row = measure.Update();
                foreach (var measurement in row.Where(x => x.Item2 == "Temperature"))
                {
                    trays[measurement.Item1].Update(measurement.Item3.ToString());
                }

                var formated = row.Select(element => $"{element.Item2}/{element.Item1}={element.Item3}");
                var log = string.Join(";", formated.ToArray());

                streamWriter.WriteLine("{0:yyyy-MM-ddTHH::mm:ss};{1}", DateTime.Now, log);
            };
            Timer flushTimer = new Timer() { Enabled = true, Interval = 5 * 60 * 1000 }; // 5 minutes
            flushTimer.Elapsed += (s, e) => streamWriter.Flush();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            streamWriter.Close();
        }
    }
}
