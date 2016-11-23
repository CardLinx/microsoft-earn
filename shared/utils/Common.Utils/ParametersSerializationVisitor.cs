//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using NCalc.Domain;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extracts the parameters from an expression
    /// </summary>
    public class ParametersSerializationVisitor : LogicalExpressionVisitor
    {
        public HashSet<string> Parameters { get; protected set; }

        public ParametersSerializationVisitor()
        {
            this.Parameters = new HashSet<string>();
        }

        public override void Visit(LogicalExpression expression)
        {
            throw new NotImplementedException("LogicalExpression is not supported");
        }

        public override void Visit(TernaryExpression expression)
        {
            throw new NotImplementedException("TernaryExpression is not supported");
        }

        public override void Visit(BinaryExpression expression)
        {
            EncapsulateNoValue(expression.LeftExpression);
            EncapsulateNoValue(expression.RightExpression);
        }

        public override void Visit(UnaryExpression expression)
        {
            EncapsulateNoValue(expression.Expression);
        }

        public override void Visit(ValueExpression expression)
        {
            return;
        }

        public override void Visit(Function function)
        {
            switch (function.Identifier.Name)
            {
                case "Contains":
                case "Event":
                    function.Expressions[0].Accept(this);
                    break;
                default:
                    throw new ArgumentException(string.Format("Function '{0} 'is not supported", function.Identifier.Name));
            }
        }

        public override void Visit(Identifier parameter)
        {
            this.Parameters.Add(parameter.Name);
        }

        protected void EncapsulateNoValue(LogicalExpression expression)
        {
            expression.Accept(this);
        }
    }
}