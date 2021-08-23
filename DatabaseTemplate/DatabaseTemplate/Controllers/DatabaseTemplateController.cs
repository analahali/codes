using Dapper;
using InformationCard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SplendidCRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseTemplateController : ControllerBase
    {
        private TemolateDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DatabaseTemplateController(TemolateDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        [HttpGet]
        public IActionResult Create()
        {
            using (IDbConnection db = _context.Database.GetDbConnection())
            {
                int f = 0;
                int s = 0;
                bool bINCLUDE_ASSIGNED_USER_ID = true;
                bool bINCLUDE_TEAM_ID = true;
                bool CUSTOM_ENABLED = true;

                string sTABLE_NAME = "ABC".ToUpper();
                string sFIELD_NAME = "comment";
                string sDATA_TYPE = "Text";
                int nMAX_SIZE = 500;

                string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = _webHostEnvironment.ContentRootPath;

                string sSqlTemplatesPath = "";
                sSqlTemplatesPath = Path.Combine(webRootPath, "SQLTemplates");

                string[] arrSqlFolders = Directory.GetDirectories(sSqlTemplatesPath);
                Encoding enc = Encoding.UTF8;
                foreach (string sSqlFolder in arrSqlFolders)
                {
                    f++;
                    if (true)
                    {

                    }
                    string sqlData = " " ;
                 
                    string[] arrSqlFolderParts = sSqlFolder.Split(Path.DirectorySeparatorChar);
                    string sFolder = arrSqlFolderParts[arrSqlFolderParts.Length - 1];
                   
                    string[] arrSqlTemplates = Directory.GetFiles(sSqlFolder, "*.sql");
                   
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
                    bool bREQUIRED = false;
                    foreach (string sSqlTemplate in arrSqlTemplates)
                    {
                        s++;
                   
                        string sSqlScriptName = Path.GetFileName(sSqlTemplate);
                        
                        
                        using (var sr = new StreamReader(sSqlTemplate, enc, true))
                        {
                            string sData = sr.ReadToEnd();
                            string tableName = sTABLE_NAME;
                            if (sSqlTemplate.EndsWith("$tablename$_AUDIT.1.sql"))
                            {
                                tableName += "_AUDIT";
                            }
                            else if (sSqlTemplate.EndsWith("$tablename$_CSTM.1.sql"))
                            {
                                tableName += "_CSTM";

                            }
                            var sbCreateTableFields = new StringBuilder();
                            var sbCreateTableIndexes = new StringBuilder();
                            sData = sData.Replace("$tablename$", sTABLE_NAME);

                            if (bINCLUDE_ASSIGNED_USER_ID)
                            {
                                sbCreateTableFields.AppendLine("		, ASSIGNED_USER_ID                   uniqueidentifier null");
                                sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName + "_ASSIGNED_USER_ID on dbo." +
                                                                tableName + " (ASSIGNED_USER_ID, DELETED, ID)");

                               }
                            if (bINCLUDE_TEAM_ID)
                            {
                                sbCreateTableFields.AppendLine("		, TEAM_ID                            uniqueidentifier null");

                                sbCreateTableFields.AppendLine("		, TEAM_SET_ID                        uniqueidentifier null");

                                if (bINCLUDE_ASSIGNED_USER_ID)
                                {
                                    sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName +
                                                                    "_TEAM_ID          on dbo." + tableName +
                                                                    " (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)");
                                    sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName +
                                                                    "_TEAM_SET_ID      on dbo." + tableName +
                                                                    " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
                                    sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName +
                                                                    "_TEAM_SET_ID      on dbo." + tableName +
                                                                    " (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)");
                                   
                                }
                                else
                                {
                                    sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName +
                                                                    "_TEAM_ID          on dbo." + tableName +
                                                                    " (TEAM_ID, DELETED, ID)");
                                    sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName +
                                                                    "_TEAM_SET_ID      on dbo." + tableName +
                                                                    " (TEAM_SET_ID, DELETED, ID)");
                                }

                              }
                            sbCreateTableFields.AppendLine("		, " + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) +
                                                      sSQL_DATA_TYPE + " " + (bREQUIRED ? "not null" : "null"));
                            if (sFIELD_NAME == "NAME" || sFIELD_NAME == "TITLE")
                                sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName + "_" + sFIELD_NAME +
                                                                "  on dbo." + tableName + " (" + sFIELD_NAME +
                                                                ", DELETED, ID)");
                            ////id indexing
                            //if (tableName.PadRight(5) == "_CSTM")
                            //{
                            //    sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID, DELETED)");

                            //}
                            //else if (tableName.PadRight(11) == "_CSTM_AUDIT")
                            //{
                            //    sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID_C, AUDIT_TOKEN, AUDIT_ACTION)");

                            //}
                            //else if (tableName.PadRight(6) == "_AUDIT")
                            //{
                            //    sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID, AUDIT_TOKEN, AUDIT_ACTION)");

                            //}
                            //else
                            //{

                            //}
                            ////---------------------
                            sData = sData.Replace("$createtablefields$", sbCreateTableFields.ToString());
                            sData = sData.Replace("$createtableindexes$", sbCreateTableIndexes.ToString());


                             if (sSqlTemplate.EndsWith("BuildAuditTable_$tablename$.1.sql"))
                            {
                                if (CUSTOM_ENABLED)
                                    sData += sData.Replace("$tablename$", "$tablename$_CSTM");
                            }



                            sqlData += sData;
                            


                        }
                    }
                    System.IO.File.WriteAllText(sSqlTemplatesPath + @"\Tables\data.sql", sqlData.ToString());

                    var script = System.IO.File.ReadAllText(sSqlTemplatesPath + @"\Tables\data.sql").Split("GO");
                    string sAudit = "";
                    string sTable = "";
                    foreach (var scpt in script)
                    {
                         //var sqlAudit = scpt.Replace(sTABLE_NAME, sTABLE_NAME+"_AUDIT");
                        sTable +=  scpt+"\n ";
                        //sAudit += sqlAudit + "\n ";

                        
                    }
                    sTable += sTable + "\n "+ sAudit;
                    var tbl = db.Execute(sTable);
                    return Ok(f + " " + s);
                }

                 
                return Ok();
            }

            
        }

        protected string CreateTrigger(string tableName, IDbConnection db)
        {
            string result = "";
            var tableColumn = db.Query<dynamic>(@"SELECT COLUMN_NAME,DATA_TYPE

                                            FROM INFORMATION_SCHEMA.COLUMNS

                                            WHERE TABLE_NAME = tableName

                                            ORDER BY ORDINAL_POSITION", new { tableName = tableName });
           
            return result;
        }
        protected string CreateAuditTableFields(string TABLE_NAME)
        {
            string result = "";
            int COLUMN_MAX_LENGTH = 20;
            string AUDIT_PK = "PKA_" + TABLE_NAME;
            string CRLF = "\r"+"\n";
              result = result + "	( AUDIT_ID" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_ID".Length)   + "uniqueidentifier   not null default(newid()) constraint " + @AUDIT_PK + " primary key" + CRLF;
            result = result + "	, AUDIT_ACTION" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_ACTION".Length) + "int                not null" + CRLF;
              result = result + "	, AUDIT_DATE" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_DATE".Length) + "datetime           not null" + CRLF;
              result = result + "	, AUDIT_VERSION" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_VERSION".Length) + "rowversion         not null" + CRLF;
              result = result + "	, AUDIT_COLUMNS" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_COLUMNS".Length) + "varbinary(128)     null" + CRLF;
              result = result + "	, AUDIT_TOKEN" + new String(' ', COLUMN_MAX_LENGTH + 1 - "AUDIT_TOKEN".Length) + "varchar(255)       null" + CRLF;

            //index
            result += " $createtablefields$ ";
            result += " )";
              result = "create index IDX_" + TABLE_NAME + " on dbo." + TABLE_NAME + "(ID_C, AUDIT_TOKEN, AUDIT_ACTION)";
             
              result = "create index IDX_" + TABLE_NAME + " on dbo." + TABLE_NAME + "(ID, AUDIT_VERSION, AUDIT_TOKEN)";
            return result;
        }
        //[HttpGet]
        //[Route("CreateTrigger")]
        //public IActionResult CreateTrigger()
        //{
        //    return Ok();
        //}

    }
}
