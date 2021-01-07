using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using APS.Extensions;
using APS.Resources;

namespace APS.Model.AccountViewModels
{
    public class LoginViewModel
    {
        [CustomRequired]
        [CustomDisplayName("Username")]
        public string Username { get; set; }

        [CustomRequired]
        [CustomDisplayName("MotPasse")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
