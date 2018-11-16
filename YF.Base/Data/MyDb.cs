using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace YF.Base.Data
{
    public class MyDb : Database<MyDb>
    {       
    

        public  Table<TEntity, TKey> Set<TEntity,TKey>()
        {
            return new Table<TEntity, TKey>(this, typeof(TEntity).Name);
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strSelSrc"></param>
        /// <param name="sort"></param>
        /// <param name="strShowCol"></param>
        /// <param name="intPageSize"></param>
        /// <param name="intCurrentIndex"></param>
        /// <returns></returns>
        public string GetPageSelString(string strSelSrc, SortCondition sort, string strShowCol, int intPageSize, int intCurrentIndex)
        {
            // select * from (select row_number() over (order by CheckID) row,* from (select * from CheckInfo) a)z
            //where row between 0 and 100
            
            
            var builder = new StringBuilder();

            builder.AppendFormat("select {0} from ( select row_number() over (order by {1} {3} ) row,* from ({2}) a)z ", strShowCol, sort.Field, strSelSrc, sort.ListSortDirection==ListSortDirection.Ascending?"ASC":"DESC");

            builder.AppendFormat(" where row between {0}*({1}-1)+1 and {0}*{1} ",  intPageSize, intCurrentIndex );

            return builder.ToString();
        }

        public  List<SqlTableSchema> GetTableSchema( string tableName)
        {
            const string sql = "sp_MShelpcolumns @table ";
            return Query<SqlTableSchema>(sql, new { table=tableName }).ToList();
        }

        public Boolean InsertTable(SqlTransaction tran, DataTable dt, string tableName, DataColumnCollection dtColum)
        {
            //打开数据库
            // GetConn();

            try
            {

                //声明SqlBulkCopy ,using释放非托管资源
                using (SqlBulkCopy sqlBc = new SqlBulkCopy(tran.Connection, SqlBulkCopyOptions.Default, tran))
                {
                    //一次批量的插入的数据量
                    sqlBc.BatchSize = 1000;
                    //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
                    sqlBc.BulkCopyTimeout = 60;
                    //設定 NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。 
                    //sqlBC.NotifyAfter = 10000;
                    // sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

                    //设置要批量写入的表
                    sqlBc.DestinationTableName = tableName;

                    //自定义的datatable和数据库的字段进行对应
                    //sqlBC.ColumnMappings.Add("id", "tel");
                    //sqlBC.ColumnMappings.Add("name", "neirong");
                    for (var i = 0; i < dtColum.Count; i++)
                    {
                        sqlBc.ColumnMappings.Add(dtColum[i].ColumnName, dtColum[i].ColumnName);

                        //sqlBC.ColumnMappings.Add(i, i);
                    }
                    //批量写入
                    sqlBc.WriteToServer(dt);

                    // tran.Commit();
                }
                //  conn.Dispose();
                //GetConn();
                return true;
            }
            catch (Exception ex)
            {
                // tran.Rollback();
                return false;

            }
            finally
            {
                //关闭数据库
                // sqlConn.Close();
            }
        }
    }
}
