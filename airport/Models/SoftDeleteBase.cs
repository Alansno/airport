namespace airport.Models
{
    public abstract class SoftDeleteBase
    {
        public bool isDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
