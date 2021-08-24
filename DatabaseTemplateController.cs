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
           //return Ok( CreateTrigger("[tr$tablename$_Ins_AUDIT]", "[$tablename$]", "$tablename$_AUDIT", "COMMENT", "ID"));
            string sqlDataFull = " ";
            string sqlTriggerDataFull = "";
            string[] arrSQLFolderTypes =
                    {
                        "BaseTables", "Tables", "Views", "Procedures", "Triggers", "Data",
                        "Terminology"
                    };
            using (IDbConnection db = _context.Database.GetDbConnection())
            {
                int f = 0;
                int s = 0;
               
                bool CUSTOM_ENABLED = true;

                string sTABLE_NAME = "ABC".ToUpper();
                string sFIELD_NAME = "comment".ToUpper();
                string sDATA_TYPE = "Text";
                int nMAX_SIZE = 500;

                string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = _webHostEnvironment.ContentRootPath;

                string sSqlTemplatesPath = "";
                sSqlTemplatesPath = Path.Combine(webRootPath, "SQLTemplates");

                string[] arrSqlFolders = Directory.GetDirectories(sSqlTemplatesPath);
                var c = arrSqlFolders.Length;
                var t = arrSqlFolders[1];
                Encoding enc = Encoding.UTF8;
                foreach (string sSqlFolder in arrSqlFolders)
                {
                    f++;
                    
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
                            var sbCreateTableFields = new StringBuilder();
                            var sbCreateTriggerFields = new StringBuilder();
                            var sbCreateTriggerFieldsData = new StringBuilder();
                            var sbCreateTableIndexes = new StringBuilder();
                            if (sFolder=="Triggers")
                            {
                                if (sSqlTemplate.EndsWith("tr$tablename$_Ins_AUDIT.1.sql"))
                                {
                                    sbCreateTriggerFields.AppendLine(", " + sFIELD_NAME);
                                    sbCreateTriggerFieldsData.AppendLine(", " + sTABLE_NAME + "_AUDIT" + "." + sFIELD_NAME);
                                    sData = sData.Replace("$tablename$", sTABLE_NAME);
                                    sData = sData.Replace("$columns$", sbCreateTriggerFields.ToString());
                                    sData = sData.Replace("$columnsdata$", sbCreateTriggerFieldsData.ToString());
                                    var tempSData = sData;

                                    sData = sData.Replace("$triggerstage$", "Create");
                                    tempSData = tempSData.Replace("$triggerstage$", "Alter");
                                    sqlData += sData;
                                    sqlData += tempSData;


                                    sqlTriggerDataFull += "\n " + sqlData;
                                    sqlTriggerDataFull += "\n " + tempSData;
                                    continue;
                                }
                            }
                            if (sSqlTemplate.EndsWith("$tablename$_AUDIT.1.sql"))
                            {
                                tableName += "_AUDIT";
                            }
                            else if (sSqlTemplate.EndsWith("$tablename$_CSTM.1.sql"))
                            {
                                tableName += "_CSTM";

                            }else if (sSqlTemplate.EndsWith("$tablename$_CSTM_AUDIT.1.sql"))
                            {
                                tableName += "_CSTM_AUDIT";

                            } 
                            
                            sData = sData.Replace("$tablename$", sTABLE_NAME);

                            
                            
                            sbCreateTableFields.AppendLine("		, " + sFIELD_NAME + Strings.Space(35 - sFIELD_NAME.Length) +
                                                      sSQL_DATA_TYPE + " " + (bREQUIRED ? "not null" : "null"));
                            if (sFIELD_NAME == "NAME" || sFIELD_NAME == "TITLE")
                                sbCreateTableIndexes.AppendLine("	create index IDX_" + tableName + "_" + sFIELD_NAME +
                                                                "  on dbo." + tableName + " (" + sFIELD_NAME +
                                                                ", DELETED, ID)");
                            ////id indexing
                            
                            if (tableName.EndsWith("_CSTM"))
                            {
                                sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID_C)");

                            }
                            else if (tableName.EndsWith("_CSTM_AUDIT"))
                            {
                     
                                sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID_C, AUDIT_TOKEN, AUDIT_ACTION)");

                            }
                            else if (tableName.EndsWith("_AUDIT") && !tableName.EndsWith("_CSTM_AUDIT"))
                            {
                                sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID, AUDIT_TOKEN, AUDIT_ACTION)");

                            }
                            else if (tableName.EndsWith(sTABLE_NAME) )

                            {
                                sbCreateTableIndexes.AppendLine("create index IDX_" + tableName + " on dbo." + tableName + "(ID)");

                            }
                           
                            ////---------------------
                            sData = sData.Replace("$createtablefields$", sbCreateTableFields.ToString());
                            sData = sData.Replace("$createtableindexes$", sbCreateTableIndexes.ToString());


                            



                            sqlData += sData;


                            sqlDataFull += "\n " + sqlData;
                        }
                    }
                    System.IO.File.WriteAllText(sSqlTemplatesPath + @"\Tables\data.sql", sqlData.ToString());

                   // var script = System.IO.File.ReadAllText(sSqlTemplatesPath + @"\Tables\data.sql").Split("GO");
                    var arrSqlData = sqlDataFull.Split("GO");
                    string sAudit = "";
                    string sTable = "";
                    foreach (var scpt in arrSqlData)
                    {
                         //var sqlAudit = scpt.Replace(sTABLE_NAME, sTABLE_NAME+"_AUDIT");
                        sTable +=  scpt+"\n ";
                        //sAudit += sqlAudit + "\n ";

                        
                    }
                    sTable += sTable + "\n "+ sAudit;
                    var tbl = db.Execute(sTable);
                   
                }

                 
                return Ok();
            }

            
        }

        protected string CreateTrigger(string TRIGGER_NAME,string TABLE_NAME,string AUDIT_TABLE,string COLUMN_NAME,string PRIMARY_KEY)
        {
            string Command = "";
            string CRLF = "\r" + "\n";
            Command+= " if not exists(select * from sys.objects where name = $TRIGGER_NAME$ and type = 'TR') begin-- then \n";
            Command = Command + "$triggerstage$ Trigger dbo." + TRIGGER_NAME + " on dbo." + TABLE_NAME + CRLF;
            Command = Command + "for insert" + CRLF;
            Command = Command + "as" + CRLF;
            Command = Command + "  begin" + CRLF;
            Command = Command + "	declare  @BIND_TOKEN varchar(255);" + CRLF;
            Command = Command + "	exec spSqlGetTransactionToken  @BIND_TOKEN out;" + CRLF;
            Command = Command + "	insert into dbo." + AUDIT_TABLE + CRLF;
            Command = Command + "	     ( AUDIT_ID" + CRLF;
            Command = Command + "	     , AUDIT_ACTION" + CRLF;
            Command = Command + "	     , AUDIT_DATE" + CRLF;
            Command = Command + "	     , AUDIT_COLUMNS" + CRLF;
            Command = Command + "	     , AUDIT_TOKEN" + CRLF;

            Command = Command + "	     , " + "$columns$" + CRLF;
            Command = Command + "	     )" + CRLF;
            Command = Command + "	select newid()" + CRLF;
            Command = Command + "	     , 0  -- insert" + CRLF;
            Command = Command + "	     , getdate()" + CRLF;
            Command = Command + "	     , columns_updated()" + CRLF;
            Command = Command + "	     ,  @BIND_TOKEN" + CRLF;
            Command = Command + "	     $columnsdata$" + CRLF;

            Command = Command + "	  from       inserted" + CRLF;
            Command = Command + "	  inner join " + TABLE_NAME + CRLF;
            Command = Command + "	          on " + TABLE_NAME + "." + PRIMARY_KEY + " = inserted." + PRIMARY_KEY + ";" + CRLF;
            return Command;
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
