using System;
using System.ComponentModel;
using System.Reflection.Emit;
using APS.Resources;

namespace APS.Extensions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        private string ResourceKey { get; set; }

        public CustomDisplayNameAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public override string DisplayName
        {
            get
            {
                string displayName = APS.Resources.Label.ResourceManager.GetString(ResourceKey); 

                return string.IsNullOrEmpty(displayName)
                    ? string.Format("[[{0}]]", ResourceKey)
                  : displayName;
            }
        }
    }
}     
      