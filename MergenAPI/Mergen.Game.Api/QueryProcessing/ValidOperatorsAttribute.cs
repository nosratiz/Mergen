using System;
using Mergen.Core.QueryProcessing;

namespace Mergen.Game.Api.QueryProcessing
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidOperatorsAttribute : Attribute
    {
        public string IgnoreValidityPermission { get; }
        public Op[] Operators { get; }

        public ValidOperatorsAttribute(string ignoreValidityPermission, params Op[] operators)
        {
            if (operators == null || operators.Length == 0)
                throw new ArgumentException("no operators provided", nameof(operators));

            IgnoreValidityPermission = ignoreValidityPermission;
            Operators = operators;
        }

        public ValidOperatorsAttribute(params Op[] operators) : this(null, operators)
        {
        }
    }
}