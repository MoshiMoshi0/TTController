using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public struct PortIdentifier
    {
        public int VendorId { get; }
        public int ProductId { get; }
        public int Id { get; }

        public PortIdentifier(int vendorId, int productId, int id)
        {
            VendorId = vendorId;
            ProductId = productId;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PortIdentifier))
                return false;

            var identifier = (PortIdentifier)obj;
            return VendorId == identifier.VendorId &&
                   ProductId == identifier.ProductId &&
                   Id == identifier.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -1710605561;
            hashCode = hashCode * -1521134295 + VendorId.GetHashCode();
            hashCode = hashCode * -1521134295 + ProductId.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }
}
