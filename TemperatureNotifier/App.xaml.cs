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
using MessageBox = System.Windows.MessageBox;
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

        string logPath;

        Timer writeTimer;

        Timer flushTimer;

        private void Application_Startup(object sender, StartupEventArgs args)
        {
            if (string.IsNullOrEmpty(TemperatureNotifier.Properties.Settings.Default.LogPath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Pick an output file";
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    TemperatureNotifier.Properties.Settings.Default.LogPath = saveFileDialog.FileName;
                    TemperatureNotifier.Properties.Settings.Default.Save();
                }
                else
                {
                    Environment.Exit(1);
                }
            }

            logPath = TemperatureNotifier.Properties.Settings.Default.LogPath;
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
                    new MenuItem("Zamknij", new EventHandler((s,a) => Current.Shutdown())),
                    new MenuItem("Pokaż wykres", new EventHandler(HandleVisualization)),
                    new MenuItem("Pokaż wykres z pliku..", new EventHandler(HandleVisualizationFromFile))
                 );
            trays["CPU Package"] = builder1.GetTray();
            TrayBuilder builder2 = new TrayBuilder("GPU Core")
                .WithColor(Color.Red)
                .AddContextMenu(
                    new MenuItem("Uruchom przy starcie", new EventHandler(SwitchStartup)) { Checked = StartupManager.IsStartupEnabled() },
                    new MenuItem("Zamknij", new EventHandler((s, a) => Current.Shutdown())),
                    new MenuItem("Pokaż wykres", new EventHandler(HandleVisualization)),
                    new MenuItem("Pokaż wykres z pliku..", new EventHandler(HandleVisualizationFromFile))
                 );
            trays["GPU Core"] = builder2.GetTray();

            trays["GPU Core"].Show();
            trays["CPU Package"].Show();

            if(File.Exists(logPath))
                streamWriter = File.AppendText(logPath);
            else
            {
                // init csv file with columns's headers row
                streamWriter = File.CreateText(logPath);
                streamWriter.WriteLine(string.Join(";", MeasuredProperties.Append("DateTime").ToArray()));
                streamWriter.Flush();
            }

            streamWriter.WriteLine("{0};{0};{0};{0};{1:yyyy-MM-ddTHH:mm:ss}", "None", DateTime.Now);

            Measure measure = new Measure(MeasuredProperties);
            writeTimer = new Timer() { Enabled = true,  Interval = 30 * 1000 }; // 30 seconds
            writeTimer.Elapsed += delegate (object s, ElapsedEventArgs e)
            {
                var row = measure.Update();
                foreach (var measurement in row.Where(x => x.Item2 == "Temperature"))
                {
                    trays[measurement.Item1].Update(measurement.Item3.ToString());
                }

                var formated = row.Select(element => element.Item3.ToString());
                var log = string.Join(";", formated.ToArray());

                streamWriter.WriteLine("{0};{1:yyyy-MM-ddTHH:mm:ss}", log, DateTime.Now);
            };
            flushTimer = new Timer() { Enabled = true, Interval = 5 * 60 * 1000 }; // 5 minutes
            flushTimer.Elapsed += (s, e) => streamWriter.Flush();
        }

        private void HandleVisualizationFromFile(object sender, EventArgs e)
        {
            try
            {
                TemperatureGraph.Visualize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            streamWriter?.Flush();
            streamWriter?.Close();
            flushTimer?.Close();
            writeTimer?.Close();
        }

        private void HandleVisualization(object sender, EventArgs e)
        {
            try
            {
                TemperatureGraph.Visualize(TemperatureNotifier.Properties.Settings.Default.LogPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
