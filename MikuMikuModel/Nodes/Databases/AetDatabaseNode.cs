using System;
using System.ComponentModel;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.TypeConverters;

namespace MikuMikuModel.Nodes.Databases
{
    public class AetDatabaseNode : BinaryFileNode<AetDatabase>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddExportHandler<AetDatabase>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<AetDatabase>( BinaryFile.Load<AetDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<AetSetInfo>( "Aet sets", Data.AetSets, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public AetDatabaseNode( string name, AetDatabase data ) : base( name, data )
        {
        }

        public AetDatabaseNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }

    public class AetSetInfoNode : Node<AetSetInfo>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "File name" )]
        public string FileName
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Sprite set id" )]
        public uint SpriteSetId
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<AetInfo>( "Aet scenes", Data.Aets, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public AetSetInfoNode( string name, AetSetInfo data ) : base( name, data )
        {
        }
    }

    public class AetInfoNode : Node<AetInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        public ushort Index
        {
            get => GetProperty<ushort>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public AetInfoNode( string name, AetInfo data ) : base( name, data )
        {
        }
    }
}