using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YF.Base.Data
{
    public  class MySqlDB<TEntity, TKey>: Database<MySqlDB<TEntity,TKey>>
    {
       
        public class MySqlDBTable<T,Tid> : Table<T, Tid>
        {
            public MySqlDBTable(Database<MySqlDB<TEntity, TKey>> database, string likelyTableName)
                : base(database, likelyTableName)
            {
            }

            /// <summary>
            /// Insert a row into the db
            /// </summary>
            /// <param name="data">Either DynamicParameters or an anonymous type or concrete type</param>
            /// <returns></returns>
            //public override int? Insert(dynamic data, bool autoId = true)
            //{
            //    var o = (object)data;
            //    List<string> paramNames = GetParamNames(o);
            //    if (autoId)
            //    {
            //        paramNames.Remove("Id");
            //    }


            //    string cols = string.Join(",", paramNames);
            //    string cols_params = string.Join(",", paramNames.Select(p => "@" + p));

            //    var sql = "insert " + TableName + " (" + cols + ") values (" + cols_params + ")";
            //    if (database.Execute(sql, o) != 1)
            //    {
            //        return null;
            //    }

            //    return (int)database.Query<decimal>("SELECT @@IDENTITY AS LastInsertedId").Single();
            //}
        }
        public Table<TEntity, TKey> Table ;

        public MySqlDB(){
            Table = new MySqlDBTable<TEntity, TKey>(this, typeof(TEntity).Name);
            }
        public static MySqlDB<TEntity, TKey> Init(DbConnection connection)
        {
            MySqlDB<TEntity, TKey> db = new MySqlDB<TEntity, TKey>();
            db.InitDatabase(connection, 0);           
            return db;
        }

        internal override Action<MySqlDB<TEntity, TKey>> CreateTableConstructorForTable()
        {
            return CreateTableConstructor(typeof(MySqlDBTable<TEntity,TKey>));
        }
    }
}

