using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra
{
    public interface IBlock
    {
        string Signature { get; }

        void Read( EndianBinaryReader reader, StringSet stringSet );
        void Write( EndianBinaryWriter writer, StringSet stringSet );
    }
}