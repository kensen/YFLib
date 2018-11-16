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
    public class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        private readonly Database<MyDb>.Table<TEntity, TKey> _table;
        // private readonly IUnitOfWork _unitOfWork;
        private readonly MyDb _mydb;

        /// <summary>
        /// 当前表明
        /// </summary>
        public String TableName
        {
            get
            {
                return _table.TableName;
            }
        }

        /// <summary>
        /// 当前连接的 DB 实体
        /// </summary>
        public MyDb DbContext
        {
            get
            {
                return _mydb;
            }
        }


        public RepositoryBase(IConnectionFactory connectionFactory)
        {
            _mydb = connectionFactory.GetDb();
            _table = _mydb.Set<TEntity, TKey>();
        }

        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        public bool Insert(TEntity entity, bool autoId = true)
        {
            return _table.Insert(entity, autoId) > 0;
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 插入实体，并返回自增ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        public int? InsertReturnId(TEntity entity, bool autoId = true)
        {
            return _table.InsertReturnId(entity, autoId);
        }

        /// <summary>
        /// 小批量插入实体列表
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        public bool Insert(ICollection<TEntity> entitys, bool autoId = true)
        {
            try
            {
                //  _unitOfWork.Begin();
                _mydb.BeginTransaction();
                var isSucceed = false;

                var paramNames = Database<MyDb>.Table<TEntity>.GetParamNames(entitys.First());
                if (autoId)
                {
                    paramNames.Remove("Id");
                }

                var cols = string.Join(",", paramNames);
                var colsParams = string.Join(",", paramNames.Select(p => "@" + p));
                var sql = " insert " + TableName + " (" + cols + ") values (" + colsParams + ") ";
                isSucceed = _mydb.Execute(sql, entitys) > 0;
                //  _unitOfWork.Commit();
                _mydb.CommitTransaction();
                return isSucceed;
            }
            catch (Exception)
            {
                _mydb.RollbackTransaction();
                return false;
            }
            finally
            {
             //_mydb.Dispose();
            }

          
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(TEntity entity)
        {
            try
            {
                return _table.Update(entity.Id, entity) > 0;
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
            //  throw new NotImplementedException();
           
        }

        /// <summary>
        /// 根据DTO 更新对应字段
        /// </summary>
        /// <typeparam name="TEditDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool Update<TEditDto>(TEditDto dto) where TEditDto : IEditDto<TKey>
        {
            try
            {
                var tableName = _table.TableName;

                var paramNames = Database<MyDb>.Table<TEntity, TKey>.GetParamNames(dto);

                var builder = new StringBuilder();
                builder.Append("update ").Append(tableName).Append(" set ");
                builder.AppendLine(string.Join(",", paramNames.Where(n => n != "Id").Select(p => p + "= @" + p)));
                builder.Append("where Id = @Id");

                var parameters = new DynamicParameters(dto);
                parameters.Add("Id", dto.Id);

                return _mydb.Execute(builder.ToString(), parameters) > 0;
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

        
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(TKey id)
        {
            try
            {
                return _table.Delete(id);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
            

        }

        /// <summary>
        /// 根据ID字符串批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool Delete(string ids)
        {
            try
            {
                var sql = new StringBuilder();
                sql.AppendFormat("delete from {0} where Id in ({1})", TableName, ids);
                return _mydb.Execute(sql.ToString()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
            
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool Delete(ICollection<TKey> ids)
        {
            //  _mydb.BeginTransaction();
            try
            {
                var isSucceed = false;
                foreach (var id in ids)
                {
                    isSucceed = Delete(id);
                    if (isSucceed) continue;
                    _mydb.RollbackTransaction();
                    return false;
                }
                _mydb.CommitTransaction();

                return isSucceed;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

           
        }

        /// <summary>
        /// 查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, dynamic param = null, bool buffered = true)
        {
            return _mydb.Query<T>(sql, param, buffered);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null)
        {
            return _mydb.Query<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null)
        {
            return _mydb.Query<TFirst, TSecond, TThird, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null)
        {
            return _mydb.Query<TFirst, TSecond, TThird, TFourth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null)
        {
            return _mydb.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout);

        }

        public IEnumerable<dynamic> Query(string sql, dynamic param = null, bool buffered = true)
        {
            return _mydb.Query(sql, param, buffered);

        }

        public SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _mydb.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
        }

        public object QuerySingle(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            SqlMapper.GridReader reader = _mydb.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
            return reader.ReadFirst();
        }

        /// <summary>
        /// 获取分页实体列表
        /// </summary>
        /// <typeparam name="T">返回实体类型</typeparam>
        /// <param name="sqlstr">查询语句</param>
        /// <param name="sort">排序条件</param>
        /// <param name="intPageSize">分页大小</param>
        /// <param name="intCurrentIndex">当前页</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<T> QueryPage<T>(string sqlstr, SortCondition sort, int intPageSize, int intCurrentIndex, out int total) where T : class
        {

            var pageSql = _mydb.GetPageSelString(sqlstr, sort, "*", intPageSize, intCurrentIndex);
            var countSql = string.Format("SELECT COUNT(*) DataCount FROM ({0}) T ", sqlstr);

            total = _mydb.Query<int>(countSql).FirstOrDefault();
            return _mydb.Query<T>(pageSql).ToList();

            //return db.Query<T>(sqlstr)
        }

        /// <summary>
        /// 自定义获取分页实体列表
        /// </summary>
        /// <typeparam name="T">自定义实体</typeparam>
        /// <param name="withTables">自定义 with table as （） 的查询语句</param>
        /// <param name="sqlTable">自定义查询的 Table SQL 语句，不包含 Where条件</param>
        /// <param name="queryBuliders">查询条件</param>
        /// <param name="sort">排序</param>
        /// <param name="intPageSize">分页大小</param>
        /// <param name="intCurrentIndex">当前页</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<T> QueryPage<T>(string withTables, string sqlTable, List<QueryBuilder> queryBuliders, SortCondition sort, int intPageSize, int intCurrentIndex, out int total) where T : class
        {
            StringBuilder sql = new StringBuilder();         
            sql.AppendFormat(" {0} where 1=1", sqlTable);

            var p = new DynamicParameters();
            foreach (var queryBulider in queryBuliders.Where(queryBulider => queryBulider != null && queryBulider.WhereItems.Count > 0))
            {
                SqlOperators.BuildParameters(queryBulider, ref p);
                string whereSql =  SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                //重构后代码
                sql.AppendFormat(queryBulider.IsAnd ? " {0} {1}" : " {0} ( {1} )",
                    queryBulider.IsGroupAnd ? "And" : "Or",whereSql == string.Empty ? "1=1" : whereSql);
                   

            }


            var pageSql = withTables+ _mydb.GetPageSelString(sql.ToString(), sort, "*", intPageSize, intCurrentIndex);
            var countSql = string.Format("SELECT COUNT(*) DataCount FROM ({0}) T ", sql);

            Console.WriteLine(withTables+sql.ToString());
            System.Diagnostics.Debug.WriteLine(withTables + sql.ToString());
            total = _mydb.ExecuteScalar<int>(withTables+countSql,p);
            return _mydb.Query<T>(pageSql,p).ToList();

            //return db.Query<T>(sqlstr)
        }

        /// <summary>
        /// 查询当前表的分页数据
        /// </summary>
        /// <param name="queryBulider">单个查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="intPageSize">分页大小</param>
        /// <param name="intCurrentIndex">当前页</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public virtual List<TEntity> QueryPage(QueryBuilder queryBulider, SortCondition sort, int intPageSize, int intCurrentIndex, out int total)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("select * from {0}", TableName);

                var p = new DynamicParameters();
                if (queryBulider != null && queryBulider.WhereItems.Count > 0)
                {
                    SqlOperators.BuildParameters(queryBulider, ref p);
                    string whereSql = SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                    sql.AppendFormat(" where {0}", whereSql == string.Empty ? "1=1" : whereSql);
                }

                var pageSql = _mydb.GetPageSelString(sql.ToString(), sort, "*", intPageSize, intCurrentIndex);
                var countSql = string.Format("SELECT COUNT(*) DataCount FROM ({0}) T ", sql);

                Console.WriteLine(sql.ToString());
                System.Diagnostics.Debug.WriteLine(sql.ToString());

                total = _mydb.ExecuteScalar<int>(countSql, p);
                return _mydb.Query<TEntity>(pageSql, p).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //_mydb.Dispose();
            }

            

            //return db.Query<T>(sqlstr)
        }


        /// <summary>
        /// 查询当前表的分页数据
        /// </summary>
        /// <param name="queryBuliders">查询条件集合</param>
        /// <param name="sort">排序条件</param>
        /// <param name="intPageSize">分页大小</param>
        /// <param name="intCurrentIndex">当前页</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public virtual List<TEntity> QueryPage(List<QueryBuilder> queryBuliders, SortCondition sort, int intPageSize, int intCurrentIndex, out int total)
        {

            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("select * from {0} where 1=1", TableName);

                //if (queryBulider != null && queryBulider.WhereItems.Count > 0)
                //{
                //    sql.AppendFormat(" where {0}", SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd));
                //}
                var p = new DynamicParameters();
                foreach (var queryBulider in queryBuliders.Where(queryBulider => queryBulider != null && queryBulider.WhereItems.Count > 0))
                {
                    SqlOperators.BuildParameters(queryBulider, ref p);

                    string whereSql = SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                    //重构后代码
                    sql.AppendFormat(queryBulider.IsAnd ? " {0} {1}" : " {0} ( {1} )",
                        queryBulider.IsGroupAnd ? "And" : "Or",
                        whereSql == string.Empty ? "1=1" : whereSql);

                }


                var pageSql = _mydb.GetPageSelString(sql.ToString(), sort, "*", intPageSize, intCurrentIndex);
                var countSql = string.Format("SELECT COUNT(*) DataCount FROM ({0}) T ", sql);

                Console.WriteLine(pageSql.ToString());
                System.Diagnostics.Debug.WriteLine(pageSql.ToString());
                total = _mydb.ExecuteScalar<int>(countSql, p);
                return _mydb.Query<TEntity>(pageSql, p).ToList();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            finally
            {
                //_mydb.Dispose();
            }

           

            //return db.Query<T>(sqlstr)
        }



        public int Execute(string sql, dynamic param = null)
        {
            try
            {
                return _mydb.Execute(sql, param);
            }catch(Exception)
            {
                throw;
            }finally
            {
                //_mydb.Dispose();
            }
            //MyDb.GetTableSchema(_mydb,)
           
        }

        public T ExecuteScalar<T>(string sql, dynamic param = null)
        {
            try
            {
                return _mydb.ExecuteScalar<T>(sql, param);
            }catch(Exception)
            {
                throw;  
            }
            finally
            {
                //_mydb.Dispose();
            }

           
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetById(TKey id)
        {
            try
            {
                return _table.Get(id);
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
       
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetList()
        {
            try
            {
                return _table.All().ToList();
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
            //StringBuilder sql = new StringBuilder();
            //sql.AppendFormat("select * from {0}", TableName);
            //return _mydb.Query<TEntity>(sql.ToString()).ToList();
         
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="queryBulider">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns></returns>
        public virtual List<TEntity> GetList(QueryBuilder queryBulider,SortCondition sort=null)
        {
            try
            {
                var sql = new StringBuilder();
                sql.AppendFormat("select * from {0}", TableName);

                var p = new DynamicParameters();

                if (queryBulider != null && queryBulider.WhereItems.Count > 0)
                {
                    //foreach ( var w in queryBulider.WhereItems){
                    //    p.Add(w.Field, w.FirstVal);
                    //}

                    SqlOperators.BuildParameters(queryBulider, ref p);

                    string whereSql = SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                    sql.AppendFormat(" where {0}", whereSql == string.Empty ? "1=1" : whereSql);
                }

                if (sort != null)
                {
                    sql.AppendFormat(" Order by {0} {1} ", sort.Field, sort.ListSortDirection == ListSortDirection.Ascending ? "Asc" : "Desc");
                }
                //测试输出生成的SQL语句
                Console.WriteLine(sql.ToString());
                System.Diagnostics.Debug.WriteLine(sql.ToString());
                return _mydb.Query<TEntity>(sql.ToString(), p).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

        
           // return _table.All().ToList();
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="queryBuliders">查询条件组</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual List<TEntity> GetList(List<QueryBuilder> queryBuliders,SortCondition sort=null)
        {
            try
            {
                var sql = new StringBuilder();
                sql.AppendFormat("select * from {0} where 1=1", TableName);

                var p = new DynamicParameters();
                foreach (var queryBulider in queryBuliders.Where(queryBulider => queryBulider != null && queryBulider.WhereItems.Count > 0))
                {
                    //重构前代码
                    //if (queryBulider.IsAnd)
                    //{
                    //    sql.AppendFormat(" {0} {1}", queryBulider.IsGroupAnd ? "And" : "Or", SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd));
                    //}
                    //else
                    //{
                    //    sql.AppendFormat(" {0} ( {1} )", queryBulider.IsGroupAnd ? "And" : "Or", SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd));
                    //}
                    SqlOperators.BuildParameters(queryBulider, ref p);

                    string whereSql = SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                    //重构后代码
                    sql.AppendFormat(queryBulider.IsAnd ? " {0} {1}" : " {0} ( {1} )",
                        queryBulider.IsGroupAnd ? "And" : "Or",
                        whereSql == string.Empty ? "1=1" : whereSql);

                }

                if (sort != null)
                {
                    sql.AppendFormat(" Order by {0} {1} ", sort.Field, sort.ListSortDirection == ListSortDirection.Ascending ? "Asc" : "Desc");
                }
                //测试输出生成的SQL语句
                Console.WriteLine(sql.ToString());
                System.Diagnostics.Debug.WriteLine(sql.ToString());
                return _mydb.Query<TEntity>(sql.ToString(), p).ToList();
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

          
            // return _table.All().ToList();
        }

        /// <summary>
        /// 自定义查询实体列表
        /// </summary>
        /// <param name="sqlCustom">指定SQL查询语句，适合多表关联，不包含Where</param>
        /// <param name="queryBuliders">查询条件组</param>
        /// <param name="sort">排序</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sqlCustom, List<QueryBuilder> queryBuliders, SortCondition sort = null)
        {
            try
            {
                var sql = new StringBuilder();
                sql.AppendFormat(" {0} where 1=1", sqlCustom);

                var p = new DynamicParameters();
                foreach (var queryBulider in queryBuliders.Where(queryBulider => queryBulider != null && queryBulider.WhereItems.Count > 0))
                {
                    SqlOperators.BuildParameters(queryBulider, ref p);
                    string whereSql = SqlOperators.BuildWhere(queryBulider.WhereItems, queryBulider.IsAnd);
                    //重构后代码
                    sql.AppendFormat(queryBulider.IsAnd ? " {0} {1}" : " {0} ( {1} )",
                        queryBulider.IsGroupAnd ? "And" : "Or",
                        whereSql == string.Empty ? "1=1" : whereSql);

                }

                if (sort != null)
                {
                    sql.AppendFormat(" Order by {0} {1} ", sort.Field, sort.ListSortDirection == ListSortDirection.Ascending ? "Asc" : "Desc");
                }

                Console.WriteLine(sql.ToString());
                System.Diagnostics.Debug.WriteLine(sql.ToString());
                return _mydb.Query<T>(sql.ToString(), p).ToList();
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

           
            // return _table.All().ToList();
        }


        public int Count()
        {
            try
            {
                var sql = new StringBuilder();
                sql.AppendFormat("select count(*) from {0}", TableName);

                return _mydb.ExecuteScalar<int>(sql.ToString());
            }catch(Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }

          
        }

        /// <summary>
        /// 大批量数据导入
        /// </summary>
        /// <param name="dt">DT必须字段对应数据库</param>
        /// <param name="hasUnitOfWork">是否在UnitOfWork进程中提交</param>
        /// <returns></returns>
        public virtual bool BulkInserts(DataTable dt, bool hasUnitOfWork = false)
        {
            try
            {

                var tran = (SqlTransaction)_mydb.BeginTransaction();
                var result = _mydb.InsertTable(tran, dt, TableName, dt.Columns);
                if (!hasUnitOfWork)
                    _mydb.CommitTransaction();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
        }

        /// <summary>
        /// 批量导入到临时表（需要配合事务）
        /// </summary>
        /// <param name="tran">事务</param>
        /// <param name="dt">导入数据</param>
        /// <param name="tmpTableName">临时表名称（如：#tmp）</param>
        /// <returns>返回成功失败，这里不执行事务提交</returns>
        public virtual bool BulkInserts(DataTable dt, string tmpTableName)
        {
            try
            {

                var tran = (SqlTransaction)_mydb.BeginTransaction();

                StringBuilder createTmpTable = new StringBuilder();
                createTmpTable.AppendFormat(@"IF Object_id('Tempdb..#tmp') IS NOT NULL   
DROP TABLE #tmp ;", tmpTableName);
                 createTmpTable.AppendFormat(" create table {0} (", tmpTableName);
                var i=0;
                foreach(DataColumn dc in dt.Columns)
                {
                        ++i;
                    createTmpTable.AppendFormat("[{0}]  {1}", dc.ColumnName, GetTableColumnType(dc.DataType));
                    if (i < dt.Columns.Count)
                    {
                        createTmpTable.Append(",");
                    }
                    
                }

                createTmpTable.Append(");");
                _mydb.Execute(createTmpTable.ToString());
                var result = _mydb.InsertTable(tran, dt, tmpTableName, dt.Columns);
                //if (!hasUnitOfWork)
                //    _mydb.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //_mydb.Dispose();
            }
        }


        private string GetTableColumnType(System.Type type)
        {
            string result = "varchar(500)";
            string sDbType = type.ToString();
            switch (sDbType)
            {
                case "System.String":
                    break;
                case "System.Int16":
                    result = "int";
                    break;
                case "System.Int32":
                    result = "int";
                    break;
                case "System.Int64":
                    result = "float";
                    break;
                case "System.Decimal":
                    result = "decimal(18,4)";
                    break;
                case "System.Double":
                    result = "decimal(18,4)";
                    break;
                case "System.DateTime":
                    result = "datetime";
                    break;
                default:
                    break;
            }
            return result;
        }

        public virtual DataTable QueryDataTable(string sql, CommandType commandType, dynamic param = null)
        {
            return _mydb.QuerySqlServerDataTable(sql, commandType, param);
        }        
    }
}
