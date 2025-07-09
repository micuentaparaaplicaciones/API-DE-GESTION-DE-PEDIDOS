namespace API.Dtos.SupplierDtos
{
    public class SupplierReadDto
    {
        public int Key { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public long RowVersion { get; set; }
    }
}
