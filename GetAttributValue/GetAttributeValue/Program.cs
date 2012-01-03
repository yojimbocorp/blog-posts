using System;

namespace GetAttributeValue
{
    class Program
    {
        static void Main(string[] args)
        {
            var value = AttributeUtil.GetAttributeValue(typeof(UserAccount), "UserAccountId", typeof(LocalizedDisplayNameAttribute),"DisplayName");

            if (value != null)
            {
                Console.WriteLine("The localized displayname is: {0}", Convert.ToString(value));
            }
            else
            {
                Console.WriteLine("Could not find the value of the attribute requested.");
            }
            Console.ReadLine();
        }
    }
}
