using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace YF.Utility.Xml
{
    /// <summary>
    /// XML 扩展类，通过配合 XmlQueryParm 查询条件实现 XML 的节点查询
    /// 作者：余庆元
    /// </summary>
    public static class XmlHelpExtensions
    {

        /// <summary>
        /// 将XmlNode转换为XElement
        /// </summary>
        /// <returns> XElment对象 </returns>
        public static XElement ToXElement(this XmlNode node)
        {
            XDocument xdoc = new XDocument();
            using (XmlWriter xmlWriter = xdoc.CreateWriter())
            {
                node.WriteTo(xmlWriter);
            }
            return xdoc.Root;
        }

        /// <summary>
        /// 将XElement转换为XmlNode
        /// </summary>
        /// <returns> 转换后的XmlNode </returns>
        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(xmlReader);
                return xml;
            }
        }

        /// <summary>
        /// 根据条件查询 XElement ，返回新的 XElement 子集
        /// </summary>
        /// <param name="value">当前 XElement</param>
        /// <param name="parm">XML查询条件，主要设置节点（必填strNode）、及节点属性和父节点属性</param>
        /// <returns>查询子集</returns>
        public static IEnumerable<XElement> QueryElements(this XElement value, XmlQueryParm parm)
        {
            IEnumerable<XElement> newlist = from item in value.Descendants(parm.strNode) select item;

            if (parm.pstrAtt != null && parm.pstrAtt.Count > 0)
            {
                foreach (KeyValuePair<string, string> i in parm.pstrAtt)
                {
                    newlist = newlist.Where(p => (string)p.Parent.Attribute(i.Key) == i.Value);
                }
            }

            if (parm.strAtt != null && parm.strAtt.Count > 0)
            {
                foreach (KeyValuePair<string, string> i in parm.strAtt)
                {
                    newlist = newlist.Where(p => (string)p.Attribute(i.Key) == i.Value);
                }
            }



            return newlist;
        }
    }


    /// <summary>
    /// 查询条件
    /// </summary>
    public class XmlQueryParm
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string strNode { get; set; }

        /// <summary>
        /// 节点属性及赋值
        /// </summary>
        public Dictionary<string, string> strAtt { get; set; }

        /// <summary>
        /// 节点父级属性及赋值
        /// </summary>
        public Dictionary<string, string> pstrAtt { get; set; }

    }
}
