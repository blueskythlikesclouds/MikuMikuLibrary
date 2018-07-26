using MikuMikuLibrary.Databases;
using System.IO;
using System.Xml.Serialization;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Archives.Farc;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Processing.Textures;
using MikuMikuLibrary.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Threading.Tasks;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibraryTest
{
    class Program
    {
        //static void FixMotion( string filepath )
        //{
        //    var motdboriginal = BinaryFile.Load<MotionDatabase>( "mot_db_original.bin" );
        //    var motdbtarget = BinaryFile.Load<MotionDatabase>( "mot_db_target.bin" );

        //    var mr = new EndianBinaryReader( File.Open( filepath, FileMode.Open, FileAccess.ReadWrite ), System.Text.Encoding.ASCII, true, Endianness.LittleEndian );
        //    var mw = new EndianBinaryWriter( mr.BaseStream, System.Text.Encoding.ASCII, true, Endianness.LittleEndian );
        //    var offsets = new List<long>();
        //    while ( true )
        //    {
        //        mr.SeekCurrent( 0xC );
        //        var offset = mr.ReadUInt32();
        //        if ( offset == 0 )
        //            break;
        //        offsets.Add( offset );
        //    }

        //    foreach ( var offset in offsets )
        //    {
        //        mr.SeekBegin( offset );
        //        var ids = new List<ushort>();
        //        while ( true )
        //        {
        //            var id = mr.ReadUInt16();
        //            if ( id == 0 && ids.Contains( 0 ) )
        //                break;
        //            ids.Add( id );
        //        }
        //        mr.SeekBegin( offset );
        //        foreach ( var id in ids.Select( x => ( ushort )motdbtarget.BoneNames.IndexOf( motdboriginal.BoneNames[ x ] ) ) )
        //        {
        //            mw.Write( id );
        //        }
        //    }
        //    mr.Close();
        //    mw.Close();
        //}

        //private static readonly char[] Invalid = Path.GetInvalidPathChars().Union( Path.GetInvalidFileNameChars() ).Except( new char[] { '/', '\\' } ).ToArray();

        //public static string ForceValidFilePath( string text )
        //{
        //    if ( text.StartsWith( "C:" ) )
        //        text = text.Substring( 2 );

        //    // Valid path force
        //    foreach ( char c in Invalid )
        //    {

        //        text = text.Replace( c.ToString(), "" );
        //    }

        //    return Path.GetFullPath( text );
        //}

        static void Main( string[] args )
        {
            //var b = BinaryFile.Load<MotionDatabase>( args[ 0 ] );
            //( new XmlSerializer( b.GetType() ) ).Serialize( File.Create( Path.ChangeExtension( args[ 0 ], "xml" ) ), b );
            //args = new string[] { @"C:\Users\Asilkan\Documents\Visual Studio 2017\Projects\MikuMikuModel\MikuMikuLibraryTest\bin\Debug\mikitm003_obj.bin.new" };
            BinaryFile.Load<Model>( args[ 0 ] ).Save( args[ 0 ] + ".new" );

            //var pvdb = File.OpenText( @"C:\Users\Asilkan\Documents\Visual Studio 2017\Projects\PPD-DSC-Converter\Database\mdata_pv_db.txt" );
            //var dict = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
            //while ( pvdb.Peek() != -1 )
            //{
            //    var line = pvdb.ReadLine();
            //    if ( ( line.StartsWith( "pv_" ) || line.StartsWith( "#pv_" ) ) && line.Contains( ".song_name_en" ) && !dict.ContainsKey( line.Substring( line.IndexOf( "pv_" ), 6 ) ) )
            //    {
            //        dict.Add( line.Substring( line.IndexOf( "pv_" ), 6 ), line.Substring( line.IndexOf( '=' ) + 1 ).Trim().Replace("\\", "∕" ).Replace("/", "∕"));
            //    }
            //}
            //args = new string[] { @"C:\Users\Asilkan\Downloads\2d\mdata_spr_db.bin" };
            //var sprdb = BinaryFile.Load<SpriteDatabase>( args[ 0 ] );
            //var dir = Path.GetDirectoryName( args[ 0 ] );
            //var dirOut = Path.Combine( dir, Path.GetFileNameWithoutExtension( args[ 0 ] ) );
            //Directory.CreateDirectory( dirOut );
            //Directory.CreateDirectory( Path.Combine( dirOut, "Logos" ) );
            //Directory.CreateDirectory( Path.Combine( dirOut, "Covers" ) );
            //Directory.CreateDirectory( Path.Combine( dirOut, "Backgrounds" ) );

            //var obj = new object();

            //Parallel.ForEach( sprdb.SpriteSets, ( ent ) =>
            // {
            //     var farc = BinaryFile.LoadIfExist<FARCArchive>( Path.Combine( dir, ent.Name + ".farc" ) );
            //     if ( farc.Contains( ent.FileName ) && ent.Name.StartsWith( "spr_sel_pv", StringComparison.OrdinalIgnoreCase ) )
            //     {
            //         Console.WriteLine( "{0}", ent.Name );
            //         var pvnam = "pv_" + ent.Name.Substring( ent.Name.IndexOf( "PV" ) + 2, 3 );
            //         if ( dict.ContainsKey( pvnam ) )
            //         {
            //             var nam = dict[ pvnam ];
            //             var spritecol = BinaryFile.Load<SpriteSet>( farc.Open( ent.FileName ) );
            //             var bitmaps = new List<Bitmap>();
            //             foreach ( var tex in spritecol.TextureSet.Textures )
            //             {
            //                 var bitmap = TextureConverter.ConvertToBitmap( tex );
            //                 bitmap.RotateFlip( RotateFlipType.Rotate180FlipX );
            //                 bitmaps.Add( bitmap );
            //             }
            //             //var outdir = Path.Combine( dirOut, nam );
            //             //Directory.CreateDirectory( outdir );
            //             int iter = 0;
            //             foreach ( var spr in spritecol )
            //             {
            //                 var bitmap = bitmaps[ spr.TextureIndex ].Clone( new Rectangle( ( int )spr.X, ( int )spr.Y, ( int )spr.Width, ( int )spr.Height ), PixelFormat.Format32bppArgb );
            //                 //bitmap.Save( Path.Combine( outdir, ent.Sprites[ spritecol.IndexOf( spr ) ].Name.ToLowerInvariant() + ".png" ) );
            //                 string outnam = iter == 0 ? "Backgrounds" : iter == 1 ? "Covers" : iter == 2 ? "Logos" : "";
            //                 var path = ForceValidFilePath( Path.Combine( dirOut, outnam, nam + ".png" ) );
            //                 bitmap.Save( path, ImageFormat.Png );
            //                 bitmap.Dispose();
            //                 iter += 1;
            //             }

            //             foreach ( var tex in bitmaps )
            //                 tex.Dispose();

            //             bitmaps.Clear();
            //         }
            //     }
            //     //else
            //     //{
            //     //    lock ( obj ) File.AppendAllText( Path.Combine( dirOut, "unused.txt" ), ent.Name + "\n" );
            //     //}
            //     farc.Dispose();
            // } );
        }
    }
}
