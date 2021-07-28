<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="DetailView.ascx.cs"
    Inherits="SplendidCRM.$modulename$.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<div id="divDetailView">
    <%@ register tagprefix="SplendidCRM" tagname="ModuleHeader" src="~/_controls/ModuleHeader.ascx" %>
    <SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="$modulename$" EnablePrint="true"
        HelpName="DetailView" EnableHelp="true" EnableFavorites="true" runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="DynamicButtons" src="~/_controls/DynamicButtons.ascx" %>
    <SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" runat="Server" />
    <%@ register tagprefix="SplendidCRM" tagname="DetailNavigation" src="~/_controls/DetailNavigation.ascx" %>
    <SplendidCRM:DetailNavigation ID="ctlDetailNavigation" Module="$modulename$" Visible="<%# !PrintView %>"
        runat="Server" />
    <asp:HiddenField ID="LAYOUT_DETAIL_VIEW" runat="server" />
    <asp:Table ID="Table5" SkinID="tabForm" runat="server">
        <asp:TableRow>
            <asp:TableCell>
                <table id="tblMain" class="tabEditView" runat="server">
                </table>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <div id="divDetailSubPanel">
        <asp:PlaceHolder ID="plcSubPanel" runat="server" />
    </div>
</div>
<%@ Register TagPrefix="SplendidCRM" TagName="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" runat="Server" />
