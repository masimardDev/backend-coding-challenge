using System;

namespace Application.Core.Unity.Attributes
{
    [Flags]
    internal enum Disallow
    {
        None = 0,
        Null = 1,
        Empty = 2,
        NullOrEmpty = Null | Empty,
        Whitespaces = 4,
        NullOrWhitespaces = Null | Empty | Whitespaces
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
    internal class ValidateAttribute : Attribute
    {
        public Disallow Disallow { get; set; }
        public int MaxLength { get; set; }
        public long MaxiumumNumericValue { get; set; }
        public long MinimumNumericValue { get; set; }

        public ValidateAttribute()
            : this(Disallow.None)
        {
        }

        public ValidateAttribute(Disallow disallow)
            : this(disallow, 0)
        {
        }

        public ValidateAttribute(Disallow disallow, int maxLength)
            : this(disallow, maxLength, long.MinValue)
        {
        }

        public ValidateAttribute(Disallow disallow, int maxLength, long minimumNumericValue)
            : this(disallow, maxLength, minimumNumericValue, long.MaxValue)
        {
        }

        public ValidateAttribute(Disallow disallow, int maxLength, long minimumNumericValue, long maximumNumericValue)
        {
            Disallow = disallow;
            MaxLength = maxLength;
            MinimumNumericValue = minimumNumericValue;
            MaxiumumNumericValue = maximumNumericValue;
        }
    }
}