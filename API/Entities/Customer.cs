namespace API.Entities
{
    public class Customer
    {
        public int Key { get; set; } // Automatically generated

        public string Identification { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty; // Manually hashed

        public DateTime CreationDate { get; set; } // Automatically set

        public DateTime? ModificationDate { get; set; } // Automatically set

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public long RowVersion { get; set; } // Automatically generated

        public virtual User? CreatedByUser { get; set; } // Optional relationship

        public virtual User? ModifiedByUser { get; set; } // Optional relationship
    }
}
