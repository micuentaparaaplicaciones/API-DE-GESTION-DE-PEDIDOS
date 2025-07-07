namespace API.Entities
{
    public class Category
    {
        public int Key { get; set; } // Automatically generated

        public string Name { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } // Automatically set

        public DateTime? ModificationDate { get; set; } // Automatically set

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public long RowVersion { get; set; } // Automatically generated

        public virtual User? CreatedByUser { get; set; } // Optional relationship

        public virtual User? ModifiedByUser { get; set; } // Optional relationship
    }
}
