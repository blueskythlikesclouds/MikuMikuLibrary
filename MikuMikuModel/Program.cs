using MikuMikuModel.GUI.Forms;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MikuMikuModel
{
    internal static class Program
    {
        public static string Name => "Miku Miku Model";
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            using ( var form = new MainForm() )
            {
                if ( args.Length > 0 && File.Exists( args[ 0 ] ) )
                    form.OpenFile( args[ 0 ] );

                Application.Run( form );
            }
        }
    }
}
