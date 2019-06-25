﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Motions
{
    public class MotionSetNode : BinaryFileNode<MotionSet>
    {
        private static readonly XmlSerializer sMotionSetInfoSerializer = new XmlSerializer( typeof( MotionSetInfo ) );

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            RegisterImportHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
                Data.Motions.Add( motion );
            } );
            RegisterImportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                Data.Motions.AddRange( motionSet.Motions );
            } );
            RegisterReplaceHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                return motionSet;
            } );
            RegisterReplaceHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                var motionSet = new MotionSet();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                    motionSet.Motions.Add( motion );
                }
                return motionSet;
            } );
            RegisterExportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                {
                    Data.Save( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ], configuration?.MotionDatabase );
                }
            } );
            RegisterCustomHandler( "Copy motion database info to clipboard", () =>
            {
                int id = -1;

                using ( var inputDialog = new InputDialog
                {
                    WindowTitle = "Enter base id for motions",
                    Input = Math.Max( 0, Data.Motions.Max( x => x.Id ) + 1 ).ToString()
                } )
                {
                    while ( inputDialog.ShowDialog() == DialogResult.OK )
                    {
                        bool result = int.TryParse( inputDialog.Input, out id );

                        if ( !result || id < 0 )
                        {
                            MessageBox.Show( "Please enter a correct id number.", "Miku Miku Model",
                                MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if ( id < 0 )
                    return;

                var motionSetInfo = new MotionSetInfo
                {
                    Id = 39,
                    Name = Path.GetFileNameWithoutExtension( Name ),
                };

                if ( motionSetInfo.Name.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetInfo.Name = motionSetInfo.Name.Remove( 0, 4 );

                foreach ( var motion in Data.Motions )
                {
                    motionSetInfo.Motions.Add( new MotionInfo
                    {
                        Name = motion.Name,
                        Id = id++,
                    } );
                }

                using ( var stringWriter = new StringWriter() )
                using ( var xmlWriter =
                    XmlWriter.Create( stringWriter,
                        new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true } ) )
                {
                    sMotionSetInfoSerializer.Serialize( xmlWriter, motionSetInfo,
                        new XmlSerializerNamespaces( new[] { XmlQualifiedName.Empty } ) );

                    Clipboard.SetText( stringWriter.ToString() );
                }
            } );

            base.Initialize();
        }

        protected override void Load( MotionSet data, Stream source ) =>
            data.Load( source,
                SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                SourceConfiguration?.MotionDatabase );

        protected override void PopulateCore()
        {
            var motionDatabase = SourceConfiguration?.MotionDatabase;
            if ( motionDatabase != null )
            {
                string motionSetName = Path.GetFileNameWithoutExtension( Name );
                if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetName = motionSetName.Substring( 4 );

                var motionSetInfo = motionDatabase.GetMotionSetInfo( motionSetName );
                if ( motionSetInfo != null && Data.Motions.Count == motionSetInfo.Motions.Count )
                    for ( int i = 0; i < motionSetInfo.Motions.Count; i++ )
                    {
                        Data.Motions[ i ].Name = motionSetInfo.Motions[ i ].Name;
                        Data.Motions[ i ].Id = motionSetInfo.Motions[ i ].Id;
                    }
            }

            Nodes.Add( new ListNode<Motion>( "Motions", Data.Motions, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionSetNode( string name, MotionSet data ) : base( name, data )
        {
        }

        public MotionSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}