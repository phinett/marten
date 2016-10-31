﻿using System;
using Marten.Linq;
using Marten.Schema;

namespace Marten.Services.Includes
{
    public class IncludeJoin<T> : IIncludeJoin
    {
        
        private readonly IQueryableDocument _mapping;
        private readonly IField _field;
        private readonly Action<T> _callback;

        public IncludeJoin(IQueryableDocument mapping, IField field, string tableAlias, Action<T> callback, JoinType joinType)
        {
            _mapping = mapping;
            _field = field;
            _callback = callback;

            TableAlias = tableAlias;
            JoinType = joinType;

        }

        public string JoinTextFor(string rootTableAlias, IQueryableDocument document = null)
        {
            var locator = document == null
                ? _field.LocatorFor(rootTableAlias)
                : document.FieldFor(_field.Members).LocatorFor(rootTableAlias);
                
            var joinOperator = JoinType == JoinType.Inner ? "INNER JOIN" : "LEFT OUTER JOIN";



            return $"{joinOperator} {_mapping.Table.QualifiedName} as {TableAlias} ON {locator} = {TableAlias}.id";
        }

        public string JoinText => JoinTextFor("d", null);

        public string TableAlias { get; }
        public JoinType JoinType { get; set; }

        public ISelector<TSearched> WrapSelector<TSearched>(IDocumentSchema schema, ISelector<TSearched> inner)
        {
            return new IncludeSelector<TSearched, T>(TableAlias, _mapping, _callback, inner, schema.ResolverFor<T>());
        }
    }
}