

using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;

string connectionString = "host=172.16.1.18;port=8123;database=mqm_test;user=default;password=123456789;compress=True;checkCompressedHash=False;compressor=lz4;";

// SQL 查询语句
string sqlQuery = "select name as TableName, `comment` as TableComment from `system`.tables where database = @schema_name;";
using (var connection = new ClickHouseConnection(connectionString))
{
    try
    {
        // 打开数据库连接
        connection.Open();

        // 创建命令对象
        using (var command = connection.CreateCommand())
        {
            // 设置查询语句
            command.CommandText = sqlQuery;

            // 添加参数并设置参数值
            command.Parameters.Add(new ClickHouseDbParameter { ParameterName = "schema_name", Value = "mqm_test" });

            // 执行查询并获取读取器
            using (var reader = command.ExecuteReader())
            {
                // 遍历结果集
                while (reader.Read())
                {
                    // 处理每一行数据
                    Console.WriteLine($"{reader["TableName"]}, {reader["TableComment"]}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        // 处理连接或查询时发生的异常
        Console.WriteLine($"Error: {ex.Message}");
    }
}


