﻿/****************************************************************************
*Copyright (c) 2018 Microsoft All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：Microsoft
*命名空间：SAEA.MVC.Mvc
*文件名： FileResult
*版本号： V2.1.5.0
*唯一标识：10362e3c-aeb1-4282-a3b2-69774773af9e
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/4/16 13:16:32
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/4/16 13:16:32
*修改人： yswenli
*版本号： V2.1.5.0
*描述：
*
*****************************************************************************/
using SAEA.BaseLibs.MVC.Http.Base;
using SAEA.MVC.Http.Base;
using System.IO;
using System.Net;
using System.Text;

namespace SAEA.MVC.Mvc
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public class FileResult : ActionResult
    {
        public new byte[] Content { get; set; }

        public FileResult(string filePath) : this(filePath, HttpMIME.GetType(filePath))
        {

        }

        /// <summary>
        /// 文件内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <param name="status"></param>
        public FileResult(string filePath, string contentType = "", HttpStatusCode status = HttpStatusCode.OK)
        {
            if (SAEAMvcApplication.IsStaticsCached)
            {
                var data = StaticResourcesCache.GetOrAdd(filePath, (k) =>
                {
                    using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Position = 0;
                        fs.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                });
                this.Content = data;
            }
            else
            {
                byte[] data = null;
                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[fs.Length];
                    fs.Position = 0;
                    fs.Read(buffer, 0, buffer.Length);
                    data = buffer;
                }
                this.Content = data;
            }
            this.ContentEncoding = Encoding.UTF8;
            this.ContentType = contentType;
            this.Status = status;
        }
    }
}
