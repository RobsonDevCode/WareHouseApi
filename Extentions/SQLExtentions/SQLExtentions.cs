using SelfProjectApi.Models.Filter;
using System.Data.SqlClient;
using System.Text;

namespace SelfProjectApi.Extentions.SQLExtentions
{
    public class SQLExtentions : ISQLExtentions
    {
        /// <inheritdoc />
        public string GenerateOrderSQLQueryDynamically(Filter filter)
        {

            if (filter == null) throw new ArgumentNullException("Filter cannot be null when generating a query");

            //add Limit for Cache 
            StringBuilder sqlQuery = new StringBuilder(@"SELECT TOP(100) 
                                                        o.[Id], o.[Number] ,o.[CountryCode], o.[SupplierId] o.[DtCreated],
                                                        c.[SupplierId], c.[SupplierFullName], c.[CardNumber]

                                                        FROM OrderTable o 

                                                        INNER JOIN SupplierTable c
                                                        ON c.SupplierId = o.SupplierId
                                                        WHERE");

            //Dynamically update the query based on the filter provided
            //apply Country code if filter is applied
            if(filter.CountryCode != null)
            {
                sqlQuery.AppendLine("o.[CountryCode] = @countryCode");
            }

            //add search param if a user has searched an order for a Supplier id
            if(!string.IsNullOrWhiteSpace(filter.SupplierNameSearch))
            {
                sqlQuery.AppendLine(@"o.[SupplierId] = @searchTerm");
            }

            //Remove where clause if we're not specifing a search value
            else
            {
                sqlQuery = sqlQuery.Replace("WHERE", "");
            }

            //Apply Descending filter if applied
            if(filter.IsDescending)
            {
                sqlQuery.AppendLine(@"ORDER BY o.[Number] DESC");
            }
            //else order by ascending 
            else
            {
                sqlQuery.AppendLine(@"ORDER BY o.[Number]");
            }

            return sqlQuery.ToString();
        }
      

        /// <inheritdoc />
        public SqlCommand GenerateSQLCommandDynamically(SqlCommand cmd, Filter filter)
        {
            if (filter == null) throw new ArgumentNullException("Filter Cannot be null when generating command");

            if (!string.IsNullOrWhiteSpace(filter.CountryCode)) cmd.Parameters.AddWithValue("@countryCode", filter.CountryCode);
            //apply search if its not null
            if (!string.IsNullOrWhiteSpace(filter.SupplierNameSearch)) cmd.Parameters.AddWithValue("@searchTerm", filter.SupplierNameSearch);

            cmd.CommandTimeout = 30;

            return cmd;
        }
      
      
        /// <inheritdoc />
        public string SafeConvertToString(SqlDataReader reader, string tableValue)
        {
            string? output;
            if (reader[tableValue] != DBNull.Value) return output = Convert.ToString(reader[tableValue]);

            else return string.Empty;
        }

        /// <inheritdoc />
        public DateTime? SafeConvertToDateTime(SqlDataReader reader, string tableValue)
        {
            DateTime output;
            if (reader[tableValue] != DBNull.Value) return output = Convert.ToDateTime(reader[tableValue]);
            else return null;
        }

        /// <inheritdoc />
        public Guid? SafeConvertToGuid(SqlDataReader reader, string tableValue)
        {
            Guid? output;
            string stringGuid = SafeConvertToString(reader, tableValue);

            if (reader[tableValue] != DBNull.Value) return output = Guid.Parse(stringGuid);
            else return null;
        }

        /// <inheritdoc />
        public int? SafeConvertToInt(SqlDataReader reader, string tableValue)
        {
            int? output;
            if (reader[tableValue] != DBNull.Value) return output = Convert.ToInt32(reader[tableValue]);
            else return null;
        }

        /// <inheritdoc />

        public double? SafeConvertToDouble(SqlDataReader reader, string tableValue)
        {
            double? output;
            if (reader[tableValue] != DBNull.Value) return output = Convert.ToDouble(reader[tableValue]);
            else return null;
        }

        
    }
}
