using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models
{
    public class Configuration
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
