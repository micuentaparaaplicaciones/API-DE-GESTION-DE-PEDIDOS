namespace API.Entities
{
    public class Product
    {
        public int Key { get; set; } // Automatically generated

        public byte[] Image { get; set; } = [];  // usar en swagger   "image": "AAECAwQ=",

        public string Name { get; set; } = string.Empty;

        public string Detail { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int AvailableQuantity { get; set; }

        public DateTime CreationDate { get; set; } // Automatically set

        public DateTime? ModificationDate { get; set; } // Automatically set

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int SuppliedBy { get; set; }

        public int CategorizedBy { get; set; }

        public long RowVersion { get; set; } // Automatically generated

        public virtual User? CreatedByUser { get; set; } // Optional relationship

        public virtual User? ModifiedByUser { get; set; } // Optional relationship

        public virtual Supplier? SuppliedBySupplier { get; set; } // Optional relationship

        public virtual Category? CategorizedByCategory { get; set; } // Optional relationship
    }
}
