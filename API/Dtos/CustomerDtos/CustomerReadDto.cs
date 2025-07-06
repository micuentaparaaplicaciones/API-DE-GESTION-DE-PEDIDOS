namespace API.Dtos.CustomerDtos
{
    public class CustomerReadDto
    {
        public int Key { get; set; }
        public required string Identification { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public long RowVersion { get; set; }
    }
}
