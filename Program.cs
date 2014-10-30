using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotSet
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            Application.ThreadException += Application_ThreadException;
            Application.Run(new Viewer());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var trace = e.Exception.StackTrace;
            MessageBox.Show(e.Exception.ToString());
        }

        private static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            var trace = e.Exception.StackTrace;
            MessageBox.Show(e.Exception.ToString());
        }

        public static void Benchmark(string tag, Func<Task> benchmarkAction, Action<string, TimeSpan> onResult)
        {
            Task.Run(async () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                await benchmarkAction();
                sw.Stop();
                onResult(tag, sw.Elapsed);
            });
        }
    }
}
