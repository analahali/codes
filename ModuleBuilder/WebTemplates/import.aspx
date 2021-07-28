<%@ Page Language="c#" MasterPageFile="~/DefaultView.Master" CodeBehind="import.aspx.cs"
    AutoEventWireup="false" Inherits="SplendidCRM.SplendidPage" %>

<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="Shortcuts" src="~/_controls/Shortcuts.ascx" %>
    <SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="$modulename$" runat="Server" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="ImportView" src="~/Import/ImportView.ascx" %>
    <SplendidCRM:ImportView ID="ctlImportView" Module="$modulename$" Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "import") >= 0 %>'
        runat="Server" />
    <asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>'
        Visible="<%# !ctlImportView.Visible %>" runat="server" />
</asp:Content>
