using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TemperatureNotifier
{
    class TemperatureTray
    {
        private readonly NotifyIcon notifyIcon;

        public string Text { get; set; }
        public Font Font { get; set; }
        public Color Color { get; set; }

        public TemperatureTray()
        {
            notifyIcon = new NotifyIcon();
            Text = "";
            Font = SystemFonts.DefaultFont;
            Color = Color.White;
        }

        public TemperatureTray(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
            Text = "";
            Font = SystemFonts.DefaultFont;
            Color = Color.White;
        }

        public void Update(string text)
        {
            Brush brush = new SolidBrush(Color);

            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(text, Font, brush, 0, 0);

            // Convert the bitmap with text to an Icon
            Icon icon = Icon.FromHandle(bitmap.GetHicon());

            notifyIcon.Icon = icon;
        }

        public void Show() => notifyIcon.Visible = true;

        public void Hide() => notifyIcon.Visible = false;
    }
}
