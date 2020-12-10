using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace MikuMikuLibrary.IBLs
{
    public class IBL : BinaryFile
    {
        public const int MaxLightCount = 4;
        public const int MaxCoefficientCount = 4;
        public const int MaxLightMapCount = 6;

        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public List<Light> Lights { get; }
        public List<DiffuseCoefficient> DiffuseCoefficients { get; }
        public List<LightMap> LightMaps { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            string token = reader.ReadLine();

            if ( token != "VF5_IBL" )
                throw new InvalidDataException( "Invalid signature (expected VF5_IBL)" );

            while ( ( token = reader.ReadLine() ) != "BINARY" )
            {
                if ( token.StartsWith( "#" ) )
                    continue;

                switch ( token )
                {
                    case "VERSION":
                    {
                        if ( reader.ReadLine() != "20050906" )
                            throw new InvalidDataException( "Invalid version number (expected 20050906)" );

                        break;
                    }

                    case "LIT_DIR":
                    {
                        int index = int.Parse( reader.ReadLine() );

                        while ( index >= Lights.Count )
                            Lights.Add( new Light() );

                        Lights[ index ].Direction = ReadVector3( reader );

                        break;
                    }

                    case "LIT_COL":
                    {
                        int index = int.Parse( reader.ReadLine() );

                        while ( index >= Lights.Count )
                            Lights.Add( new Light() );

                        Lights[ index ].Color = ReadVector3( reader );

                        break;
                    }

                    case "DIFF_COEF":
                    {
                        int index = int.Parse( reader.ReadLine() );

                        while ( index >= DiffuseCoefficients.Count )
                            DiffuseCoefficients.Add( new DiffuseCoefficient() );

                        var coefficient = DiffuseCoefficients[ index ];

                        coefficient.R = ReadMatrix4x4( reader );
                        coefficient.G = ReadMatrix4x4( reader );
                        coefficient.B = ReadMatrix4x4( reader );

                        break;
                    }

                    case "LIGHT_MAP":
                    {
                        int index = int.Parse( reader.ReadLine() );

                        while ( index >= LightMaps.Count )
                            LightMaps.Add( new LightMap() );

                        var lightMap = LightMaps[ index ];

                        if ( reader.ReadLine() != "RGBA16F_CUBE" )
                            throw new InvalidDataException( "Invalid format (expected RGBA16_F)" );

                        string widthHeightLine = reader.ReadLine();

                        var split = widthHeightLine.Split( ' ' );

                        lightMap.Width = int.Parse( split[ 0 ] );
                        lightMap.Height = int.Parse( split[ 1 ] );

                        break;
                    }
                }
            }

            foreach ( var lightMap in LightMaps )
            {
                for ( int i = 0; i < lightMap.Sides.Length; i++ )
                    lightMap.Sides[ i ] = reader.ReadHalfs( lightMap.Width * lightMap.Height * 4 );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.WriteLine( "VF5_IBL" );
            writer.WriteLine( "VERSION" );
            writer.WriteLine( "20050906" );

            for ( int i = 0; i < Lights.Count; i++ )
            {
                var light = Lights[ i ];

                writer.WriteLine( "LIT_DIR" );
                writer.WriteLine( "{0}", i );
                WriteVector3( light.Direction, writer );

                writer.WriteLine( "LIT_COL" );
                writer.WriteLine( "{0}", i );
                WriteVector3( light.Color, writer );
            }

            for ( int i = 0; i < DiffuseCoefficients.Count; i++ )
            {
                var coefficient = DiffuseCoefficients[ i ];

                writer.WriteLine( "DIFF_COEF" );
                writer.WriteLine( "{0}", i );

                WriteMatrix4x4( coefficient.R, writer );
                WriteMatrix4x4( coefficient.G, writer );
                WriteMatrix4x4( coefficient.B, writer );
            }

            for ( int i = 0; i < LightMaps.Count; i++ )
            {
                var lightMap = LightMaps[ i ];

                writer.WriteLine( "LIGHT_MAP" );
                writer.WriteLine( "{0}", i );
                writer.WriteLine( "RGBA16F_CUBE" );
                writer.WriteLine( "{0} {1}", lightMap.Width, lightMap.Height );
            }

            writer.WriteLine( "BINARY" );

            foreach ( var lightMap in LightMaps )
            {
                foreach ( var bytes in lightMap.Sides )
                    writer.Write( bytes );
            }
        }

        // I know anything onwards here is terrible don't judge me please

        private Vector3 ReadVector3( EndianBinaryReader reader )
        {
            string line = reader.ReadLine();
            var split = line.Split( ' ' );

            return new Vector3(
                float.Parse( split[ 0 ], CultureInfo.InvariantCulture ),
                float.Parse( split[ 1 ], CultureInfo.InvariantCulture ),
                float.Parse( split[ 2 ], CultureInfo.InvariantCulture )
            );
        }

        private Matrix4x4 ReadMatrix4x4( EndianBinaryReader reader )
        {
            // scanf is shook

            Matrix4x4 result;

            string line = reader.ReadLine();
            var split = line.Split( ' ' );

            result.M11 = float.Parse( split[ 0 ], CultureInfo.InvariantCulture );
            result.M12 = float.Parse( split[ 1 ], CultureInfo.InvariantCulture );
            result.M13 = float.Parse( split[ 2 ], CultureInfo.InvariantCulture );
            result.M14 = float.Parse( split[ 3 ], CultureInfo.InvariantCulture );

            line = reader.ReadLine();
            split = line.Split( ' ' );

            result.M21 = float.Parse( split[ 0 ], CultureInfo.InvariantCulture );
            result.M22 = float.Parse( split[ 1 ], CultureInfo.InvariantCulture );
            result.M23 = float.Parse( split[ 2 ], CultureInfo.InvariantCulture );
            result.M24 = float.Parse( split[ 3 ], CultureInfo.InvariantCulture );

            line = reader.ReadLine();
            split = line.Split( ' ' );

            result.M31 = float.Parse( split[ 0 ], CultureInfo.InvariantCulture );
            result.M32 = float.Parse( split[ 1 ], CultureInfo.InvariantCulture );
            result.M33 = float.Parse( split[ 2 ], CultureInfo.InvariantCulture );
            result.M34 = float.Parse( split[ 3 ], CultureInfo.InvariantCulture );

            line = reader.ReadLine();
            split = line.Split( ' ' );

            result.M41 = float.Parse( split[ 0 ], CultureInfo.InvariantCulture );
            result.M42 = float.Parse( split[ 1 ], CultureInfo.InvariantCulture );
            result.M43 = float.Parse( split[ 2 ], CultureInfo.InvariantCulture );
            result.M44 = float.Parse( split[ 3 ], CultureInfo.InvariantCulture );

            return result;
        }

        private void WriteVector3( Vector3 value, EndianBinaryWriter writer )
        {
            writer.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0} {1} {2}", value.X, value.Y, value.Z ) );
        }

        private void WriteMatrix4x4( Matrix4x4 value, EndianBinaryWriter writer )
        {
            writer.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0} {1} {2} {3}", value.M11, value.M12, value.M13, value.M14 ) );
            writer.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0} {1} {2} {3}", value.M21, value.M22, value.M23, value.M24 ) );
            writer.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0} {1} {2} {3}", value.M31, value.M32, value.M33, value.M34 ) );
            writer.WriteLine( string.Format( CultureInfo.InvariantCulture, "{0} {1} {2} {3}", value.M41, value.M42, value.M43, value.M44 ) );
        }

        public IBL()
        {
            Lights = new List<Light>( MaxLightCount );
            DiffuseCoefficients = new List<DiffuseCoefficient>( MaxCoefficientCount );
            LightMaps = new List<LightMap>( MaxLightMapCount );
        }
    }
}