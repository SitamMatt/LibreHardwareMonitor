using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
using XmlNodeList = Windows.Data.Xml.Dom.XmlNodeList;

namespace TemperatureNotifier
{
    public static class ToastFacade
    {
        public static void Show(string header, string text)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            //Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            
               stringElements[0].AppendChild(toastXml.CreateTextNode(header));
            stringElements[1].AppendChild(toastXml.CreateTextNode(text));



            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier("ELO").Show(toast);
        }
    }
}
