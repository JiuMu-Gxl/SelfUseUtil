using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SelfUseUtil.Helper
{
    public static class XmlHelper
    {
        /// <summary>
        /// 获取所有子元素
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static List<XmlElement> GetXmlNode(string xmlStr) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            //获取全部节点
            return GetAllNodes(xmlDoc.DocumentElement.ChildNodes, new List<XmlElement>());
        }

        /// <summary>
        /// 根据属性值获取同级节点的内容
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <param name="peerNodeName"></param>
        public static void GetSingleNodePeerValueByAttribute(
            string xmlStr, string nodeName, 
            string attributeName, string attributeValue, 
            string peerNodeName)
        {
            // 解析 XML 字符串为 XDocument 对象
            XDocument xmlDoc = XDocument.Parse(xmlStr);

            // 查找出院诊断同级的 text 元素
            XElement outDiagnosisElement = xmlDoc.Descendants(nodeName)
                .FirstOrDefault(element => (string)element.Attribute(attributeName) == attributeValue);

            if (outDiagnosisElement != null)
            {
                XElement siblingTextElement = outDiagnosisElement.ElementsAfterSelf(peerNodeName).FirstOrDefault();
                if (siblingTextElement != null)
                {
                    string siblingText = (string)siblingTextElement;
                    Console.WriteLine("查询属性节点同级的文本：" + siblingText.Trim());
                    Console.WriteLine("查询属性节点同级完整信息" + siblingTextElement.ToString());
                }
                else
                {
                    Console.WriteLine("未找到查询属性节点同级的文本");
                }
            }
            else
            {
                Console.WriteLine("未找到查询属性节点");
            }
        }

        /// <summary>
        /// 获取查询节点内容
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <param name="nodeName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public static void GetSingleNodeByAttribute(string xmlStr, string nodeName, string attributeName, string attributeValue)
        {
            // 解析 XML 字符串为 XDocument 对象
            XDocument xmlDoc = XDocument.Parse(xmlStr);

            // 查找出院诊断同级的 text 元素
            XElement outDiagnosisElement = xmlDoc.Descendants(nodeName)
                .FirstOrDefault(element => (string)element.Attribute(attributeName) == attributeValue);

            if (outDiagnosisElement != null)
            {
                string siblingText = (string)outDiagnosisElement;
                Console.WriteLine("查询属性节点内容：" + siblingText.Trim());
                Console.WriteLine("查询属性节点完整信息" + outDiagnosisElement.ToString());
            }
            else
            {
                Console.WriteLine("未找到此查询属性节点");
            }
        }

        /// <summary>
        /// 根据节点名称获取节点信息
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <param name="nodeName"></param>
        public static void GetSingleNode(string xmlStr, string nodeName)
        {
            // 解析 XML 字符串为 XDocument 对象
            XDocument xmlDoc = XDocument.Parse(xmlStr);
            XElement outDiagnosisElement = xmlDoc.Descendants(nodeName).FirstOrDefault();
            if (outDiagnosisElement != null)
            {
                string siblingText = (string)outDiagnosisElement;
                Console.WriteLine("当前节点内容：" + siblingText.Trim()); 
                Console.WriteLine("当前节点完整信息" + outDiagnosisElement.ToString());
            }
            else
            {
                Console.WriteLine("未找到此节点");
            }
        }

        /// <summary>
        /// 递归获取当前节点下所有子节点
        /// </summary>
        /// <param name="nodelist"></param>
        /// <param name="listnode"></param>
        /// <returns></returns>
        private static List<XmlElement> GetAllNodes(XmlNodeList nodelist, List<XmlElement> listnode)
        {
            foreach (XmlElement element in nodelist)
            {
                //如果这个节点没有出现过，则添加到list列表
                if (!listnode.Any(a => a.Name == element.Name))
                {
                    listnode.Add(element);
                }

                if (element.ChildNodes[0] is XmlText)
                {
                    continue;
                }
                else
                {
                    GetAllNodes(element.ChildNodes, listnode);
                }
            }

            return listnode;
        }

        /// <summary>
        /// 创建xml例子
        /// </summary>
        public static void CreadXml() {
            XElement requestElement = new XElement("Request",
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XElement("From", "ORG.JH.EMR.KEQZYZQRM"),
                new XElement("FromVersion", "1.0.0.0"),
                new XElement("To", "ORG.BJCA.MMR"),
                new XElement("ToVersion", "3.1.1.0"),
                new XElement("Version", "3.1.1.0"),
                new XElement("Identification", "121505224606901452-124434-2-3-0-1"),
                new XElement("Date", "2018-10-23"),
                new XElement("Time", "13:59:35"),
                new XElement("Count", "1"),
                new XElement("ArchiveType", "ANNOUNCE")
            );

            XElement informationElement = new XElement("Information",
                 new XElement("Patient",
                    new XElement("PatientName", "庞学义"),
                    new XElement("MedRecordNo", "2000021250"),
                    new XElement("RegNo", "20000212503"),
                    new XElement("PatientID", "124434")
                ),
                new XElement("Episode",
                    new XElement("EpisodeID", "124434"),
                    new XElement("AdmDate", "2023-08-03"),
                    new XElement("DisDate", "2023-08-05"),
                    new XElement("AdmType", "3"),
                    new XElement("AdmNum", "3")
                )
            );
            var documents = new XElement("Documents");
            var count = 5;
            var docTtal = new XElement("DocTotal", count);
            documents.Add(docTtal);

            for (int i = 0; i < count; i++)
            {
                var document = GetDocument();
                documents.Add(document);
            }
            informationElement.Add(documents);
            requestElement.Add(informationElement);
            XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", null), requestElement);

            Console.WriteLine(string.Format(xmlDocument.ToString()));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlDocument.Save(memoryStream);
                // 将MemoryStream的内容转换为byte数组
                byte[] byteArray = memoryStream.ToArray();
            }

            xmlDocument.Save("output.xml");
            Console.WriteLine("XML 文件创建成功！");

            XElement GetDocument()
            {
                return new XElement("Document",
                    new XElement("DocIndex", 1),
                    new XElement("DocTotal", 0),
                    new XElement("DocItemCode", "RYJL.0001"),
                    new XElement("DocUniqueID", "12443431"),
                    new XElement("DocTitle", "病历文书"),
                    new XElement("DocDesc", "入院记录-通用"),
                    new XElement("DocFormat", "application/pdf"),
                    new XElement("ApplyNumber"),
                    new XElement("Late", "N"),
                    new XElement("DocFile", "ftp://172.16.10.80:21/ftp/2023/08/05/MEI/124434/入院记录_124434_3_121505224606901452-124434-2-3-0-1.pdf"),
                    new XElement("DocXML", "ftp://172.16.10.80:21/ftp/2023/08/05/MEI/124434/入院记录_124434_3_121505224606901452-124434-2-3-0-1.xml")
                );
            }
        }

    }
}
