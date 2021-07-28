<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ModuleBuilder.Default" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
    <%@ Register TagPrefix="SplendidCRM" Tagname="ListView" Src="ListView.ascx" %>
    <SplendidCRM:ListView ID="ctlListView" Runat="Server" />
    <asp:Label ID="lblAccessError" Text='<%# L10n.Term(".LBL_UNAUTH_ADMIN") %>' Visible='<%# !ctlListView.Visible %>' CssClass="error" EnableViewState="false" Runat="Server" />
    <asp:Label ID="lblBuilderDisabled" Text='<%# L10n.Term("ModuleBuilder.LBL_MODULE_BUILDER_DISABLED") %>' Visible='<%# Sql.ToBoolean(Utils.AppSettings["DisableModuleBuilder"]) %>' CssClass="error" EnableViewState="false" Runat="Server" />
</asp:Content>