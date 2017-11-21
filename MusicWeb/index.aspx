<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="MusicWeb.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="gvAlbums" runat="server"></asp:GridView>
    </div>
        <div>
            <asp:GridView ID="gvReturnDataJsonFormat" runat="server"></asp:GridView>
        </div>
    </form>
</body>
</html>
