using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace CCWFM
{
    //This Calss Added By Mohamed Hashem 27-05-2019 instead of System.Diagonistic.Stopwatch
    public class CalculateTime
    {
        public static long before;
        public static long after;
        public static TimeSpan elapsedTime;

        public static void Start()
        {
            before = DateTime.Now.Ticks;
        }
        public static void Stop()
        {
            after = DateTime.Now.Ticks;
            elapsedTime = new TimeSpan(after - before);
        }

        public static void GetTotalTime(string Message)
        {
            //using (StreamWriter writer = File.AppendText("F:\\time.txt"))
            //{
            //    writer.WriteLine("{0}: {1:hh\\:mm\\:ss} ,as Total of: {2}", Message, elapsedTime, elapsedTime.ToString());
            //}
        }

        public static void WriteLine(string Message)
        {
            //using (StreamWriter writer = File.AppendText("F:\\time.txt")) //new StreamWriter("F:\\time.txt")
            //{
            //    writer.WriteLine("{0}", Message);
            //}
        }

        public static void Write(string Message)
        {
            //using (StreamWriter writer = File.AppendText("F:\\time.txt"))
            //{
            //    writer.Write("{0}", Message);
            //}
        }

        public static void Clear()
        {
           // File.WriteAllText("F:\\time.txt", String.Empty);
        }

    }
}
