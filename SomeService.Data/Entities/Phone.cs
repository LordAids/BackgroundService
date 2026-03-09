using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SomeService.Data.Entities
{
    [Table("Phones")]
    public class Phone : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }

        public int OfficeId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? Additional { get; set; }

        [ForeignKey(nameof(OfficeId))]
        public Office Office { get; set; }
    }
}
