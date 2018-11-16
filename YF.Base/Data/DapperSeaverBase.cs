namespace YF.Base.Data
{
    public abstract class DapperSeaverBase
    {
        protected DapperSeaverBase(IConnectionFactory connection)
        {
            UnitOfWork = new UnitOfWork(connection);
        }

        /// <summary>
        /// 获取或设置 单元操作对象
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; private set; }
    }
}
