using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace bot_supreme
{
    internal static class SchedulerMgr
    {
        public static bool CreateTask(string name, string filepath, string arg, DateTime time)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                         CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.StartInfo.Arguments =
                    $@"/C schtasks /create /tn ""{name}"" /tr ""\""{filepath}\"" {arg} "" /sc once /sd {time
                        .ToString("dd/MM/yyyy")} /st {time.ToString("HH:mm:ss")} /f";
               Console.WriteLine(process.StartInfo.Arguments);

                process.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool DeleteTask(string name)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe" ,
                      CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.StartInfo.Arguments =
                    $@"/C schtasks /Delete /tn ""{name}"" /f";
                //  Console.WriteLine(process.StartInfo.Arguments);
                process.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
