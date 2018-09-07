using MikuMikuModel.GUI.Forms;
using System;
using System.Windows.Forms;

namespace MikuMikuModel
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            using ( var form = new MainForm() )
            {
                Application.Run( form );

                if ( args.Length > 0 )
                    form.OpenFile( args[ 0 ] );
            }
        }
    }
}
