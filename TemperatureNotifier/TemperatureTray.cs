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
        private MainWindow mainWindow;
        private ContextMenu contextMenu;
        private NotifyIcon notifyIcon;
        private Font font;
        private Color color;
        public TemperatureTray(MainWindow pMainWindow, Font font, Color color)
        {
            mainWindow = pMainWindow;
            this.color = color;
            this.font = font;
            notifyIcon = new NotifyIcon();
            ShowText("0", font, color);
            notifyIcon.Text = "Tray test";
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += TrayOpenClick;

            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(0,
                new MenuItem("Odtwórz", new System.EventHandler(TrayOpenClick)));
            contextMenu.MenuItems.Add(1,
                new MenuItem("Zamknij", new EventHandler(TrayExitClick)));
            notifyIcon.ContextMenu = contextMenu;
        }
        private void TrayExitClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void TrayOpenClick(object sender, EventArgs e)
        {
            mainWindow.Show();
            mainWindow.Activate();
        }
        //public void ShowTrayInformation(string Title, string Content)
        //{
        //    notifyIcon.BalloonTipTitle = Title;
        //    notifyIcon.BalloonTipText = Content;
        //    notifyIcon.BalloonTipIcon = ToolTipIcon.None;
        //    notifyIcon.Visible = true;
        //    notifyIcon.ShowBalloonTip(30000);

        //    notifyIcon.BalloonTipClicked += delegate (object sender, EventArgs args)
        //    {
        //        mainWindow.Show();
        //        mainWindow.Activate();
        //    };
        //}
        public void ShowText(string text, Font font, Color col)
        {
            Brush brush = new SolidBrush(col);

            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(text, font, brush, 0, 0);

            // Convert the bitmap with text to an Icon
            Icon icon = Icon.FromHandle(bitmap.GetHicon());

            notifyIcon.Icon = icon;
        }

        public void ShowText(string text)
        {
            Brush brush = new SolidBrush(color);

            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(text, font, brush, 0, 0);

            // Convert the bitmap with text to an Icon
            Icon icon = Icon.FromHandle(bitmap.GetHicon());

            notifyIcon.Icon = icon;
        }

    }
}
