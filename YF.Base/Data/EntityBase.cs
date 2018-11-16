using System;
using System.ComponentModel.DataAnnotations;

namespace YF.Base.Data
{
   public class EntityBase<TKey>
    {
       protected EntityBase()
       {
           CreatedTime = DateTime.Now;
           IsDeleted = false;          
       }

       /// <summary>
       /// 主键
       /// </summary>
       [Key]
       public TKey Id { get; set; }

       /// <summary>
       /// 是否删除（伪删除）
       /// </summary>
       public bool IsDeleted { get; set; }

       /// <summary>
       /// 创建时间
       /// </summary>
       public DateTime CreatedTime { get; set; }

       ///// <summary>
       ///// 获取或设置 版本控制标识，用于处理并发
       ///// </summary>
       //[ConcurrencyCheck]
       //[Timestamp]
       //public byte[] Timestamp { get; set; }

       /// <summary>
       /// 判断两个实体是否是同一数据记录的实体
       /// </summary>
       /// <param name="obj"></param>
       /// <returns></returns>
       public override bool Equals(object obj)
       {
           if (obj == null)
           {
               return false;
           }
           var entity = obj as EntityBase<TKey>;
           if (entity == null)
           {
               return false;
           }
           return Id.Equals(entity.Id) && CreatedTime.Equals(entity.CreatedTime);
       }

       /// <summary>
       /// 用作特定类型的哈希函数。
       /// </summary>
       /// <returns>
       /// 当前 <see cref="T:System.Object"/> 的哈希代码。
       /// </returns>
       public override int GetHashCode()
       {
           return Id.GetHashCode() ^ CreatedTime.GetHashCode();
       }

       
    }
}
