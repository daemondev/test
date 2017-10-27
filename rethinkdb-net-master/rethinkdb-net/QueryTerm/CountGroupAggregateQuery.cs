﻿using RethinkDb.Spec;
using System;
using System.Linq.Expressions;

namespace RethinkDb.QueryTerm
{
    public class CountGroupAggregateQuery<TKey, TRecord> : IGroupingQuery<TKey, int>
    {
        private readonly IGroupingQuery<TKey, TRecord[]> groupingQuery;
        private readonly Expression<Func<TRecord, bool>> predicate;

        public CountGroupAggregateQuery(IGroupingQuery<TKey, TRecord[]> groupingQuery, Expression<Func<TRecord, bool>> predicate)
        {
            this.groupingQuery = groupingQuery;
            this.predicate = predicate;
        }

        public Term GenerateTerm(IQueryConverter queryConverter)
        {
            var term = new Term()
            {
                type = Term.TermType.COUNT,
            };
            term.args.Add(groupingQuery.GenerateTerm(queryConverter));
            if (predicate != null)
            {
                if (predicate.NodeType != ExpressionType.Lambda)
                    throw new NotSupportedException("Unsupported expression type");
                term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, bool>(queryConverter, predicate));
            }
            return term;
        }
    }
}
