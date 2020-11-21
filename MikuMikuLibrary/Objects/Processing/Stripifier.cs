namespace MikuMikuLibrary.Objects.Processing
{
    public static class Stripifier
    {
        public static uint[] Stripify( uint[] indices )
        {
            return Native.Stripifier.Stripify( indices );
        }
    }
}