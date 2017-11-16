using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class file_list_asp : Page
{
    string fn;
    string folderPath {
        get { return fn != "/" ? fn : ""; }
        set { fn = value != @"\" ? value : "/"; }
    }
    static class ActionMode { public const int DELETE_FILE = 0, DELETE_FOLDER = 1; };
    const string rootPath = "file";
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["path"] != null){
            if(Regex.IsMatch(Request.QueryString["path"],"^$|^\\s+$")){
                folderPath="/";
            }else if(Regex.IsMatch(Request.QueryString["path"],"^.*\\.\\..*$")){
                message.Text = "路徑中不允許包含'..'";
                messageBox.GroupingText="錯誤";
                messageBox.Visible = true;
                folderPath="/";
            }else if(!new DirectoryInfo(Server.MapPath("~") + $"\\{rootPath}\\{Request.QueryString["path"]}").Exists){
                message.Text = $"路徑'{Request.QueryString["path"]}'不存在";
                messageBox.GroupingText="錯誤";
                messageBox.Visible = true;
                folderPath="/";
            }else
                folderPath=Request.QueryString["path"];
        }else
            folderPath="/";
        if (fn == "/")
            upFolder.Visible = false;
        if (!IsPostBack) {
            Panel.GroupingText += $"<h3 > 路徑: {fn} </h3>";
        }
        Table fileList = new Table();
        TableRow tableRow = new TableRow();
        TableCell tableCell = new TableCell();
        Label tableTitle = new Label();
        tableTitle.Text = "檔案名稱";
        tableTitle.Style.Value = "font-weight:800;";
        tableCell.Controls.Add(tableTitle);
        tableRow.Cells.Add(tableCell);
        fileList.Rows.Add(tableRow);
        DirectoryInfo DI = new DirectoryInfo(Server.MapPath("~") + @"\" + rootPath + folderPath);
        if (DI.GetDirectories().Length == 0 && DI.GetFiles().Length == 0)
        {
            tableRow = new TableRow();
            tableCell = new TableCell();
            tableTitle = new Label();
            tableTitle.Text = "目錄是空的";
            tableCell.Controls.Add(tableTitle);
            tableRow.Cells.Add(tableCell);
            fileList.Rows.Add(tableRow);
        }
        foreach (DirectoryInfo di in DI.GetDirectories())
        {
            tableRow = new TableRow();
            tableCell = new TableCell();
            HyperLink directoryLink = new HyperLink();
            directoryLink.Text = di.Name;
            directoryLink.NavigateUrl = $"{HttpRuntime.AppDomainAppVirtualPath}/file_list_asp.aspx?path={folderPath}/{di.Name}";
            tableCell.Controls.Add(directoryLink);
            Button deleteFolderButton = new Button();
            deleteFolderButton.Text = "刪除";
            deleteFolderButton.Click += DeleteFolderButton_Click;
            deleteFolderButton.Attributes.Add("directoryName", di.Name);
            tableCell.Controls.Add(deleteFolderButton);
            tableRow.Cells.Add(tableCell);
            fileList.Rows.Add(tableRow);
        }
        foreach (FileInfo FI in DI.GetFiles())
        {
            tableRow = new TableRow();
            tableCell = new TableCell();
            HyperLink fileLink = new HyperLink();
            fileLink.Text = FI.Name;
            fileLink.NavigateUrl = $"{HttpRuntime.AppDomainAppVirtualPath}/{rootPath}{folderPath}/{FI.Name}";
            fileLink.Attributes.Add("target", "_blank");
            tableCell.Controls.Add(fileLink);
            Button downloadButton = new Button();
            downloadButton.Text = "下載";
            downloadButton.Click += DownloadButton_Click;
            downloadButton.Attributes.Add("fileName", FI.Name);
            tableCell.Controls.Add(downloadButton);
            Button deleteButton = new Button();
            deleteButton.Text = "刪除";
            deleteButton.Click += DeleteButton_Click;
            deleteButton.Attributes.Add("fileName", FI.Name);
            tableCell.Controls.Add(deleteButton);
            tableRow.Cells.Add(tableCell);
            fileList.Rows.Add(tableRow);
        }
        Panel.Controls.Add(fileList);
    }

    private void DeleteFolderButton_Click(object sender, EventArgs e)
    {
        Button deleteButton = (Button)sender;
        message.Text = $"確定要刪除資料夾'{deleteButton.Attributes["directoryName"]}'?";
        messageBox.GroupingText="確認";
        messageBox.Attributes.Add("directoryName", deleteButton.Attributes["directoryName"]);
        messageBox.Attributes.Add("ActionMode", ActionMode.DELETE_FOLDER.ToString());
        messageBox.Visible = true;
    }
    private void DeleteFolder(DirectoryInfo directory)
    {
        DirectoryInfo[] subDirectory = directory.GetDirectories();
        FileInfo[] file = directory.GetFiles();
        foreach (DirectoryInfo d in subDirectory)
            DeleteFolder(d);
        foreach (FileInfo f in file)
            f.Delete();
        directory.Delete();
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        Button deleteButton = (Button)sender;
        message.Text = $"確定要刪除檔案'{deleteButton.Attributes["fileName"]}'?";
        messageBox.GroupingText="確認";
        messageBox.Attributes.Add("fileName", deleteButton.Attributes["fileName"]);
        messageBox.Attributes.Add("ActionMode", ActionMode.DELETE_FILE.ToString());
        messageBox.Visible = true;
    }

    private void DownloadButton_Click(object sender, EventArgs e)
    {
        Button downloadButton = (Button)sender;
        Response.AddHeader("Content-Disposition", $"attachment;filename={downloadButton.Attributes["fileName"]}");
        using (FileStream FS = new FileStream(HttpRuntime.AppDomainAppPath + $"\\{rootPath}{folderPath}\\" + downloadButton.Attributes["fileName"], FileMode.Open))
        {
            byte[] b = new byte[FS.Length];
            int count, sum = 0;
            while ((count = FS.Read(b, sum, b.Length - sum)) > 0)
                sum += count;
            Response.BinaryWrite(b);
        }
        Response.End();
    }

    protected void upload_Click(object sender, EventArgs e)
    {
        if (FileUpload.HasFiles)
        {
            foreach (HttpPostedFile upFile in FileUpload.PostedFiles)
                upFile.SaveAs($"{HttpRuntime.AppDomainAppPath}{rootPath}{folderPath}\\{upFile.FileName}");
        }
        Response.Redirect(Request.Url.ToString());
    }

    protected void upFolder_Click(object sender, EventArgs e)
    {
        DirectoryInfo folder = new DirectoryInfo(Server.MapPath("~") + @"\" +rootPath + folderPath);
        Response.Redirect(
            $"{HttpRuntime.AppDomainAppVirtualPath}/file_list_asp.aspx?path={new Regex($"^(.*?)[/\\\\]{Regex.Escape(folder.Name)}[/\\\\]?$").Replace(folderPath,"$1")}");
    }

    protected void createFolder_Click(object sender, EventArgs e)
    {
        if (!Regex.IsMatch(folderName.Text, "^$|^\\s+$"))
        {
            try
            {
                new DirectoryInfo($"{HttpRuntime.AppDomainAppPath}{rootPath}{folderPath}\\{folderName.Text}").Create();
                Response.Redirect(Request.Url.ToString());
            }catch(Exception err)
            {
                message.Text = $"資料夾建立失敗，{err.Message}";
                messageBox.GroupingText = "錯誤";
                messageBox.Visible = true;
            }
        }else{
            message.Text = "資料夾名稱不可以為空白";
            messageBox.GroupingText="錯誤";
            messageBox.Visible = true;
        }
    }

    protected void ok_Click(object sender, EventArgs e)
    {
        if (messageBox.Attributes["ActionMode"] !=null)
            switch (Convert.ToInt32(messageBox.Attributes["ActionMode"]))
            {
                case ActionMode.DELETE_FILE:
                    FileInfo file = new FileInfo(HttpRuntime.AppDomainAppPath + $"\\{rootPath}{folderPath}\\" + messageBox.Attributes["fileName"]);
                    try
                    {
                        file.Delete();
                        Response.Redirect(Request.Url.ToString());
                    }
                    catch (Exception err){
                        message.Text = $"檔案刪除失敗，{err.Message}";
                        messageBox.GroupingText = "錯誤";
                        messageBox.Visible = true;
                    }
                    break;
                case ActionMode.DELETE_FOLDER:
                    DirectoryInfo directory = new DirectoryInfo(HttpRuntime.AppDomainAppPath + $"\\{rootPath}{folderPath}\\" + messageBox.Attributes["directoryName"]);
                    try
                    {
                        DeleteFolder(directory);
                        Response.Redirect(Request.Url.ToString());
                    }
                    catch (Exception err)
                    {
                        message.Text = $"資料夾刪除失敗，{err.Message}";
                        messageBox.GroupingText = "錯誤";
                        messageBox.Visible = true;
                    }
                    break;
            }
        messageBox.Visible = false;
    }

    protected void cancel_Click(object sender, EventArgs e)
    {
        messageBox.Visible = false;
    }
}