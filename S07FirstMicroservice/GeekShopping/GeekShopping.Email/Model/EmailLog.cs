using GeekShopping.Email.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.Email.Model
{
    [Table("email_logs")]
    public class EmailLog : BaseEntity
    {
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("log")]
        public string Log { get; set; } = string.Empty;

        [Column("send_date")]
        public DateTime SendDate { get; set; }
    }
}