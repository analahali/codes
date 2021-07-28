<%@ Control CodeBehind="PopupView.ascx.cs" Language="c#" AutoEventWireup="false"
    Inherits="SplendidCRM.$modulename$.PopupView" %>
<div id="divPopupView">
    <%@ register tagprefix="SplendidCRM" tagname="SearchView" src="~/_controls/SearchView.ascx" %>
    <SplendidCRM:SearchView ID="ctlSearchView" Module="$modulename$" IsPopupSearch="true"
        ShowSearchTabs="false" Visible="<%# !PrintView %>" runat="Server" />
    <script type="text/javascript">
        function Select$modulenamesingular$(sPARENT_ID, sPARENT_NAME) {
            if (window.opener != null && window.opener.Change$modulenamesingular$ != null) {
                window.opener.Change$modulenamesingular$(sPARENT_ID, sPARENT_NAME);
                window.close();
                window.opener.OnSelectFromPopup('$modulename$');
            }
            else {
                alert('Original window has closed.  $modulenamesingular$ cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
            }
        }
        function SelectChecked() {
            if (window.opener != null && window.opener.Change$modulenamesingular$ != null) {
                var sSelectedItems = document.getElementById('<%= ctlCheckAll.SelectedItems.ClientID %>').value;
                window.opener.Change$modulenamesingular$(sSelectedItems, '');
                window.close();
            }
            else {
                alert('Original window has closed.  $modulenamesingular$ cannot be assigned.');
            }
        }
        function Clear() {
            if (window.opener != null && window.opener.Change$modulenamesingular$ != null) {
                window.opener.Change$modulenamesingular$('', '');
                window.close();
            }
            else {
                alert('Original window has closed.  $modulenamesingular$ cannot be assigned.');
            }
        }
        function Cancel() {
            window.close();
        }
    </script>
    <%@ register tagprefix="SplendidCRM" tagname="ListHeader" src="~/_controls/ListHeader.ascx" %>
    <SplendidCRM:ListHeader Title='<%# m_sMODULE + ".LBL_LIST_FORM_TITLE" %>' runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="DynamicButtons" src="~/_controls/DynamicButtons.ascx" %>
    <SplendidCRM:DynamicButtons ID="ctlDynamicButtons" runat="Server" />
    <asp:UpdatePanel Visible='<%# !Sql.ToBoolean(Application["CONFIG.disable_popupview_inline"]) %>'
        runat="server">
        <ContentTemplate>
            <asp:Button ID="btnCreateInline" CommandName="NewRecord.Show" OnCommand="Page_Command"
                Text='<%# L10n.Term(m_sMODULE + ".LNK_NEW_$tablenamesingular$") %>' CssClass="button"
                Style="margin-bottom: 4px;" Visible="<%# !this.IsMobile %>" runat="server" />
            <asp:Panel ID="pnlNewRecordInline" Style="display: none" runat="server">
             <%--   <%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="NewRecord.ascx" %>
                <SplendidCRM:NewRecord ID="ctlNewRecord" Width="100%" EditView="PopupView.Inline"
                    ShowCancel="true" Runat="Server" />--%>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <SplendidCRM:SplendidGrid ID="grdMain" SkinID="grdPopupView" EnableViewState="true"
        runat="server">
        <Columns>
            <asp:TemplateColumn HeaderText="" ItemStyle-Width="2%">
                <ItemTemplate>
                    <%# grdMain.InputCheckbox(!PrintView && bMultiSelect, ctlCheckAll.FieldName, Sql.ToGuid(Eval("ID")), ctlCheckAll.SelectedItems) %></ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </SplendidCRM:SplendidGrid>
    <%@ register tagprefix="SplendidCRM" tagname="CheckAll" src="~/_controls/CheckAll.ascx" %>
    <SplendidCRM:CheckAll ID="ctlCheckAll" Visible="<%# !PrintView && bMultiSelect %>"
        runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="DumpSQL" src="~/_controls/DumpSQL.ascx" %>
    <SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" runat="Server" />
</div>
