 if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ABC' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.ABC';
	Create Table dbo.ABC
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_ABC primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, TEAM_ID                            uniqueidentifier null
		, TEAM_SET_ID                        uniqueidentifier null
		, comment                            nvarchar(500) null

		)

	create index IDX_ABC_ASSIGNED_USER_ID on dbo.ABC (ASSIGNED_USER_ID, DELETED, ID)
	create index IDX_ABC_TEAM_ID          on dbo.ABC (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)
	create index IDX_ABC_TEAM_SET_ID      on dbo.ABC (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)

  end
GO


if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ABC_AUDIT' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.ABC_AUDIT';
	Create Table dbo.ABC_AUDIT
	    ( AUDIT_ID                    uniqueidentifier   not null default(newid()) constraint PKA_ABC primary key
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

				, ASSIGNED_USER_ID                   uniqueidentifier null
		, TEAM_ID                            uniqueidentifier null
		, TEAM_SET_ID                        uniqueidentifier null
		, comment                            nvarchar(500) null

		)

	create index IDX_ABC_AUDIT_ASSIGNED_USER_ID on dbo.ABC_AUDIT (ASSIGNED_USER_ID, DELETED, ID)
	create index IDX_ABC_AUDIT_TEAM_ID          on dbo.ABC_AUDIT (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID)
	create index IDX_ABC_AUDIT_TEAM_SET_ID      on dbo.ABC_AUDIT (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID)

  end
GO
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ABC_CSTM' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.ABC_CSTM';
	Create Table dbo.ABC_CSTM
		( ID_C                               uniqueidentifier not null constraint PK_ABC_CSTM primary key
		)
  end
GO


