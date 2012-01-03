using System;
using System.Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GetAttributeValue
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly DisplayAttribute _displayAttribute;

        public LocalizedDisplayNameAttribute(string resourceName)
            : base(resourceName)
        {
            
            _displayAttribute = new DisplayAttribute()
            {
                ResourceType = typeof(GetAttributeValue.Global),
                Name = resourceName
            };
        }

        public override string DisplayName
        {
            get
            {
                try
                {
                    return _displayAttribute.GetName();
                }
                catch (InvalidOperationException)
                {
                    return "Resource [" + _displayAttribute.Name + "] is not defined.";
                }
            }
        }
    }
}
