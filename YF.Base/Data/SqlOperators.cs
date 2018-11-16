using Dapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YF.Base.Data
{
    public static class SqlOperators
    {


        public enum OperationMethod
        {
            ///// <summary>
            ///// 与操作
            ///// </summary>
            //[Description("与")]
            //And,

            ///// <summary>
            ///// 或操作
            ///// </summary>       
            //[Description("或")]
            //Or,

            /// <summary>
            /// 等于
            /// </summary>
            [Description("等于")]
            Equal,

            /// <summary>
            /// 小于
            /// </summary>
             [Description("小于")]
            Less,

            /// <summary>
            /// 小于等于
            /// </summary>
             [Description("小于等于")]
            LessOrEqual,

            /// <summary>
            /// 大于
            /// </summary>
             [Description("大于")]
            Greater,

            /// <summary>
            /// 大于等于
            /// </summary>
             [Description("大于等于")]
            GreaterOrEqual,

            /// <summary>
            /// 不等于
            /// </summary>
             [Description("不等于")]
            Unequal,

            /// <summary>
            /// 区间
            /// </summary>
             [Description("区间")]
            Between,

            /// <summary>
            /// 左模糊查询 like 'XX%'
            /// </summary>
             [Description("左模糊查询")]
            LikeLeft,

            /// <summary>
            /// 全包含模糊查询 like '%XX%'
            /// </summary>
             [Description("模糊查询")]
            LikeCentre,

            /// <summary>
            /// 右模糊查询 like ‘%XX' 
            /// </summary>
             [Description("右模糊查询")]
            LikeRight,

            /// <summary>
            /// In 查询
            /// </summary>
            [Description("In 查询")]
            In,

            /// <summary>
            /// 存在
            /// </summary>
           [Description("存在")]
            Exists,

            /// <summary>
            /// 不存在
            /// </summary>
            [Description("不存在")]
            NotExists,

              /// <summary>
              /// 空查询
              /// </summary>
            [Description("IsNull 空查询")]
            IsNull,

              /// <summary>
              /// 空查询
              /// </summary>
            [Description("IsNotNull 非空查询")]
            IsNotNull

        }

        public enum SqlFuncOperation
        {
            /// <summary>
            /// 日期格式化函数
            /// </summary>
             [Description("日期格式化函数")]
            Convert,

            /// <summary>
            /// 截取字符串，从左边开始
            /// </summary>
            [Description("截取字符串，从左边开始")]
            Left,

            /// <summary>
            /// 截取字符串，从右开始
            /// </summary>
            [Description("截取字符串，从右开始")]
            Right,

            /// <summary>
            /// 截取字符串，自定义位置和长度
            /// </summary>
            [Description("截取字符串，自定义位置和长度")]
            Substring,

            /// <summary>
            /// 字符串位置检索
            /// </summary>
             [Description("字符串位置检索")]
            Charindex,

            /// <summary>
            /// 获取长度
            /// </summary>
            [Description("获取长度")]
            Len,

                   /// <summary>
                   /// IsNull
                   /// </summary>
            [Description("获取长度")]
            IsNull


        }

        /// <summary>
        /// SQL字段格式化SQL函数调用
        /// </summary>
        /// <param name="str">当前字段字符串</param>
        /// <param name="func">使用的SQL函数</param>
        /// <param name="formatStr">格式化的字段</param>
        /// <param name="param">格式化参数</param>
        /// <returns></returns>
        public static string FormatSqlFunc(this string str, SqlFuncOperation func, string field, params string[] param)
        {
            string formatStr = "";
            // var pm = param;
            //pm.a

            if (string.IsNullOrEmpty(field))
            {
                field = str;
            }
            
          //  pm.AddDynamicParams(param);

            switch (func)
            {
                case SqlFuncOperation.Left:
                    formatStr = string.Format(" LEFT('{0}',{1}) ", field, param[0]);
                    break;
                case SqlFuncOperation.Right:
                    formatStr = string.Format(" RIGHT('{0}',{1}) ", field, param[0]);
                    break;
                case SqlFuncOperation.Substring:
                    formatStr = string.Format(" SUBSTRING('{0}',{1},{2}) ", field, param[0], param[1]);
                    break;
                case SqlFuncOperation.Len:
                    formatStr = string.Format(" LEN('{0}') ", field);
                    break;
                case SqlFuncOperation.Charindex:
                    formatStr = string.Format(" CHARINDEX('{0}',{1}) ", param[0], field);
                    break;
                case SqlFuncOperation.Convert:
                    formatStr = string.Format(" CONVERT(varchar(100),{0},{1}) ", field, param[0]);
                    break;
                case SqlFuncOperation.IsNull:
                    formatStr = string.Format(" IsNull({0},'{1}') ", field, param[0]);
                    break;
                    
            }

            return formatStr+"::"+str;
        }

        /// <summary>
        /// 创建查询条件语句
        /// </summary>
        /// <param name="whereItems"></param>
        /// <param name="isAnd"></param>
        /// <returns></returns>
        public static string BuildWhere(List<WhereItem> whereItems, bool isAnd)
        {
            var strSql = new StringBuilder();
            var cn = isAnd ? "AND" : "OR";

            var i = 0;
            //whereItems.Remove()
            List<string> formatFieldValList = new List<string>();
            //foreach(var item in whereItems.Where(a=>a.FirstVal!=string.Empty ) )
           foreach (var item in whereItems)
            {
                var fields = Regex.Split(item.Field, "::", RegexOptions.IgnoreCase);
                if (!string.IsNullOrEmpty(item.FirstVal))
                {
                    if (i > 0)
                        strSql.Append(cn);
                    var fieldVal = "";
                    if (string.IsNullOrEmpty(item.renParameter))
                    {
                        fieldVal = "@" + (fields.Length > 1 ? fields[1].Trim() : fields[0].Trim());
                    }
                    else
                    {
                        fieldVal = "@" + item.renParameter.Trim();
                    }

                    var t = formatFieldValList.Where(a => a.Contains(fieldVal));
                    if (t.Count() > 0)
                    {
                        fieldVal = fieldVal + t.Count();
                    }

                    formatFieldValList.Add(fieldVal);
                    var fieldVal2 = fieldVal+"end";
                    var field = fields[0];

                    var firstVals = Regex.Split(item.FirstVal, "::", RegexOptions.IgnoreCase);
                    var firstVal =  firstVals[0];

                    //@CreatedTime
                    //CreatedTime
                    //var test = " CONVERT(varchar(100),CreatedTime,23) ".Replace("CreatedTime", "@CreatedTime");

                    if (firstVals.Length > 1)
                    {
                        fieldVal = firstVal.Replace(field, fieldVal);
                    }

                    if(!string.IsNullOrEmpty( item.SecondVal))
                    {
                        var SecondVals = Regex.Split(item.SecondVal, "::", RegexOptions.IgnoreCase);
                        var SecondVal = SecondVals[0];
                        if (SecondVals.Length > 1)
                        {
                            fieldVal2 = SecondVal.Replace(field, fieldVal2 );
                        }
                    }

                    if (item.OperationMethod == OperationMethod.In)
                    {
                        var inParameterList = item.FirstVal.Split(',');
                        string formatParam=string.Empty;
                        int index=0;
                        foreach (var p in inParameterList)
                        {
                            formatParam += fieldVal + (index == 0 ? "" : index.ToString()) + ",";
                            index++;
                        }
                        formatParam ="("+ formatParam.Substring(0, formatParam.Length - 1)+")";

                        strSql.AppendFormat(" {0} {1} {2} ", field, GetOpStr(item.OperationMethod), formatParam);
                    }
                    else
                    {
                        strSql.AppendFormat(" {0} {1} {2} ", field, GetOpStr(item.OperationMethod),
                    item.OperationMethod == OperationMethod.Between
                        ? fieldVal + " And " + fieldVal2
                        :  fieldVal);
                    }

                    //原修改参数前代码
                    //strSql.AppendFormat(" {0} {1} {2} ", item.Field, GetOpStr(item.OperationMethod),
                    //   item.OperationMethod == OperationMethod.Between
                    //       ? CreateParam(item.OperationMethod, item.FirstVal, item.SecondVal)
                    //       : CreateParam(item.OperationMethod, item.FirstVal));

                    //if (i < whereItems.Where(a => a.FirstVal != string.Empty).ToList().Count - 1)
                    //strSql.Append(cn);
                    i++;
                }
                else if (item.OperationMethod == OperationMethod.IsNull || item.OperationMethod==OperationMethod.IsNotNull)
                {
                    if (i > 0)
                        strSql.Append(cn);
                    var field = fields[0];
                    strSql.AppendFormat(" {0} {1} ", field, GetOpStr(item.OperationMethod));

                    //if (i < whereItems.ToList().Count - 1)
                    //    strSql.Append(cn);
                    i++;
                }
                else if (item.OperationMethod == OperationMethod.In)
                {
                    if (i > 0)
                        strSql.Append(cn);
                    var field = fields[0];
                    strSql.AppendFormat(" {0} {1} ('')", field, GetOpStr(item.OperationMethod));
                    i++;
                }
               
            }

            //var a = strSql.ToString().LastIndexOf(cn);
            //strSql = strSql.Length > 0 ? strSql.Remove(strSql.ToString().LastIndexOf(cn), cn.Length) : strSql;
            return strSql.ToString();
        }

        /// <summary>
        /// 创建查询语句的动态参数
        /// </summary>
        /// <param name="queryBuilder"></param>
        /// <param name="p"></param>
        public static void BuildParameters(QueryBuilder queryBuilder,ref DynamicParameters p)
        {
            List<string> formatFieldList = new List<string>();
            foreach (var w in queryBuilder.WhereItems)
            {
                var fields = Regex.Split(w.Field, "::", RegexOptions.IgnoreCase);
                var field = fields.Length > 1 ? fields[1].Trim() : fields[0].Trim();

                if (string.IsNullOrEmpty(w.renParameter))
                {
                            var t = formatFieldList.Where(a => a.Contains(field));
                            if (t.Count() > 0)
                            {
                                field = field + t.Count();
                            }
                }
                else
                {
                    field = w.renParameter;
                }
               

                formatFieldList.Add(field);

                var firstVals = Regex.Split(w.FirstVal, "::", RegexOptions.IgnoreCase);
                var firstVal = firstVals.Length > 1 ? firstVals[1] : firstVals[0];
              

                if (w.OperationMethod == SqlOperators.OperationMethod.Between)
                {
                    var secondVals = Regex.Split(w.SecondVal, "::", RegexOptions.IgnoreCase);
                    var secondVal = secondVals.Length > 1 ? secondVals[1] : secondVals[0];
                    p.Add(field, firstVal);
                    p.Add(field + "end", secondVal);
                }
                else if (w.OperationMethod == SqlOperators.OperationMethod.In)
                {
                    var inParameterValList = w.FirstVal.Split(',');
                    string formatParam = string.Empty;
                    int index = 0;
                    foreach (var inVal in inParameterValList)
                    {
                        formatParam = field + (index == 0 ? "" : index.ToString());
                        p.Add(formatParam, inVal);
                        index++;
                    }

                }
                else
                {
                    p.Add(field, SqlOperators.CreateParam(w.OperationMethod, firstVal));
                }

            }
        }

        /// <summary>
        /// 比较符
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetOpStr(OperationMethod method)
        {
            var opstr="";
            switch (method)
            {
                case OperationMethod.LikeCentre:                   
                case OperationMethod.LikeLeft:
                case OperationMethod.LikeRight:
                    opstr= "LIKE";
                    break;
                case OperationMethod.Equal:
                    opstr= "=";
                    break;
                case OperationMethod.Greater:
                    opstr= ">";
                       break;
                case OperationMethod.GreaterOrEqual:
                    opstr= ">=";
                       break;
                case OperationMethod.In:
                    opstr= "IN";
                       break;
                case OperationMethod.Less:
                    opstr= "<";
                       break;
                case OperationMethod.LessOrEqual:
                    opstr= "<=";
                       break;
                case OperationMethod.Unequal:
                    opstr= "<>";
                       break;  
                case OperationMethod.Between:
                       opstr = "Between";
                       break;
                case OperationMethod.Exists:
                       opstr = "Exists";
                       break;
                case OperationMethod.NotExists:
                       opstr = "Not Exists";
                       break;
                case OperationMethod.IsNull:
                    opstr = "Is Null";
                    break;
                case OperationMethod.IsNotNull:
                    opstr = "Is Not Null";
                    break;
            }
            return opstr;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="method">参数类型</param>
        /// <param name="value">第一个参数</param>
        /// <param name="value2">第二个参数</param>
        /// <returns></returns>
        public static object CreateParam(OperationMethod method, object value,object value2=null)
        {
            switch (method)
            {
                case OperationMethod.LikeLeft:
                    return string.Format("{0}%", value);
                case OperationMethod.LikeCentre:
                    return string.Format("%{0}%", value);
                case OperationMethod.LikeRight:
                    return string.Format("%{0}", value);
                case OperationMethod.Equal:                   
                case OperationMethod.Greater:                 
                case OperationMethod.GreaterOrEqual:  
                case OperationMethod.Less:                 
                case OperationMethod.LessOrEqual:                  
                case OperationMethod.Unequal:
                    return string.Format("{0}", value);
                case OperationMethod.In:
                case OperationMethod.Exists:
                case OperationMethod.NotExists:
                    return string.Format("({0})", value);
                case OperationMethod.Between:
                    return string.Format(" '{0}' And '{1}' ", value,value2);
           
            }
            return value;
        }



    }

}
