<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.$modulename$.ListView" %>
<div id="divListView">
    <%@ register tagprefix="SplendidCRM" tagname="ModuleHeader" src="~/_controls/ModuleHeader.ascx" %>
    <SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="$modulename$" Title=".moduleList.Home"
        EnablePrint="true" HelpName="index" EnableHelp="true" runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="SearchView" src="~/_controls/SearchView.ascx" %>
    <SplendidCRM:SearchView ID="ctlSearchView" Module="$modulename$" Visible="<%# !PrintView %>"
        runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="ExportHeader" src="~/_controls/ExportHeader.ascx" %>
    <SplendidCRM:ExportHeader ID="ctlExportHeader" Module="$modulename$" Title="$modulename$.LBL_LIST_FORM_TITLE"
        runat="Server" />
    <asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
        <asp:Label ID="lblError" CssClass="error" EnableViewState="false" runat="server" />
    </asp:Panel>
    <asp:HiddenField ID="LAYOUT_LIST_VIEW" runat="server" />
    <SplendidCRM:SplendidGrid ID="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>"
        EnableViewState="true" runat="server">
        <Columns>
            <asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
                <ItemTemplate>
                    <%# grdMain.InputCheckbox(!PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE), ctlCheckAll.FieldName, Sql.ToGuid(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center"
                ItemStyle-Wrap="false">
                <ItemTemplate>
                   <%-- <asp:HyperLink onclick=<%# "return SplendidCRM_ChangeFavorites(this, \'" + m_sMODULE + "\', \'" + Sql.ToString(Eval("ID")) + "\')" %>
                        runat="server">
						<asp:Image name='<%# "favAdd_" + Sql.ToString(Eval("ID")) %>' SkinID="favorites_add"    style='<%# "display:" + ( Sql.IsEmptyGuid(Eval("FAVORITE_RECORD_ID")) ? "inline" : "none") %>' ToolTip='<%# L10n.Term(".LBL_ADD_TO_FAVORITES"     ) %>' Runat="server" />
						<asp:Image name='<%# "favRem_" + Sql.ToString(Eval("ID")) %>' SkinID="favorites_remove" style='<%# "display:" + (!Sql.IsEmptyGuid(Eval("FAVORITE_RECORD_ID")) ? "inline" : "none") %>' ToolTip='<%# L10n.Term(".LBL_REMOVE_FROM_FAVORITES") %>' Runat="server" />
                    </asp:HyperLink>--%>
                    <asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit", Sql.ToGuid(Eval("ASSIGNED_USER_ID"))) >= 0 %>'
                        NavigateUrl='<%# "~/" + m_sMODULE + "/edit.aspx?id=" + Eval("ID") %>' ToolTip='<%# L10n.Term(".LNK_EDIT") %>'
                        runat="server">
						<asp:Image SkinID="edit_inline" Runat="server" />
                    </asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </SplendidCRM:SplendidGrid>
    <%@ register tagprefix="SplendidCRM" tagname="CheckAll" src="~/_controls/CheckAll.ascx" %>
    <SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && !IsMobile && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE) %>"
        runat="Server" />
    
    <%@ register tagprefix="SplendidCRM" tagname="DumpSQL" src="~/_controls/DumpSQL.ascx" %>
    <SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" runat="Server" />
</div>
