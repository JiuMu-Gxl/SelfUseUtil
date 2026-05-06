using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkService.MockApi.Models.Iot
{
    [Table("mqtt_acl")]
    [Index(nameof(Username))]
    public class MqttAcl
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        public string Username { get; set; } = null!;

        [Required]
        [Column("permission")]
        public string Permission { get; set; } = null!;

        [Required]
        [Column("action")]
        public string Action { get; set; } = null!;

        [Required]
        [Column("topic")]
        public string Topic { get; set; } = null!;

        [Column("qos")]
        public short? Qos { get; set; }

        [Column("retain")]
        public short? Retain { get; set; }
    }
}
