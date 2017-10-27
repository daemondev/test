﻿using RethinkDb.Spec;
using System;
using System.Linq.Expressions;

namespace RethinkDb.QueryTerm
{
    public class InnerJoinQuery<TLeft, TRight, TResult> : ISequenceQuery<TResult>
    {
        private ISequenceQuery<TLeft> leftQuery;
        private ISequenceQuery<TRight> rightQuery;
        private Expression<Func<TLeft, TRight, bool>> joinPredicate;

        public InnerJoinQuery(ISequenceQuery<TLeft> leftQuery, ISequenceQuery<TRight> rightQuery, Expression<Func<TLeft, TRight, bool>> joinPredicate)
        {
            this.leftQuery = leftQuery;
            this.rightQuery = rightQuery;
            this.joinPredicate = joinPredicate;
        }

        public Term GenerateTerm(IQueryConverter queryConverter)
        {
            var filterTerm = new Term()
            {
                type = Term.TermType.INNER_JOIN,
            };
            filterTerm.args.Add(leftQuery.GenerateTerm(queryConverter));
            filterTerm.args.Add(rightQuery.GenerateTerm(queryConverter));
            filterTerm.args.Add(ExpressionUtils.CreateFunctionTerm<TLeft, TRight, bool>(queryConverter, joinPredicate));
            return filterTerm;
        }
    }
}
