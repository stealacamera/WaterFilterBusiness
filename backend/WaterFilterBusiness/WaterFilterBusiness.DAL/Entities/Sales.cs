using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.DAL.Entities
{
    public class Sale : Entity
    {
        public int MeetingId { get; set; }
        public bool IsVerified { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        public double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime AuthenticatedAt { get; set; }
        public string AuthenticationNote { get; set; }
    }
}
