using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Configuration;


namespace AutoSetSysTime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }

    public class SysDateTimeService
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        public static bool SetLocalTimeByStr(int second)
        {
            var flag = false;
            var sysTime = new SystemTime();
            var dt = Convert.ToDateTime(ConfigurationManager.AppSettings["SetTime"]);
            sysTime.wYear = Convert.ToUInt16(dt.Year);
            sysTime.wMonth = Convert.ToUInt16(dt.Month);
            sysTime.wDay = Convert.ToUInt16(dt.Day);
            sysTime.wHour = Convert.ToUInt16(dt.Hour);
            sysTime.wMinute = Convert.ToUInt16(dt.Minute);
            sysTime.wSecond = Convert.ToUInt16(dt.Second);
            try
            {
                flag = SetLocalTime(ref sysTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("SetSystemDateTime函数执行异常" + e.Message);
            }
            return flag;
        }
    }

    class Program
    {
        private const int Interval = 1000 * 5;

        static void Main(string[] args)
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {

                var timer = new Timer(Interval);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
                Timer_Elapsed(null, null);
            }
            else
            {
                Console.WriteLine("请以管理员身份运行此程序。。");
            }
            Console.ReadKey();
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currentTime = Convert.ToDateTime(ConfigurationManager.AppSettings["SetTime"]);
            Console.WriteLine(currentTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var flag = SysDateTimeService.SetLocalTimeByStr(Interval*-1);
            Console.WriteLine(flag);
        }
    }
}
