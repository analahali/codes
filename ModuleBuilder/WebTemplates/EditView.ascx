<%@ Control Language="c#" AutoEventWireup="false" CodeBehind="EditView.ascx.cs" Inherits="SplendidCRM.$modulename$.EditView"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<div id="divEditView">
    <%@ register tagprefix="SplendidCRM" tagname="ModuleHeader" src="~/_controls/ModuleHeader.ascx" %>
    <SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="$modulename$" EnablePrint="false"
        HelpName="EditView" EnableHelp="true" runat="Server" />
    <p>
        <%@ register tagprefix="SplendidCRM" tagname="DynamicButtons" src="~/_controls/DynamicButtons.ascx" %>
        <SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" ShowRequired="true"
            runat="Server" />
        <asp:HiddenField ID="LAYOUT_EDIT_VIEW" runat="server" />
        <asp:Table ID="Table5" SkinID="tabForm" runat="server">
            <asp:TableRow>
                <asp:TableCell>
                    <table id="tblMain" class="tabEditView" runat="server">
                      
                    </table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </p>
    <div id="divEditSubPanel">
        <asp:PlaceHolder ID="plcSubPanel" runat="server" />
    </div>
    <SplendidCRM:DynamicButtons ID="ctlFooterButtons" Visible="<%# !PrintView %>" ShowRequired="false"
        runat="Server" />
</div>
<%@ Register TagPrefix="SplendidCRM" TagName="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" runat="Server" />
<div style="visibility: hidden">
   
</div>
<script type="text/javascript">
    function OnSelectFromPopup(module) {
    
    }
</script>
