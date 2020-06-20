namespace MikuMikuLibrary.Motions
{
    public class Key
    {
        public float Value { get; set; }
        public float Tangent { get; set; }
        public ushort Frame { get; set; }

        public override string ToString() => 
            $"{Frame}, {Value}, {Tangent}";
    }
}