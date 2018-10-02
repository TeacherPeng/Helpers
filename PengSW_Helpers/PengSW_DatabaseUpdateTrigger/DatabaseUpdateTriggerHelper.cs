using System.Data.SqlClient;
using System.Text;

namespace PengSW.DatabaseUpdateTrigger
{
    /// <summary>
    /// 创建基于SqlConnection的表更新触发器和更新记录表
    ///     通常在创建数据库时，调用CreateUpdateRecord方法在数据库指定的表上创建触发器，将指定表的更新操作记录到指定的更新记录表中。
    ///     更新记录表随时保持一条记录，每个需要记录更新时间的表在更新记录表中占一个字段，以被更新的表名作为记录表中的字段名。
    /// </summary>
    public static class DatabaseUpdateTriggerHelper
    {
        //CREATE TABLE [dbo].[updaterecord](
        //    [Schedules] [datetime] NOT NULL,
        //    [Actions] [datetime] NOT NULL,
        //    [Targets] [datetime] NOT NULL
        //) ON [PRIMARY]
        //INSERT INTO [dbo].[updaterecord]
        //([Schedules],[Actions],[Targets])
        //VALUES('1900-1-1 0:0:0', '1900-1-1 0:0:0', '1900-1-1 0:0:0')
        public static string Sql_CreateUpdateTable(string aUpdateTableName, params string[] aTargetTableNames)
        {
            StringBuilder aSqlBuiler = new StringBuilder();
            aSqlBuiler.Append("CREATE TABLE [dbo].[");
            aSqlBuiler.Append(aUpdateTableName);
            aSqlBuiler.Append("](");
            foreach (string aTableName in aTargetTableNames) aSqlBuiler.Append(string.Format("[{0}] [datetime] NOT NULL, ", aTableName));
            aSqlBuiler.Remove(aSqlBuiler.Length - 2, 2);
            aSqlBuiler.AppendLine(") ON [PRIMARY]");
            aSqlBuiler.Append("INSERT INTO [dbo].[");
            aSqlBuiler.Append(aUpdateTableName);
            aSqlBuiler.Append("] (");
            foreach (string aTableName in aTargetTableNames) aSqlBuiler.Append(string.Format("[{0}],", aTableName));
            aSqlBuiler.Remove(aSqlBuiler.Length - 1, 1);
            aSqlBuiler.Append(") VALUES(");
            foreach (string aTableName in aTargetTableNames) aSqlBuiler.Append("'1900-1-1 0:0:0',");
            aSqlBuiler.Remove(aSqlBuiler.Length - 1, 1);
            aSqlBuiler.AppendLine(")");
            return aSqlBuiler.ToString();
        }

        //@"CREATE TRIGGER RecordSchedulesUpdate
        //ON [Schedules]
        //AFTER INSERT, DELETE, UPDATE
        //AS
        //UPDATE [updaterecord] SET Schedules=GETDATE()");
        public static string Sql_CreateUpdateTrigger(string aUpdateTableName, string aTargetTableName)
        {
            StringBuilder aSqlBuilder = new StringBuilder();
            aSqlBuilder.Append("CREATE TRIGGER [Record");
            aSqlBuilder.Append(aTargetTableName);
            aSqlBuilder.Append("Update] ON [");
            aSqlBuilder.Append(aTargetTableName);
            aSqlBuilder.Append("] AFTER INSERT, DELETE, UPDATE AS UPDATE [");
            aSqlBuilder.Append(aUpdateTableName);
            aSqlBuilder.Append("] SET [");
            aSqlBuilder.Append(aTargetTableName);
            aSqlBuilder.Append("]=GETDATE()");
            return aSqlBuilder.ToString();
        }

        public static void CreateUpdateRecord(this SqlConnection aConnection, string aUpdateTableName, params string[] aTargetTableNames)
        {
            SqlCommand aSqlCommand = aConnection.CreateCommand();
            aSqlCommand.CommandText = Sql_CreateUpdateTable(aUpdateTableName, aTargetTableNames);
            aSqlCommand.ExecuteNonQuery();
            foreach (string aTableName in aTargetTableNames)
            {
                aSqlCommand.CommandText = Sql_CreateUpdateTrigger(aUpdateTableName, aTableName);
                aSqlCommand.ExecuteNonQuery();
            }
        }
    }
}
