using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.IBLs;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Misc;
using MikuMikuModel.Nodes.IO;

namespace MikuMikuModel.Nodes.IBLs
{
    public class IBLNode : BinaryFileNode<IBL>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public List<Light> Lights => GetProperty<List<Light>>();

        [DisplayName( "Diffuse coefficients" )]
        public List<DiffuseCoefficient> DiffuseCoefficients => GetProperty<List<DiffuseCoefficient>>();

        [DisplayName( "Light-maps" )] 
        public List<LightMap> LightMaps => GetProperty<List<LightMap>>();

        protected override void Initialize()
        {
            AddReplaceHandler<IBL>( BinaryFile.Load<IBL> );
            AddExportHandler<IBL>( filePath => Data.Save( filePath ) );

            AddCustomHandler( "Export All", () =>
            {
            } );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public IBLNode( string name, IBL data ) : base( name, data )
        {
        }

        public IBLNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}