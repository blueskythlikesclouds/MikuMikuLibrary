using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    [Flags]
    public enum LayerFlags
    {
        /// <summary>
        /// Toggles layer visuals on or off
        /// </summary>
        VideoActive = 1 << 0,

        /// <summary>
        /// Toggles layer sounds on or off
        /// </summary>
        AudioActive = 1 << 1,

        EffectsActive = 1 << 2,

        /// <summary>
        /// Toggles motion blur on or off for the layer
        /// </summary>
        MotionBlur = 1 << 3,

        FrameBlending = 1 << 4,

        /// <summary>
        /// Locks layer contents, preventing all changes
        /// </summary>
        Locked = 1 << 5,

        /// <summary>
        /// Hides the current layer when the "Hide Shy Layers" composition switch is selected
        /// </summary>
        Shy = 1 << 6,

        Collapse = 1 << 7,

        AutoOrientRotation = 1 << 8,

        /// <summary>
        /// Identifies the layer as an adjustment layer
        /// </summary>
        AdjustmentLayer = 1 << 9,

        TimeRemapping = 1 << 10,

        /// <summary>
        /// Identifies the layer as a 3D layer
        /// </summary>
        LayerIs3D = 1 << 11,

        LookAtCamera = 1 << 12,

        LookAtPointOfInterest = 1 << 13,

        /// <summary>
        /// Includes the current layer in previews and renders, ignoring layers without this switch set
        /// </summary>
        Solo = 1 << 14,

        MarkersLocked = 1 << 15
    }

    public enum LayerQuality
    {
        None = 0,
        Wireframe = 1,
        Draft = 2,
        Best = 3
    }

    public class Layer
    {
        internal long ReferenceOffset { get; private set; }
        internal ItemType ItemType { get; private set; }
        internal long ItemOffset { get; private set; }
        internal long ParentOffset { get; private set; }

        public string Name { get; set; }

        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float OffsetTime { get; set; }
        public float TimeScale { get; set; }

        public LayerFlags Flags { get; set; }
        public LayerQuality Quality { get; set; }

        public object Item { get; set; }

        public Layer Parent { get; set; }

        public List<Marker> Markers { get; }

        public LayerAudio Audio { get; set;}
        public LayerVideo Video { get; set;}

        internal void Read( EndianBinaryReader reader )
        {
            ReferenceOffset = reader.Offset;

            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            OffsetTime = reader.ReadSingle();
            TimeScale = reader.ReadSingle();

            Flags = ( LayerFlags ) reader.ReadUInt16();
            Quality = ( LayerQuality ) reader.ReadByte();
            ItemType = ( ItemType ) reader.ReadByte();
            ItemOffset = reader.ReadOffset();
            ParentOffset = reader.ReadOffset();

            int markerCount = reader.ReadInt32();
            reader.ReadOffset( () =>
            {
                Markers.Capacity = markerCount;

                for ( int i = 0; i < markerCount; i++ )
                {
                    var marker = new Marker();
                    marker.Read( reader );
                    Markers.Add( marker );
                }
            } );

            reader.ReadOffset( () =>
            {
                Video = new LayerVideo();
                Video.Read( reader );
            } );

            reader.ReadOffset( () =>
            {
                Audio = new LayerAudio();
                Audio.Read( reader );
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            ReferenceOffset = writer.Offset;

            writer.AddStringToStringTable( Name );

            writer.Write( StartTime );
            writer.Write( EndTime );
            writer.Write( OffsetTime );
            writer.Write( TimeScale );

            writer.Write( ( ushort ) Flags );
            writer.Write( ( byte ) Quality );

            var itemTypeAndOffsetGetter = GetItemTypeAndOffsetGetter();

            writer.Write( ( byte ) itemTypeAndOffsetGetter.ItemType );
            writer.DelayWriteOffsetIf( itemTypeAndOffsetGetter.OffsetGetter != null, itemTypeAndOffsetGetter.OffsetGetter );
            writer.DelayWriteOffsetIf( Parent != null, () => Parent.ReferenceOffset );

            writer.Write( Markers.Count );
            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var marker in Markers )
                    marker.Write( writer );
            } );
            
            writer.ScheduleWriteOffsetIf( Video != null, 8, AlignmentMode.Left, () =>
            {
                Video.Write( writer );
            } );

            writer.ScheduleWriteOffsetIf( Audio != null, 8, AlignmentMode.Left, () =>
            {
                Audio.Write( writer );
            } );
        }

        private ( ItemType ItemType, Func<long> OffsetGetter ) GetItemTypeAndOffsetGetter()
        {
            switch ( Item )
            {
                case Video video:
                    return ( ItemType.Video, () => video.ReferenceOffset );

                case Audio audio:
                    return ( ItemType.Audio, () => audio.ReferenceOffset );

                case Composition composition:
                    return ( ItemType.Composition, () => composition.ReferenceOffset );
            }

            return ( ItemType.None, null );
        }

        internal void ResolveReferences( Composition composition, Scene scene )
        {
            switch ( ItemType )
            {
                case ItemType.Video:
                    Item = scene.Videos.FirstOrDefault( x => x.ReferenceOffset == ItemOffset );
                    break;

                case ItemType.Audio:
                    Item = scene.Audios.FirstOrDefault( x => x.ReferenceOffset == ItemOffset );
                    break;

                case ItemType.Composition:
                    Item = scene.Compositions.FirstOrDefault( x => x.ReferenceOffset == ItemOffset );
                    break;
            }

            Parent = composition.Layers.FirstOrDefault( x => x.ReferenceOffset == ParentOffset );
        }

        public Layer()
        {
            Markers = new List<Marker>();
        }
    }

    internal enum ItemType
    {
        None = 0,
        Video = 1,
        Audio = 2,
        Composition = 3
    }
}