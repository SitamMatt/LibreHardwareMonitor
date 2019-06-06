using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using SystemFonts = System.Drawing.SystemFonts;
using Color = System.Drawing.Color;
using System.ComponentModel;

namespace TemperatureNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClosingWindow;

            Computer computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true
            };

            computer.Open();

            var trays = new Dictionary<string, TemperatureTray>();
            trays["CPU Package"] = new TemperatureTray(this, SystemFonts.DefaultFont, Color.CadetBlue);
            trays["GPU Core"] = new TemperatureTray(this, SystemFonts.DefaultFont, Color.Orchid);


            Timer timer = new Timer() { Enabled = true, Interval = 5000 };
            timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                foreach (IHardware hardware in computer.Hardware)
                {
                    hardware.Update();
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            if (trays.TryGetValue(sensor.Name, out TemperatureTray tray))
                            {
                                tray.ShowText(sensor.Value.ToString()); ;
                            }
                        }
                    }
                }
            };
        }

        private void OnClosingWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
