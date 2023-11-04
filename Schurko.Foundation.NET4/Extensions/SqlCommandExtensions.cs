
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class SqlCommandExtensions
    {
        public static string AsString(this SqlParameterCollection col)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (col != null)
            {
                foreach (SqlParameter sqlParameter in (DbParameterCollection)col)
                {
                    if (sqlParameter.Direction != ParameterDirection.ReturnValue)
                    {
                        if (stringBuilder.Length > 0)
                            stringBuilder.Append(", \r\n");
                        string format = "{0}={1}";
                        switch (sqlParameter.SqlDbType)
                        {
                            case SqlDbType.Char:
                            case SqlDbType.DateTime:
                            case SqlDbType.NChar:
                            case SqlDbType.NText:
                            case SqlDbType.NVarChar:
                            case SqlDbType.Text:
                            case SqlDbType.VarChar:
                            case SqlDbType.Xml:
                            case SqlDbType.Date:
                            case SqlDbType.DateTime2:
                                format = "{0}='{1}'";
                                break;
                        }
                        object obj = sqlParameter.Value;
                        if (obj == DBNull.Value)
                            format = "{0}=null";
                        if (obj is bool)
                            obj = (bool)obj ? 1 : 0;
                        if (sqlParameter.Direction == ParameterDirection.InputOutput || sqlParameter.Direction == ParameterDirection.Output)
                            format = string.Format("out {0}", format);
                        stringBuilder.AppendFormat(format, sqlParameter.ParameterName, obj);
                    }
                }
            }
            if (stringBuilder.Length > 0)
                stringBuilder.Append("\r\n");
            return stringBuilder.ToString();
        }

        public static bool SafeAdd(
          this SqlParameterCollection col,
          string paramName,
          object paramValue)
        {
            if (col == null || col.Count == 0)
                return false;
            if (!paramName.StartsWith("@"))
                paramName = "@" + paramName;
            bool flag = false;
            if (col.Contains(paramName))
            {
                col[paramName].Value = paramValue;
                flag = true;
            }
            return flag;
        }

        public static string AsSQL(this SqlCommand cmd)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string format = "EXEC {0}\r\n {1}";
            stringBuilder.AppendFormat("-- {0}\r\n --{1}\r\n", cmd.Connection.ConnectionString, cmd.CommandType);
            stringBuilder.AppendFormat("USE {0}\r\n", cmd.Connection.Database);
            if (cmd.Parameters.Contains("@RETURN_VALUE"))
            {
                stringBuilder.Append("DECLARE @RETURN_VALUE int\r\n");
                format = "EXEC\t@return_value = {0}\r\n {1}\r\nSELECT\t'Return Value' = @return_value";
            }
            stringBuilder.AppendFormat(format, cmd.CommandText, cmd.Parameters.AsString());
            return stringBuilder.ToString();
        }

        public static int ReturnValue(this SqlCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd), "The command object is null, cannot call the Extension method ReturnValue()");
            return cmd.CommandType != CommandType.StoredProcedure || !cmd.Parameters.Contains("@RETURN_VALUE") ? 0 : (int)cmd.Parameters["@RETURN_VALUE"].Value;
        }

        public static void ValidateReturnValue(this SqlCommand cmd)
        {
            int num = cmd.ReturnValue();
            if (num != 0)
                throw new Exception(string.Format("Sql Error code returned for {0} - [{1}]\n\rSQL - {2}", cmd.CommandText, num, cmd.AsSQL()));
        }

        public static int GetIdentifier(this SqlCommand cmd, string identifierField)
        {
            if (string.IsNullOrEmpty(identifierField))
                throw new ArgumentNullException(nameof(identifierField));
            if (!cmd.Parameters.Contains(identifierField))
                throw new IndexOutOfRangeException("The specified field is not contained within the Command.Parameters collection : " + identifierField);
            if (!identifierField.StartsWith("@"))
                identifierField = "@" + identifierField;
            object obj = cmd.Parameters[identifierField].Value;
            return obj != DBNull.Value ? (int)obj : throw new InvalidOperationException("Idenfifier field NULL");
        }
    }
}
