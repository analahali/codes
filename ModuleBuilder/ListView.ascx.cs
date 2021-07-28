
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SplendidCRM._devtools;

namespace SplendidCRM.Administration.ModuleBuilder
{
    /// <summary>
    ///     Summary description for ListView.
    /// </summary>
    public class ListView : SplendidControl
    {
        protected CheckBox CREATE_CODE_BEHIND;
        protected CheckBox CUSTOM_ENABLED;
        protected TextBox DISPLAY_NAME;
        protected CheckBox IMPORT_ENABLED;
        // 09/12/2011  .  REST_ENABLED provides a way to enable/disable a module in the REST API. 
        protected CheckBox INCLUDE_ASSIGNED_USER_ID;
        protected CheckBox INCLUDE_TEAM_ID;
        protected CheckBox IS_ADMIN;
        protected CheckBox MOBILE_ENABLED;
        protected TextBox MODULE_NAME;
        protected CheckBox OVERWRITE_EXISTING;
        protected CheckBox REPORT_ENABLED;
        protected CheckBox REST_ENABLED;
        protected TextBox TABLE_NAME;
        protected CheckBox TAB_ENABLED;
        protected CheckBoxList chkRelationships;
        protected DataTable dtFields;
        protected GridView grdMain;
        protected Label lblError;
        protected Label lblProgress;
        protected RequiredFieldValidator reqDISPLAY_NAME;
        protected RequiredFieldValidator reqMODULE_NAME;
        protected RequiredFieldValidator reqTABLE_NAME;
        protected HtmlInputHidden txtACTIVE_TAB;

        protected void DISPLAY_NAME_Changed(object sender, EventArgs e)
        {
            if (DISPLAY_NAME.Text.Length > 0)
            {
                MODULE_NAME.Text = DISPLAY_NAME.Text.Replace(" ", "");
                TABLE_NAME.Text = DISPLAY_NAME.Text.Replace(" ", "_").Replace("-", "_").ToUpper();
                DbProviderFactory dbf = DbProviderFactories.GetFactory();
                using (IDbConnection con = dbf.CreateConnection())
                {
                    con.Open();
                    string sSQL;
                    sSQL = "select *                         " + ControlChars.CrLf
                           + "  from vwMODULES                 " + ControlChars.CrLf
                           + " where MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf;
                    using (IDbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sSQL;
                        Sql.AddParameter(cmd, "@MODULE_NAME", MODULE_NAME.Text);
                        using (IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (rdr.Read())
                            {
                                MOBILE_ENABLED.Checked = Sql.ToBoolean(rdr["MODULE_ENABLED"]);
                                TAB_ENABLED.Checked = Sql.ToBoolean(rdr["TAB_ENABLED"]);
                                CUSTOM_ENABLED.Checked = Sql.ToBoolean(rdr["CUSTOM_ENABLED"]);
                                TABLE_NAME.Text = Sql.ToString(rdr["TABLE_NAME"]);
                                REPORT_ENABLED.Checked = Sql.ToBoolean(rdr["REPORT_ENABLED"]);
                                IMPORT_ENABLED.Checked = Sql.ToBoolean(rdr["IMPORT_ENABLED"]);
                                // 09/12/2011  .  REST_ENABLED provides a way to enable/disable a module in the REST API. 
                                REST_ENABLED.Checked = Sql.ToBoolean(rdr["REST_ENABLED"]);
                                IS_ADMIN.Checked = Sql.ToBoolean(rdr["IS_ADMIN"]);
                                INCLUDE_ASSIGNED_USER_ID.Checked =
                                    Sql.ToBoolean(Application["Modules." + MODULE_NAME.Text + ".Teamed"]);
                                INCLUDE_TEAM_ID.Checked =
                                    Sql.ToBoolean(Application["Modules." + MODULE_NAME.Text + ".Assigned"]);
                                rdr.Close();

                                cmd.Parameters.Clear();
                                sSQL = "select ColumnName as FIELD_NAME        " + ControlChars.CrLf
                                       +
                                       "     , dbo.fnL10nTerm('en-US', @MODULE_NAME, 'LBL_'      + ColumnName) as EDIT_LABEL" +
                                       ControlChars.CrLf
                                       +
                                       "     , dbo.fnL10nTerm('en-US', @MODULE_NAME, 'LBL_LIST_' + ColumnName) as LIST_LABEL" +
                                       ControlChars.CrLf
                                       +
                                       "     , (case when dbo.fnSqlColumns_IsEnum(@VIEW_NAME, ColumnName, CsType) = 1 then 'Dropdown' " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.NVarChar'                            then 'Text'     " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.VarChar'                             then 'Text'     " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.Text'                                then 'Text Area'" +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.NText'                               then 'Text Area'" +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.TinyInt'                             then 'Integer'  " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.Int'                                 then 'Integer'  " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.BigInt'                              then 'Integer'  " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.Real'                                then 'Decimal'  " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.Money'                               then 'Money'    " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.Bit'                                 then 'Checkbox' " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.DateTime'                            then 'Date'     " +
                                       ControlChars.CrLf
                                       +
                                       "             when SqlDbType = 'SqlDbType.UniqueIdentifier'                    then 'Guid'     " +
                                       ControlChars.CrLf
                                       + "             else CsType               " + ControlChars.CrLf
                                       + "        end)      as DATA_TYPE         " + ControlChars.CrLf
                                       + "     , length     as MAX_SIZE          " + ControlChars.CrLf
                                       + "     , (case IsNullable when 1 then 0 else 1 end) as REQUIRED" +
                                       ControlChars.CrLf
                                       + "  from vwSqlColumns                    " + ControlChars.CrLf
                                       + " where ObjectName = @TABLE_NAME        " + ControlChars.CrLf
                                       + " order by colid                        " + ControlChars.CrLf;

                                cmd.CommandText = sSQL;
                                Sql.AddParameter(cmd, "@MODULE_NAME", MODULE_NAME.Text);
                                Sql.AddParameter(cmd, "@VIEW_NAME", "vw" + MODULE_NAME.Text.ToUpper());
                                Sql.AddParameter(cmd, "@TABLE_NAME", TABLE_NAME.Text);
                                using (DbDataAdapter da = dbf.CreateDataAdapter())
                                {
                                    ((IDbDataAdapter) da).SelectCommand = cmd;
                                    dtFields = new DataTable();
                                    da.Fill(dtFields);

                                     
                                    DataRow rowNew = dtFields.NewRow();
                                    dtFields.Rows.Add(rowNew);

                                    ViewState["Fields"] = dtFields;
                                    grdMain.DataSource = dtFields;
                                    // 02/03/2007  .  Start with last line enabled for editing. 
                                    grdMain.EditIndex = dtFields.Rows.Count - 1;
                                    grdMain.DataBind();
                                }
                            }
                        }
                    }
                    // 03/07/2010  .  Update the relationship checkboxes. 
                    foreach (ListItem itm in chkRelationships.Items)
                    {
                        itm.Selected = false;
                    }
                    sSQL = "select MODULE_NAME                " + ControlChars.CrLf
                           + "  from vwDETAILVIEWS_RELATIONSHIPS" + ControlChars.CrLf
                           + " where DETAIL_NAME = @DETAIL_NAME " + ControlChars.CrLf
                           + " order by MODULE_NAME             " + ControlChars.CrLf;
                    using (IDbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sSQL;
                        Sql.AddParameter(cmd, "@DETAIL_NAME", MODULE_NAME.Text + ".DetailView");
                        using (IDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
                                ListItem itm = chkRelationships.Items.FindByValue(sMODULE_NAME);
                                if (itm != null)
                                {
                                    itm.Selected = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string CamelCase(string sName)
        {
            string[] arrName = sName.Split('_');
            for (int i = 0; i < arrName.Length; i++)
            {
                if (String.Compare(arrName[i], "ID", true) == 0)
                    arrName[i] = arrName[i].ToUpper();
                else
                    arrName[i] = arrName[i].Substring(0, 1).ToUpper() + arrName[i].Substring(1).ToLower();
            }
            sName = String.Join(" ", arrName);
            return sName;
        }

        private string RemoveComments(string sScript)
        {
            var rdr = new StringReader(sScript);
            var sb = new StringBuilder();
            var wtr = new StringWriter(sb);
            string sLine = null;
            while ((sLine = rdr.ReadLine()) != null)
            {
                if (!sLine.StartsWith("--") && !sLine.StartsWith("\t--") && !sLine.Contains("\t\t--"))
                    wtr.WriteLine(sLine);
            }
            return sb.ToString();
        }

        protected void Page_Command(object sender, CommandEventArgs e)
        {
            try
            {
                lblProgress.Text = String.Empty;
                if (e.CommandName == "Generate")
                {
                    string sDISPLAY_NAME = DISPLAY_NAME.Text;
                    string sDISPLAY_NAME_SINGULAR = sDISPLAY_NAME;
                    string sMODULE_NAME = MODULE_NAME.Text;
                    string sMODULE_NAME_SINGULAR = sMODULE_NAME;
                    string sTABLE_NAME = TABLE_NAME.Text.ToUpper();
                    string sTABLE_NAME_SINGULAR = sTABLE_NAME;
                    bool bTAB_ENABLED = TAB_ENABLED.Checked;
                    bool bMOBILE_ENABLED = MOBILE_ENABLED.Checked;
                    bool bCUSTOM_ENABLED = CUSTOM_ENABLED.Checked;
                    bool bREPORT_ENABLED = REPORT_ENABLED.Checked;
                    bool bIMPORT_ENABLED = IMPORT_ENABLED.Checked;
                    bool bREST_ENABLED = REST_ENABLED.Checked;
                    bool bIS_ADMIN = IS_ADMIN.Checked;
                    bool bINCLUDE_ASSIGNED_USER_ID = INCLUDE_ASSIGNED_USER_ID.Checked;
                    bool bINCLUDE_TEAM_ID = INCLUDE_TEAM_ID.Checked;
                    bool bOVERWRITE_EXISTING = OVERWRITE_EXISTING.Checked;
                    bool bCREATE_CODE_BEHIND = CREATE_CODE_BEHIND.Checked;

                    if (sDISPLAY_NAME_SINGULAR.ToLower().EndsWith("ies"))
                        sDISPLAY_NAME_SINGULAR =
                            sDISPLAY_NAME_SINGULAR.Substring(0, sDISPLAY_NAME_SINGULAR.Length - 3) + "y";
                    else if (sDISPLAY_NAME_SINGULAR.ToLower().EndsWith("s"))
                        sDISPLAY_NAME_SINGULAR = sDISPLAY_NAME_SINGULAR.Substring(0, sDISPLAY_NAME_SINGULAR.Length - 1);
                    if (sMODULE_NAME_SINGULAR.ToLower().EndsWith("ies"))
                        sMODULE_NAME_SINGULAR = sMODULE_NAME_SINGULAR.Substring(0, sMODULE_NAME_SINGULAR.Length - 3) +
                                                "y";
                    else if (sMODULE_NAME_SINGULAR.ToLower().EndsWith("s"))
                        sMODULE_NAME_SINGULAR = sMODULE_NAME_SINGULAR.Substring(0, sMODULE_NAME_SINGULAR.Length - 1);
                    if (sTABLE_NAME_SINGULAR.ToLower().EndsWith("ies"))
                        sTABLE_NAME_SINGULAR = sTABLE_NAME_SINGULAR.Substring(0, sTABLE_NAME_SINGULAR.Length - 3) + "Y";
                    else if (sTABLE_NAME_SINGULAR.ToLower().EndsWith("s"))
                        sTABLE_NAME_SINGULAR = sTABLE_NAME_SINGULAR.Substring(0, sTABLE_NAME_SINGULAR.Length - 1);

                    string sWebTemplatesPath = Server.MapPath("~/Administration/ModuleBuilder/WebTemplates");
                    if (!bCREATE_CODE_BEHIND)
                        sWebTemplatesPath = Server.MapPath("~/Administration/ModuleBuilder/WebTemplatesLive");
                    string sSqlTemplatesPath = Server.MapPath("~/Administration/ModuleBuilder/SqlTemplates");
                    string sWebModulePath = Server.MapPath((bIS_ADMIN ? "~/Administration/" : "~/") + sMODULE_NAME);
                    string sSqlScriptsPath = Path.Combine(Server.MapPath("~/"), "db_script");
                    string modulePath = Server.MapPath("~/" + MODULE_NAME.Text + "/");
                    string moduleDBPath = Server.MapPath("~/db_script/");
                    try
                    {
                        if (!Directory.Exists(sWebModulePath))
                        {
                            Directory.CreateDirectory(sWebModulePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblProgress.Text += "<font class=error>Failed to create " + sWebModulePath + ":" + ex.Message +
                                            "</font><br>" + ControlChars.CrLf;
                    }
                    try
                    {
                        if (!Directory.Exists(sSqlScriptsPath))
                        {
                            Directory.CreateDirectory(sSqlScriptsPath);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        lblProgress.Text += "<font class=error>Failed to create " + sSqlScriptsPath + ":" + ex.Message +
                                            "</font><br>" + ControlChars.CrLf;
                    }

                    var sbCreateTableFields = new StringBuilder();
                    var sbCreateTableIndexes = new StringBuilder();
                    var sbCreateViewFields = new StringBuilder();
                    var sbCreateViewJoins = new StringBuilder();
                    var sbCreateProcedureParameters = new StringBuilder();
                    var sbCreateProcedureInsertInto = new StringBuilder();
                    var sbCreateProcedureInsertValues = new StringBuilder();
                    var sbCreateProcedureUpdate = new StringBuilder();
                    var sbCreateProcedureNormalizeTeams = new StringBuilder();
                    var sbCreateProcedureUpdateTeams = new StringBuilder();
                    var sbAlterTableFields = new StringBuilder();
                    var sbCallUpdateProcedure = new StringBuilder();
                    var sbMassUpdateProcedureFields = new StringBuilder();
                    var sbMassUpdateProcedureSets = new StringBuilder();
                    var sbMassUpdateTeamNormalize = new StringBuilder();
                    var sbMassUpdateTeamAdd = new StringBuilder();
                    var sbMassUpdateTeamUpdate = new StringBuilder();
                    var sbMergeProcedureUpdates = new StringBuilder();
                    var sbModuleGridViewData = new StringBuilder();
                    var sbModuleGridViewPopup = new StringBuilder();
                    var sbModuleDetailViewData = new StringBuilder();
                    var sbModuleEditViewData = new StringBuilder();
                    var sbModuleEditViewSearchBasic = new StringBuilder();
                    var sbModuleEditViewSearchAdvanced = new StringBuilder();
                    var sbModuleEditViewSearchPopup = new StringBuilder();
                    var sbModuleTerminology = new StringBuilder();
                    // 08/08/2013  .  Add delete and undelete of relationships. 
                    var sbDeleteProcedureUpdates = new StringBuilder();
                    var sbUndeleteProcedureUpdates = new StringBuilder();
                    // 03/07/2010  .  GridViewIndex will start at 1 to make room for the checkbox. 
                    // 03/05/2011  .  Start at 2 to make room for edit button. 
                    int nGridViewIndex = 2;
                    int nGridViewPopupIndex = 1;
                    int nGridViewMAX = 3;
                    int nDetailViewIndex = 0;
                    int nEditViewIndex = 0;
                    int nEditViewSearchBasicIndex = 0;
                    int nEditViewSearchAdvancedIndex = 0;
                    int nEditViewSearchPopupIndex = 0;
                    int nEditViewSearchBasicMAX = 1;

                    if (bINCLUDE_ASSIGNED_USER_ID)
                    {
                        sbCallUpdateProcedure.AppendLine(
                            "									Guid gASSIGNED_USER_ID = new DynamicControl(this, rowCurrent, \"ASSIGNED_USER_ID\").ID;");
                        sbCallUpdateProcedure.AppendLine("									if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )");
                        sbCallUpdateProcedure.AppendLine("										gASSIGNED_USER_ID = Security.USER_ID;");
                    }
                    if (bINCLUDE_TEAM_ID)
                    {
                        sbCallUpdateProcedure.AppendLine(
                            "									Guid gTEAM_ID          = new DynamicControl(this, rowCurrent, \"TEAM_ID\"         ).ID;");
                        sbCallUpdateProcedure.AppendLine("									if ( Sql.IsEmptyGuid(gTEAM_ID) )");
                        sbCallUpdateProcedure.AppendLine("										gTEAM_ID = Security.TEAM_ID;");
                    }
                    sbCallUpdateProcedure.AppendLine("									SqlProcs.sp" + sTABLE_NAME + "_Update");
                    sbCallUpdateProcedure.AppendLine("										( ref gID");
                    if (bINCLUDE_ASSIGNED_USER_ID)
                    {
                        sbCreateTableFields.AppendLine("		, ASSIGNED_USER_ID                   uniqueidentifier null");
                        sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME + "_ASSIGNED_USER_ID on dbo." +
                                                        sTABLE_NAME + " (ASSIGNED_USER_ID, DELETED, ID)");

                        sbAlterTableFields.AppendLine(
                            "if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME +
                            "' and COLUMN_NAME = 'ASSIGNED_USER_ID') begin -- then");
                        sbAlterTableFields.AppendLine("	print 'alter table " + sTABLE_NAME +
                                                      " add ASSIGNED_USER_ID uniqueidentifier null';");
                        sbAlterTableFields.AppendLine("	alter table " + sTABLE_NAME +
                                                      " add ASSIGNED_USER_ID uniqueidentifier null;");
                        sbAlterTableFields.AppendLine("	create index IDX_" + sTABLE_NAME + "_ASSIGNED_USER_ID on dbo." +
                                                      sTABLE_NAME + " (ASSIGNED_USER_ID, DELETED, ID)");
                        sbAlterTableFields.AppendLine("end -- if;");
                        sbAlterTableFields.AppendLine("");

                        sbCreateProcedureParameters.AppendLine("	, @ASSIGNED_USER_ID                   uniqueidentifier");
                        sbCreateProcedureInsertInto.AppendLine("			, ASSIGNED_USER_ID                   ");
                        sbCreateProcedureInsertValues.AppendLine("			, @ASSIGNED_USER_ID                   ");
                        sbCreateProcedureUpdate.AppendLine(
                            "		     , ASSIGNED_USER_ID                     = @ASSIGNED_USER_ID                   ");

                        sbMassUpdateProcedureFields.AppendLine("	, @ASSIGNED_USER_ID  uniqueidentifier");
                        sbMassUpdateProcedureSets.AppendLine(
                            "			     , ASSIGNED_USER_ID  = isnull(@ASSIGNED_USER_ID, ASSIGNED_USER_ID)");

                        sbCallUpdateProcedure.AppendLine("										, gASSIGNED_USER_ID");
                    }
                    if (bINCLUDE_TEAM_ID)
                    {
                        sbCreateTableFields.AppendLine("		, TEAM_ID                            uniqueidentifier null");
                    
                        sbCreateTableFields.AppendLine("		, TEAM_SET_ID                        uniqueidentifier null");
                      
                        if (bINCLUDE_ASSIGNED_USER_ID)
                        {
                            sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                            "_TEAM_ID          on dbo." + sTABLE_NAME +
                                                            " (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)");
                            sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                            "_TEAM_SET_ID      on dbo." + sTABLE_NAME +
                                                            " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
                        }
                        else
                        {
                            sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                            "_TEAM_ID          on dbo." + sTABLE_NAME +
                                                            " (TEAM_ID, DELETED, ID)");
                            sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                            "_TEAM_SET_ID      on dbo." + sTABLE_NAME +
                                                            " (TEAM_SET_ID, DELETED, ID)");
                        }

                        sbAlterTableFields.AppendLine(
                            "if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME +
                            "' and COLUMN_NAME = 'TEAM_ID') begin -- then");
                        sbAlterTableFields.AppendLine("	print 'alter table " + sTABLE_NAME +
                                                      " add TEAM_ID uniqueidentifier null';");
                        sbAlterTableFields.AppendLine("	alter table " + sTABLE_NAME +
                                                      " add TEAM_ID uniqueidentifier null;");
                        if (bINCLUDE_ASSIGNED_USER_ID)
                            sbAlterTableFields.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                          "_TEAM_ID          on dbo." + sTABLE_NAME +
                                                          " (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)");
                        else
                            sbAlterTableFields.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                          "_TEAM_ID          on dbo." + sTABLE_NAME +
                                                          " (TEAM_ID, DELETED, ID)");
                        sbAlterTableFields.AppendLine("end -- if;");
                        sbAlterTableFields.AppendLine("");
                        sbAlterTableFields.AppendLine(
                            "if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME +
                            "' and COLUMN_NAME = 'TEAM_SET_ID') begin -- then");
                        sbAlterTableFields.AppendLine("	print 'alter table " + sTABLE_NAME +
                                                      " add TEAM_SET_ID uniqueidentifier null';");
                        sbAlterTableFields.AppendLine("	alter table " + sTABLE_NAME +
                                                      " add TEAM_SET_ID uniqueidentifier null;");
                        if (bINCLUDE_ASSIGNED_USER_ID)
                            sbAlterTableFields.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                          "_TEAM_SET_ID      on dbo." + sTABLE_NAME +
                                                          " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
                        else
                            sbAlterTableFields.AppendLine("	create index IDX_" + sTABLE_NAME +
                                                          "_TEAM_SET_ID      on dbo." + sTABLE_NAME +
                                                          " (TEAM_SET_ID, DELETED, ID)");
                        sbAlterTableFields.AppendLine("end -- if;");
                        sbAlterTableFields.AppendLine("");

                        sbCreateProcedureParameters.AppendLine("	, @TEAM_ID                            uniqueidentifier");
                        sbCreateProcedureParameters.AppendLine("	, @TEAM_SET_LIST                      varchar(8000)");
                        sbCreateProcedureInsertInto.AppendLine("			, TEAM_ID                            ");
                        sbCreateProcedureInsertInto.AppendLine("			, TEAM_SET_ID                        ");
                        sbCreateProcedureInsertValues.AppendLine("			, @TEAM_ID                            ");
                        sbCreateProcedureInsertValues.AppendLine("			, @TEAM_SET_ID                        ");
                        sbCreateProcedureUpdate.AppendLine(
                            "		     , TEAM_ID                              = @TEAM_ID                            ");
                        sbCreateProcedureUpdate.AppendLine(
                            "		     , TEAM_SET_ID                          = @TEAM_SET_ID                        ");

                        sbCreateProcedureNormalizeTeams.AppendLine("	declare @TEAM_SET_ID         uniqueidentifier;");
                        sbCreateProcedureNormalizeTeams.AppendLine(
                            "	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;");

                        sbMassUpdateProcedureFields.AppendLine("	, @TEAM_ID           uniqueidentifier");
                        sbMassUpdateProcedureFields.AppendLine("	, @TEAM_SET_LIST     varchar(8000)");
                        sbMassUpdateProcedureFields.AppendLine("	, @TEAM_SET_ADD      bit");
                        sbMassUpdateProcedureSets.AppendLine(
                            "			     , TEAM_ID           = isnull(@TEAM_ID         , TEAM_ID         )");
                        sbMassUpdateProcedureSets.AppendLine(
                            "			     , TEAM_SET_ID       = isnull(@TEAM_SET_ID     , TEAM_SET_ID     )");

                        // 09/16/2009  .  Needed to define @OLD_SET_ID. 
                        sbMassUpdateTeamNormalize.AppendLine("	declare @TEAM_SET_ID  uniqueidentifier;");
                        sbMassUpdateTeamNormalize.AppendLine("	declare @OLD_SET_ID   uniqueidentifier;");
                        sbMassUpdateTeamNormalize.AppendLine("");
                        sbMassUpdateTeamNormalize.AppendLine(
                            "	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;");

                        sbMassUpdateTeamAdd.AppendLine(
                            "		if @TEAM_SET_ADD = 1 and @TEAM_SET_ID is not null begin -- then");
                        sbMassUpdateTeamAdd.AppendLine("				select @OLD_SET_ID = TEAM_SET_ID");
                        sbMassUpdateTeamAdd.AppendLine("				     , @TEAM_ID    = isnull(@TEAM_ID, TEAM_ID)");
                        sbMassUpdateTeamAdd.AppendLine("				  from " + sTABLE_NAME);
                        sbMassUpdateTeamAdd.AppendLine("				 where ID                = @ID");
                        sbMassUpdateTeamAdd.AppendLine("				   and DELETED           = 0;");
                        sbMassUpdateTeamAdd.AppendLine("			if @OLD_SET_ID is not null begin -- then");
                        sbMassUpdateTeamAdd.AppendLine(
                            "				exec dbo.spTEAM_SETS_AddSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @OLD_SET_ID, @TEAM_ID, @TEAM_SET_ID;");
                        sbMassUpdateTeamAdd.AppendLine("			end -- if;");
                        sbMassUpdateTeamAdd.AppendLine("		end -- if;");

                       

                        sbCallUpdateProcedure.AppendLine("										, gTEAM_ID");
                        sbCallUpdateProcedure.AppendLine(
                            "										, new DynamicControl(this, rowCurrent, \"TEAM_SET_LIST\"                      ).Text");
                    }
                    string sFIRST_TEXT_FIELD = String.Empty;
                    foreach (DataRow row in dtFields.Rows)
                    {
                        string sFIELD_NAME = Sql.ToString(row["FIELD_NAME"]).ToUpper();
                        string sEDIT_LABEL = Sql.ToString(row["EDIT_LABEL"]);
                        string sLIST_LABEL = Sql.ToString(row["LIST_LABEL"]);
                        string sDATA_TYPE = Sql.ToString(row["DATA_TYPE"]);
                        int nMAX_SIZE = Sql.ToInteger(row["MAX_SIZE"]);
                        bool bREQUIRED = Sql.ToBoolean(row["REQUIRED"]);
                        
                        if (String.IsNullOrEmpty(sFIELD_NAME)
                            || sFIELD_NAME == "ID"
                            || sFIELD_NAME == "DELETED"
                            || sFIELD_NAME == "CREATED_BY"
                            || sFIELD_NAME == "DATE_ENTERED"
                            || sFIELD_NAME == "MODIFIED_USER_ID"
                            || sFIELD_NAME == "DATE_MODIFIED"
                            || sFIELD_NAME == "DATE_MODIFIED_UTC"
                            || (sFIELD_NAME == "ASSIGNED_USER_ID" && bINCLUDE_ASSIGNED_USER_ID)
                            || (sFIELD_NAME == "TEAM_ID" && bINCLUDE_TEAM_ID)
                            || (sFIELD_NAME == "TEAM_SET_ID" && bINCLUDE_TEAM_ID)
                            )
                        {
                            continue;
                        }
                        string sSQL_DATA_TYPE = String.Empty;
                        switch (sDATA_TYPE)
                        {
                            case "Text":
                                sSQL_DATA_TYPE = "nvarchar(" + nMAX_SIZE + ")";
                                break;
                            case "Text Area":
                                sSQL_DATA_TYPE = "ntext";
                                break;
                            case "Integer":
                                sSQL_DATA_TYPE = "int";
                                break;
                            case "bigint":
                                sSQL_DATA_TYPE = "bigint";
                                break;
                            case "Decimal":
                                sSQL_DATA_TYPE = "float";
                                break;
                            case "Money":
                                sSQL_DATA_TYPE = "money";
                                break;
                            case "Checkbox":
                                sSQL_DATA_TYPE = "bit";
                                break;
                            case "Date":
                                sSQL_DATA_TYPE = "datetime";
                                break;
                            case "Dropdown":
                                sSQL_DATA_TYPE = "nvarchar(50)";
                                break;
                            case "Guid":
                                sSQL_DATA_TYPE = "uniqueidentifier";
                                break;
                        }
                        sbCreateTableFields.AppendLine("		, " + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) +
                                                       sSQL_DATA_TYPE + " " + (bREQUIRED ? "not null" : "null"));

                        // 03/07/2011  .  We need the ability to alter a table to add new fields, just in case Assigned User and Team Management are enabled during re-generation. 
                        sbAlterTableFields.AppendLine(
                            "if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + sTABLE_NAME +
                            "' and COLUMN_NAME = '" + sFIELD_NAME + "') begin -- then");
                        sbAlterTableFields.AppendLine("	print 'alter table " + sTABLE_NAME + " add " + sFIELD_NAME + " " +
                                                      sSQL_DATA_TYPE + " null';");
                        sbAlterTableFields.AppendLine("	alter table " + sTABLE_NAME + " add " + sFIELD_NAME + " " +
                                                      sSQL_DATA_TYPE + " null;");
                        sbAlterTableFields.AppendLine("end -- if;");
                        sbAlterTableFields.AppendLine("");

                        if (sFIELD_NAME == "NAME" || sFIELD_NAME == "TITLE")
                            sbCreateTableIndexes.AppendLine("	create index IDX_" + sTABLE_NAME + "_" + sFIELD_NAME +
                                                            "  on dbo." + sTABLE_NAME + " (" + sFIELD_NAME +
                                                            ", DELETED, ID)");
                        sbCreateViewFields.AppendLine("     , " + sTABLE_NAME + "." + sFIELD_NAME);
                        sbCreateProcedureParameters.AppendLine("	, @" + sFIELD_NAME +
                                                               Strings.Space(35 - sFIELD_NAME.Length) + sSQL_DATA_TYPE);
                        sbCreateProcedureInsertInto.AppendLine("			, " + sFIELD_NAME +
                                                               Strings.Space(35 - sFIELD_NAME.Length));
                        sbCreateProcedureInsertValues.AppendLine("			, @" + sFIELD_NAME +
                                                                 Strings.Space(35 - sFIELD_NAME.Length));
                        sbCreateProcedureUpdate.AppendLine("		     , " + sFIELD_NAME +
                                                           Strings.Space(35 - sFIELD_NAME.Length) + "  = @" +
                                                           sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) + "");

                        if (Sql.IsEmptyString(sEDIT_LABEL))
                            sLIST_LABEL = CamelCase(sFIELD_NAME);
                        if (Sql.IsEmptyString(sEDIT_LABEL))
                            sEDIT_LABEL = sLIST_LABEL + ":";
                        sbModuleTerminology.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly 'LBL_" + sFIELD_NAME + "'" +
                                                       Strings.Space(50 - sFIELD_NAME.Length) + ", 'en-US', '" +
                                                       sMODULE_NAME + "', null, null, '" + sEDIT_LABEL + "';");
                        sbModuleTerminology.AppendLine("exec dbo.spTERMINOLOGY_InsertOnly 'LBL_LIST_" + sFIELD_NAME +
                                                       "'" + Strings.Space(45 - sFIELD_NAME.Length) + ", 'en-US', '" +
                                                       sMODULE_NAME + "', null, null, '" + sLIST_LABEL + "';");

                        sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME +
                                                          ".DetailView', " + nDetailViewIndex + ", '" + sMODULE_NAME +
                                                          ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                          "', '{0}', null;");
                        nDetailViewIndex++;
                        switch (sDATA_TYPE)
                        {
                            case "Text":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, " +
                                                                nMAX_SIZE + ", 35, null;");
                                // 09/04/2009  .  Add Auto-Complete to the NAME search field. 
                                if (sFIELD_NAME == "NAME")
                                    sbModuleEditViewSearchAdvanced.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME +
                                        ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME +
                                        ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, " + nMAX_SIZE + ", 35, '" + sMODULE_NAME + "', null;");
                                else
                                    sbModuleEditViewSearchAdvanced.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME +
                                        ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, " + nMAX_SIZE + ", 35, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                   
                                    if (sFIELD_NAME == "NAME")
                                    {
                                        sbModuleEditViewSearchBasic.AppendLine(
                                            "	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME +
                                            ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME +
                                            ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                            ", 1, " + nMAX_SIZE + ", 35, '" + sMODULE_NAME + "', null;");
                                        sbModuleEditViewSearchPopup.AppendLine(
                                            "	exec dbo.spEDITVIEWS_FIELDS_InsAutoComplete '" + sMODULE_NAME +
                                            ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME +
                                            ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                            ", 1, " + nMAX_SIZE + ", 35, '" + sMODULE_NAME + "', null;");
                                    }
                                    else
                                    {
                                        sbModuleEditViewSearchBasic.AppendLine(
                                            "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                            ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME +
                                            ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                            ", 1, " + nMAX_SIZE + ", 35, null;");
                                        sbModuleEditViewSearchPopup.AppendLine(
                                            "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                            ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME +
                                            ".LBL_" + sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                            ", 1, " + nMAX_SIZE + ", 35, null;");
                                    }
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                
                                if (nGridViewIndex == 2)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '35%', 'listViewTdLinkS1', 'ID', '" +
                                                                    (bIS_ADMIN ? "~/Administration/" : "~/") +
                                                                    sMODULE_NAME + "/view.aspx?id={0}', null, '" +
                                                                    sMODULE_NAME + "', " +
                                                                    (bINCLUDE_ASSIGNED_USER_ID
                                                                        ? "'ASSIGNED_USER_ID'"
                                                                        : "null") + ";");
                                    sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                     sMODULE_NAME + ".PopupView', " +
                                                                     nGridViewPopupIndex + ", '" + sMODULE_NAME +
                                                                     ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                     "', '" + sFIELD_NAME +
                                                                     "', '45%', 'listViewTdLinkS1', 'ID " + sFIELD_NAME +
                                                                     "', 'Select" + sMODULE_NAME_SINGULAR +
                                                                     "(''{0}'', ''{1}'');', null, '" + sMODULE_NAME +
                                                                     "', " +
                                                                     (bINCLUDE_ASSIGNED_USER_ID
                                                                         ? "'ASSIGNED_USER_ID'"
                                                                         : "null") + ";");
                                    nGridViewIndex++;
                                    nGridViewPopupIndex++;
                                }
                                else if (nGridViewIndex < nGridViewMAX)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '20%';");
                                    sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                     sMODULE_NAME + ".PopupView', " +
                                                                     nGridViewPopupIndex + ", '" + sMODULE_NAME +
                                                                     ".LBL_LIST_" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                     "', '" + sFIELD_NAME + "', '20%';");
                                    nGridViewIndex++;
                                    nGridViewPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").Text");
                                if (Sql.IsEmptyString(sFIRST_TEXT_FIELD))
                                    sFIRST_TEXT_FIELD = sFIELD_NAME;
                                break;
                            case "Text Area":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1,   1, 70, 3;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1,   1, 70, 3;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1,   1, 70, 3;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine    '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1,   1, 70, 3;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                if (nGridViewIndex == 0)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '35%', 'listViewTdLinkS1', 'ID', '" +
                                                                    (bIS_ADMIN ? "~/Administration/" : "~/") +
                                                                    sMODULE_NAME + "/view.aspx?id={0}', null, '" +
                                                                    sMODULE_NAME + "', " +
                                                                    (bINCLUDE_ASSIGNED_USER_ID
                                                                        ? "'ASSIGNED_USER_ID'"
                                                                        : "null") + ";");
                                    nGridViewIndex++;
                                }
                                else if (nGridViewIndex < nGridViewMAX)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '20%';");
                                    nGridViewIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").Text");
                                break;
                            case "Integer":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 10, 10, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 10, 10, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                if (nGridViewIndex == 0)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '35%', 'listViewTdLinkS1', 'ID', '" +
                                                                    (bIS_ADMIN ? "~/Administration/" : "~/") +
                                                                    sMODULE_NAME + "/view.aspx?id={0}', null, '" +
                                                                    sMODULE_NAME + "', " +
                                                                    (bINCLUDE_ASSIGNED_USER_ID
                                                                        ? "'ASSIGNED_USER_ID'"
                                                                        : "null") + ";");
                                    nGridViewIndex++;
                                }
                                else if (nGridViewIndex < nGridViewMAX)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '20%';");
                                    nGridViewIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) +
                                                                 ").IntegerValue");
                                break;
                            case "bigint":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 10, 10, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 10, 10, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                if (nGridViewIndex == 0)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '35%', 'listViewTdLinkS1', 'ID', '" +
                                                                    (bIS_ADMIN ? "~/Administration/" : "~/") +
                                                                    sMODULE_NAME + "/view.aspx?id={0}', null, '" +
                                                                    sMODULE_NAME + "', " +
                                                                    (bINCLUDE_ASSIGNED_USER_ID
                                                                        ? "'ASSIGNED_USER_ID'"
                                                                        : "null") + ";");
                                    nGridViewIndex++;
                                }
                                else if (nGridViewIndex < nGridViewMAX)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" + sFIELD_NAME + "', '" + sFIELD_NAME +
                                                                    "', '20%';");
                                    nGridViewIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) +
                                                                 ").IntegerValue");
                                break;
                            case "Decimal":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 10, 10, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 10, 10, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").FloatValue");
                                break;
                            case "Money":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBound       '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 10, 10, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 10, 10, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBound        '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 10, 10, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) +
                                                                 ").DecimalValue");
                                break;
                            case "Checkbox":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl     '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 'CheckBox', null, null, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 'CheckBox', null, null, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 'CheckBox', null, null, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 'CheckBox', null, null, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").Checked");
                                break;
                            case "Date":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl     '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                                                ", 1, 'DatePicker', null, null, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                    ", 1, 'DatePicker', null, null, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 'DatePicker', null, null, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) +
                                        ", 1, 'DatePicker', null, null, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").DateValue");
                                break;
                            case "Dropdown":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                                                sFIELD_NAME.ToLower() + "_dom', null, null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                    sFIELD_NAME.ToLower() + "_dom', null, null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                        sFIELD_NAME.ToLower() + "_dom', null, null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                        sFIELD_NAME.ToLower() + "_dom', null, null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) +
                                                                 ").SelectedValue");
                                break;
                            case "Guid":
                                sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsChange      '" +
                                                                sMODULE_NAME + ".EditView', " + nEditViewIndex + ", '" +
                                                                sMODULE_NAME + ".LBL_" + sFIELD_NAME + "', '" +
                                                                sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                                                sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) +
                                                                "_NAME', 'return " + sMODULE_NAME_SINGULAR +
                                                                "Popup();', null;");
                                sbModuleEditViewSearchAdvanced.AppendLine(
                                    "	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME +
                                    ".SearchAdvanced', " + nEditViewSearchAdvancedIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                    sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                    sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " +
                                    sMODULE_NAME_SINGULAR + "Popup();', null;");
                                nEditViewIndex++;
                                nEditViewSearchAdvancedIndex++;
                                if (nEditViewSearchBasicIndex < nEditViewSearchBasicMAX)
                                {
                                    sbModuleEditViewSearchBasic.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME +
                                        ".SearchBasic', " + nEditViewSearchBasicIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                        sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " +
                                        sMODULE_NAME_SINGULAR + "Popup();', null;");
                                    sbModuleEditViewSearchPopup.AppendLine(
                                        "	exec dbo.spEDITVIEWS_FIELDS_InsChange       '" + sMODULE_NAME +
                                        ".SearchPopup', " + nEditViewSearchPopupIndex + ", '" + sMODULE_NAME + ".LBL_" +
                                        sFIELD_NAME + "', '" + sFIELD_NAME + "', " + (bREQUIRED ? 1 : 0) + ", 1, '" +
                                        sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) + "_NAME', 'return " +
                                        sMODULE_NAME_SINGULAR + "Popup();', null;");
                                    nEditViewSearchBasicIndex++;
                                    nEditViewSearchPopupIndex++;
                                }
                                if (nGridViewIndex == 0)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" +
                                                                    sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) +
                                                                    "_NAME', '" +
                                                                    sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) +
                                                                    "_NAME', '35%', 'listViewTdLinkS1', 'ID', '" +
                                                                    (bIS_ADMIN ? "~/Administration/" : "~/") +
                                                                    sMODULE_NAME + "/view.aspx?id={0}', null, '" +
                                                                    sMODULE_NAME + "', " +
                                                                    (bINCLUDE_ASSIGNED_USER_ID
                                                                        ? "'ASSIGNED_USER_ID'"
                                                                        : "null") + ";");
                                    nGridViewIndex++;
                                }
                                else if (nGridViewIndex < nGridViewMAX)
                                {
                                    sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" +
                                                                    sMODULE_NAME + ".ListView', " + nGridViewIndex +
                                                                    ", '" + sMODULE_NAME + ".LBL_LIST_" + sFIELD_NAME +
                                                                    "', '" +
                                                                    sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) +
                                                                    "_NAME', '" +
                                                                    sFIELD_NAME.Substring(0, sFIELD_NAME.Length - 3) +
                                                                    "_NAME', '20%';");
                                    nGridViewIndex++;
                                }
                                sbCallUpdateProcedure.AppendLine("										, new DynamicControl(this, rowCurrent, \"" +
                                                                 sFIELD_NAME + "\"" +
                                                                 Strings.Space(35 - sFIELD_NAME.Length) + ").ID");
                                break;
                        }
                    }
                    if (Sql.IsEmptyString(sFIRST_TEXT_FIELD))
                        sFIRST_TEXT_FIELD = "NAME";
                    sbCallUpdateProcedure.AppendLine("										, trn");
                    sbCallUpdateProcedure.AppendLine("										);");
                    if (bINCLUDE_ASSIGNED_USER_ID)
                    {
                        sbCreateViewFields.AppendLine("     , " + sTABLE_NAME + ".ASSIGNED_USER_ID");
                        sbCreateViewFields.AppendLine("     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO");

                        sbCreateViewJoins.AppendLine("  left outer join USERS                      USERS_ASSIGNED");
                        sbCreateViewJoins.AppendLine("               on USERS_ASSIGNED.ID        = " + sTABLE_NAME +
                                                     ".ASSIGNED_USER_ID");

                        
                        sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME +
                                                          ".DetailView', " + nDetailViewIndex +
                                                          ", '.LBL_ASSIGNED_TO'                , 'ASSIGNED_TO'                      , '{0}'        , null;");
                        nDetailViewIndex++;
                       
                        sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup '" + sMODULE_NAME +
                                                        ".EditView', " + nEditViewIndex +
                                                        ", '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO'        , 'Users', null;");

                        nEditViewIndex++;
                        if (!bINCLUDE_TEAM_ID)
                        {
                            sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     '" +
                                                              sMODULE_NAME + ".DetailView', " + nDetailViewIndex +
                                                              ", null;");
                            nDetailViewIndex++;
                            sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBlank       '" +
                                                            sMODULE_NAME + ".EditView', " + nEditViewIndex + ", null;");
                            nEditViewIndex++;
                        }
                        sbModuleEditViewSearchBasic.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsControl      '" +
                                                               sMODULE_NAME + ".SearchBasic'    , " +
                                                               nEditViewSearchBasicIndex +
                                                               ", '.LBL_CURRENT_USER_FILTER', 'CURRENT_USER_ONLY', 0, null, 'CheckBox', 'return ToggleUnassignedOnly();', null, null;");
                        nEditViewSearchBasicIndex++;
                        sbModuleEditViewSearchAdvanced.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    '" +
                                                                  sMODULE_NAME + ".SearchAdvanced' , " +
                                                                  nEditViewSearchAdvancedIndex +
                                                                  ", '.LBL_ASSIGNED_TO'     , 'ASSIGNED_USER_ID', 0, null, 'AssignedUser'    , null, 6;");
                        nEditViewSearchAdvancedIndex++;

                        sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME +
                                                        ".ListView', " + nGridViewIndex +
                                                        ", '.LBL_LIST_ASSIGNED_USER'                  , 'ASSIGNED_TO'     , 'ASSIGNED_TO'     , '10%';");
                        sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME +
                                                         ".PopupView', " + nGridViewPopupIndex +
                                                         ", '.LBL_LIST_ASSIGNED_USER'                  , 'ASSIGNED_TO'     , 'ASSIGNED_TO'     , '10%';");
                        nGridViewIndex++;
                        nGridViewPopupIndex++;
                    }
                    if (bINCLUDE_TEAM_ID)
                    {
                        sbCreateViewFields.AppendLine("     , TEAMS.ID                    as TEAM_ID");
                        sbCreateViewFields.AppendLine("     , TEAMS.NAME                  as TEAM_NAME");
                       
                        sbCreateViewFields.AppendLine("     , TEAM_SETS.ID                as TEAM_SET_ID");
                        sbCreateViewFields.AppendLine("     , TEAM_SETS.TEAM_SET_NAME     as TEAM_SET_NAME");

                        sbCreateViewJoins.AppendLine("  left outer join TEAMS");
                        sbCreateViewJoins.AppendLine("               on TEAMS.ID                 = " + sTABLE_NAME +
                                                     ".TEAM_ID");
                        sbCreateViewJoins.AppendLine("              and TEAMS.DELETED            = 0");
                       
                        sbCreateViewJoins.AppendLine("  left outer join TEAM_SETS");
                        sbCreateViewJoins.AppendLine("               on TEAM_SETS.ID             = " + sTABLE_NAME +
                                                     ".TEAM_SET_ID");
                        sbCreateViewJoins.AppendLine("              and TEAM_SETS.DELETED        = 0");

                        if (!bINCLUDE_ASSIGNED_USER_ID)
                        {
                            sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBlank     '" +
                                                              sMODULE_NAME + ".DetailView', " + nDetailViewIndex +
                                                              ", null;");
                            nDetailViewIndex++;
                            sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsBlank       '" +
                                                            sMODULE_NAME + ".EditView', " + nEditViewIndex + ", null;");
                            nEditViewIndex++;
                        }
                        
                        sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME +
                                                          ".DetailView', " + nDetailViewIndex +
                                                          ", 'Teams.LBL_TEAM'                  , 'TEAM_NAME'                        , '{0}'        , null;");
                        nDetailViewIndex++;
                        sbModuleEditViewData.AppendLine("	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup '" + sMODULE_NAME +
                                                        ".EditView', " + nEditViewIndex +
                                                        ", 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;");
                        nEditViewIndex++;

                        sbModuleGridViewData.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME +
                                                        ".ListView', " + nGridViewIndex +
                                                        ", 'Teams.LBL_LIST_TEAM'                      , 'TEAM_NAME'       , 'TEAM_NAME'       , '5%';");
                        sbModuleGridViewPopup.AppendLine("	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     '" + sMODULE_NAME +
                                                         ".PopupView', " + nGridViewPopupIndex +
                                                         ", 'Teams.LBL_LIST_TEAM'                      , 'TEAM_NAME'       , 'TEAM_NAME'       , '10%';");
                        nGridViewIndex++;
                        nGridViewPopupIndex++;
                    }
                    sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME +
                                                      ".DetailView', " + nDetailViewIndex +
                                                      ", '.LBL_DATE_MODIFIED'              , 'DATE_MODIFIED .LBL_BY MODIFIED_BY', '{0} {1} {2}', null;");
                    nDetailViewIndex++;
                    sbModuleDetailViewData.AppendLine("	exec dbo.spDETAILVIEWS_FIELDS_InsBound     '" + sMODULE_NAME +
                                                      ".DetailView', " + nDetailViewIndex +
                                                      ", '.LBL_DATE_ENTERED'               , 'DATE_ENTERED .LBL_BY CREATED_BY'  , '{0} {1} {2}', null;");
                    nDetailViewIndex++;

                    sbModuleEditViewSearchBasic.AppendLine("");

                    foreach (ListItem chk in chkRelationships.Items)
                    {
                        if (chk.Selected)
                        {
                            string sRELATED_MODULE = chk.Value;
                            string sRELATED_TABLE =
                                Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);

                            sbMergeProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
                            sbMergeProcedureUpdates.AppendLine("	   set " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
                            sbMergeProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
                            sbMergeProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
                            sbMergeProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
                            sbMergeProcedureUpdates.AppendLine("	 where " + sTABLE_NAME_SINGULAR +
                                                               "_ID       = @MERGE_ID");
                            sbMergeProcedureUpdates.AppendLine("	   and DELETED          = 0;");
                            sbMergeProcedureUpdates.AppendLine("");

                           
                            sbDeleteProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
                            sbDeleteProcedureUpdates.AppendLine("	   set DELETED          = 1");
                            sbDeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
                            sbDeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
                            sbDeleteProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
                            sbDeleteProcedureUpdates.AppendLine("	  where " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
                            sbDeleteProcedureUpdates.AppendLine("	   and DELETED          = 0;");
                            sbDeleteProcedureUpdates.AppendLine("");

                            sbUndeleteProcedureUpdates.AppendLine("	update " + sTABLE_NAME + "_" + sRELATED_TABLE);
                            sbUndeleteProcedureUpdates.AppendLine("	   set DELETED          = 1");
                            sbUndeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED    = getdate()");
                            sbUndeleteProcedureUpdates.AppendLine("	     , DATE_MODIFIED_UTC= getutcdate()");
                            sbUndeleteProcedureUpdates.AppendLine("	     , MODIFIED_USER_ID = @MODIFIED_USER_ID");
                            sbUndeleteProcedureUpdates.AppendLine("	  where " + sTABLE_NAME_SINGULAR + "_ID       = @ID");
                            sbUndeleteProcedureUpdates.AppendLine("	   and DELETED          = 0;");
                            sbUndeleteProcedureUpdates.AppendLine("");
                        }
                    }
                    
                    var sbModuleEditViewDataInline = new StringBuilder();
                    var sbModulePopupViewDataInline = new StringBuilder();
                    sbModuleEditViewDataInline.Append(
                        sbModuleEditViewData.ToString()
                            .Replace("'" + sMODULE_NAME + ".EditView'", "'" + sMODULE_NAME + ".EditView.Inline'"));
                    sbModulePopupViewDataInline.Append(
                        sbModuleEditViewData.ToString()
                            .Replace("'" + sMODULE_NAME + ".EditView'", "'" + sMODULE_NAME + ".PopupView.Inline'"));

                    Encoding enc = Encoding.UTF8;
                    var dtSQLScripts = new DataTable();
                    dtSQLScripts.Columns.Add("FOLDER");
                    dtSQLScripts.Columns.Add("NAME");
                    dtSQLScripts.Columns.Add("PROCEDURE_NAME");
                    dtSQLScripts.Columns.Add("SQL_SCRIPT");
                    dtSQLScripts.Columns.Add("CODE_WRAPPER");

                    var vwFieldsNAME = new DataView(dtFields);
                    vwFieldsNAME.RowFilter = "FIELD_NAME = 'NAME'";
                    string[] arrSqlFolders = Directory.GetDirectories(sSqlTemplatesPath);
                    var sqlData = new StringBuilder();
                    foreach (string sSqlFolder in arrSqlFolders)
                    {
                        //lblProgress.Text += sSqlFolder + "<br>" + ControlChars.CrLf;

                        string[] arrSqlFolderParts = sSqlFolder.Split(Path.DirectorySeparatorChar);
                        string sFolder = arrSqlFolderParts[arrSqlFolderParts.Length - 1];
                        string[] arrSqlTemplates = Directory.GetFiles(sSqlFolder, "*.sql");
                        foreach (string sSqlTemplate in arrSqlTemplates)
                        {
                            //lblProgress.Text += sSqlTemplate + "<br>" + ControlChars.CrLf;
                            if (sSqlTemplate.IndexOf("$relatedmodule$") >= 0 ||
                                sSqlTemplate.IndexOf("$relatedtable$") >= 0)
                                continue;
                           
                            if (!bINCLUDE_TEAM_ID && sSqlTemplate.IndexOf("TEAMS") >= 0)
                                continue;

                            string sSqlScriptName = Path.GetFileName(sSqlTemplate);
                            sSqlScriptName = sSqlScriptName.Replace("$modulename$", sMODULE_NAME);
                            sSqlScriptName = sSqlScriptName.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
                            sSqlScriptName = sSqlScriptName.Replace("$tablename$", sTABLE_NAME);
                            sSqlScriptName = sSqlScriptName.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);

                            DataRow rowSQL = dtSQLScripts.NewRow();
                            dtSQLScripts.Rows.Add(rowSQL);
                            rowSQL["FOLDER"] = sFolder;
                            rowSQL["NAME"] = sSqlScriptName;
                            rowSQL["PROCEDURE_NAME"] = sSqlScriptName.Split('.')[0];
                            using (var sr = new StreamReader(sSqlTemplate, enc, true))
                            {
                                string sData = sr.ReadToEnd();
                                if (!bINCLUDE_ASSIGNED_USER_ID && sSqlTemplate.Contains("GRIDVIEWS_COLUMNS"))
                                {
                                    sData = sData.Replace("\'ASSIGNED_USER_ID\'", "null");
                                }
                                sData = sData.Replace("$displayname$", sDISPLAY_NAME);
                                sData = sData.Replace("$displaynamesingular$", sDISPLAY_NAME_SINGULAR);
                                sData = sData.Replace("$modulename$", sMODULE_NAME);
                                sData = sData.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
                                sData = sData.Replace("$tablename$", sTABLE_NAME);
                                sData = sData.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);

                                sData = sData.Replace("$tablename$", sTABLE_NAME);
                                sData = sData.Replace("$tabenabled$", bTAB_ENABLED ? "1" : "0");
                                sData = sData.Replace("$mobileenabled$", bMOBILE_ENABLED ? "1" : "0");
                                sData = sData.Replace("$customenabled$", bCUSTOM_ENABLED ? "1" : "0");
                                sData = sData.Replace("$reportenabled$", bREPORT_ENABLED ? "1" : "0");
                                sData = sData.Replace("$importenabled$", bIMPORT_ENABLED ? "1" : "0");
                                sData = sData.Replace("$restenabled$", bREST_ENABLED ? "1" : "0");
                                sData = sData.Replace("$isadmin$", bIS_ADMIN ? "1" : "0");
                                sData = sData.Replace("$administrationfolder$", bIS_ADMIN ? "Administration/" : "");
                                sData = sData.Replace("$taborder$", "100");

                                sData = sData.Replace("$createtablefields$", sbCreateTableFields.ToString());
                                sData = sData.Replace("$createtableindexes$", sbCreateTableIndexes.ToString());
                                sData = sData.Replace("$createviewfields$", sbCreateViewFields.ToString());
                                sData = sData.Replace("$createprocedureparameters$",
                                    sbCreateProcedureParameters.ToString());
                                sData = sData.Replace("$createprocedureinsertinto$",
                                    sbCreateProcedureInsertInto.ToString());
                                sData = sData.Replace("$createprocedureinsertvalues$",
                                    sbCreateProcedureInsertValues.ToString());
                                sData = sData.Replace("$createprocedureupdate$", sbCreateProcedureUpdate.ToString());
                                sData = sData.Replace("$createprocedurenormalizeteams$",
                                    sbCreateProcedureNormalizeTeams.ToString());
                                sData = sData.Replace("$createprocedureupdateteams$",
                                    sbCreateProcedureUpdateTeams.ToString());
                                sData = sData.Replace("$altertablefields$", sbAlterTableFields.ToString());

                                sData = sData.Replace("$createviewjoins$", sbCreateViewJoins.ToString());
                                sData = sData.Replace("$massupdateviewfields$", sbMassUpdateProcedureFields.ToString());
                                sData = sData.Replace("$massupdatesets$", sbMassUpdateProcedureSets.ToString());
                                sData = sData.Replace("$massupdateteamnormalize$", sbMassUpdateTeamNormalize.ToString());
                                sData = sData.Replace("$massupdateteamadd$", sbMassUpdateTeamAdd.ToString());
                                sData = sData.Replace("$massupdateteamupdate$", sbMassUpdateTeamUpdate.ToString());

                                sData = sData.Replace("$mergeupdaterelationship$", sbMergeProcedureUpdates.ToString());

                                sData = sData.Replace("$modulegridviewdata$", sbModuleGridViewData.ToString());
                                sData = sData.Replace("$modulegridviewpopup$", sbModuleGridViewPopup.ToString());
                                sData = sData.Replace("$moduledetailviewdata$", sbModuleDetailViewData.ToString());
                                sData = sData.Replace("$moduleeditviewdata$", sbModuleEditViewData.ToString());
                                sData = sData.Replace("$moduleeditviewdatainline$",
                                    sbModuleEditViewDataInline.ToString());
                                sData = sData.Replace("$modulepopupviewdatainline$",
                                    sbModulePopupViewDataInline.ToString());
                                sData = sData.Replace("$moduleeditviewsearchbasic$",
                                    sbModuleEditViewSearchBasic.ToString());
                                sData = sData.Replace("$moduleeditviewsearchadvanced$",
                                    sbModuleEditViewSearchAdvanced.ToString());
                                sData = sData.Replace("$moduleeditviewsearchpopup$",
                                    sbModuleEditViewSearchPopup.ToString());

                                sData = sData.Replace("$moduleterminology$", sbModuleTerminology.ToString());
                                sData = sData.Replace("$relatedterminology$", "");

                                sData = sData.Replace("$deleteprocedureupdates$", sbDeleteProcedureUpdates.ToString());
                                sData = sData.Replace("$undeleteprocedureupdates$",
                                    sbUndeleteProcedureUpdates.ToString());

                                if (sSqlTemplate.Contains("sp$tablename$_Update.1.sql"))
                                {
                                    if (vwFieldsNAME.Count > 0)
                                    {
                                        sData = sData.Replace("--exec dbo.spSUGARFAVORITES_UpdateName",
                                            "exec dbo.spSUGARFAVORITES_UpdateName");
                                    }
                                }
                                rowSQL["SQL_SCRIPT"] = sData;

                                //string sSqlScriptPath = Path.Combine(sSqlScriptsPath, sFolder);
                                //try
                                //{
                                //    if (!Directory.Exists(sSqlScriptPath))
                                //    {
                                //        Directory.CreateDirectory(sSqlScriptPath);
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    lblProgress.Text += "<font class=error>Failed to create " + sSqlScriptPath + ":" +
                                //                        ex.Message + "</font><br>" + ControlChars.CrLf;
                                //}

                                //string sSqlScriptFile = Path.Combine(sSqlScriptPath, sSqlScriptName);
                                //try
                                //{
                                //    lblProgress.Text += sSqlScriptFile + "<br>" + ControlChars.CrLf;
                                //    if (bOVERWRITE_EXISTING && File.Exists(sSqlScriptFile))
                                //        File.Delete(sSqlScriptFile);
                                //    using (StreamWriter stm = File.CreateText(sSqlScriptFile))
                                //    {
                                //        stm.Write(sData);
                                //    }

                                //}
                                //catch (Exception ex)
                                //{
                                //    lblProgress.Text += "<font class=error>" + sSqlScriptFile + ":" + ex.Message +
                                //                        "</font><br>" + ControlChars.CrLf;
                                //}
                                sqlData.Append(sData);
                            }
                        }
                    }

                    foreach (ListItem chk in chkRelationships.Items)
                    {
                        if (chk.Selected)
                        {
                            string sRELATED_MODULE = chk.Value;
                            string sRELATED_MODULE_SINGULAR = sRELATED_MODULE;
                            string sRELATED_TABLE =
                                Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
                            string sRELATED_TABLE_SINGULAR = sRELATED_TABLE;
                            if (sRELATED_MODULE_SINGULAR.ToLower().EndsWith("ies"))
                                sRELATED_MODULE_SINGULAR =
                                    sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length - 3) + "Y";
                            else if (sRELATED_MODULE_SINGULAR.ToLower().EndsWith("s"))
                                sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0,
                                    sRELATED_MODULE_SINGULAR.Length - 1);
                            if (sRELATED_TABLE_SINGULAR.ToLower().EndsWith("ies"))
                                sRELATED_TABLE_SINGULAR =
                                    sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length - 3) + "Y";
                            else if (sRELATED_TABLE_SINGULAR.ToLower().EndsWith("s"))
                                sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0,
                                    sRELATED_TABLE_SINGULAR.Length - 1);

                            foreach (string sSqlFolder in arrSqlFolders)
                            {
                                //lblProgress.Text += sSqlFolder + "<br>" + ControlChars.CrLf;

                                string[] arrSqlFolderParts = sSqlFolder.Split(Path.DirectorySeparatorChar);
                                string sFolder = arrSqlFolderParts[arrSqlFolderParts.Length - 1];
                                string[] arrSqlTemplates = Directory.GetFiles(sSqlFolder, "*.sql");
                                foreach (string sSqlTemplate in arrSqlTemplates)
                                {
                                    //lblProgress.Text += sSqlTemplate + "<br>" + ControlChars.CrLf;
                                    if (sSqlTemplate.IndexOf("$relatedmodule$") < 0 &&
                                        sSqlTemplate.IndexOf("$relatedtable$") < 0)
                                        continue;

                                    string sSqlScriptName = Path.GetFileName(sSqlTemplate);
                                    sSqlScriptName = sSqlScriptName.Replace("$modulename$", sMODULE_NAME);
                                    sSqlScriptName = sSqlScriptName.Replace("$modulenamesingular$",
                                        sMODULE_NAME_SINGULAR);
                                    sSqlScriptName = sSqlScriptName.Replace("$tablename$", sTABLE_NAME);
                                    sSqlScriptName = sSqlScriptName.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);
                                    sSqlScriptName = sSqlScriptName.Replace("$relatedmodule$", sRELATED_MODULE);
                                    sSqlScriptName = sSqlScriptName.Replace("$relatedmodulesingular$",
                                        sRELATED_MODULE_SINGULAR);
                                    sSqlScriptName = sSqlScriptName.Replace("$relatedtable$", sRELATED_TABLE);
                                    sSqlScriptName = sSqlScriptName.Replace("$relatedtablesingular$",
                                        sRELATED_TABLE_SINGULAR);

                                    DataRow rowSQL = dtSQLScripts.NewRow();
                                    dtSQLScripts.Rows.Add(rowSQL);
                                    rowSQL["FOLDER"] = sFolder;
                                    rowSQL["NAME"] = sSqlScriptName;
                                    rowSQL["PROCEDURE_NAME"] = sSqlScriptName.Split('.')[0];
                                    using (var sr = new StreamReader(sSqlTemplate, enc, true))
                                    {
                                        string sData = sr.ReadToEnd();
                                        if (sSqlTemplate.EndsWith("vw$tablename$_$relatedtable$.1.sql"))
                                        {
                                            if (sRELATED_TABLE == "DOCUMENTS")
                                            {
                                                sData = sData.Replace("     , vw$relatedtable$.NAME ",
                                                    "--     , vw$relatedtable$.NAME ");
                                            }
                                            if (sRELATED_TABLE == "PRODUCTS")
                                            {
                                                sData = sData.Replace("     , vw$relatedtable$.ID   ",
                                                    "--     , vw$relatedtable$.ID   ");
                                                sData = sData.Replace("     , vw$relatedtable$.NAME ",
                                                    "--     , vw$relatedtable$.NAME ");
                                            }
                                        }
                                        else if (
                                            sSqlTemplate.EndsWith(
                                                "DYNAMIC_BUTTONS $modulename$.$relatedmodule$.1.sql"))
                                        {
                                            if (sRELATED_MODULE == "Emails" || sRELATED_MODULE == "Notes")
                                            {
                                                sData =
                                                    sData.Replace(
                                                        "exec dbo.spDYNAMIC_BUTTONS_InsPopup  '$modulename$.$relatedmodule$', 1, '$modulename$', 'edit', '$relatedmodule$', 'list', '$relatedmodulesingular$Popup();'",
                                                        "--exec dbo.spDYNAMIC_BUTTONS_InsPopup  '$modulename$.$relatedmodule$', 1, '$modulename$', 'edit', '$relatedmodule$', 'list', '$relatedmodulesingular$Popup();'");
                                            }
                                        }
                                        else if (sSqlTemplate.EndsWith("BuildAuditTable_$tablename$.1.sql"))
                                        {
                                            if (CUSTOM_ENABLED.Checked)
                                                sData += sData.Replace("$tablename$", "$tablename$_CSTM");
                                        }
                                        sData = sData.Replace("$modulename$", sMODULE_NAME);
                                        sData = sData.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
                                        sData = sData.Replace("$tablename$", sTABLE_NAME);
                                        sData = sData.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);
                                        sData = sData.Replace("$relatedmodule$", sRELATED_MODULE);
                                        sData = sData.Replace("$relatedmodulesingular$", sRELATED_MODULE_SINGULAR);
                                        sData = sData.Replace("$relatedtable$", sRELATED_TABLE);
                                        sData = sData.Replace("$relatedtablesingular$", sRELATED_TABLE_SINGULAR);
                                        if (bINCLUDE_ASSIGNED_USER_ID)
                                            sData = sData.Replace("$relatedviewassigned$",
                                                "     , " + sTABLE_NAME + ".ASSIGNED_USER_ID as " + sTABLE_NAME_SINGULAR +
                                                "_ASSIGNED_USER_ID");
                                        else
                                            sData = sData.Replace("$relatedviewassigned$", String.Empty);
                                        rowSQL["SQL_SCRIPT"] = sData;

                                        //string sSqlScriptPath = Path.Combine(sSqlScriptsPath, sFolder);
                                        //try
                                        //{
                                        //    if (!Directory.Exists(sSqlScriptPath))
                                        //    {
                                        //        Directory.CreateDirectory(sSqlScriptPath);
                                        //    }
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    lblProgress.Text += "<font class=error>Failed to create " + sSqlScriptPath +
                                        //                        ":" + ex.Message + "</font><br>" + ControlChars.CrLf;
                                        //}

                                        //string sSqlScriptFile = Path.Combine(sSqlScriptPath, sSqlScriptName);
                                        //try
                                        //{
                                        //    lblProgress.Text += sSqlScriptFile + "<br>" + ControlChars.CrLf;
                                        //    if (bOVERWRITE_EXISTING && File.Exists(sSqlScriptFile))
                                        //        File.Delete(sSqlScriptFile);
                                        //    using (StreamWriter stm = File.CreateText(sSqlScriptFile))
                                        //    {
                                        //        stm.Write(sData);
                                        //    }
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    lblProgress.Text += "<font class=error>" + sSqlScriptFile + ":" + ex.Message +
                                        //                        "</font><br>" + ControlChars.CrLf;
                                        //}
                                        sqlData.Append(sData);
                                    }
                                }
                            }
                        }
                    }

                    var vwSQLScripts = new DataView(dtSQLScripts);

                  
                    string[] arrSQLFolderTypes =
                    {
                        "BaseTables", "Tables", "Views", "Procedures", "Triggers", "Data",
                        "Terminology"
                    };
                    DbProviderFactory dbf = DbProviderFactories.GetFactory();
                    using (IDbConnection con = dbf.CreateConnection())
                    {
                        con.Open();
                        bool bSQLAzure = false;
                        if (Sql.IsSQLServer(con))
                        {
                            using (IDbCommand cmd = con.CreateCommand())
                            {
                                cmd.CommandText = "select @@VERSION";
                                string sSqlVersion = Sql.ToString(cmd.ExecuteScalar());
                                if (sSqlVersion.StartsWith("Microsoft SQL Azure") ||
                                    (sSqlVersion.IndexOf("SQL Server") > 0 && sSqlVersion.IndexOf("CloudDB") > 0))
                                {
                                    bSQLAzure = true;
                                }
                            }
                        }
                        foreach (string sSQLFolderType in arrSQLFolderTypes)
                        {
                            
                            for (int nFolderLevel = 0; nFolderLevel <= 9; nFolderLevel++)
                            {
                                vwSQLScripts.RowFilter = "FOLDER = '" + sSQLFolderType + "' and NAME like '%." +
                                                         nFolderLevel + ".sql'";
                                foreach (DataRowView row in vwSQLScripts)
                                {
                                    string sNAME = Sql.ToString(row["NAME"]);
                                    string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
                                    string sSQL_SCRIPT = Sql.ToString(row["SQL_SCRIPT"]);
                                    try
                                    {
                                        lblProgress.Text += sSQLFolderType + "\\" + sNAME + "<br>" +
                                                            ControlChars.CrLf;
                                        if (Sql.IsSQLServer(con))
                                        {
                                            sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\ngo\r\n", "\r\nGO\r\n");
                                            sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\nGo\r\n", "\r\nGO\r\n");
                                            if (bSQLAzure)
                                            {
                                                if (sSQLFolderType == "Functions" ||
                                                    sSQLFolderType.StartsWith("Views") ||
                                                    sSQLFolderType.StartsWith("Procedures"))
                                                {
                                                    sSQL_SCRIPT = sSQL_SCRIPT.Replace("\r\n",
                                                        "\r\n");
                                                }
                                            }
                                        }

                                        using (IDbCommand cmd = con.CreateCommand())
                                        {
                                            cmd.CommandType = CommandType.Text;
                                            string[] aCommands = null;
                                            if (Sql.IsSQLServer(con))
                                            {
                                                aCommands = Strings.Split(sSQL_SCRIPT, "\r\nGO\r\n", -1,
                                                    CompareMethod.Text);
                                            }
                                            else if (Sql.IsOracle(con))
                                            {
                                                aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1,
                                                    CompareMethod.Text);
                                            }
                                            else if (Sql.IsMySQL(con))
                                            {
                                                sSQL_SCRIPT = RemoveComments(sSQL_SCRIPT);
                                                aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1,
                                                    CompareMethod.Text);
                                            }
                                            else if (Sql.IsDB2(con))
                                            {
                                                sSQL_SCRIPT = RemoveComments(sSQL_SCRIPT);
                                                aCommands = Strings.Split(sSQL_SCRIPT, "\r\n/\r\n", -1,
                                                    CompareMethod.Text);
                                            }
                                            foreach (string sCommand in aCommands)
                                            {
                                                if (Sql.IsOracle(con))
                                                {
                                                    cmd.CommandText = sCommand;
                                                   cmd.CommandText = cmd.CommandText.Replace("\r\n", "\n");
                                                }
                                                else
                                                    cmd.CommandText = sCommand;
                                                cmd.CommandText =
                                                    cmd.CommandText.TrimStart(" \t\r\n".ToCharArray());
                                                cmd.CommandText =
                                                    cmd.CommandText.TrimEnd(" \t\r\n".ToCharArray());
                                                if (cmd.CommandText.Length > 0)
                                                {
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                        if (sSQLFolderType == "Procedures")
                                        {
                                            string sSQL;
                                            sSQL = "select *                       " + ControlChars.CrLf
                                                   + "  from vwSqlColumns            " + ControlChars.CrLf
                                                   + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
                                                   + "   and ObjectType = 'P'        " + ControlChars.CrLf
                                                   + " order by colid                " + ControlChars.CrLf;
                                            using (IDbCommand cmd = con.CreateCommand())
                                            {
                                                cmd.CommandText = sSQL;
                                                Sql.AddParameter(cmd, "@OBJECTNAME",
                                                    Sql.MetadataName(cmd, sPROCEDURE_NAME));
                                                using (DbDataAdapter da = dbf.CreateDataAdapter())
                                                {
                                                    ((IDbDataAdapter) da).SelectCommand = cmd;
                                                    using (var dt = new DataTable())
                                                    {
                                                        da.Fill(dt);
                                                        DataRowCollection colRows = dt.Rows;
                                                        var sb = new StringBuilder();
                                                        sb.AppendLine("	public partial class SqlProcs");
                                                        sb.AppendLine("	{");
                                                        Procedures.BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows,
                                                            false, false);
                                                        Procedures.BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows,
                                                            false, true);
                                                        Procedures.BuildWrapper(ref sb, sPROCEDURE_NAME, ref colRows,
                                                            true, false);
                                                        sb.AppendLine("	}");
                                                        string sCODE_WRAPPER = sb.ToString();
                                                        
                                                        sCODE_WRAPPER =
                                                            sCODE_WRAPPER.Replace("DbProviderFactory",
                                                                "SplendidCRM.DbProviderFactory");
                                                        sCODE_WRAPPER =
                                                            sCODE_WRAPPER.Replace("DbProviderFactories",
                                                                "SplendidCRM.DbProviderFactories");
                                                       
                                                        row["CODE_WRAPPER"] = sCODE_WRAPPER;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblProgress.Text += "<font class=error>" + sNAME + ":" + ex.Message +
                                                            "</font><br>" + ControlChars.CrLf;
                                    }
                                }
                            }
                        }
                    }
                    //}

                    string[] arrWebTemplates = Directory.GetFiles(sWebTemplatesPath);
                    foreach (string sWebTemplate in arrWebTemplates)
                    {
                        if (sWebTemplate.IndexOf("$relatedmodule$") >= 0 || sWebTemplate.IndexOf("$relatedtable$") >= 0)
                            continue;
                       

                        using (var sr = new StreamReader(sWebTemplate, enc, true))
                        {
                            string sData = sr.ReadToEnd();
                            if (bIS_ADMIN)
                                sData = sData.Replace("SplendidCRM.$modulename$",
                                    "SplendidCRM.Administration.$modulename$");
                            sData = sData.Replace("$modulename$", sMODULE_NAME);
                            sData = sData.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
                            sData = sData.Replace("$tablename$", sTABLE_NAME);
                            sData = sData.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);
                            sData = sData.Replace("$firsttextfield$", sFIRST_TEXT_FIELD);

                            string sWebTemplateName = Path.GetFileName(sWebTemplate);
                            if (sWebTemplateName.StartsWith("DetailView.ascx"))
                            {
                                if (!bINCLUDE_ASSIGNED_USER_ID)
                                {
                                    sData = sData.Replace("Sql.ToGuid(rdr[\"ASSIGNED_USER_ID\"])", "Guid.Empty");
                                }
                            }
                            else if (sWebTemplateName.StartsWith("ListView.ascx"))
                            {
                                if (!bINCLUDE_ASSIGNED_USER_ID)
                                {
                                    sData = sData.Replace("Sql.ToGuid(rdr[\"ASSIGNED_USER_ID\"])", "Guid.Empty");
                                    // 03/05/2011  . Now that we have added the edit button, we need to remove Assigned User. 
                                    sData = sData.Replace(", Sql.ToGuid(Eval(\"ASSIGNED_USER_ID\"))", String.Empty);
                                    // 03/07/2011  .  Remove ASSIGNED_USER_ID from arrSelectFields. 
                                    sData = sData.Replace("arrSelectFields.Add(\"ASSIGNED_USER_ID\");", String.Empty);
                                }
                                if (bINCLUDE_ASSIGNED_USER_ID && bINCLUDE_TEAM_ID)
                                    sData = sData.Replace("$callmassupdateprocedure$",
                                        "SqlProcs.sp" + sTABLE_NAME +
                                        "_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, ctlMassUpdate.PRIMARY_TEAM_ID, ctlMassUpdate.TEAM_SET_LIST, ctlMassUpdate.ADD_TEAM_SET, trn);");
                                else if (bINCLUDE_ASSIGNED_USER_ID)
                                    sData = sData.Replace("$callmassupdateprocedure$",
                                        "SqlProcs.sp" + sTABLE_NAME +
                                        "_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, trn);");
                                else if (bINCLUDE_TEAM_ID)
                                    sData = sData.Replace("$callmassupdateprocedure$",
                                        "SqlProcs.sp" + sTABLE_NAME +
                                        "_MassUpdate(sIDs, ctlMassUpdate.PRIMARY_TEAM_ID, ctlMassUpdate.TEAM_SET_LIST, ctlMassUpdate.ADD_TEAM_SET, trn);");
                                else // 10/12/2009  .  Remove insertion if not used. 
                                    sData = sData.Replace("$callmassupdateprocedure$", "");
                            }
                            else if (sWebTemplateName.StartsWith("ListView.ascx.cs"))
                            {
                                if (!bINCLUDE_ASSIGNED_USER_ID)
                                {
                                    // 03/07/2011  .  Remove ASSIGNED_USER_ID from arrSelectFields. 
                                    sData = sData.Replace("arrSelectFields.Add(\"ASSIGNED_USER_ID\");", String.Empty);
                                }
                            }
                            else if (sWebTemplateName.StartsWith("EditView.ascx"))
                            {
                                sData = sData.Replace("$callupdateprocedure$", sbCallUpdateProcedure.ToString());
                            }
                            else if (sWebTemplateName.StartsWith("NewRecord.ascx"))
                            {
                                // 06/22/2010  .  The NewRecord controls do not use rowCurrent. 
                                sData = sData.Replace("$callupdateprocedure$",
                                    sbCallUpdateProcedure.ToString().Replace("rowCurrent,", "null,"));
                            }

                            // 03/08/2010  .  Allow user to select Live deployment, but we still prefer the code-behind method. 
                            //if (!bCREATE_CODE_BEHIND)
                            //{
                            // 03/06/2010  .  Insert the SqlProc wrapper into each file as needed. 
                            vwSQLScripts.RowFilter = "FOLDER = 'Procedures'";
                            foreach (DataRowView row in vwSQLScripts)
                            {
                                string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
                                string sCODE_WRAPPER = Sql.ToString(row["CODE_WRAPPER"]);
                                if (!Sql.IsEmptyString(sCODE_WRAPPER))
                                {
                                    if (sData.IndexOf("SqlProcs." + sPROCEDURE_NAME) >= 0)
                                    {
                                        sData = sData.Replace("//$sqlprocs$",
                                            sCODE_WRAPPER + ControlChars.CrLf + ControlChars.CrLf + "//$sqlprocs$");
                                    }
                                }
                            }
                            //  }
                            sData = sData.Replace("//$sqlprocs$", String.Empty);

                            string sWebModuleFile = Path.Combine(sWebModulePath, sWebTemplateName);
                            try
                            {
                                lblProgress.Text += sWebModuleFile + "<br>" + ControlChars.CrLf;
                                if (bOVERWRITE_EXISTING && File.Exists(sWebModuleFile))
                                    File.Delete(sWebModuleFile);
                                using (StreamWriter stm = File.CreateText(sWebModuleFile))
                                {
                                    stm.Write(sData);
                                }
                            }
                            catch (Exception ex)
                            {
                                lblProgress.Text += "<font class=error>" + sWebTemplate + ":" + ex.Message +
                                                    "</font><br>" + ControlChars.CrLf;
                            }
                        }
                    }

                    foreach (ListItem chk in chkRelationships.Items)
                    {
                        if (chk.Selected)
                        {
                            string sRELATED_MODULE = chk.Value;
                            string sRELATED_MODULE_SINGULAR = sRELATED_MODULE;
                            string sRELATED_TABLE =
                                Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
                            string sRELATED_TABLE_SINGULAR = sRELATED_TABLE;
                            if (sRELATED_MODULE_SINGULAR.ToLower().EndsWith("ies"))
                                sRELATED_MODULE_SINGULAR =
                                    sRELATED_MODULE_SINGULAR.Substring(0, sRELATED_MODULE_SINGULAR.Length - 3) + "Y";
                            else if (sRELATED_MODULE_SINGULAR.ToLower().EndsWith("s"))
                                sRELATED_MODULE_SINGULAR = sRELATED_MODULE_SINGULAR.Substring(0,
                                    sRELATED_MODULE_SINGULAR.Length - 1);
                            if (sRELATED_TABLE_SINGULAR.ToLower().EndsWith("ies"))
                                sRELATED_TABLE_SINGULAR =
                                    sRELATED_TABLE_SINGULAR.Substring(0, sRELATED_TABLE_SINGULAR.Length - 3) + "Y";
                            else if (sRELATED_TABLE_SINGULAR.ToLower().EndsWith("s"))
                                sRELATED_TABLE_SINGULAR = sRELATED_TABLE_SINGULAR.Substring(0,
                                    sRELATED_TABLE_SINGULAR.Length - 1);

                            foreach (string sWebTemplate in arrWebTemplates)
                            {
                                if (sWebTemplate.IndexOf("$relatedmodule$") < 0 &&
                                    sWebTemplate.IndexOf("$relatedtable$") < 0)
                                    continue;
                                // 03/07/2010  .  The code-behind files are not used so that the generated files will be immediately accessible. 
                                // 08/25/2010  .  We still want to allow code-behind files. 
                                //if ( sWebTemplate.EndsWith(".cs") )
                                //	continue;

                                using (var sr = new StreamReader(sWebTemplate, enc, true))
                                {
                                    string sData = sr.ReadToEnd();
                                    // 09/12/2009  .  If this is an admin module, then place in the Administration namespace. 
                                    if (bIS_ADMIN)
                                        sData = sData.Replace("SplendidCRM.$modulename$",
                                            "SplendidCRM.Administration.$modulename$");
                                    sData = sData.Replace("$modulename$", sMODULE_NAME);
                                    sData = sData.Replace("$modulenamesingular$", sMODULE_NAME_SINGULAR);
                                    sData = sData.Replace("$tablename$", sTABLE_NAME);
                                    sData = sData.Replace("$tablenamesingular$", sTABLE_NAME_SINGULAR);
                                    sData = sData.Replace("$relatedmodule$", sRELATED_MODULE);
                                    sData = sData.Replace("$relatedmodulesingular$", sRELATED_MODULE_SINGULAR);
                                    sData = sData.Replace("$relatedtable$", sRELATED_TABLE);
                                    sData = sData.Replace("$relatedtablesingular$", sRELATED_TABLE_SINGULAR);

                                    // 03/03/2011  .  Project and ProjectTask need to be corrected. 
                                    if (sWebTemplate.EndsWith("$relatedmodule$.ascx"))
                                    {
                                        if (sRELATED_MODULE == "Project")
                                        {
                                            sData = sData.Replace("~/Project/NewRecord.ascx",
                                                "~/Projects/NewRecord.ascx");
                                        }
                                        else if (sRELATED_MODULE == "ProjectTask")
                                        {
                                            sData = sData.Replace("~/ProjectTask/NewRecord.ascx",
                                                "~/ProjectTasks/NewRecord.ascx");
                                        }
                                    }
                                    if (sWebTemplate.EndsWith("$relatedmodule$.ascx.cs"))
                                    {
                                        if (sRELATED_MODULE == "Project")
                                        {
                                            sData = sData.Replace("SplendidCRM.Project.NewRecord",
                                                "SplendidCRM.Projects.NewRecord");
                                        }
                                        else if (sRELATED_MODULE == "ProjectTask")
                                        {
                                            sData = sData.Replace("SplendidCRM.ProjectTask.NewRecord",
                                                "SplendidCRM.ProjectTasks.NewRecord");
                                        }
                                    }

                                    string sWebTemplateName = Path.GetFileName(sWebTemplate);
                                    sWebTemplateName = sWebTemplateName.Replace("$modulename$", sMODULE_NAME);
                                    sWebTemplateName = sWebTemplateName.Replace("$modulenamesingular$",
                                        sMODULE_NAME_SINGULAR);
                                    sWebTemplateName = sWebTemplateName.Replace("$tablename$", sTABLE_NAME);
                                    sWebTemplateName = sWebTemplateName.Replace("$tablenamesingular$",
                                        sTABLE_NAME_SINGULAR);
                                    sWebTemplateName = sWebTemplateName.Replace("$relatedmodule$", sRELATED_MODULE);
                                    sWebTemplateName = sWebTemplateName.Replace("$relatedmodulesingular$",
                                        sRELATED_MODULE_SINGULAR);
                                    sWebTemplateName = sWebTemplateName.Replace("$relatedtable$", sRELATED_TABLE);
                                    sWebTemplateName = sWebTemplateName.Replace("$relatedtablesingular$",
                                        sRELATED_TABLE_SINGULAR);

                                    // 03/08/2010  .  Allow user to select Live deployment, but we still prefer the code-behind method. 
                                    //if (!bCREATE_CODE_BEHIND)
                                    //{
                                    // 03/06/2010  .  Insert the SqlProc wrapper into each file as needed. 
                                    vwSQLScripts.RowFilter = "FOLDER = 'Procedures'";
                                    foreach (DataRowView row in vwSQLScripts)
                                    {
                                        string sPROCEDURE_NAME = Sql.ToString(row["PROCEDURE_NAME"]);
                                        string sCODE_WRAPPER = Sql.ToString(row["CODE_WRAPPER"]);
                                        if (!Sql.IsEmptyString(sCODE_WRAPPER))
                                        {
                                            if (sData.IndexOf("SqlProcs." + sPROCEDURE_NAME) >= 0)
                                            {
                                                sData = sData.Replace("//$sqlprocs$",
                                                    sCODE_WRAPPER + ControlChars.CrLf + ControlChars.CrLf +
                                                    "//$sqlprocs$");
                                            }
                                        }
                                    }
                                    //}
                                    sData = sData.Replace("//$sqlprocs$", String.Empty);

                                    string sWebModuleFile = Path.Combine(sWebModulePath, sWebTemplateName);
                                    try
                                    {
                                        lblProgress.Text += sWebModuleFile + "<br>" + ControlChars.CrLf;
                                        if (bOVERWRITE_EXISTING && File.Exists(sWebModuleFile))
                                            File.Delete(sWebModuleFile);
                                        using (StreamWriter stm = File.CreateText(sWebModuleFile))
                                        {
                                            stm.Write(sData);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lblProgress.Text += "<font class=error>" + sWebTemplate + ":" + ex.Message +
                                                            "</font><br>" + ControlChars.CrLf;
                                    }
                                }
                            }
                        }
                    }
                    // 03/08/2010  .  Allow user to select Live deployment, but we still prefer the code-behind method. 
                    //if (bCREATE_CODE_BEHIND)
                    //{
                    // 03/07/2010  .  After a successful build, we need to reload the cached data. 
                    SplendidInit.InitApp(HttpContext.Current);
                    SplendidInit.LoadUserPreferences(Security.USER_ID, Sql.ToString(Session["USER_SETTINGS/THEME"]),
                        Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
                    //}
                    var txtDeveloverName = FindControl("txtDeveloverName") as TextBox;
                    File.WriteAllText(Path.Combine(modulePath, MODULE_NAME.Text + ".sql"), sqlData.ToString());

                    int TotalFiles = Directory.GetFiles(moduleDBPath, "*.sql").Length;

                    File.WriteAllText(
                        Path.Combine(moduleDBPath,
                            (TotalFiles < 10 ? "00" : (TotalFiles < 100 ? "0" : "")) + TotalFiles + "_Create" +
                            MODULE_NAME.Text + "Module" + "_" + txtDeveloverName.Text.Trim() + "_" +
                            DateTime.Now.ToString("ddMMMyyyy") + ".sql"), sqlData.ToString());


                    lblProgress.Text += MODULE_NAME.Text + DateTime.Now.ToString("ddMMMyyyy") + ".sql";
                }
            }
            catch (Exception ex)
            {
                SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
                lblProgress.Text += "<font class=error>" + ex.Message + "</font>";
            }
        }


        private void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
            // 01/11/2006  .  Only a developer/administrator should see this. 
            // 03/08/2010  .  The Module Builder can now be run in production. 
            // 03/08/2010  .  We now have a Web.config flag to disable the module builder. 
            // 03/10/2010  .  Apply full ACL security rules. 
            Visible = (Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
            if (!Visible || Sql.ToBoolean(Utils.AppSettings["DisableModuleBuilder"]))
                // Request.ServerVariables["SERVER_NAME"] != "localhost" )
            {
                // 03/17/2010  .  We need to rebind the parent in order to get the error message to display. 
                Parent.DataBind();
                return;
            }

            // 10/18/2010  .  The required fields need to be bound manually. 
            reqDISPLAY_NAME.DataBind();
            reqMODULE_NAME.DataBind();
            reqTABLE_NAME.DataBind();
            if (!IsPostBack)
            {
#if DEBUG
                // 03/08/2010  .  Allow user to select Live deployment, but we still prefer the code-behind method. 
                CREATE_CODE_BEHIND.Checked = true;
#endif
                foreach (DataControlField col in grdMain.Columns)
                {
                    if (!Sql.IsEmptyString(col.HeaderText))
                    {
                        col.HeaderText = L10n.Term(col.HeaderText);
                    }
                    var cf = col as CommandField;
                    if (cf != null)
                    {
                        cf.EditText = L10n.Term(cf.EditText);
                        cf.DeleteText = L10n.Term(cf.DeleteText);
                        cf.UpdateText = L10n.Term(cf.UpdateText);
                        cf.CancelText = L10n.Term(cf.CancelText);
                    }
                }

                DbProviderFactory dbf = DbProviderFactories.GetFactory();
                using (IDbConnection con = dbf.CreateConnection())
                {
                    con.Open();
                    string sSQL;
                    sSQL = "select MODULE_NAME       " + ControlChars.CrLf
                           + "     , DISPLAY_NAME      " + ControlChars.CrLf
                           + "  from vwMODULES         " + ControlChars.CrLf
                           + " where MODULE_ENABLED = 1" + ControlChars.CrLf
                           + "   and CUSTOM_ENABLED = 1" + ControlChars.CrLf
                           + "   and REPORT_ENABLED = 1" + ControlChars.CrLf
                           + "   and IS_ADMIN       = 0" + ControlChars.CrLf
                           + " order by MODULE_NAME    " + ControlChars.CrLf;
                    using (IDbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sSQL;
                        using (DbDataAdapter da = dbf.CreateDataAdapter())
                        {
                            ((IDbDataAdapter) da).SelectCommand = cmd;
                            var dtModules = new DataTable();
                            da.Fill(dtModules);
                            foreach (DataRow row in dtModules.Rows)
                            {
                                row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
                            }
                            chkRelationships.DataSource = dtModules;
                            chkRelationships.DataBind();
                        }
                    }
                }

                dtFields = new DataTable();
                var colFIELD_NAME = new DataColumn("FIELD_NAME", Type.GetType("System.String"));
                var colEDIT_LABEL = new DataColumn("EDIT_LABEL", Type.GetType("System.String"));
                var colLIST_LABEL = new DataColumn("LIST_LABEL", Type.GetType("System.String"));
                var colDATA_TYPE = new DataColumn("DATA_TYPE", Type.GetType("System.String"));
                var colMAX_SIZE = new DataColumn("MAX_SIZE", Type.GetType("System.Int32"));
                var colREQUIRED = new DataColumn("REQUIRED", Type.GetType("System.Boolean"));
                dtFields.Columns.Add(colFIELD_NAME);
                dtFields.Columns.Add(colEDIT_LABEL);
                dtFields.Columns.Add(colLIST_LABEL);
                dtFields.Columns.Add(colDATA_TYPE);
                dtFields.Columns.Add(colMAX_SIZE);
                dtFields.Columns.Add(colREQUIRED);

                DataRow rowID = dtFields.NewRow();
                DataRow rowDELETED = dtFields.NewRow();
                DataRow rowCREATED_BY = dtFields.NewRow();
                DataRow rowDATE_ENTERED = dtFields.NewRow();
                DataRow rowMODIFIED_USER_ID = dtFields.NewRow();
                DataRow rowDATE_MODIFIED = dtFields.NewRow();
                DataRow rowNAME = dtFields.NewRow();
                rowID["FIELD_NAME"] = "ID";
                rowID["DATA_TYPE"] = "Guid";
                rowID["REQUIRED"] = true;
                rowID["EDIT_LABEL"] = L10n.Term(".LBL_ID");
                rowID["LIST_LABEL"] = L10n.Term(".LBL_LIST_ID");

                rowDELETED["FIELD_NAME"] = "DELETED";
                rowDELETED["DATA_TYPE"] = "Checkbox";
                rowDELETED["REQUIRED"] = true;
                rowDELETED["EDIT_LABEL"] = L10n.Term(".LBL_DELETED");
                rowDELETED["LIST_LABEL"] = L10n.Term(".LBL_LIST_DELETED");

                rowCREATED_BY["FIELD_NAME"] = "CREATED_BY";
                rowCREATED_BY["DATA_TYPE"] = "Guid";
                rowCREATED_BY["REQUIRED"] = false;
                rowCREATED_BY["EDIT_LABEL"] = L10n.Term(".LBL_CREATED_BY");
                rowCREATED_BY["LIST_LABEL"] = L10n.Term(".LBL_LIST_CREATED_BY");

                rowDATE_ENTERED["FIELD_NAME"] = "DATE_ENTERED";
                rowDATE_ENTERED["DATA_TYPE"] = "Date";
                rowDATE_ENTERED["REQUIRED"] = true;
                rowDATE_ENTERED["EDIT_LABEL"] = L10n.Term(".LBL_DATE_ENTERED");
                rowDATE_ENTERED["LIST_LABEL"] = L10n.Term(".LBL_LIST_DATE_ENTERED");

                rowMODIFIED_USER_ID["FIELD_NAME"] = "MODIFIED_USER_ID";
                rowMODIFIED_USER_ID["DATA_TYPE"] = "Guid";
                rowMODIFIED_USER_ID["REQUIRED"] = false;
                rowMODIFIED_USER_ID["EDIT_LABEL"] = L10n.Term(".LBL_MODIFIED_USER_ID");
                rowMODIFIED_USER_ID["LIST_LABEL"] = L10n.Term(".LBL_LIST_MODIFIED_USER_ID");

                rowDATE_MODIFIED["FIELD_NAME"] = "DATE_MODIFIED";
                rowDATE_MODIFIED["DATA_TYPE"] = "Date";
                rowDATE_MODIFIED["REQUIRED"] = true;
                rowDATE_MODIFIED["EDIT_LABEL"] = L10n.Term(".LBL_DATE_MODIFIED");
                rowDATE_MODIFIED["LIST_LABEL"] = L10n.Term(".LBL_LIST_DATE_MODIFIED");

                rowNAME["FIELD_NAME"] = "NAME";
                rowNAME["DATA_TYPE"] = "Text";
                rowNAME["REQUIRED"] = true;
                rowNAME["EDIT_LABEL"] = L10n.Term("Name:");
                rowNAME["LIST_LABEL"] = L10n.Term("Name");
                rowNAME["MAX_SIZE"] = 150;

                dtFields.Rows.Add(rowID);
                dtFields.Rows.Add(rowDELETED);
                dtFields.Rows.Add(rowCREATED_BY);
                dtFields.Rows.Add(rowDATE_ENTERED);
                dtFields.Rows.Add(rowMODIFIED_USER_ID);
                dtFields.Rows.Add(rowDATE_MODIFIED);
                dtFields.Rows.Add(rowNAME);

                // 03/27/2007  .  Always add blank line to allow quick editing. 
                DataRow rowNew = dtFields.NewRow();
                dtFields.Rows.Add(rowNew);

                ViewState["Fields"] = dtFields;
                grdMain.DataSource = dtFields;
                // 02/03/2007  .  Start with last line enabled for editing. 
                grdMain.EditIndex = dtFields.Rows.Count - 1;
                grdMain.DataBind();
                var txtDeveloverName = FindControl("txtDeveloverName") as TextBox;
                if (txtDeveloverName != null)
                    txtDeveloverName.Text =
                        Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Security.FULL_NAME).Replace(" ", "");
            }
            else
            {
                dtFields = ViewState["Fields"] as DataTable;
                grdMain.DataSource = dtFields;
                // 03/31/2007  .  Don't bind the grid, otherwise edits will be lost. 
                //grdMain.DataBind();
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            m_sMODULE = "ModuleBuilder";
            // 05/06/2010  .  The menu will show the admin Module Name in the Six theme. 
            SetMenu(m_sMODULE);
        }

        #endregion

        #region Field Editing

        protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 03/05/2011  .  We need to manually set the data type. 
                var lstDATA_TYPE = e.Row.FindControl("DATA_TYPE") as DropDownList;
                if (lstDATA_TYPE != null)
                {
                    try
                    {
                        Utils.SetValue(lstDATA_TYPE, Sql.ToString(DataBinder.Eval(e.Row.DataItem, "DATA_TYPE")));
                    }
                    catch
                    {
                    }
                }
            }
        }

        protected void grdMain_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // 02/07/2010  .  Defensive programming, make sure that the fields table exists before using. 
            if (dtFields != null)
            {
                DataRow[] aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                if (e.NewEditIndex < aCurrentRows.Length)
                {
                    DataRow row = aCurrentRows[e.NewEditIndex];
                    string sFIELD_NAME = Sql.ToString(row["FIELD_NAME"]);
                    // 09/16/2009  .  DATE_MODIFIED_UTC is a new common field used to sync. 
                    if (sFIELD_NAME == "ID"
                        || sFIELD_NAME == "DELETED"
                        || sFIELD_NAME == "CREATED_BY"
                        || sFIELD_NAME == "DATE_ENTERED"
                        || sFIELD_NAME == "MODIFIED_USER_ID"
                        || sFIELD_NAME == "DATE_MODIFIED"
                        || sFIELD_NAME == "DATE_MODIFIED_UTC"
                        )
                    {
                        lblError.Text = "This field cannot be edited.";
                        return;
                    }
                }
                grdMain.EditIndex = e.NewEditIndex;
                grdMain.DataSource = dtFields;
                grdMain.DataBind();
            }
        }

        protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (dtFields != null)
            {
                //dtFields.Rows.RemoveAt(e.RowIndex);
                //dtFields.Rows[e.RowIndex].Delete();
                DataRow[] aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                if (e.RowIndex < aCurrentRows.Length)
                {
                    DataRow row = aCurrentRows[e.RowIndex];
                    string sFIELD_NAME = Sql.ToString(row["FIELD_NAME"]);
                    // 09/16/2009  .  DATE_MODIFIED_UTC is a new common field used to sync. 
                    if (sFIELD_NAME == "ID"
                        || sFIELD_NAME == "DELETED"
                        || sFIELD_NAME == "CREATED_BY"
                        || sFIELD_NAME == "DATE_ENTERED"
                        || sFIELD_NAME == "MODIFIED_USER_ID"
                        || sFIELD_NAME == "DATE_MODIFIED"
                        || sFIELD_NAME == "DATE_MODIFIED_UTC"
                        )
                    {
                        lblError.Text = "This field cannot be deleted.";
                        return;
                    }
                }
                aCurrentRows[e.RowIndex].Delete();

                aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                // 02/04/2007  .  Always allow editing of the last empty row. Add blank row if necessary. 
                if (aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length - 1]["FIELD_NAME"]))
                {
                    DataRow rowNew = dtFields.NewRow();
                    dtFields.Rows.Add(rowNew);
                    aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                }
                ViewState["Fields"] = dtFields;
                grdMain.DataSource = dtFields;
                // 03/15/2007  .  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
                grdMain.EditIndex = aCurrentRows.Length - 1;
                grdMain.DataBind();
            }
        }

        protected void grdMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (dtFields != null)
            {
                GridViewRow gr = grdMain.Rows[e.RowIndex];
                var txtFIELD_NAME = gr.FindControl("FIELD_NAME") as TextBox;
                var txtEDIT_LABEL = gr.FindControl("EDIT_LABEL") as TextBox;
                var txtLIST_LABEL = gr.FindControl("LIST_LABEL") as TextBox;
                var lstDATA_TYPE = gr.FindControl("DATA_TYPE") as DropDownList;
                var txtMAX_SIZE = gr.FindControl("MAX_SIZE") as TextBox;
                var chkREQUIRED = gr.FindControl("REQUIRED") as CheckBox;

                DataRow row = dtFields.Rows[e.RowIndex];
                if (txtFIELD_NAME != null) row["FIELD_NAME"] = txtFIELD_NAME.Text;
                // 02/07/2010  .  Defensive programming, was not validating txtEDIT_LABEL. 
                if (txtEDIT_LABEL != null) row["EDIT_LABEL"] = txtEDIT_LABEL.Text;
                if (txtLIST_LABEL != null) row["LIST_LABEL"] = txtLIST_LABEL.Text;
                if (lstDATA_TYPE != null) row["DATA_TYPE"] = lstDATA_TYPE.SelectedValue;
                if (txtMAX_SIZE != null) row["MAX_SIZE"] = Sql.ToInteger(txtMAX_SIZE.Text);
                if (chkREQUIRED != null) row["REQUIRED"] = chkREQUIRED.Checked ? 1 : 0;

                DataRow[] aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                // 03/30/2007  .  Always allow editing of the last empty row. Add blank row if necessary. 
                if (aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length - 1]["FIELD_NAME"]))
                {
                    DataRow rowNew = dtFields.NewRow();
                    dtFields.Rows.Add(rowNew);
                    aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                }

                ViewState["Fields"] = dtFields;
                grdMain.DataSource = dtFields;
                // 03/30/2007  .  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
                grdMain.EditIndex = aCurrentRows.Length - 1;
                grdMain.DataBind();
            }
        }

        protected void grdMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grdMain.EditIndex = -1;
            if (dtFields != null)
            {
                DataRow[] aCurrentRows = dtFields.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
                grdMain.DataSource = dtFields;
                // 03/15/2007  .  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
                grdMain.EditIndex = aCurrentRows.Length - 1;
                grdMain.DataBind();
            }
        }

        #endregion
    }
}