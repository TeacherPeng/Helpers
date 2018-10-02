using System;
using System.Data.SqlClient;
using System.Linq;

namespace PengSW.DatabaseUpdateTrigger
{
    public class DatabaseUpdateCheck
    {
        public DatabaseUpdateCheck(string aConnectionString, string aUpdateTableName)
        {
            _ConnectionString = aConnectionString;
            _UpdateTableName = aUpdateTableName;
            _SelectSql = string.Format("SELECT * FROM [{0}]", _UpdateTableName);
        }

        class TableTimeStamp
        {
            public TableTimeStamp(string aTableName, DateTime aUpdateTime) { TableName = aTableName; UpdateTime = aUpdateTime; }
            public string TableName;
            public DateTime UpdateTime;
        }

        public string[] GetUpdatedTableNames()
        {
            // 读更新记录表的第一条记录（约定只用第一条记录）
            string[] aUpdatedTableNames = null;
            using (SqlConnection aSqlConnection = new SqlConnection(_ConnectionString))
            {
                aSqlConnection.Open();
                aUpdatedTableNames = GetUpdatedTableNames(aSqlConnection);
                aSqlConnection.Close();
            }
            return aUpdatedTableNames;
        }

        public string[] GetUpdatedTableNames(SqlConnection aSqlConnection)
        {
            // 读更新记录表的第一条记录（约定只用第一条记录）
            TableTimeStamp[] aUpdateRecord = null;
            SqlCommand aSqlCommand = aSqlConnection.CreateCommand();
            aSqlCommand.CommandText = _SelectSql;
            SqlDataReader aSqlDataReader = aSqlCommand.ExecuteReader();
            if (aSqlDataReader.Read())
            {
                aUpdateRecord = new TableTimeStamp[aSqlDataReader.FieldCount];
                for (int i = 0; i < aSqlDataReader.FieldCount; i++)
                {
                    aUpdateRecord[i] = new TableTimeStamp(aSqlDataReader.GetName(i), aSqlDataReader.GetDateTime(i));
                }
            }

            // 收集有更新的数据表名
            string[] aUpdatedTableNames = null;
            if (aUpdateRecord == null) return null;
            if (_LastUpdateRecord == null)
                aUpdatedTableNames = (from r in aUpdateRecord select r.TableName).ToArray();
            else
                aUpdatedTableNames = (from r in aUpdateRecord let r0 = (from x in _LastUpdateRecord where x.TableName == r.TableName select x).FirstOrDefault() where r0 != null && r.UpdateTime != r0.UpdateTime select r.TableName).ToArray();
            if (aUpdatedTableNames != null && aUpdatedTableNames.Length == 0) aUpdatedTableNames = null;

            // 保存最新的更新记录
            _LastUpdateRecord = aUpdateRecord;

            return aUpdatedTableNames;
        }

        private string _ConnectionString;
        private string _UpdateTableName;
        private string _SelectSql;
        private TableTimeStamp[] _LastUpdateRecord;
    }
}
