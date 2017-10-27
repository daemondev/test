﻿using RethinkDb.Spec;
using System;
using System.Linq.Expressions;

namespace RethinkDb.QueryTerm
{
    public class AvgAggregateQuery<TRecord, TAvgType> : ISingleObjectQuery<TAvgType>
    {
        private readonly ISequenceQuery<TRecord> sequenceQuery;
        private readonly Expression<Func<TRecord, TAvgType>> field;

        public AvgAggregateQuery(ISequenceQuery<TRecord> sequenceQuery, Expression<Func<TRecord, TAvgType>> field)
        {
            this.sequenceQuery = sequenceQuery;
            this.field = field;
        }

        public Term GenerateTerm(IQueryConverter queryConverter)
        {
            var term = new Term()
            {
                type = Term.TermType.AVG,
            };
            term.args.Add(sequenceQuery.GenerateTerm(queryConverter));
            if (field != null)
            {
                if (field.NodeType != ExpressionType.Lambda)
                    throw new NotSupportedException("Unsupported expression type");
                term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TAvgType>(queryConverter, field));
            }
            return term;
        }
    }
}