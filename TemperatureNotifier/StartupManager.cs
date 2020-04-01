using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace TemperatureNotifier
{
    static class StartupManager
    {
        private static string _taskName = "TemperatureNotifierStartup";
        public static bool IsStartupEnabled()
        {
            using (TaskService taskService = new TaskService())
            {
                Task task = taskService.GetTask(_taskName);
                return task != null;
            }
        }

        public static void EnableStartup()
        {
            string ExecutablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Temperature Notifier Startup";
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;

                // Create a trigger that will fire the task at this time every other day
                td.Triggers.Add(new LogonTrigger());

                // Create an action that will launch Program whenever the trigger fires
                td.Actions.Add(new ExecAction(ExecutablePath, null, null));

                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition(_taskName, td);
            }
        }

        public static void DisableStartup()
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(_taskName);
                if (task == null) return;
                var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                    throw new Exception($"Cannot delete task with your current identity '{identity.Name}' permissions level." +
                    "You likely need to run this application 'as administrator' even if you are using an administrator account.");
                ts.RootFolder.DeleteTask(_taskName);
            }
        }
    }
}
