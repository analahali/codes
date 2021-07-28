<%@ Page Language="c#" MasterPageFile="~/DefaultView.Master" AutoEventWireup="false"
    Inherits="SplendidCRM.SplendidPage" %>

<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="Shortcuts" src="~/_controls/Shortcuts.ascx" %>
    <SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="$modulename$" runat="Server" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="EditView" src="EditView.ascx" %>
    <SplendidCRM:EditView ID="ctlEditView" Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "edit") >= 0 %>'
        Runat="Server" />
    <asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>'
        Visible="<%# !ctlEditView.Visible %>" runat="server" />
</asp:Content>
