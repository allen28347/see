using System;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class file_download : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["path"] != null && Request["name"] != null) {
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Request["name"]);
            using (FileStream FS = new FileStream(HttpRuntime.AppDomainAppPath + @"\" + Request["path"], FileMode.Open))
            {
                byte[] b = new byte[FS.Length];
                int count,sum=0;
                while ((count = FS.Read(b, sum, b.Length - sum)) > 0)
                    sum += count; 
                Response.BinaryWrite(b);
            }
            Response.End();
        }
    }
}