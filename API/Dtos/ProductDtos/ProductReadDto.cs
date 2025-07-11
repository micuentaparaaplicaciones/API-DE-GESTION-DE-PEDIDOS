namespace API.Dtos.ProductDtos
{
    public class ProductReadDto
    {
        public int Key { get; set; } 

        public byte[] Image { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public string Detail { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int AvailableQuantity { get; set; }

        public DateTime CreationDate { get; set; } 

        public DateTime? ModificationDate { get; set; } 

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int SuppliedBy { get; set; }

        public int CategorizedBy { get; set; }

        public long RowVersion { get; set; } 
    }
}
