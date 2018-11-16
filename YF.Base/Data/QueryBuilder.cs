using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace YF.Base.Data
{
    /// <summary>
    /// Where 条件项
    /// </summary>
   public  class WhereItem
    {
       public string Field { get; set; }

       public string FirstVal { get; set; }

       public string SecondVal { get; set; }

       public SqlOperators.OperationMethod OperationMethod { get; set; }

       public bool HasFUNC { get; set; }

        public string renParameter { get; set; }

    }

    /// <summary>
   /// 查询条件组，一组查询条件包含多个 Where 条件项
    /// </summary>
    public class QueryBuilder
    {
        /// <summary>
        /// 是否是 And 查询
        /// </summary>
        public bool IsAnd { get; set; }

        /// <summary>
        /// 多条件组的时候是否是 And 查询
        /// </summary>
        public bool IsGroupAnd { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public List<WhereItem> WhereItems { get; set; }

        public QueryBuilder()
        {
            WhereItems = new List<WhereItem>();
            IsGroupAnd = true;
            IsAnd = true;
        }
    }
    
    /// <summary>
    /// 排序条件
    /// </summary>
    public class SortCondition
    {
        public string Field { get; set; }

        public ListSortDirection ListSortDirection { get; set; }

        public SortCondition(string sortField)
            : this(sortField, ListSortDirection.Ascending)
        { }

        /// <summary>
        /// 构造一个排序字段名称和排序方式的排序条件
        /// </summary>
        /// <param name="sortField">字段名称</param>
        /// <param name="listSortDirection">排序方式</param>
        public SortCondition(string sortField, ListSortDirection listSortDirection)
        {
            Field = sortField;
            ListSortDirection = listSortDirection;
        }

      
      
    }

    /// <summary>
    /// 支持泛型的列表字段排序条件
    /// </summary>
    /// <typeparam name="T">列表元素类型</typeparam>
    public class SortCondition<T> : SortCondition
    {
        /// <summary>
        /// 使用排序字段 初始化一个<see cref="SortCondition"/>类型的新实例
        /// </summary>
        public SortCondition(Expression<Func<T, object>> keySelector)
            : this(keySelector, ListSortDirection.Ascending)
        { }

        /// <summary>
        /// 使用排序字段与排序方式 初始化一个<see cref="SortCondition"/>类型的新实例
        /// </summary>
        public SortCondition(Expression<Func<T, object>> keySelector, ListSortDirection listSortDirection)
            : base(GetPropertyName(keySelector), listSortDirection)
        { }

        /// <summary>
        /// 从泛型委托获取属性名
        /// </summary>
        private static string GetPropertyName(Expression<Func<T, object>> keySelector)
        {
            var param = keySelector.Parameters.First().Name;
            string operand = (((dynamic)keySelector.Body).Operand).ToString();
            operand = operand.Substring(param.Length + 1, operand.Length - param.Length - 1);
            return operand;
        }
    }
}
