<%@ Page Language="c#" MasterPageFile="~/DefaultView.Master" AutoEventWireup="false"
    Inherits="SplendidCRM.SplendidPage" %>

<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="Shortcuts" src="~/_controls/Shortcuts.ascx" %>
    <SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="$modulename$" runat="Server" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="ListView" src="ListView.ascx" %>
    <SplendidCRM:ListView ID="ctlListView" Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "list") >= 0 %>'
        Runat="Server" />
    <asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>'
        Visible="<%# !ctlListView.Visible %>" runat="server" />
</asp:Content>
