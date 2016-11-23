//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
    using NCalc.Domain;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using ValueType = NCalc.Domain.ValueType;

    /// <summary>
    /// Serializes a boolean expression to a Scope syntax boolean expression
    /// </summary>
    public class ScopeSerializationVisitor : LogicalExpressionVisitor
    {
        private readonly NumberFormatInfo _numberFormatInfo;
        private Stack<BinaryExpressionType> ops = new Stack<BinaryExpressionType>();
        private string parameterPrefix;

        /// <summary>
        /// Creates a new instance of <see cref="ScopeSerializationVisitor"/>
        /// </summary>
        /// <param name="parameterPrefix">Use this argument if a prefix is needed for parameter names in the Scope script (e.g. MyTable.MyField instead of just MyField)</param>
        public ScopeSerializationVisitor(string parameterPrefix = null)
        {
            this.Result = new StringBuilder();
            _numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            this.parameterPrefix = parameterPrefix;
        }

        public StringBuilder Result { get; protected set; }

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
            this.ops.Push(expression.Type);
            EncapsulateNoValue(expression.LeftExpression);

            switch (expression.Type)
            {
                case BinaryExpressionType.And:
                    Result.Append("&& ");
                    break;

                case BinaryExpressionType.Or:
                    Result.Append("|| ");
                    break;

                case BinaryExpressionType.Div:
                    Result.Append("/ ");
                    break;

                case BinaryExpressionType.Equal:
                    Result.Append("== ");
                    break;

                case BinaryExpressionType.Greater:
                    Result.Append("> ");
                    break;

                case BinaryExpressionType.GreaterOrEqual:
                    Result.Append(">= ");
                    break;

                case BinaryExpressionType.Lesser:
                    Result.Append("< ");
                    break;

                case BinaryExpressionType.LesserOrEqual:
                    Result.Append("<= ");
                    break;

                case BinaryExpressionType.Minus:
                    Result.Append("- ");
                    break;

                case BinaryExpressionType.Modulo:
                    Result.Append("% ");
                    break;

                case BinaryExpressionType.NotEqual:
                    Result.Append("!= ");
                    break;

                case BinaryExpressionType.Plus:
                    Result.Append("+ ");
                    break;

                case BinaryExpressionType.Times:
                    Result.Append("* ");
                    break;

                case BinaryExpressionType.BitwiseAnd:
                    Result.Append("& ");
                    break;

                case BinaryExpressionType.BitwiseOr:
                    Result.Append("| ");
                    break;

                case BinaryExpressionType.BitwiseXOr:
                    Result.Append("~ ");
                    break;

                case BinaryExpressionType.LeftShift:
                    Result.Append("<< ");
                    break;

                case BinaryExpressionType.RightShift:
                    Result.Append(">> ");
                    break;
            }

            EncapsulateNoValue(expression.RightExpression);
            this.ops.Pop();
        }

        public override void Visit(UnaryExpression expression)
        {
            bool enclose = true;
            switch (expression.Type)
            {
                case UnaryExpressionType.Not:
                    Result.Append("!");
                    break;

                case UnaryExpressionType.Negate:
                    Result.Append("-");
                    if (expression.Expression is ValueExpression)
                    {
                        enclose = false;
                    }
                    break;

                case UnaryExpressionType.BitwiseNot:
                    Result.Append("~");
                    break;
            }

            if (enclose)
            {
                Result.Append("(");
            }

            EncapsulateNoValue(expression.Expression);

            if (enclose)
            {
                // trim spaces before adding a closing paren
                while (Result[Result.Length - 1] == ' ')
                {
                    Result.Remove(Result.Length - 1, 1);
                }
                Result.Append(") ");
            }
        }

        public override void Visit(ValueExpression expression)
        {
            switch (expression.Type)
            {
                case ValueType.Boolean:
                    Result.Append(expression.Value.ToString().ToLowerInvariant()).Append(" ");
                    break;

                case ValueType.Float:
                    Result.Append(decimal.Parse(expression.Value.ToString()).ToString(_numberFormatInfo)).Append(" ");
                    break;

                case ValueType.Integer:
                    Result.Append(expression.Value.ToString()).Append(" ");
                    break;

                case ValueType.String:
                    Result.Append("\"").Append(expression.Value.ToString()).Append("\"").Append(" ");
                    break;
            }
        }

        public override void Visit(Function function)
        {
            switch (function.Identifier.Name)
            {
                case "Contains":
                    Result.Append("string.Format(\"~{0}~\", ");
                    function.Expressions[0].Accept(this);
                    Result.Append(").IndexOf(");
                    ScopeSerializationVisitor functionArgumentVisitor = new ScopeSerializationVisitor();
                    function.Expressions[1].Accept(functionArgumentVisitor);
                    var argument = functionArgumentVisitor.Result.ToString().Trim();
                    functionArgumentVisitor = new ScopeSerializationVisitor();
                    function.Expressions[2].Accept(functionArgumentVisitor);
                    bool exact = false;
                    if (!bool.TryParse(functionArgumentVisitor.Result.ToString(), out exact))
                    {
                        exact = false;
                    }

                    if (exact)
                    {
                        argument = string.Format("\"~{0}~\"", argument.Trim(new char[] { '"' }));
                    }

                    Result.Append(argument).Append(", StringComparison.InvariantCultureIgnoreCase) >= 0 ");
                    break;
                case "Event":
                    // C# expression should look like (<EventName> > 0 && <cutOffDate> [>\<] <EventName>)
                    Result.Append("(");
                    function.Expressions[0].Accept(this);
                    Result.Append("> 0 && ");

                    var now = DateTime.UtcNow;
                    var eventArgumentVisitor = new ScopeSerializationVisitor();
                    function.Expressions[1].Accept(eventArgumentVisitor);
                    int days = 0;
                    int.TryParse(eventArgumentVisitor.Result.ToString(), out days);
                    string op = " < ";
                    if (days < 0)
                    {
                        op = " > ";
                    }

                    var cutOffDate = now.AddDays(-Math.Abs(days));
                    int cutOffDateInt = cutOffDate.Year * 10000 + cutOffDate.Month * 100 + cutOffDate.Day;
                    Result.Append(cutOffDateInt.ToString(CultureInfo.InvariantCulture));
                    Result.Append(op);
                    function.Expressions[0].Accept(this);
                    Result.Append(")");
                    break;
                default:
                    throw new ArgumentException(string.Format("Function '{0} 'is not supported", function.Identifier.Name));
            }
        }

        public override void Visit(Identifier parameter)
        {
            this.Result.Append(string.Format("{0}{1}", this.parameterPrefix, parameter.Name)).Append(" ");
        }

        protected void EncapsulateNoValue(LogicalExpression expression)
        {
            if (expression is BinaryExpression)
            {
                var type = (expression as BinaryExpression).Type;
                var isNewBlock = type != this.ops.Peek() && (type == BinaryExpressionType.And || type == BinaryExpressionType.Or);
                if (isNewBlock)
                {
                    Result.Append("(");
                }

                expression.Accept(this);

                if (isNewBlock)
                {
                    // trim spaces before adding a closing paren
                    while (Result[Result.Length - 1] == ' ')
                    {
                        Result.Remove(Result.Length - 1, 1);
                    }

                    Result.Append(") ");
                }
            }
            else
            {
                expression.Accept(this);
            }
        }
    }
}