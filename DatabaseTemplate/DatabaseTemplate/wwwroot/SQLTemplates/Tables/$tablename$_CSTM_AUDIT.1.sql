if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '$tablename$_CSTM_AUDIT' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.$tablename$_CSTM_AUDIT';
	Create Table dbo.$tablename$_CSTM_AUDIT
	    ( AUDIT_ID                    uniqueidentifier   not null default(newid()) constraint PKA_$tablename$ primary key
	    , AUDIT_ACTION                int                not null
	    , AUDIT_DATE                  datetime           not null
	    , AUDIT_VERSION               rowversion         not null
	    , AUDIT_COLUMNS               varbinary(128)     null
	    , AUDIT_TOKEN                 varchar(255)       null
		,ID_C                         uniqueidentifier   null 
	

		)

$createtableindexes$
  end
GO
