using System;

namespace NET.Domain.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public string RuleName { get; }

        public BusinessRuleException(string ruleName) : base($"Business rule violation: {ruleName}")
        {
            RuleName = ruleName;
        }

        public BusinessRuleException(string ruleName, string message) : base(message)
        {
            RuleName = ruleName;
        }

        public BusinessRuleException(string ruleName, string message, Exception innerException) : base(message, innerException)
        {
            RuleName = ruleName;
        }
    }
}