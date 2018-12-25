namespace TTController.Common
{
    public struct PortIdentifier
    {
        public int ControllerVendorId { get; }
        public int ControllerProductId { get; }
        public byte Id { get; }

        public PortIdentifier(int controllerVendorId, int controllerProductId, byte id)
        {
            ControllerVendorId = controllerVendorId;
            ControllerProductId = controllerProductId;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PortIdentifier))
                return false;

            var identifier = (PortIdentifier)obj;
            return ControllerVendorId == identifier.ControllerVendorId &&
                   ControllerProductId == identifier.ControllerProductId &&
                   Id == identifier.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -1710605561;
            hashCode = hashCode * -1521134295 + ControllerVendorId.GetHashCode();
            hashCode = hashCode * -1521134295 + ControllerProductId.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }
}
