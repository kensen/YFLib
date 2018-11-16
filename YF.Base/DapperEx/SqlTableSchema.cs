using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
   public class SqlTableSchema
    {
       public string Col_Name { get; set; }

       public int Col_Id { get; set; }

       public string Col_Typename { get; set; }

       public int Col_Len { get; set; }

       public bool Col_Null { get; set; }

       public bool Col_Identity { get; set; }
    }
}
