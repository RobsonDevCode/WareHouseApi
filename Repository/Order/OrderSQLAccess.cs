using NLog;
using SelfProjectApi.Configuration;
using SelfProjectApi.Extentions.SQLExtentions;
using SelfProjectApi.Models.ApiResponses;
using SelfProjectApi.Models.Filter;
using SelfProjectApi.Models.Orders;
using SelfProjectApi.Models.Sales;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace SelfProjectApi.Repository
{
    public class OrderSQLAccess : IOrderSQLAccess
    {
        private readonly ISQLExtentions _sqlExtentions;
        private readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public OrderSQLAccess(ISQLExtentions sqlExtentions)
        {
            _sqlExtentions = sqlExtentions;
        }
        ///<inheritdoc/>
        public async Task<ApiRespone> AddNewOrdersAsyncEndpoint(List<BulkOrder> ordersToAdd)
        {
            try
            {
                if (ordersToAdd == null) return new ApiRespone
                {
                    StatusCode = 400,
                    Message = "Error creating bulk order, list of orders were NULL"
                };

                //All orders tied to the bulk order will have the same id 
                var orderId = ordersToAdd.Select(o => o.OrderId).FirstOrDefault();

                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString.OrderDataBase))
                {
                    using SqlBulkCopy sqlBulkCopy = new(connection);

                    sqlBulkCopy.DestinationTableName = "OrderTable";
                    sqlBulkCopy.BulkCopyTimeout = 60;

                    //create datatable so we can map to our sql table 
                    DataTable dataTable = CreateDataTable(ordersToAdd);

                    connection.Open();

                    await sqlBulkCopy.WriteToServerAsync(dataTable);

                }

                _logger.Info($"Bulk order {orderId} has succesfully been made at {DateTime.Now}");

                return new ApiRespone
                {
                    StatusCode = 201,
                    Message = "Bulk Order has succesfully been made"
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        ///<inheritdoc/>
        public async Task<ApiRespone> CancelOrderAsyncEndpoint(string orderId, string userId)
        {
            try
            {
                int rowsAffected = 0;

                //Cancelling process will have an effect on mutiple tables so we use a Stored procedure.
                using (SqlConnection connection = new(Settings.ConnectionString.OrderDataBase))
                {
                    connection.Open();

                    using SqlCommand cmd = new SqlCommand("usp_del_order_request",connection);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                  rowsAffected =  await cmd.ExecuteNonQueryAsync();
                }

                if(rowsAffected == 0)
                {
                  var result = new ApiRespone
                    {
                        StatusCode = 400,
                        Message = $"Satus 400: Error occured while Deleting Order {orderId}. Either order id or group id is Null or Incorrect"
                    };

                    return result;
                }

                _logger.Info($"Cancelled Order {orderId}, by {userId}");

                //order has been cancelled
                return new ApiRespone
                {
                    StatusCode = 200,
                    Message = $"Order: {orderId} has been cancelled been by {userId}"
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }


        ///<inheritdoc/>
        public async Task<List<Order>> GetOrdersAsyncEndPoint(Filter filter)
        {
            if(filter == null) throw new ArgumentNullException("Filter cannot be null");

            // set SQL query dynamically based on filter 
            string sql = _sqlExtentions.GenerateOrderSQLQueryDynamically(filter);

            try
            {
                List<Order> orders = new();

                using (SqlConnection connection = new(Settings.ConnectionString.OrderDataBase))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sql, connection);

                    cmd.CommandTimeout = 30;

                    //generate Command to match the filter and dynamic SQL query
                    _sqlExtentions.GenerateSQLCommandDynamically(cmd, filter);

                    //get orders from reader
                    orders = await orderSQLDataReader(cmd);
                }

                return orders;
            }

            catch (SqlException ex) 
            {
                _logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// CreateDataTable: creates a datatable Mapped to orders to bulk upload to sql
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>

        private DataTable CreateDataTable(List<BulkOrder> orders)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Number", typeof(int));
            dt.Columns.Add("CountryCode", typeof(string));
            dt.Columns.Add("DateTime", typeof(DateTime));

            for (int i = 0; i < orders.Count(); i++)
            {
                dt.Rows.Add(orders[i].OrderId,
                            orders[i].OrderNumber,
                            orders[i].CountryCode,
                            orders[i].DateCreated);

            }

            return dt;
        }

        private async Task<List<Order>> orderSQLDataReader(SqlCommand command)
        {
            var reader = await command.ExecuteReaderAsync();
            List<Order> orders = new List<Order>();

            while(await reader.ReadAsync())
            {
                orders.Add(new Order()
                {
                    OrderId = _sqlExtentions.SafeConvertToGuid(reader, "Id") ?? throw new SqlNullValueException("Data Quality issue: OrderId is returning null from the from DB, please check table tied to the connection string"),
                    OrderNumber = _sqlExtentions.SafeConvertToInt(reader, "Number") ?? throw new SqlNullValueException("Data Quality issue: OrderNumber is returning null from the from DB, please check table tied to the connection string"),
                    CountryCode = _sqlExtentions.SafeConvertToString(reader, "CountryCode"),
                    DateCreated = _sqlExtentions.SafeConvertToDateTime(reader, "DtCreated"),

                    SupplierDetails = new()
                    {
                        Id = (_sqlExtentions.SafeConvertToString(reader, "SupplierId") == string.Empty) ? throw new SqlNullValueException("Data Quality issue: SupplierId is returning null from the from DB, please check table tied to the connection string")
                        : _sqlExtentions.SafeConvertToString(reader, "SupplierId"),

                        CardNumber = _sqlExtentions.SafeConvertToString(reader, "CardNumber"),
                        FullName = _sqlExtentions.SafeConvertToString(reader, "FullName")
                    }

                   
                });
            }

            return orders;
        }
    }
}
