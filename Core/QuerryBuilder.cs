using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class QueryBuilder
    {

        private StringBuilder queryBuilder;

        public QueryBuilder()
        {
            queryBuilder = new StringBuilder();
        }

        public QueryBuilder AddSelectWhere(string[] keys, string table, Dictionary<string, object> values)
        {
            string query = string.Concat("SELECT ", string.Join(",", keys), " FROM ", table);
            string valuesString = string.Empty;

            if (values.Count > 0)
            {
                byte index = 0;
                foreach (KeyValuePair<string, object> entry in values)
                {
                    if (index == 0)
                    {
                        valuesString = string.Concat(valuesString, entry.Key, "=@", entry.Key);
                        index++;
                    }
                    else
                    {
                        valuesString = string.Concat(valuesString, " AND ", entry.Key, "=@", entry.Key);
                    }
                }
                query = string.Concat(query, " WHERE ", valuesString);
                queryBuilder.Append(query).Append(";");
            }
            return this;
        }

        public QueryBuilder LastInsertId(string variableName)
        {
            queryBuilder.Append("SELECT LAST_INSERT_ID() as ")
                .Append(variableName)
                .Append(";");
            return this;
        }

        public QueryBuilder LastInsertId()
        {
            return LastInsertId("last_id");
        }

        public override string ToString()
        {
            return queryBuilder.ToString();
        }
    }
}
