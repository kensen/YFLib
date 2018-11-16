//using System.Transactions;

namespace YF.Base.Data
{
   public class UnitOfWork:IUnitOfWork //,IDependency
    {     

       private readonly MyDb _db;
      // private TransactionScope transactionScope;

       /// <summary>
       /// 初始化构造函数
       /// </summary>
       /// <param name="connectionFactory"></param>
       public UnitOfWork(IConnectionFactory connectionFactory)
        {         
            _db = connectionFactory.GetDb();
        }

       /// <summary>
       /// 启动事务
       /// </summary>
       public void Begin()
        {                   
             _db.BeginTransaction();
        }

       /// <summary>
       /// 提交事务
       /// </summary>
        public void Commit()
        {         
            _db.CommitTransaction();
        }

       /// <summary>
       /// 回滚事务
       /// </summary>
        public void Rollback()
        {           
            _db.RollbackTransaction();
        }     

       /// <summary>
       /// 销毁对象
       /// </summary>
        public void Dispose()
        {         
            _db.Dispose();
        }

    }
}
