-- 01/19/2010 Paul.  Don't create the audit tables on an Offline Client database. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'SYSTEM_SYNC_CONFIG' and TABLE_TYPE = 'BASE TABLE') begin -- then
	exec dbo.spSqlBuildAuditTable   '$tablename$';
	exec dbo.spSqlBuildAuditTrigger '$tablename$';
	exec dbo.spSqlBuildAuditView    '$tablename$';

		exec dbo.spSqlBuildAuditTable'$tablename$_CSTM';
	exec dbo.spSqlBuildAuditTrigger '$tablename$_CSTM';
	--exec dbo.spSqlBuildAuditView    '$tablename$_CSTM';
end -- if;
GO


