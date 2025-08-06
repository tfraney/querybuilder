using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlKata
{
    public partial class Query
    {

        public Query Select(params string[] columns)
        {
            return Select(columns.AsEnumerable());
        }

        public Query Select(IEnumerable<string> columns)
        {
            Method = "select";

            columns = columns
                .Select(x => Helper.ExpandExpression(x))
                .SelectMany(x => x)
                .ToArray();


            foreach (var column in columns)
            {
                AddComponent("select", new Column
                {
                    Name = column
                });
            }

            return this;
        }
        /// <summary>
        /// Sets ROW_NUMBER() with Partition Group,
        /// and Over Clause with order optional and direction as ascending (ASC) as default 
        /// </summary>
        /// <param name="alias">returns name of your row count column</param>
        /// <param name="partitionColumns"> array of columns to group by in partition </param>
        /// <param name="orderBy"> ex: (PARTITION BY Code ORDER BY Id DESC, Name ASC) = [["Id","DESC"],["Name"]]. Optional </param>
        /// <param name="bindings"> allows direct values as parameters ? to inject into select columns </param>
        public Query SelectRowNumber(string alias, string[] partitionColumns,  string[][] orderBy = null, params object[] bindings)
        {
            alias = alias.ToUpper().Trim();
            if (partitionColumns?.Any() ?? false) {              
                var orderby = BuildOrderBy(orderBy);
                var orderByStr = !string.IsNullOrWhiteSpace(orderby) ? $" {VERB.OrderBy} {orderby}" : string.Empty;
                var statement = $"{VERB.RowNumberPartitioned} {string.Join(',',partitionColumns)}{orderByStr}) {VERB.As} [{alias}]";
                return SelectRaw(statement, bindings);
            }
            return SelectRowNumber(alias, orderBy, bindings);

        }

        /// <summary>
        /// Sets ROW_NUMBER() with Partition Group,
        /// and Over Clause with Random generator (NEWID()) 
        /// </summary>
        /// <param name="alias">returns name of your row count column</param>
        /// <param name="partitionColumns"> array of columns to group by in partition </param>        
        /// <param name="bindings"> allows direct values as parameters ? to inject into select columns </param>
        public Query SelectRowNumberAtRandom(string alias, string[] partitionColumns, params object[] bindings)
        {
            string[][] randomId = [["NEWID()"]];  
            return SelectRowNumber(alias, partitionColumns, randomId, bindings);
        }

        /// <summary>
        /// Sets ROW_NUMBER() with Over Clause with Random generator (NEWID()) 
        /// </summary>
        /// <param name="alias">returns name of your row count column</param>               
        /// <param name="bindings"> allows direct values as parameters ? to inject into select columns </param>
        public Query SelectRowNumberAtRandom(string alias, params object[] bindings)
        {
            string[][] randomId = [["NEWID()"]];
            return SelectRowNumber(alias, randomId, bindings);
        }

        /// <summary>
        /// Sets ROW_NUMBER() with optional order by ASC/DESC,
        /// and Over Clause with order optional and direction as ascending (ASC) as default 
        /// </summary>
        /// <param name="alias">returns name of your row count column</param>
        /// <param name="orderBy"> ex: (ORDER BY Id DESC, Name ASC) = [["Id","DESC"],["Name"]]. Optional </param>
        /// <param name="bindings"> allows direct values as parameters ? to inject into select columns </param>
        public Query SelectRowNumber(string alias, string[][] orderBy = null, params object[] bindings)
        {
            alias = alias.ToUpper().Trim();
            var orderby = BuildOrderBy(orderBy);
            var statement = !string.IsNullOrWhiteSpace(orderby) ?
                $"{VERB.RowNumberOrderBy} {BuildOrderBy(orderBy)}) {VERB.As} [{alias}]" :
                $"{VERB.RowNumber} {VERB.As} [{alias}]";
            return SelectRaw(statement, bindings);
        }


        /// <summary>
        /// Add a new "raw" select expression to the query.
        /// </summary>
        /// <returns></returns>
        public Query SelectRaw(string sql, params object[] bindings)
        {
            Method = "select";

            AddComponent("select", new RawColumn
            {
                  
                Expression = sql,
                Bindings = bindings,
            });

            return this;
        }

        public Query Select(Query query, string alias)
        {
            Method = "select";

            query = query.Clone();

            AddComponent("select", new QueryColumn
            {
                Query = query.As(alias),
            });

            return this;
        }

        public Query Select(Func<Query, Query> callback, string alias)
        {
            return Select(callback.Invoke(NewChild()), alias);
        }

        public Query SelectAggregate(string aggregate, string column, Query filter = null)
        {
            Method = "select";

            AddComponent("select", new AggregatedColumn
            {
                Column = new Column { Name = column },
                Aggregate = aggregate,
                Filter = filter,
            });

            return this;
        }

        public Query SelectAggregate(string aggregate, string column, Func<Query, Query> filter)
        {
            if (filter == null)
            {
                return SelectAggregate(aggregate, column);
            }

            return SelectAggregate(aggregate, column, filter.Invoke(NewChild()));
        }

        public Query SelectSum(string column, Func<Query, Query> filter = null)
        {
            return SelectAggregate("sum", column, filter);
        }

        public Query SelectCount(string column, Func<Query, Query> filter = null)
        {
            return SelectAggregate("count", column, filter);
        }

        public Query SelectAvg(string column, Func<Query, Query> filter = null)
        {
            return SelectAggregate("avg", column, filter);
        }

        public Query SelectMin(string column, Func<Query, Query> filter = null)
        {
            return SelectAggregate("min", column, filter);
        }

        private string BuildOrderBy(string[][] orderBy = null)
        {

            var haveOrder = orderBy?.Any() ?? false;
            if (haveOrder)
            {
                var orderByStr = string.Join(',',
                    orderBy.Select(o => $"{o[0]} {(o.Length > 1 && o[1].ToUpper().Trim() == VERB.Desc ? VERB.Desc : string.Empty)}"));
                return orderByStr;
            }
            else return string.Empty;
        }


        public Query SelectMax(string column, Func<Query, Query> filter = null)
        {
            return SelectAggregate("max", column, filter);
        }
    }
}
