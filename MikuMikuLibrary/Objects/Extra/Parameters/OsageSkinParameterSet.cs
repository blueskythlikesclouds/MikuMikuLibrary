using System;
using System.Collections.Generic;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
    public class OsageSkinParameterSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<OsageSkinParameter> Parameters { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
                ReadModern();
            else
                ReadClassic();

            void ReadClassic()
            {
                var paramTree = new ParameterTree( reader );

                foreach ( string key in paramTree.Keys )
                {
                    var param = new OsageSkinParameter();

                    param.Name = key;

                    paramTree.OpenScope( key );
                    {
                        param.Read( paramTree );
                    }
                    paramTree.CloseScope();

                    Parameters.Add( param );
                }
            }

            void ReadModern()
            {
                int count0, count1, count2;
                long offset0, offset1, offset2;

                if ( section.Format == BinaryFormat.X )
                {
                    count0 = reader.ReadInt32();
                    count1 = reader.ReadInt32();
                    count2 = reader.ReadInt32();
                    offset0 = reader.ReadOffset();
                    offset1 = reader.ReadOffset();
                    offset2 = reader.ReadOffset();
                }
                else
                {
                    count0 = reader.ReadInt32();
                    offset0 = reader.ReadOffset();             
                    count1 = reader.ReadInt32();
                    offset1 = reader.ReadOffset();    
                    count2 = reader.ReadInt32();
                    offset2 = reader.ReadOffset();
                }

                reader.ReadAtOffset( offset0, () =>
                {
                    Parameters.Capacity = count0;

                    for ( int i = 0; i < count0; i++ )
                    {
                        var parameter = new OsageSkinParameter();
                        parameter.Read( reader );
                        Parameters.Add( parameter );
                    }
                } );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
                WriteModern();
            else
                WriteClassic();

            void WriteClassic()
            {
                var paramWriter = new ParameterTreeWriter();

                writer.WriteLine( "# This file was generated automatically. DO NOT EDIT." );

                foreach ( var parameter in Parameters )
                    parameter.Write( paramWriter );

                paramWriter.Flush( writer.BaseStream );
            }

            void WriteModern()
            {
                if ( section.Format == BinaryFormat.X )
                {
                    writer.Write( Parameters.Count );
                    writer.WriteNulls( 2 * sizeof( uint ) );

                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, WriteParameters );
                    writer.WriteNulls( 2 * sizeof( ulong ) );
                }
                else
                {
                    writer.Write( Parameters.Count );
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, WriteParameters );
                    writer.WriteNulls( 4 * sizeof( uint ) );
                }
            }

            void WriteParameters()
            {
                foreach ( var parameter in Parameters )
                    parameter.Write( writer );
            }
        }

        public override void Save( string filePath )
        {
            // Assume it's being exported for F 2nd PS3
            if ( Format.IsClassic() && filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                Format = BinaryFormat.F2nd;

            // And vice versa
            else if ( Format.IsModern() && filePath.EndsWith( ".txt", StringComparison.OrdinalIgnoreCase ) )
                Format = BinaryFormat.DT;

            base.Save( filePath );
        }

        public OsageSkinParameterSet()
        {
            Parameters = new List<OsageSkinParameter>();
        }
    }
}