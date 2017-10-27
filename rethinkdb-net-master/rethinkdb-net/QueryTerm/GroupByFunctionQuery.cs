﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using RethinkDb.DatumConverters;
using RethinkDb.Spec;

namespace RethinkDb.QueryTerm
{
    public abstract class GroupByFunctionQueryBase<TRecord, TKey> : IGroupingQuery<TKey, TRecord[]>
    {
        private ISequenceQuery<TRecord> sequenceQuery;

        protected GroupByFunctionQueryBase(ISequenceQuery<TRecord> sequenceQuery)
        {
            this.sequenceQuery = sequenceQuery;
        }

        protected abstract void GenerateFunctionTerms(Term term, IQueryConverter queryConverter);

        public Term GenerateTerm(IQueryConverter queryConverter)
        {
            var term = new Term()
            {
                type = Term.TermType.GROUP,
            };
            term.args.Add(sequenceQuery.GenerateTerm(queryConverter));
            GenerateFunctionTerms(term, queryConverter);
            return term;
        }
    }

    public class GroupByFunctionQuery<TRecord, TKey> : GroupByFunctionQueryBase<TRecord, TKey>
    {
        private Expression<Func<TRecord, TKey>> keyExpression;

        public GroupByFunctionQuery(ISequenceQuery<TRecord> sequenceQuery, Expression<Func<TRecord, TKey>> keyExpression)
            : base(sequenceQuery)
        {
            this.keyExpression = keyExpression;
        }

        protected override void GenerateFunctionTerms(Term term, IQueryConverter queryConverter)
        {
            if (keyExpression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey>(queryConverter, keyExpression));
        }
    }

    public class GroupByFunctionQuery<TRecord, TKey1, TKey2, TGroupingKey> : GroupByFunctionQueryBase<TRecord, TGroupingKey>
    {
        private Expression<Func<TRecord, TKey1>> key1Expression;
        private Expression<Func<TRecord, TKey2>> key2Expression;

        public GroupByFunctionQuery(ISequenceQuery<TRecord> sequenceQuery, Expression<Func<TRecord, TKey1>> key1Expression, Expression<Func<TRecord, TKey2>> key2Expression)
            : base(sequenceQuery)
        {
            this.key1Expression = key1Expression;
            this.key2Expression = key2Expression;
        }

        protected override void GenerateFunctionTerms(Term term, IQueryConverter queryConverter)
        {
            if (key1Expression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey1>(queryConverter, key1Expression));

            if (key2Expression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey2>(queryConverter, key2Expression));
        }
    }

    public class GroupByFunctionQuery<TRecord, TKey1, TKey2, TKey3, TGroupingKey> : GroupByFunctionQueryBase<TRecord, TGroupingKey>
    {
        private Expression<Func<TRecord, TKey1>> key1Expression;
        private Expression<Func<TRecord, TKey2>> key2Expression;
        private Expression<Func<TRecord, TKey3>> key3Expression;

        public GroupByFunctionQuery(ISequenceQuery<TRecord> sequenceQuery, Expression<Func<TRecord, TKey1>> key1Expression, Expression<Func<TRecord, TKey2>> key2Expression, Expression<Func<TRecord, TKey3>> key3Expression)
            : base(sequenceQuery)
        {
            this.key1Expression = key1Expression;
            this.key2Expression = key2Expression;
            this.key3Expression = key3Expression;
        }

        protected override void GenerateFunctionTerms(Term term, IQueryConverter queryConverter)
        {
            if (key1Expression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey1>(queryConverter, key1Expression));

            if (key2Expression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey2>(queryConverter, key2Expression));

            if (key3Expression.NodeType != ExpressionType.Lambda)
                throw new NotSupportedException("Unsupported expression type");
            term.args.Add(ExpressionUtils.CreateFunctionTerm<TRecord, TKey3>(queryConverter, key3Expression));
        }
    }
}
