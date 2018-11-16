using System.Data;

namespace YF.Base.Data
{
   public interface IConnectionFactory
    {
        IDbConnection GetConnection();
      //  IDbTransaction GetTransaction();
        MyDb GetDb();
    }
}
