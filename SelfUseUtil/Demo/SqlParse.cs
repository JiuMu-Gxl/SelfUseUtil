using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Newtonsoft.Json;

namespace SelfUseUtil.Demo
{
    public static class SqlParse
    {
        /// <summary>
        /// 解析T-Sql（不支持PgSql）
        /// </summary>
        /// <param name="sql"></param>
        public static void SqlParserParse(string sql) {
            var result = Parser.Parse(sql);
            Console.WriteLine(result.BatchCount);
            Console.WriteLine(result.Script.Sql);
            Console.WriteLine("-------------------------------");
            IterateSqlNode(result.Script);

            void IterateSqlNode(SqlCodeObject sqlCodeObject, int indent = 0)
            {
                if (sqlCodeObject.Children == null)
                    return;
                foreach (var child in sqlCodeObject.Children)
                {
                    var type = child.GetType().Name;
                    Console.WriteLine($"{new string(' ', indent)}Type:{type}, Sql:{(child.Sql)}\n");
                    IterateSqlNode(child, indent + 2);
                }
            }
        }

        /// <summary>
        /// 解析T-Sql（不支持PgSql）
        /// Microsoft.SqlServer.TransactSql.ScriptDom 包
        /// </summary>
        /// <param name="sql"></param>
        public static void TransactTSqlParser(string sql) {
            var parser = new TSql140Parser(false);
            IList<ParseError> errors;
            TSqlFragment fragment = parser.Parse(new StringReader(sql), out errors);
            foreach (ParseError error in errors)
            {
                Console.WriteLine(error.Message);
                return;
            }
            foreach (var item in fragment.ScriptTokenStream)
            {
                if (item.TokenType == TSqlTokenType.WhiteSpace)
                {
                    Console.WriteLine("---------------------------------------------");
                }
                else
                {
                    Console.WriteLine($"{item.Text}--{item.TokenType}");
                }
            }
            var data = fragment.GetType().GetProperty("Batches").GetValue(fragment);
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(fragment));
        }
    }
}
