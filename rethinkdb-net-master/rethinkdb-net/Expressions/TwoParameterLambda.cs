﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RethinkDb.DatumConverters;
using RethinkDb.Spec;

namespace RethinkDb.Expressions
{
    // Note: Not thread-safe, do not share an instance across threads.
    class TwoParameterLambda<TParameter1, TParameter2, TReturn> : BaseExpression, IExpressionConverterTwoParameter<TParameter1, TParameter2, TReturn>
    {
        #region Public interface

        private readonly IDatumConverterFactory datumConverterFactory;
        private string parameter1Name;
        private string parameter2Name;

        public TwoParameterLambda(IDatumConverterFactory datumConverterFactory, DefaultExpressionConverterFactory expressionConverterFactory)
            : base(expressionConverterFactory)
        {
            this.datumConverterFactory = datumConverterFactory;
        }

        public Term CreateFunctionTerm(Expression<Func<TParameter1, TParameter2, TReturn>> expression)
        {
            var funcTerm = new Term() {
                type = Term.TermType.FUNC
            };

            var parametersTerm = new Term() {
                type = Term.TermType.MAKE_ARRAY,
            };
            parametersTerm.args.Add(new Term() {
                type = Term.TermType.DATUM,
                datum = new Datum() {
                    type = Datum.DatumType.R_NUM,
                    r_num = 3
                }
            });
            parametersTerm.args.Add(new Term() {
                type = Term.TermType.DATUM,
                datum = new Datum() {
                    type = Datum.DatumType.R_NUM,
                    r_num = 4
                }
            });
            funcTerm.args.Add(parametersTerm);

            this.parameter1Name = expression.Parameters[0].Name;
            this.parameter2Name = expression.Parameters[1].Name;

            funcTerm.args.Add(MapExpressionToTerm(expression.Body));

            return funcTerm;
        }

        #endregion
        #region Abstract implementation

        private Term MapExpressionToTerm(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                {
                    var parameterExpr = (ParameterExpression)expr;
                    int parameterIndex;
                    if (parameterExpr.Name == parameter1Name)
                        parameterIndex = 3;
                    else if (parameterExpr.Name == parameter2Name)
                        parameterIndex = 4;
                    else
                        throw new InvalidOperationException("Unmatched parameter name:" + parameterExpr.Name);

                    return new Term() {
                        type = Term.TermType.VAR,
                        args = {
                            new Term() {
                                type = Term.TermType.DATUM,
                                datum = new Datum() {
                                    type = Datum.DatumType.R_NUM,
                                    r_num = parameterIndex
                                },
                            }
                        }
                    };
                }
                
                case ExpressionType.Convert:
                {
                    // In some cases the CLR can insert a type-cast when a generic type constrant is present on a
                    // generic type that's a parameter.  We pretty much just ignore those casts.  It might be
                    // valid to use the cast to switch to a different datum converter?, but the use-case isn't
                    // really clear right now.  We do check that the type-cast makes sense for the parameter type,
                    // but it's just to feel safer; it seems like the compiler should've made sure about that.

                    var convertExpression = (UnaryExpression)expr;
                    if (convertExpression.Operand.NodeType != ExpressionType.Parameter)
                        // If this isn't a cast on a Parameter, just drop it through to do continue processing.
                        return SimpleMap(datumConverterFactory, expr);

                    // Otherwise; type-check it, and then just strip the Convert node out and recursivemap the inside.
                    var parameterExpr = (ParameterExpression)convertExpression.Operand;
                    if (!convertExpression.Type.IsAssignableFrom(parameterExpr.Type))
                        throw new NotSupportedException(String.Format(
                            "Cast on parameter expression not currently supported (from type {0} to type {1})",
                            parameterExpr.Type, convertExpression.Type));

                    return RecursiveMap(parameterExpr);
                }
                
                default:
                    return SimpleMap(datumConverterFactory, expr);
            }
        }

        protected override Term RecursiveMap(Expression expression)
        {
            return MapExpressionToTerm(expression);
        }

        #endregion
    }
}
