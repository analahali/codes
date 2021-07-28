<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ModuleBuilder.ListView" %>

<script type="text/javascript">
    function SelectWizardTab(key) {
        for (var i = 1; i <= 5; i++) {
            var sListClass = '';
            var sLinkClass = '';
            var sListStyle = 'none';

            if (key == i) {
                sListStyle = 'block';
            }
            document.getElementById('divWizardStep' + i).style.display = sListStyle;
        }
        document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value = key;
    }

    function WizardBack() {
        try {
            var nWizardPage = parseInt(document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value);
            if (nWizardPage > 1) {
                nWizardPage--;
                document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value = nWizardPage;
                SelectWizardTab(nWizardPage);
            }
        } catch (ex) {
        }
        return true;
    }

    function WizardNext() {
        try {
            var nWizardPage = parseInt(document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value);
            if (nWizardPage < 4) {
                nWizardPage++;
                document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value = nWizardPage;
                SelectWizardTab(nWizardPage);
            }
        } catch (ex) {
        }
        return true;
    }

    function capitalize(s) {
        return s[0].toUpperCase() + s.slice(1);
    }

    String.prototype.Trim = function() {
        return this.replace(/(^\s*)|(\s*$)/g, '');
    };

    function ucFirstAllWords(str) {
        var pieces = str.split(" ");
        for (var i = 0; i < pieces.length; i++) {
            var j = pieces[i].charAt(0).toUpperCase();
            pieces[i] = j + pieces[i].substr(1);
        }
        return pieces.join(" ");
    }

    function FieldNameChanged(fldFIELD_NAME) {
        var c = document.getElementById(fldFIELD_NAME.id);
        c.value = c.value.toUpperCase().replace(' ', '_');
        var userContext = fldFIELD_NAME.id.replace('FIELD_NAME', '');
        var fldPREVIOUS_NAME = document.getElementById(userContext + 'PREVIOUS_NAME');
        if (fldPREVIOUS_NAME.value != fldFIELD_NAME.value) {
            if (fldFIELD_NAME.value.length > 0) {
                var sFIELD_NAME = fldFIELD_NAME.value.Trim();
                sFIELD_NAME = sFIELD_NAME.replace('_', ' ');
                sFIELD_NAME = ucFirstAllWords(sFIELD_NAME.toLowerCase());

                var fldEDIT_LABEL = document.getElementById(userContext + 'EDIT_LABEL');
                var fldLIST_LABEL = document.getElementById(userContext + 'LIST_LABEL');
                if (fldEDIT_LABEL != null) fldEDIT_LABEL.value = sFIELD_NAME + ':';
                if (fldLIST_LABEL != null) fldLIST_LABEL.value = sFIELD_NAME;
            }
        }
    }
</script>
<input id="txtACTIVE_TAB" type="hidden" value="1" runat="server" />
<div id="divListView">
    <%@ Register TagPrefix="SplendidCRM" TagName="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
    <SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="ModuleBuilder" Title="ModuleBuilder.LBL_MODULEBUILDER"
                              EnableModuleLabel="false" EnablePrint="false" HelpName="index" EnableHelp="true"
                              runat="Server" />
    <asp:UpdatePanel UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:Table SkinID="tabEditViewButtons" Visible="<%# !PrintView %>" runat="server">
                <asp:TableRow>
                    <asp:TableCell ID="tdButtons" Width="10%" VerticalAlign="Top" Wrap="false">
                        <asp:Button ID="btnBack" UseSubmitBehavior="false" OnClientClick=" WizardBack(); return false; "
                                    CssClass="btn  btn-primary" Text='<%# "  " + L10n.Term(".LBL_BACK_BUTTON_LABEL") + "  " %>'
                                    ToolTip='<%# L10n.Term(".LBL_BACK_BUTTON_TITLE") %>' runat="server" />&nbsp;
                        <asp:Button ID="btnNext" UseSubmitBehavior="false" OnClientClick=" WizardNext(); return false; "
                                    CssClass="btn  btn-primary" Text='<%# "  " + L10n.Term(".LBL_NEXT_BUTTON_LABEL") + "  " %>'
                                    ToolTip='<%# L10n.Term(".LBL_NEXT_BUTTON_TITLE") %>' runat="server" />&nbsp;
                        <asp:Button Visible="<%# bDebug %>" CommandName="Generate" OnCommand="Page_Command"
                                    CssClass="btn  btn-primary" Text='<%# "  " + L10n.Term("ModuleBuilder.LBL_GENERATE_BUTTON_LABEL") + "  " %>'
                                    ToolTip='<%# L10n.Term("ModuleBuilder.LBL_GENERATE_BUTTON_TITLE") %>' runat="server" />&nbsp;
                    </asp:TableCell>
                    <asp:TableCell>
                        &nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" runat="server" />
                    </asp:TableCell>
                    <asp:TableCell ID="tdRequired" HorizontalAlign="Right" VerticalAlign="Top" Wrap="false"
                                   Visible="false" CssClass="td_required">
                        <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
                        &nbsp;
                        <asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' Runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <div id="divWizardStep1" style="display: <%= bDebug || txtACTIVE_TAB.Value == "1" ? "block" : "none" %>">
                <asp:Table SkinID="tabForm" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Table SkinID="tabEditView" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_WIZARD_STEP1") %>' runat="server" /></h4></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_DISPLAY_NAME") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:TextBox ID="DISPLAY_NAME" size="25" MaxLength="25" OnTextChanged="DISPLAY_NAME_Changed"
                                                     AutoPostBack="true" runat="server" />
                                        <ajaxToolkit:AutoCompleteExtender ID="autoDISPLAY_NAME" TargetControlID="DISPLAY_NAME"
                                                                          ServiceMethod="MODULES_MODULE_NAME_List" ServicePath="~/Administration/Modules/AutoComplete.asmx"
                                                                          MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12"
                                                                          runat="server" />
                                        <asp:RequiredFieldValidator ID="reqDISPLAY_NAME" ControlToValidate="DISPLAY_NAME"
                                                                    ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false"
                                                                    EnableViewState="false" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_DISPLAY_NAME_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_NAME") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:TextBox ID="MODULE_NAME" size="25" MaxLength="25" runat="server" />
                                        <asp:RequiredFieldValidator ID="reqMODULE_NAME" ControlToValidate="MODULE_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>'
                                                                    CssClass="required" EnableClientScript="false" EnableViewState="false" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_MODULE_NAME_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_TABLE_NAME") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:TextBox ID="TABLE_NAME" size="25" MaxLength="30" runat="server" />
                                        <asp:RequiredFieldValidator ID="reqTABLE_NAME" ControlToValidate="TABLE_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>'
                                                                    CssClass="required" EnableClientScript="false" EnableViewState="false" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_TABLE_NAME_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top">
                                        <asp:Label ID="Label1" Text="Developer Name:" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:TextBox ID="txtDeveloverName" size="25" MaxLength="30" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top">
                                        <asp:Label ID="Label3" Text='Developer Name ' runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_TAB_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="TAB_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_TAB_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_MOBILE_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="MOBILE_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_MOBILE_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_CUSTOM_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="CUSTOM_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_CUSTOM_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_REPORT_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="REPORT_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_REPORT_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_IMPORT_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="IMPORT_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_IMPORT_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_REST_ENABLED") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="REST_ENABLED" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_REST_ENABLED_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_IS_ADMIN") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="IS_ADMIN" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_IS_ADMIN_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_INCLUDE_ASSIGNED_USER_ID") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="INCLUDE_ASSIGNED_USER_ID" Checked="true" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_INCLUDE_ASSIGNED_USER_ID_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_INCLUDE_TEAM_ID") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="Top">
                                        <asp:CheckBox ID="INCLUDE_TEAM_ID" Checked="true" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell Width="50%" CssClass="dataLabel" VerticalAlign="Top"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_INCLUDE_TEAM_ID_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="15%"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_OVERWRITE_EXISTING") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell CssClass="dataField" VerticalAlign="Top" Width="35%">
                                        <asp:CheckBox ID="OVERWRITE_EXISTING" Checked="false" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="50%"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_OVERWRITE_EXISTING_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="15%"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_CREATE_CODE_BEHIND") %>' runat="server" /></asp:TableCell>
                                    <asp:TableCell CssClass="dataField" VerticalAlign="Top" Width="35%">
                                        <asp:CheckBox ID="CREATE_CODE_BEHIND" Checked="false" CssClass="checkbox" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell CssClass="dataLabel" VerticalAlign="Top" Width="50%"><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_CREATE_CODE_BEHIND_INSTRUCTIONS") %>' runat="server" /></asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <div id="divWizardStep2" style="display: <%= bDebug || txtACTIVE_TAB.Value == "2" ? "block" : "none" %>">
                <asp:Table SkinID="tabForm" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Table SkinID="tabEditView" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell><h4><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_WIZARD_STEP2") %>' runat="server" /></h4></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:GridView ID="grdMain" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"
                                                      AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" OnRowCreated="grdMain_RowCreated"
                                                      OnRowDataBound="grdMain_RowDataBound" OnRowEditing="grdMain_RowEditing" OnRowDeleting="grdMain_RowDeleting"
                                                      OnRowUpdating="grdMain_RowUpdating" OnRowCancelingEdit="grdMain_RowCancelingEdit"
                                                      Width="100%" runat="server">
                                            <RowStyle CssClass="oddListRowS1" VerticalAlign="Top" />
                                            <AlternatingRowStyle CssClass="evenListRowS1" VerticalAlign="Top" />
                                            <HeaderStyle CssClass="listViewThS1" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_FIELD_NAME" ItemStyle-Width="20%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("FIELD_NAME") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:HiddenField ID="PREVIOUS_NAME" Value='<%# Eval("FIELD_NAME") %>' runat="server" />
                                                        <asp:TextBox ID="FIELD_NAME" Text='<%# Eval("FIELD_NAME") %>' MaxLength="30" Width="150"
                                                                     autocomplete="off" onblur="FieldNameChanged(this);" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_EDIT_LABEL" ItemStyle-Width="20%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("EDIT_LABEL") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="EDIT_LABEL" Text='<%# Eval("EDIT_LABEL") %>' MaxLength="50" Width="150"
                                                                     autocomplete="off" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_LIST_LABEL" ItemStyle-Width="20%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("LIST_LABEL") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="LIST_LABEL" Text='<%# Eval("LIST_LABEL") %>' MaxLength="50" Width="150"
                                                                     autocomplete="off" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_DATA_TYPE" ItemStyle-Width="20%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("DATA_TYPE") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="DATA_TYPE" runat="server">
                                                            <asp:ListItem Value="Text">Text</asp:ListItem>
                                                            <asp:ListItem Value="Text Area">Text Area</asp:ListItem>
                                                            <asp:ListItem Value="Integer">Integer</asp:ListItem>
                                                            <asp:ListItem Value="Decimal">Decimal</asp:ListItem>
                                                            <asp:ListItem Value="Money">Money</asp:ListItem>
                                                            <asp:ListItem Value="Checkbox">Checkbox</asp:ListItem>
                                                            <asp:ListItem Value="Date">Date</asp:ListItem>
                                                            <asp:ListItem Value="Dropdown">Dropdown</asp:ListItem>
                                                            <asp:ListItem Value="Guid">Guid</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_MAX_SIZE" ItemStyle-Width="10%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("MAX_SIZE") %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="MAX_SIZE" Text='<%# Sql.ToString(Eval("MAX_SIZE")) %>' MaxLength="10"
                                                                     Width="60" autocomplete="off" runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ModuleBuilder.LBL_LIST_REQUIRED" ItemStyle-Width="10%"
                                                                   HeaderStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Sql.ToBoolean(Eval("REQUIRED")) %>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="REQUIRED" Checked='<%# Sql.ToBoolean(Eval("REQUIRED")) %>' CssClass="checkbox"
                                                                      runat="server" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:CommandField ButtonType="Button" ShowEditButton="true" ShowDeleteButton="true"
                                                                  ControlStyle-CssClass="btn  btn-primary" EditText=".LBL_EDIT_BUTTON_LABEL" DeleteText=".LBL_DELETE_BUTTON_LABEL"
                                                                  UpdateText=".LBL_UPDATE_BUTTON_LABEL" CancelText=".LBL_CANCEL_BUTTON_LABEL" ItemStyle-Width="10%"
                                                                  ItemStyle-Wrap="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <div id="divWizardStep3" style="display: <%= bDebug || txtACTIVE_TAB.Value == "3" ? "block" : "none" %>">
                <asp:Table SkinID="tabForm" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Table SkinID="tabEditView" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_WIZARD_STEP3") %>' runat="server" /></h4></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:CheckBoxList ID="chkRelationships" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME"
                                                          CssClass="checkbox" runat="server">
                                        </asp:CheckBoxList>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <div id="divWizardStep4" style="display: <%= bDebug || txtACTIVE_TAB.Value == "4" ? "block" : "none" %>">
                <asp:Table SkinID="tabForm" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>
                            <asp:Table SkinID="tabEditView" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("ModuleBuilder.LBL_WIZARD_STEP4") %>' runat="server" /></h4></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Button CommandName="Generate" OnCommand="Page_Command" CssClass="btn  btn-primary" Text='<%# "  " + L10n.Term("ModuleBuilder.LBL_GENERATE_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("ModuleBuilder.LBL_GENERATE_BUTTON_TITLE") %>' Runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="lblProgress" EnableViewState="false" runat="server" />
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>