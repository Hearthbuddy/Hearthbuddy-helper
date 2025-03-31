using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace HearthHelper
{
    ///XML
    public class UtilsXml
    {
        string _xmlPath;//文件所在路径

        ///初始化一个配置，配置所在路径
        public UtilsXml(string xmlPath)
        {
            _xmlPath = Path.GetFullPath(xmlPath);
        }

        ///写入配置，value=写入的值，nodes=节点
        public void Write(string value, params string[] nodes)
        {
            //初始化
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(_xmlPath)) xmlDoc.Load(_xmlPath);
            else xmlDoc.LoadXml("<XmlConfig />");
            XmlNode xmlRoot = xmlDoc.ChildNodes[0];

            //新增、编辑节点
            string xpath = string.Join("/", nodes);
            XmlNode node = xmlDoc.SelectSingleNode(xpath);
            if (node == null) node = makeXPath(xmlDoc, xmlRoot, xpath);
            node.InnerText = value;

            //保存
            xmlDoc.Save(_xmlPath);
        }

        ///读取配置，nodes=节点
        public string Read(params string[] nodes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(_xmlPath)) return null;
            else xmlDoc.Load(_xmlPath);
            string xpath = string.Join("/", nodes);
            XmlNode node = xmlDoc.SelectSingleNode("/XmlConfig/" + xpath);
            if (node == null) return null;
            return node.InnerText;
        }

        //递归根据xpath的方式进行创建节点
        static private XmlNode makeXPath(XmlDocument doc, XmlNode parent, string xpath)
        {
            //在XPath抓住下一个节点的名称；父级如果是空的则返回
            string[] partsOfXPath = xpath.Trim('/').Split('/');
            string nextNodeInXPath = partsOfXPath.First();
            if (string.IsNullOrEmpty(nextNodeInXPath)) return parent;

            //获取或从名称创建节点
            XmlNode node = parent.SelectSingleNode(nextNodeInXPath);
            if (node == null) node = parent.AppendChild(doc.CreateElement(nextNodeInXPath));

            //加入的阵列作为一个XPath表达式和递归余数
            string rest = String.Join("/", partsOfXPath.Skip(1).ToArray());
            return makeXPath(doc, node, rest);
        }
    }
}

