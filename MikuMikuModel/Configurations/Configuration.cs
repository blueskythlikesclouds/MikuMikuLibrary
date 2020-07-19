using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Configurations
{
    public class Configuration : ICloneable, IEquatable<Configuration>
    {
        private static readonly Dictionary<Type, XmlSerializer> sSerializers;

        private ObjectDatabase mObjectDatabase;
        private TextureDatabase mTextureDatabase;
        private BoneDatabase mBoneDatabase;
        private MotionDatabase mMotionDatabase;

        public string Name { get; set; }
        public string ObjectDatabaseFilePath { get; set; }
        public string TextureDatabaseFilePath { get; set; }
        public string BoneDatabaseFilePath { get; set; }
        public string MotionDatabaseFilePath { get; set; }

        public DirectoryInfo BaseDirectory => new DirectoryInfo( ResourceStore.GetPath( Path.Combine( "Configurations", Name ) ) );

        [XmlIgnore]
        public ObjectDatabase ObjectDatabase
        {
            get => mObjectDatabase ?? ( mObjectDatabase = Load( ObjectDatabaseFilePath, BinaryFile.Load<ObjectDatabase> ) );
            set => mObjectDatabase = value;
        }

        [XmlIgnore]
        public TextureDatabase TextureDatabase
        {
            get => mTextureDatabase ?? ( mTextureDatabase = Load( TextureDatabaseFilePath, BinaryFile.Load<TextureDatabase> ) );
            set => mTextureDatabase = value;
        }

        [XmlIgnore]
        public BoneDatabase BoneDatabase
        {
            get => mBoneDatabase ?? ( mBoneDatabase = Load( BoneDatabaseFilePath, BinaryFile.Load<BoneDatabase> ) );
            set => mBoneDatabase = value;
        }

        [XmlIgnore]
        public MotionDatabase MotionDatabase
        {
            get => mMotionDatabase ?? ( mMotionDatabase = Load( MotionDatabaseFilePath, filePath =>
            {
                if ( !filePath.EndsWith( ".farc", StringComparison.OrdinalIgnoreCase ) )
                    return BinaryFile.Load<MotionDatabase>( filePath );

                using ( var farcArchive = BinaryFile.Load<FarcArchive>( filePath ) )
                using ( var entryStream = farcArchive.Open( "mot_db.bin", EntryStreamMode.MemoryStream ) )
                {
                    return BinaryFile.Load<MotionDatabase>( entryStream );
                }
            } ) );
            set => mMotionDatabase = value;
        }

        public object Clone()
        {
            return new Configuration
            {
                Name = Name,
                ObjectDatabaseFilePath = ObjectDatabaseFilePath,
                TextureDatabaseFilePath = TextureDatabaseFilePath,
                BoneDatabaseFilePath = BoneDatabaseFilePath,
                MotionDatabaseFilePath = MotionDatabaseFilePath
            };
        }

        public bool Equals( Configuration other )
        {
            return StringEquals( other.Name, Name ) &&
                   StringEquals( other.ObjectDatabaseFilePath, ObjectDatabaseFilePath ) &&
                   StringEquals( other.TextureDatabaseFilePath, TextureDatabaseFilePath ) &&
                   StringEquals( other.BoneDatabaseFilePath, BoneDatabaseFilePath ) &&
                   StringEquals( other.MotionDatabaseFilePath, MotionDatabaseFilePath );

            bool StringEquals( string left, string right ) => 
                ( string.IsNullOrEmpty( left ) && string.IsNullOrEmpty( right ) ) || left == right;
        }

        public void Save()
        {
            Save( ObjectDatabase );
            Save( TextureDatabase );
            Save( BoneDatabase );
            Save( MotionDatabase );
        }

        public void Clean()
        {
            Delete<ObjectDatabase>();
            Delete<TextureDatabase>();
            Delete<BoneDatabase>();
            Delete<MotionDatabase>();

            mObjectDatabase = null;
            mTextureDatabase = null;
            mBoneDatabase = null;
            mMotionDatabase = null;
        }

        public void SaveTextureDatabase()
        {
            if ( mTextureDatabase == null || string.IsNullOrEmpty( TextureDatabaseFilePath ) )
                return;

            mTextureDatabase.Save( TextureDatabaseFilePath );
            Save( TextureDatabase );
        }

        private static XmlSerializer GetSerializer<T>()
        {
            if ( !sSerializers.TryGetValue( typeof( T ), out var serializer ) )
                sSerializers[ typeof( T ) ] = serializer = new XmlSerializer( typeof( T ) );

            return serializer;
        }

        private T Load<T>( string filePath, Func<string, T> loader ) where T : class
        {
            if ( !File.Exists( filePath ) )
                return null;

            string xmlFilePath = GetPath( $"{typeof( T ).Name}.xml" );

            if ( File.Exists( xmlFilePath ) )
            {
                try
                {
                    var serializer = GetSerializer<T>();

                    using ( var reader = new StreamReader( xmlFilePath, Encoding.UTF8 ) )
                        return ( T ) serializer.Deserialize( reader );
                }
                catch
                {
                    // ignored
                }
            }

            BackupFile( filePath );

            var obj = loader( filePath );
            {
                Save( obj );
            }

            return obj;
        }

        private void Save<T>( T obj ) where T : class
        {
            if ( obj == null )
                return;

            var serializer = GetSerializer<T>();

            using ( var writer = new StreamWriter( GetPath( $"{typeof( T ).Name}.xml" ), false, Encoding.UTF8 ) ) 
                serializer.Serialize( writer, obj );
        }

        private void Delete<T>()
        {
            string filePath = Path.Combine( BaseDirectory.FullName, $"{typeof( T ).Name}.xml" );

            if ( File.Exists( filePath ) )
                File.Delete( filePath );
        }

        private void BackupFile( string filePath ) => 
            File.Copy( filePath, GetPath( Path.Combine( "Sources", Path.GetFileName( filePath ) ) ), true );

        private string GetPath( string relativePath )
        {
            var fileInfo = new FileInfo( Path.Combine( BaseDirectory.FullName, relativePath ) );

            fileInfo.Directory?.Create();
            return fileInfo.FullName;
        }

        public override string ToString()
        {
            return Name;
        }

        static Configuration()
        {
            sSerializers = new Dictionary<Type, XmlSerializer>();
        }
    }
}