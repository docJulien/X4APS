using System;
using System.ComponentModel.DataAnnotations;

namespace APS.Model
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Application { get; set; }

        [Required]
        public DateTime Logged { get; set; }

        [MaxLength(50)]
        [Required]
        public string Level { get; set; }

        [Required]
        public string Message { get; set; }

        [MaxLength(250)]
        public string UserName { get; set; }

        public string ServerName { get; set; }
        public string Port { get; set; }
        public string URL { get; set; }
        public bool? Https { get; set; }
        
        [MaxLength(100)]
        public string ServerAddress { get; set; }
        [MaxLength(100)]
        public string RemoteAddress { get; set; }
        [MaxLength(250)]
        public string Logger { get; set; }

        public string Callsite { get; set; }

        public string Exception { get; set; }
    }
}
