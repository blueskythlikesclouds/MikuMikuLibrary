using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MikuMikuModel.Configurations
{
    public class Configuration : ICloneable, IEquatable<Configuration>
    {
        public static readonly string BackupDirectory = Path.Combine(
            Path.GetDirectoryName( Application.ExecutablePath ), "Backups" );

        private ObjectDatabase objectDatabase;
        private TextureDatabase textureDatabase;
        private BoneDatabase boneDatabase;

        public string Name { get; set; }
        public string ObjectDatabaseFilePath { get; set; }
        public string TextureDatabaseFilePath { get; set; }
        public string BoneDatabaseFilePath { get; set; }

        public ObjectDatabase ObjectDatabase
        {
            get
            {
                if ( objectDatabase == null && File.Exists( ObjectDatabaseFilePath ) )
                {
                    objectDatabase = BinaryFile.Load<ObjectDatabase>( ObjectDatabaseFilePath );
                    DoBackup( ObjectDatabaseFilePath );
                }

                return objectDatabase;
            }
            set => objectDatabase = value;
        }

        public TextureDatabase TextureDatabase
        {
            get
            {
                if ( textureDatabase == null && File.Exists( TextureDatabaseFilePath ) )
                {
                    textureDatabase = BinaryFile.Load<TextureDatabase>( TextureDatabaseFilePath );
                    DoBackup( TextureDatabaseFilePath );
                }

                return textureDatabase;
            }
            set => textureDatabase = value;
        }

        public BoneDatabase BoneDatabase
        {
            get
            {
                if ( boneDatabase == null && File.Exists( BoneDatabaseFilePath ) )
                {
                    boneDatabase = BinaryFile.Load<BoneDatabase>( BoneDatabaseFilePath );
                    DoBackup( BoneDatabaseFilePath );
                }

                return boneDatabase;
            }
            set => boneDatabase = value;
        }

        public object Clone()
        {
            return new Configuration
            {
                Name = Name,
                ObjectDatabase = ObjectDatabase,
                ObjectDatabaseFilePath = ObjectDatabaseFilePath,
                TextureDatabase = TextureDatabase,
                TextureDatabaseFilePath = TextureDatabaseFilePath,
                BoneDatabase = BoneDatabase,
                BoneDatabaseFilePath = BoneDatabaseFilePath,
            };
        }

        public void DoBackup( string sourceFileName )
        {
            Directory.CreateDirectory( BackupDirectory );
            string destinationFileName = Path.Combine( BackupDirectory, $"{Name}_{Path.GetFileName( sourceFileName )}" );
            File.Copy( sourceFileName, destinationFileName, true );
            Debug.WriteLine( $"Did backup of {sourceFileName} to {destinationFileName}" );
        }

        public bool Equals( Configuration other )
        {
            return other.Name == Name &&
                other.ObjectDatabaseFilePath == ObjectDatabaseFilePath &&
                other.TextureDatabaseFilePath == TextureDatabaseFilePath &&
                other.BoneDatabaseFilePath == BoneDatabaseFilePath;
        }
    }
}
