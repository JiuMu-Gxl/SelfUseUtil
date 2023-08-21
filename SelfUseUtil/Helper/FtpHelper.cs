using Antlr4.Runtime.Atn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    /// <summary>
    /// FTP操作工具类
    /// </summary>
    public class FtpHelper
    {
        private string ftpServer { get; set; } = "ftp://10.22.3.157:21/";
        private string username { get; set; } = "LocalFtp";
        private string password { get; set; } = "123456";
        private string remotePath { get; set; } = "path/to/your/multilevel/folder/";

        public FtpHelper()
        {

        }

        public FtpHelper(string ftpServer, string username, string password, string remotePath)
        {
            this.ftpServer = ftpServer;
            this.username = username;
            this.password = password;
            this.remotePath = remotePath;
        }

        /// <summary>
        /// FTP创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public void CreateFolders(string path = "") {
            if (!string.IsNullOrEmpty(path)) remotePath = path;
            // 组合完整的FTP路径
            string fullRemotePath = ftpServer + remotePath;
            Console.WriteLine($"完整FTP路径：{fullRemotePath}");

            string[] folders = remotePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = ftpServer;

            foreach (string folder in folders)
            {
                currentPath += "/" + folder;
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(currentPath);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    request.Credentials = new NetworkCredential(username, password);

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine($"Folder {folder} created successfully.");
                    }
                }
                catch (WebException ex)
                {
                    if (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        Console.WriteLine($"Folder {folder} already exists.");
                    }
                    else
                    {
                        Console.WriteLine($"Error creating folder {folder}: {ex.Message}");
                    }
                }
            }
        }

        public bool UploadFile(byte[] fileByte, ref string erroinfo, string path = "")
        {
            if (!string.IsNullOrEmpty(path)) remotePath = path;
            // 组合完整的FTP路径
            string fullRemotePath = ftpServer + remotePath;
            Console.WriteLine($"完整FTP路径：{fullRemotePath}");
            FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(fullRemotePath));
            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(username, password);
            reqFtp.KeepAlive = false;
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            reqFtp.ContentLength = fileByte.Length;

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int currentIndex = 0;
            int totalLength = fileByte.Length;
            try
            {
                Stream strm = reqFtp.GetRequestStream();
                while (totalLength - currentIndex > 0)
                {
                    int currentLength = totalLength - currentIndex > buffLength
                        ? buffLength
                        : totalLength - currentIndex;
                    Array.Copy(fileByte, currentIndex, buff, 0, currentLength);
                    strm.Write(buff, 0, currentLength);
                    currentIndex += currentLength;
                }

                strm.Close();
                erroinfo = "完成";
                return true;
            }
            catch (Exception ex)
            {
                erroinfo += ex.Message;
                return false;
            }
        }

    }
}
