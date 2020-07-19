using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MikuMikuModel.GUI.Forms;

namespace MikuMikuModel
{
    public static class Program
    {
        private static string sVersion;

        public static string Name => "Miku Miku Model";

        public static string Version
        {
            get
            {
                if ( !string.IsNullOrEmpty( sVersion ) )
                    return sVersion;

                return sVersion = FixUpVersionString( Assembly.GetExecutingAssembly().GetName().Version.ToString() );
            }
        }

        public static string FixUpVersionString( string version )
        {
            while ( version.EndsWith( ".0" ) )
                version = version.Remove( version.Length - 2, 2 );

            if ( !version.Contains( "." ) )
                version += ".0";

            return version;
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main( string[] args )
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.AddMessageFilter( new AltKeyFilter() );

            using ( var form = new MainForm() )
            {
                if ( args.Length > 0 && File.Exists( args[ 0 ] ) )
                    form.OpenFile( args[ 0 ] );

                Application.Run( form );
            }
        }

        private class AltKeyFilter : IMessageFilter
        {
            public bool PreFilterMessage( ref Message m )
            {
                return m.Msg == 0x0104 && ( ( int ) m.LParam & 0x20000000 ) != 0;
            }
        }
    }
}