<%@ Control CodeBehind="$relatedmodule$.ascx.cs" Language="c#" AutoEventWireup="false"
    Inherits="SplendidCRM.$modulename$.$relatedmodule$" %>
<script type="text/javascript">
    function $relatedmodulesingular$Popup() {
        return ModulePopup('$relatedmodule$', '<%= txt$relatedtablesingular$_ID.ClientID %>', null, 'ClearDisabled=1', true, null);
    }
</script>
<input id="txt$relatedtablesingular$_ID" type="hidden" runat="server" />
<%@ Register TagPrefix="SplendidCRM" TagName="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="div$modulename$$relatedmodule$" Title="$relatedmodule$.LBL_MODULE_NAME"
    runat="Server" />
<div id="div$modulename$$relatedmodule$" style='<%= "display:" + (CookieValue("div$modulename$$relatedmodule$") != "1" ? "inline": "none") %>'>
    <%@ register tagprefix="SplendidCRM" tagname="DynamicButtons" src="~/_controls/DynamicButtons.ascx" %>
    <SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" runat="Server" />
    <asp:Panel ID="pnlNewRecordInline" Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) %>'
        Style="display: none" runat="server">
        <%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/$relatedmodule$/NewRecord.ascx" %>
        <SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="EditView.Inline"
            ShowCancel="true" ShowHeader="false" ShowFullForm="false" ShowTopButtons="true"
            Runat="Server" />
    </asp:Panel>
    <%@ register tagprefix="SplendidCRM" tagname="SearchView" src="~/_controls/SearchView.ascx" %>
    <SplendidCRM:SearchView ID="ctlSearchView" Module="$relatedmodule$" SearchMode="SearchSubpanel"
        IsSubpanelSearch="true" ShowSearchTabs="false" ShowDuplicateSearch="false" ShowSearchViews="false"
        Visible="false" runat="Server" />
    <SplendidCRM:SplendidGrid ID="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>"
        EnableViewState="true" runat="server">
        <Columns>
            <asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left"
                ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:ImageButton Visible='<%# !bEditView && SplendidCRM.Security.GetUserAccess("$relatedmodule$", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>'
                        CommandName="$relatedmodule$.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "$relatedtablesingular$_ID") %>'
                        OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>'
                        SkinID="edit_inline" runat="server" />
                    <asp:LinkButton Visible='<%# !bEditView && SplendidCRM.Security.GetUserAccess("$relatedmodule$", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>'
                        CommandName="$relatedmodule$.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "$relatedtablesingular$_ID") %>'
                        OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>'
                        runat="server" />
                    &nbsp; <span onclick="return confirm('<%= L10n.TermJavaScript("$modulename$.NTC_REMOVE_$tablenamesingular$_CONFIRMATION") %>')">
                        <asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "$tablenamesingular$_ASSIGNED_USER_ID"))) >= 0 %>'
                            CommandName="$relatedmodule$.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "$relatedtablesingular$_ID") %>'
                            OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>'
                            SkinID="delete_inline" runat="server" />
                        <asp:LinkButton Visible='<%# SplendidCRM.Security.GetUserAccess("$modulename$", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "$tablenamesingular$_ASSIGNED_USER_ID"))) >= 0 %>'
                            CommandName="$relatedmodule$.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "$relatedtablesingular$_ID") %>'
                            OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>'
                            runat="server" />
                    </span>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </SplendidCRM:SplendidGrid>
</div>
