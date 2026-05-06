using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkService.MockApi.Models.Iot
{
    [Table("mqtt_user")]
    [Index(nameof(Username), IsUnique = true)]
    public class MqttUser
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; } = null!;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [Column("salt")]
        public string Salt { get; set; } = null!;

        [Column("is_superuser")]
        public bool IsSuperuser { get; set; } = false;

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
