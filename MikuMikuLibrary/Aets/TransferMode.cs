using System;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public enum BlendMode
    {
        None = 0,
        Copy = 1,
        Behind = 2,
        Normal = 3,
        Dissolve = 4,
        Add = 5,
        Multiply = 6,
        Screen = 7,
        Overlay = 8,
        SoftLight = 9,
        HardLight = 10,
        Darken = 11,
        Lighten = 12,
        ClassicDifference = 13,
        Hue = 14,
        Saturation = 15,
        Color = 16,
        Luminosity = 17,
        StencilAlpha = 18,
        StencilLuma = 19,
        SilhouetteAlpha = 20,
        SilhouetteLuma = 21,
        LuminescentPremul = 22,
        AlphaAdd = 23,
        ClassicColorDodge = 24,
        ClassicColorBurn = 25,
        Exclusion = 26,
        Difference = 27,
        ColorDodge = 28,
        ColorBurn = 29,
        LinearDodge = 30,
        LinearBurn = 31,
        LinearLight = 32,
        VividLight = 33,
        PinLight = 34,
        HardMix = 35,
        LighterColor = 36,
        DarkerColor = 37,
        Subtract = 38,
        Divide = 39
    }

    public enum TrackMatte
    {
        NoTrackMatte = 0,
        Alpha = 1,
        NotAlpha = 2,
        Luma = 3,
        NotLuma = 4
    }

    [Flags]
    public enum TransferFlags
    {
        PreserveAlpha = 1 << 0,
        RandomizeDissolve = 1 << 1
    }

    public class TransferMode
    {
        public BlendMode BlendMode { get; set; }
        public TransferFlags Flags { get; set; }
        public TrackMatte TrackMatte { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            BlendMode = ( BlendMode ) reader.ReadByte();
            Flags = ( TransferFlags ) reader.ReadByte();
            TrackMatte = ( TrackMatte ) reader.ReadByte();
            
            reader.SeekCurrent( 1 );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( byte ) BlendMode );
            writer.Write( ( byte ) Flags );
            writer.Write( ( byte ) TrackMatte );
            writer.WriteNulls( 1 );
        }
    }
}