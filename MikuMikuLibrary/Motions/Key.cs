namespace MikuMikuLibrary.Motions
{
    public class Key
    {
        public float Value { get; set; }
        public float Interpolation { get; set; }
        public int FrameIndex { get; set; }

        public override string ToString() =>
            $"{FrameIndex}, {Value}, {Interpolation}";
    }
}