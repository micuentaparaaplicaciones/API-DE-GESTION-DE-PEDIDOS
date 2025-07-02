namespace API.Entities
{
    public class User
    {
        public int Key { get; set; } // se genera automáticamente

        public string Identification { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty; // se hashea manualmente

        public string Role { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } // se establece automáticamente

        public DateTime? ModificationDate { get; set; } // se establece automáticamente

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public long RowVersion { get; set; } // se genera automáticamente

        public virtual User? CreatedByUser { get; set; } // relación opcional 

        public virtual User? ModifiedByUser { get; set; } // relación opcional 
    }
}
