if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '$tablename$_AUDIT' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.$tablename$_AUDIT';
	Create Table dbo.$tablename$_AUDIT
	    ( AUDIT_ID                    uniqueidentifier   not null default(newid()) constraint PKA_$tablename$ primary key
	    , AUDIT_ACTION                int                not null
	    , AUDIT_DATE                  datetime           not null
	    , AUDIT_VERSION               rowversion         not null
	    , AUDIT_COLUMNS               varbinary(128)     null
	    , AUDIT_TOKEN                 varchar(255)       null
		,ID                                 uniqueidentifier  null 
		, DELETED                            bit  null 
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime  null 
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime  null 
		, DATE_MODIFIED_UTC                  datetime null 

		$createtablefields$
		)

$createtableindexes$
  end
GO
