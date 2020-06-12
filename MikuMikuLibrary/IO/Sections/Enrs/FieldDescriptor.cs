namespace MikuMikuLibrary.IO.Sections.Enrs
{
    public enum ValueType
    {
        Int16,
        Int32,
        Int64
    }

    public class FieldDescriptor
    {
        public long Position { get; set; }
        public int RepeatCount { get; set; }
        public ValueType ValueType { get; set; }
    }
}