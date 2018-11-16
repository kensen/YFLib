namespace YF.Base.Data
{
    /// <summary>
    /// 添加DTO
    /// </summary>
    public interface IAddDto
    { }

    /// <summary>
    /// 只读视图DTO
    /// </summary>
    public interface IViewDto { }

    /// <summary>
    /// 编辑DTO
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEditDto<TKey>
    {
        /// <summary>
        /// 获取或设置 主键，唯一标识
        /// </summary>
        TKey Id { get; set; }
    }
}
