using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.Collections;
using APS.Resources;

namespace APS.Extensions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CustomRequiredAttribute : RequiredAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return String.Format(Error.ChampObligatoire, name);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // On force le nouveau message dans le data-val-required
            context.Attributes.Add("data-val-required", String.Format(Error.ChampObligatoire, context.ModelMetadata.DisplayName));
        }
    }
}