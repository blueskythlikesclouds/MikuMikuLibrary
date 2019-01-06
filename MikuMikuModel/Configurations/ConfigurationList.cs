using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MikuMikuModel.Configurations
{
    public class ConfigurationList : ICloneable, IEquatable<ConfigurationList>
    {
        private static ConfigurationList sInstance;
        private static readonly XmlSerializer sSerializer = new XmlSerializer( typeof( ConfigurationList ) );

        public static ConfigurationList Instance
        {
            get
            {
                if ( sInstance == null )
                {
                    if ( !File.Exists( FilePath ) )
                    {
                        sInstance = new ConfigurationList();
                        sInstance.Save();
                    }

                    else
                    {
                        using ( var stream = File.OpenRead( FilePath ) )
                            sInstance = ( ConfigurationList )sSerializer.Deserialize( stream );
                    }
                }

                return sInstance;
            }
        }

        public static string FilePath => Path.ChangeExtension( Application.ExecutablePath, "xml" );
        public static string BackupFilePath => Path.ChangeExtension( Path.ChangeExtension( Application.ExecutablePath, null ) + "-Backup", "xml" );

        private Configuration mCurrentConfiguration;

        public List<Configuration> Configurations { get; }

        [XmlIgnore]
        public Configuration CurrentConfiguration
        {
            get => mCurrentConfiguration;
            set
            {
                mCurrentConfiguration = value;

                if ( !Configurations.Contains( value ) )
                    Configurations.Add( value );
            }
        }

        public void DetermineCurrentConfiguration( string referenceFilePath )
        {
            mCurrentConfiguration = FindConfiguration( referenceFilePath ) ?? mCurrentConfiguration;
        }

        public Configuration FindConfiguration( string referenceFilePath )
        {
            var directoryPath = Path.GetFullPath( Path.GetDirectoryName( referenceFilePath ) ) + Path.DirectorySeparatorChar;
            foreach ( var configuration in Configurations )
            {
                var directoryPath2 = Path.GetFullPath( Path.GetDirectoryName( configuration.ObjectDatabaseFilePath ) ) + Path.DirectorySeparatorChar;
                if ( directoryPath.StartsWith( directoryPath2, StringComparison.OrdinalIgnoreCase ) )
                    return configuration;
            }

            return null;
        }

        public void Save()
        {
            if ( File.Exists( FilePath ) )
                File.Copy( FilePath, BackupFilePath, true );

            using ( var stream = File.Create( FilePath ) )
                sSerializer.Serialize( stream, this );
        }

        public object Clone()
        {
            var clone = new ConfigurationList();
            clone.Configurations.Capacity = Configurations.Count;

            foreach ( var configuration in Configurations )
                clone.Configurations.Add( ( Configuration )configuration.Clone() );

            return clone;
        }

        public bool Equals( ConfigurationList other )
        {
            if ( ReferenceEquals( other, this ) )
                return true;

            if ( other.Configurations.Count == Configurations.Count )
            {
                for ( int i = 0; i < other.Configurations.Count; i++ )
                {
                    if ( !other.Configurations[ i ].Equals( Configurations[ i ] ) )
                        return false;
                }

                return true;
            }

            return false;
        }

        private ConfigurationList()
        {
            Configurations = new List<Configuration>();
        }
    }
}
