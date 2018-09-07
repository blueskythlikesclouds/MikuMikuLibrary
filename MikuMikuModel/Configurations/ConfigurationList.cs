using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MikuMikuModel.Configurations
{
    public class ConfigurationList : ICloneable, IEquatable<ConfigurationList>
    {
        private static ConfigurationList instance;
        private static readonly XmlSerializer serializer = new XmlSerializer( typeof( ConfigurationList ) );

        public static ConfigurationList Instance
        {
            get
            {
                if ( instance == null )
                {
                    if ( !File.Exists( FilePath ) )
                    {
                        instance = new ConfigurationList();
                        instance.Save();
                    }

                    else
                    {
                        using ( var stream = File.OpenRead( FilePath ) )
                            instance = ( ConfigurationList )serializer.Deserialize( stream );
                    }
                }

                return instance;
            }
        }

        public static string FilePath
        {
            get { return Path.ChangeExtension( Application.ExecutablePath, "xml" ); }
        }

        public static string BackupFilePath
        {
            get { return Path.ChangeExtension( Path.ChangeExtension( Application.ExecutablePath, null ) + "-Backup", "xml" ); }
        }

        private Configuration currentConfiguration;

        public List<Configuration> Configurations { get; }

        [XmlIgnore]
        public Configuration CurrentConfiguration
        {
            get { return currentConfiguration; }
            set
            {
                currentConfiguration = value;

                if ( !Configurations.Contains( value ) )
                    Configurations.Add( value );
            }
        }

        public void DetermineCurrentConfiguration( string referenceFilePath )
        {
            currentConfiguration = FindConfiguration( referenceFilePath ) ?? currentConfiguration;
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
            {
                File.Copy( FilePath, BackupFilePath, true );
            }

            using ( var stream = File.Create( FilePath ) )
                serializer.Serialize( stream, this );
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
