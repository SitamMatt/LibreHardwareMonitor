using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureNotifier
{
    using Measurement = Tuple<string, string, float?>;
    class Measure
    {
        private KeyValuePair<string, SensorType>[] MeasuredPropertiesFields;
        Computer computer;
        public Measure(string[] fields)
        {
            var cos = fields.Select(element =>
            {
                string[] parts = element.Split('/');
                SensorType sensorType;
                switch (parts[0])
                {
                    case "Temperature": sensorType = SensorType.Temperature; break;
                    case "Load": sensorType = SensorType.Load; break;
                    default: sensorType = 0; break;
                }
                return new KeyValuePair<string, SensorType>(parts[1], sensorType);
            }).ToArray();
            MeasuredPropertiesFields = cos;
            computer = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
            };
            computer.Open();
        }

        public List<Measurement> Update()
        {
            List<Measurement> MeasuredProperties = new List<Measurement>();
            foreach (IHardware hardware in computer.Hardware)
            {
                hardware.Update();
                MeasuredProperties.AddRange(
                    hardware.Sensors
                    .Where(sensor => MeasuredPropertiesFields.Any(x=>x.Key==sensor.Name && x.Value == sensor.SensorType) )
                    .Select(selected => new Measurement(selected.Name, ToSensorTypeName(selected.SensorType), selected.Value))
                );

            }
            return MeasuredProperties;
        }

        private string ToSensorTypeName(SensorType sensor)
        {
            switch (sensor)
            {
                case SensorType.Temperature: return "Temperature"; break;
                case SensorType.Load: return "Load"; break;
                default: return ""; break;
            }
        }
    }
}
