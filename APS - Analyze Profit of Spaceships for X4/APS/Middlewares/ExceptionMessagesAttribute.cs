using System;
using System.ComponentModel.DataAnnotations;

namespace APS.Middlewares
{
    public class ExceptionMessagesAttribute : Attribute
    {
        [Required]
        public string ResourceKey { get; set; }
    }
}
