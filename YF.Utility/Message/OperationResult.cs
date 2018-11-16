using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YF.Utility.Message
{
    /// <summary>
    /// 业务操作结果信息类，对操作结果进行封装
    /// </summary>
    public class OperationResult : OperationResult<object>
    {
        #region 构造函数

        /// <summary>
        /// 初始化一个<see cref="OperationResult"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType)
            : this(resultType, null, null)
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType, string message)
            : this(resultType, message, null)
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType, string message, object data)
            : base(resultType, message, data)
        { }

        #endregion
    }

    /// <summary>
    /// 泛型版本的业务操作结果信息类，对操作结果进行封装
    /// </summary>
    /// <typeparam name="T">返回数据的类型</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType)
            : this(resultType, null, default(T))
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType, string message)
            : this(resultType, message, default(T))
        { }

        /// <summary>
        /// 初始化一个<see cref="OperationResult{T}"/>类型的新实例
        /// </summary>
        public OperationResult(OperationResultType resultType, string message, T data)
        {
            ResultType = resultType;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 获取或设置 操作结果类型
        /// </summary>
        public OperationResultType ResultType { get; set; }

        /// <summary>
        /// 获取或设置 操作返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置 操作返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///  获取或设置 操作返回总记录数
        /// </summary>
        public int? Total { get; set; }

        public void ResultError(string Message)
        {
            this.ResultType = OperationResultType.Error;
            this.Message = Message;
        }
    }
}
