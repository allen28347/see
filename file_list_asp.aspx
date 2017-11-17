<%@ Page Language="C#" AutoEventWireup="true" CodeFile="file_list_asp.aspx.cs" Inherits="file_list_asp" %>

<!DOCTYPE html>
<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>簡易式檔案伺服器</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>檔案清單</h2>
        <asp:FileUpload ID="FileUpload" Text="上傳檔案" runat="server" AllowMultiple="True"/>
        <asp:Button ID="upload" runat="server" Text="上傳檔案" OnClick="upload_Click" />&nbsp;&nbsp;
        資料及名稱:<asp:TextBox ID="folderName" runat="server"></asp:TextBox><asp:Button ID="createFolder" runat="server" Text="建立資料夾" OnClick="createFolder_Click" />
        <asp:Panel ID="messageBox" GroupingText="確認" runat="server" Visible="False" BackColor="#FFFFCC">
            <center>
            <asp:Label ID="message" runat="server" Text=""></asp:Label><br/>
            <asp:Button ID="ok" runat="server" Text="確定" OnClick="ok_Click" />&nbsp;&nbsp;
            <asp:Button ID="cancel" runat="server" Text="取消" OnClick="cancel_Click" />
            </center>
        </asp:Panel>
        <asp:Panel ID="Panel" runat="server">
            <asp:Button ID="upFolder" runat="server" Text="上一層" OnClick="upFolder_Click" />
        </asp:Panel>
    </form>
</body>
</html>
