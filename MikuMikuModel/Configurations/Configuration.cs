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

        private ObjectDatabase mObjectDatabase;
        private TextureDatabase mTextureDatabase;
        private BoneDatabase mBoneDatabase;

        public string Name { get; set; }
        public string ObjectDatabaseFilePath { get; set; }
        public string TextureDatabaseFilePath { get; set; }
        public string BoneDatabaseFilePath { get; set; }

        public ObjectDatabase ObjectDatabase
        {
            get
            {
                if ( mObjectDatabase == null && File.Exists( ObjectDatabaseFilePath ) )
                {
                    mObjectDatabase = BinaryFile.Load<ObjectDatabase>( ObjectDatabaseFilePath );
                    DoBackup( ObjectDatabaseFilePath );
                }

                return mObjectDatabase;
            }
            set => mObjectDatabase = value;
        }

        public TextureDatabase TextureDatabase
        {
            get
            {
                if ( mTextureDatabase == null && File.Exists( TextureDatabaseFilePath ) )
                {
                    mTextureDatabase = BinaryFile.Load<TextureDatabase>( TextureDatabaseFilePath );
                    DoBackup( TextureDatabaseFilePath );
                }

                return mTextureDatabase;
            }
            set => mTextureDatabase = value;
        }

        public BoneDatabase BoneDatabase
        {
            get
            {
                if ( mBoneDatabase == null && File.Exists( BoneDatabaseFilePath ) )
                {
                    mBoneDatabase = BinaryFile.Load<BoneDatabase>( BoneDatabaseFilePath );
                    DoBackup( BoneDatabaseFilePath );
                }

                return mBoneDatabase;
            }
            set => mBoneDatabase = value;
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
