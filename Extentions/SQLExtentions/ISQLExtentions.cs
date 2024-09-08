using SelfProjectApi.Models.Filter;
using System.Data.SqlClient;

namespace SelfProjectApi.Extentions.SQLExtentions
{
    public interface ISQLExtentions
    {
        /// <summary>
        /// GenerateSQLQuery: Generates an SQL query dynamically based on filter model
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>An SQL qurey based on the params provided</returns>
        public string GenerateOrderSQLQueryDynamically(Filter filter);


        /// <summary>
        /// GenerateSQLCommandDynamically: Generates SQL paramters for SQL query dynamically based on filter model
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="filter"></param>
        /// <returns>A SQL Command based on the paramters provided</returns>
        public SqlCommand GenerateSQLCommandDynamically(SqlCommand cmd, Filter filter);

        ///<summary>
        /// SafeConverters: SQL DB nulls have different behaveral properties to C#, these methods ensure we convert a reader value safley even when its null
        /// </summary>

        /// <summary>
        /// SafeConvertToString: Takes values from an SQL Data Reader and converts them safely into a string value or a C# null
        /// </summary>
        /// <param name="reader">Current Data Reader </param>
        /// <param name="tableValue">Value of the column being mapped </param>
        /// <returns>Value from reader OR NUll</returns>
        public string SafeConvertToString(SqlDataReader reader, string tableValue);

        /// <summary>
        /// SafeConvertToString: Takes values from an SQL Data Reader and converts them safely into a DateTime value or a C# null
        /// </summary>
        /// <param name="reader">Current Data Reader </param>
        /// <param name="tableValue">Value of the column being mapped </param>
        /// <returns>Value from reader OR NUll</returns>
        public DateTime? SafeConvertToDateTime(SqlDataReader reader, string tableValue);

        /// <summary>
        ///  Takes values from an SQL Data Reader and converts them safely into a int value or a C# null
        /// </summary>
        /// <param name="reader">Current Data Reader </param>
        /// <param name="tableValue">Value of the column being mapped </param>
        /// <returns>Value from reader OR NUll</returns>
        public int? SafeConvertToInt(SqlDataReader reader, string tableValue);

        /// <summary>
        /// Takes values from an SQL Data Reader and converts them safely into a decimal value or a C# null
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableValue"></param>
        /// <returns></returns>
        public double? SafeConvertToDouble(SqlDataReader reader, string tableValue);
        /// <summary>
        /// Takes values from an SQL Data Reader and converts them safely into a GUID value or a C# null
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableValue"></param>
        /// <returns>Value from reader OR NUll</returns>
        public Guid? SafeConvertToGuid(SqlDataReader reader, string tableValue);

    }
}
