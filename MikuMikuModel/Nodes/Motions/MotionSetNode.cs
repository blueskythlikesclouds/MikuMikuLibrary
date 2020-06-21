using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
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
            AddImportHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
                Data.Motions.Add( motion );
            } );
            AddImportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                Data.Motions.AddRange( motionSet.Motions );
            } );
            AddReplaceHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                return motionSet;
            } );
            AddReplaceHandler<Motion>( filePath =>
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
            AddExportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                {
                    Data.Save( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ], configuration?.MotionDatabase );
                }
            } );
            AddCustomHandler( "Copy motion set info to clipboard", () =>
            {
                uint motionSetId = 39;
                uint motionId = 0xFFFFFFFF;

                var motionDatabase = ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase;

                if ( motionDatabase != null && motionDatabase.MotionSets.Count > 0 )
                {
                    motionSetId = motionDatabase.MotionSets.Max( x => x.Id ) + 1;
                    motionId = motionDatabase.MotionSets.SelectMany( x => x.Motions ).Max( x => x.Id ) + 1;
                }

                else
                {
                    using ( var inputDialog = new InputDialog
                    {
                        WindowTitle = "Enter base id for motions",
                        Input = Math.Max( 0, Data.Motions.Max( x => x.Id ) + 1 ).ToString()
                    } )
                    {
                        while ( inputDialog.ShowDialog() == DialogResult.OK )
                        {
                            bool result = uint.TryParse( inputDialog.Input, out motionId );

                            if ( !result || motionId == 0xFFFFFFFF )
                                MessageBox.Show( "Please enter a correct id number.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );

                            else
                                break;
                        }
                    }
                }

                if ( motionId == 0xFFFFFFFF )
                    return;

                var motionSetInfo = new MotionSetInfo
                {
                    Id = motionSetId,
                    Name = Path.GetFileNameWithoutExtension( Name ).ToUpperInvariant()
                };

                if ( motionSetInfo.Name.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetInfo.Name = motionSetInfo.Name.Remove( 0, 4 );

                foreach ( var motion in Data.Motions )
                {
                    motionSetInfo.Motions.Add( new MotionInfo
                    {
                        Name = motion.Name,
                        Id = motionId++
                    } );
                }

                using ( var stringWriter = new StringWriter() )
                using ( var xmlWriter = XmlWriter.Create( stringWriter, 
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
            data.Load( source, SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ], SourceConfiguration?.MotionDatabase );

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
                {
                    for ( int i = 0; i < motionSetInfo.Motions.Count; i++ )
                    {
                        Data.Motions[ i ].Name = motionSetInfo.Motions[ i ].Name;
                        Data.Motions[ i ].Id = motionSetInfo.Motions[ i ].Id;
                    }
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