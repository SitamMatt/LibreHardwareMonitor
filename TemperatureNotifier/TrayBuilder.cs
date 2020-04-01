using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TemperatureNotifier
{
    class TrayBuilder
    {
        public delegate void MenuAction(object sender, EventArgs e);
        private readonly TemperatureTray tray;
        private ContextMenu contextMenu;
        private readonly NotifyIcon notifyIcon;
        public TrayBuilder()
        {
            notifyIcon = new NotifyIcon();
            tray = new TemperatureTray(notifyIcon);
        }

        public TrayBuilder(string text) : this()
        {
            tray.Text = text;
        }

        public TrayBuilder WithColor(Color color)
        {
            tray.Color = color;
            return this;
        }

        public TrayBuilder WithFont(Font font)
        {
            tray.Font = font;
            return this;
        }

        public TrayBuilder AddContextMenu(params MenuItem[] menuItems)
        {
            contextMenu = new ContextMenu();
            contextMenu.MenuItems.AddRange(menuItems);
            notifyIcon.ContextMenu = contextMenu;
            return this;
        }

        public TemperatureTray GetTray() => tray;
    }
}
