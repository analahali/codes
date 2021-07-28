<%@ Page Language="c#" MasterPageFile="~/PopupView.Master" AutoEventWireup="false"
    Inherits="SplendidCRM.SplendidPopup" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
    <%@ register tagprefix="SplendidCRM" tagname="PopupView" src="PopupView.ascx" %>
    <SplendidCRM:PopupView ID="ctlPopupView" MultiSelect="true" Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "list") >= 0 %>'
        Runat="Server" />
    <asp:Label ID="lblAccessError" ForeColor="Red" EnableViewState="false" Text='<%# L10n.Term("ACL.LBL_NO_ACCESS") %>'
        Visible="<%# !ctlPopupView.Visible %>" runat="server" />
</asp:Content>
