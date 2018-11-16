using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace YF.Base.Data
{
    interface IRepository<TEntity,TKey>:IDependency where TEntity: EntityBase<TKey>
    {
        #region 属性

        /// <summary>
        /// 获取 当前单元操作对象
        /// </summary>
      //  IUnitOfWork UnitOfWork { get; }

        ///// <summary>
        ///// 获取 当前实体类型的查询数据集
        ///// </summary>
        //IQueryable<TEntity> Entities { get; }

        #endregion

        TEntity GetById(TKey key);

        /// <summary>
        /// 单个插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        bool Insert(TEntity entity,bool autoId=true);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        bool Insert(ICollection<TEntity> entitys, bool autoId = true);

        /// <summary>
        /// 实体更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(TEntity entity);

        /// <summary>
        /// DTO实体更新
        /// </summary>
        /// <typeparam name="TEditDto"></typeparam>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool Update<TEditDto>(TEditDto dto)
            where TEditDto : IEditDto<TKey>;

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(TKey id);

      

        /// <summary>
        /// 批量ID删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(string ids);

        /// <summary>
        /// 批量ID删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete(ICollection<TKey> ids);

        /// <summary>
        /// 根据SQL查询当前实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, dynamic param = null, bool buffered = true);

        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);

        IEnumerable<dynamic> Query(string sql, dynamic param = null, bool buffered = true);

        SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        ///// <summary>
        ///// 根据SQL查询 ViewDTO 实体集合
        ///// </summary>
        ///// <typeparam name="TViewDto"></typeparam>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //IEnumerator<TViewDto> Query<TViewDto>(string sql, dynamic param = null);

        List<T> QueryPage<T>(string sqlstr, SortCondition sort, int intPageSize, int intCurrentIndex, out int total) where T : class;

    }
}
