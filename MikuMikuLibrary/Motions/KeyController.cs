namespace MikuMikuLibrary.Motions
{
    public class KeyController
    {
        public string Name { get; set; }
        public KeySetVector Position { get; set; }
        public KeySetVector Rotation { get; set; }

        public void Merge( KeyController other )
        {
            if ( Position == null )
                Position = other.Position;
            else if ( other.Position != null )
                Position.Merge( other.Position );

            if ( Rotation == null )
                Rotation = other.Rotation;
            else if ( other.Rotation != null )
                Rotation.Merge( other.Rotation );
        }

        public void Sort()
        {
            Position?.Sort();
            Rotation?.Sort();
        }
    }
}