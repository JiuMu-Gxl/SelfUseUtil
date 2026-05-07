using Microsoft.EntityFrameworkCore;
using WorkService.MockApi.Models.Iot;

namespace WorkService.MockApi.Models
{
    public partial class MqttDbContext : DbContext
    {
        public MqttDbContext(DbContextOptions<MqttDbContext> options)
        : base(options)
        {
        }

        public DbSet<MqttUser> MqttUsers => Set<MqttUser>();
        public DbSet<MqttAcl> MqttAcls => Set<MqttAcl>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MQTT_USER（补充默认值 + 精准控制）
            modelBuilder.Entity<MqttUser>(entity =>
            {
                entity.Property(x => x.Created)
                      .HasDefaultValueSql("NOW()");

                entity.Property(x => x.IsSuperuser)
                      .HasDefaultValue(false);
            });

            // MQTT_ACL（索引补充）
            modelBuilder.Entity<MqttAcl>(entity =>
            {
                entity.HasIndex(x => x.Username)
                      .HasDatabaseName("mqtt_acl_username_idx");
            });
        }
    }
}
