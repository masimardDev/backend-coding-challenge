using System;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.MatchingRules
{
    internal class TypeAttributeMatchingRule : IMatchingRule
    {
        public Type AttributeType { get; private set; }

        internal TypeAttributeMatchingRule(Type attributeType)
        {
            if (!typeof (Attribute).IsAssignableFrom(attributeType))
            {
                throw new ArgumentException("Specified attributeType must be an Attribute", "attributeType");
            }

            AttributeType = attributeType;
        }

        public bool Matches(MethodBase member)
        {
            return member.DeclaringType.GetCustomAttribute(AttributeType, true) != null;
        }
    }
}