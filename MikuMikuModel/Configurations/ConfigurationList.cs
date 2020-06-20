using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Configurations
{
    public class ConfigurationList : ICloneable, IEquatable<ConfigurationList>
    {
        private static ConfigurationList sInstance;
        private static readonly XmlSerializer sSerializer = new XmlSerializer( typeof( ConfigurationList ) );

        private Configuration mCurrentConfiguration;

        public static ConfigurationList Instance
        {
            get
            {
                if ( sInstance != null )
                    return sInstance;

                if ( !File.Exists( FilePath ) )
                {
                    sInstance = new ConfigurationList();
                    sInstance.Save();
                }

                else
                {
                    try
                    {
                        using ( var stream = File.OpenRead( FilePath ) ) 
                            sInstance = ( ConfigurationList ) sSerializer.Deserialize( stream );
                    }

                    catch
                    {
                        sInstance = new ConfigurationList();
                        sInstance.Save();
                    }
                }

                return sInstance;
            }
        }

        public static string FilePath => ResourceStore.GetPath( "ConfigurationList.xml" );

        public List<Configuration> Configurations { get; }

        [XmlIgnore]
        public Configuration CurrentConfiguration
        {
            get => mCurrentConfiguration;
            set
            {
                mCurrentConfiguration = value;

                if ( !Configurations.Contains( value ) && value != null )
                    Configurations.Add( value );
            }
        }

        public object Clone()
        {
            var clone = new ConfigurationList();

            clone.Configurations.Capacity = Configurations.Count;

            foreach ( var configuration in Configurations )
                clone.Configurations.Add( ( Configuration ) configuration.Clone() );

            return clone;
        }

        public bool Equals( ConfigurationList other )
        {
            if ( ReferenceEquals( other, this ) )
                return true;

            return other.Configurations.Count == Configurations.Count &&
                   !other.Configurations.Where( ( t, i ) => !t.Equals( Configurations[ i ] ) ).Any();
        }

        public void DetermineCurrentConfiguration( string referenceFilePath ) => 
            mCurrentConfiguration = FindConfiguration( referenceFilePath ) ?? mCurrentConfiguration;

        public Configuration FindConfiguration( string referenceFilePath )
        {
            string directoryPath = Path.GetFullPath( Path.GetDirectoryName( referenceFilePath ) ) + Path.DirectorySeparatorChar;

            foreach ( var configuration in Configurations )
            {
                bool result = false;

                result |= ComparePath( configuration.ObjectDatabaseFilePath );
                result |= ComparePath( configuration.TextureDatabaseFilePath );
                result |= ComparePath( configuration.BoneDatabaseFilePath );
                result |= ComparePath( configuration.MotionDatabaseFilePath );

                if ( result )
                    return configuration;
            }

            bool ComparePath( string path )
            {
                return !string.IsNullOrEmpty( path ) &&
                       directoryPath.StartsWith( Path.GetFullPath( Path.GetDirectoryName( path ) ) + Path.DirectorySeparatorChar,
                           StringComparison.OrdinalIgnoreCase );
            }

            return null;
        }

        public void Save()
        {
            using ( var stream = File.Create( FilePath ) ) 
                sSerializer.Serialize( stream, this );

            foreach ( var configuration in Configurations )
                configuration.Save();
        }

        private ConfigurationList()
        {
            Configurations = new List<Configuration>();
        }
    }
}