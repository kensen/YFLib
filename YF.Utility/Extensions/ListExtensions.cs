using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YF.Utility.Extensions
{
    public static class ListExtensions
    {   

        public static DataTable ToDataTable<T>(this List<T> value)
        {
            // ====== list to Datable 转化
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            try
            {
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (T item in value)
                {
                    for (int i = 0; i < values.Length; i++)
                        values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    dt.Rows.Add(values);
                }
            }catch
            {
                return null;
            }           

            return dt;
            // ====== list to Datable 转化 End
        }

    }
}
