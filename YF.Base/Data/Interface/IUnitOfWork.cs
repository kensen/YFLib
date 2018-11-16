namespace YF.Base.Data
{
   public interface IUnitOfWork
    {
        void Begin();
        void Commit();
        void Rollback();
      

    }
}
