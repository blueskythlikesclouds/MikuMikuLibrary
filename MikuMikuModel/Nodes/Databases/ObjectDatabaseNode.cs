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
    public class ObjectDatabaseNode : BinaryFileNode<ObjectDatabase>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            AddExportHandler<ObjectDatabase>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<ObjectDatabase>( BinaryFile.Load<ObjectDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<ObjectSetInfo>( "Object sets", Data.ObjectSets, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public ObjectDatabaseNode( string name, ObjectDatabase data ) : base( name, data )
        {
        }

        public ObjectDatabaseNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }

    public class ObjectSetInfoNode : Node<ObjectSetInfo>
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
        [DisplayName( "Texture file name" )]
        public string TextureFileName
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Archive file name" )]
        public string ArchiveFileName
        {
            get => GetProperty<string>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<ObjectInfo>( "Objects", Data.Objects, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public ObjectSetInfoNode( string name, ObjectSetInfo data ) : base( name, data )
        {
        }
    }

    public class ObjectInfoNode : Node<ObjectInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [Category( "General" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint Id
        {
            get => GetProperty<uint>();
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

        public ObjectInfoNode( string name, ObjectInfo data ) : base( name, data )
        {
        }
    }
}