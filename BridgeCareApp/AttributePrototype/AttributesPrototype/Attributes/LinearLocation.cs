namespace AttributesPrototype.Attributes
{
    public class LinearLocation : Location
    {
        public double Start { get; }

        public double End { get; }

        public Direction Direction { get; }

        // The uniqueIdentifier can really be any uniquely identifiable string
        // of characters. (ROUTE-BMP-EMP-DIR for example).
        public LinearLocation(double start, double end, Direction direction, string uniqueIdentifier) : base(uniqueIdentifier)
        {
        }
    }
}
