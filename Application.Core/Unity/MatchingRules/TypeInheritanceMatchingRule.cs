using System;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Application.Core.Unity.MatchingRules
{
    internal class TypeInheritanceMatchingRule : IMatchingRule
    {
        public Type TypeToMatch { get; private set; }

        internal TypeInheritanceMatchingRule(Type type)
        {
            TypeToMatch = type;
        }

        public bool Matches(MethodBase member)
        {
            return TypeToMatch.IsAssignableFrom(member.DeclaringType);
        }
    }
}