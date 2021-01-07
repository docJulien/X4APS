using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using APS.Extensions;
using APS.Resources;

namespace APS.Model.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [CustomRequired]
        [CustomDisplayName("Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "CourrielInvalide")]
        public string Email { get; set; }
    }
}
