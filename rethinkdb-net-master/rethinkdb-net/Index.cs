﻿using System;
using System.Linq.Expressions;

namespace RethinkDb
{
    public class Index<TRecord, TIndex> : IIndex<TRecord, TIndex>
    {
        private readonly ITableQuery<TRecord> table;
        private readonly string name;
        private readonly Expression<Func<TRecord, TIndex>> indexAccessor;

        public Index(ITableQuery<TRecord> table, string name, Expression<Func<TRecord, TIndex>> indexAccessor)
        {
            this.table = table;
            this.name = name;
            this.indexAccessor = indexAccessor;
        }

        public ITableQuery<TRecord> Table
        {
            get { return table; }
        }

        public string Name
        {
            get { return name; }
        }

        public Expression<Func<TRecord, TIndex>> IndexAccessor
        {
            get { return indexAccessor; }
        }
    }
}
