using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YF.Utility.Xml
{
    /// <summary>
    /// XML操作的抽象类
    /// 针对不同XML文件，继承之后通过初始化赋值XML属性。然后通过 XmlHelpExtensions 扩展方法实现读取扩展
    /// 余庆元 2014-09-04
    /// </summary>
    public abstract class XmlBase
    {
        public XElement XML { get; set; }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="path"></param>
        protected void SetXML(string path)
        {
            XML = XElement.Load(path);
        }

        protected void SetXMLByString(string XMLString)
        {
            XML = XElement.Parse(XMLString);
        }


     
    }
}
