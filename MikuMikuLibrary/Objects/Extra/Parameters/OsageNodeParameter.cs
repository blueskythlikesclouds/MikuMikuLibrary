using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
    public class OsageNodeParameter
    {
        public float HingeYMin { get; set; }
        public float HingeYMax { get; set; }
        public float HingeZMin { get; set; }
        public float HingeZMax { get; set; }
        public float Radius { get; set; }
        public float Weight { get; set; }
        public float InertialCancel { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            HingeYMin = reader.ReadSingle();
            HingeYMax = reader.ReadSingle();
            HingeZMin = reader.ReadSingle();
            HingeZMax = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Weight = reader.ReadSingle();
            InertialCancel = reader.ReadSingle();
            reader.SkipNulls( 4 ); // Padding?
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( HingeYMin );
            writer.Write( HingeYMax );
            writer.Write( HingeZMin );
            writer.Write( HingeZMax );
            writer.Write( Radius );
            writer.Write( Weight );
            writer.Write( InertialCancel );
            writer.WriteNulls( 4 );
        }

        internal void Read( ParameterTree tree )
        {
            HingeYMin = tree.Get<float>( "hinge_ymin", -180 );
            HingeYMax = tree.Get<float>( "hinge_ymax", 180 );         
            HingeZMin = tree.Get<float>( "hinge_zmin", -180 );
            HingeZMax = tree.Get<float>( "hinge_zmax", 180 );
            Radius = tree.Get<float>( "coli_r" );
            Weight = tree.Get<float>( "weight", 1 );
            InertialCancel = tree.Get<float>( "inertial_cancel" );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "hinge_ymin", HingeYMin );
            writer.Write( "hinge_ymax", HingeYMax );          
            writer.Write( "hinge_zmin", HingeZMin );
            writer.Write( "hinge_zmax", HingeZMax );
            writer.Write( "coli_r", Radius );
            writer.Write( "inertial_cancel", InertialCancel );
            writer.Write( "weight", Weight );
        }
    }
}