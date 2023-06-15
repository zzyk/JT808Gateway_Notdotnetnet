using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JT808.Gateway.Client
{
    /// <summary>
    /// JT808报表选项
    /// </summary>
    public class JT808ReportOptions
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = $"JT808Report.{DateTime.Now.ToString("yyyyMMddHHssmm")}.txt";
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 全路径
        /// </summary>
        public string FileFullPath { get { return Path.Combine(FilePath, FileName); } }
        /// <summary>
        /// 间隔
        /// </summary>
        public int Interval { get; set; } = 3;
        public void FileExistsAndCreate()
        {
            if(!File.Exists(FileFullPath))
            {
                File.Create(FileFullPath).Close();
            }
        }
    }
}
