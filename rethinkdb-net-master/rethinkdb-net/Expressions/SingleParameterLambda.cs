﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RethinkDb.DatumConverters;
using RethinkDb.Spec;

namespace RethinkDb.Expressions
{
    class SingleParameterLambda<TParameter1, TReturn> : BaseExpression, IExpressionConverterOneParameter<TParameter1, TReturn>
    {
        #region Public interface

        private readonly IDatumConverterFactory datumConverterFactory;

        public SingleParameterLambda(IDatumConverterFactory datumConverterFactory, DefaultExpressionConverterFactory expressionConverterFactory)
            : base(expressionConverterFactory)
        {
            this.datumConverterFactory = datumConverterFactory;
        }

        public Term CreateFunctionTerm(Expression<Func<TParameter1, TReturn>> expression)
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
                    r_num = 2
                }
            });
            funcTerm.args.Add(parametersTerm);

            funcTerm.args.Add(MapExpressionToTerm(expression.Body));

            return funcTerm;
        }
            
        protected override Term RecursiveMap(Expression expression)
        {
            return MapExpressionToTerm(expression);
        }

        private Term MapExpressionToTerm(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                {
                    return new Term()
                    {
                        type = Term.TermType.VAR,
                        args = {
                            new Term() {
                                type = Term.TermType.DATUM,
                                datum = new Datum() {
                                    type = Datum.DatumType.R_NUM,
                                    r_num = 2
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

        #endregion
    }
}
