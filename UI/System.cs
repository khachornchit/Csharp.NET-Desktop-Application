/*
 * Writer: Stevie Boon
 * Create: June 11, 2013
 * 
 * Class : System
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

using System.Reflection;

namespace XSystem
{
    class System
    {
        public static bool IsProcessRuning()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //Loop through the running processes in with the same name 
            foreach (Process process in processes)
            {
                //Ignore the current process 
                if (process.Id != current.Id)
                {
                    //Make sure that the process is running from the exe file. 
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        MessageBox.Show(process.ProcessName + " is running already.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
