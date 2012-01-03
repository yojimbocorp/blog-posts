using System;
using System.Reflection;

namespace GetAttributeValue
{
    public class AttributeUtil
    {
        public static object GetAttributeValue(Type objectType, string propertyName, Type attributeType, string attributePropertyName)
        {
            var propertyInfo = objectType.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                if (Attribute.IsDefined(propertyInfo, attributeType))
                {
                    var attributeInstance = Attribute.GetCustomAttribute(propertyInfo, attributeType);
                    if (attributeInstance != null)
                    {
                        foreach (PropertyInfo info in attributeType.GetProperties())
                        {
                            if (info.CanRead &&
                                String.Compare(info.Name, attributePropertyName, StringComparison.InvariantCultureIgnoreCase) ==
                                0)
                            {
                                return info.GetValue(attributeInstance, null);
                            }
                        }
                    }
                }
            }

            return null;
        }


    }
}
