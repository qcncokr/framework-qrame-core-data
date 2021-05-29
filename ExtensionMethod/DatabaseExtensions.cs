using Qrame.CoreFX.ExtensionMethod;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrame.CoreFX.Data.ExtensionMethod
{
	public static partial class DatabaseExtensions
	{
		public static DataSet ExecuteDataSet(this DbCommand @this, DatabaseFactory databaseFactory)
		{
			var ds = new DataSet();

			if (databaseFactory.SqlFactory == null)
			{
				ds = null;
			}
			else
			{
				DbDataAdapter dataAdapter = databaseFactory.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = @this;
				dataAdapter.Fill(ds);
			}

			return ds;
		}

		public static DataTable ExecuteDataTable(this DbCommand @this, DatabaseFactory databaseFactory)
		{
			var dt = new DataTable();

			if (databaseFactory.SqlFactory == null)
			{
				dt = null;
			}
			else
			{
				DbDataAdapter dataAdapter = databaseFactory.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = @this;
				dataAdapter.Fill(dt);
			}

			return dt;
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction = null)
		{
			using (var command = @this.Connection.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;

				if (transaction != null)
				{
					command.Transaction = transaction;
				}

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				var ds = new DataSet();
				DbDataAdapter dataAdapter = @this.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = command;
				dataAdapter.Fill(ds);

				return ds;
			}
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, Action<DbCommand> commandFactory)
		{
			using (var command = @this.Connection.CreateCommand())
			{
				commandFactory(command);

				var ds = new DataSet();
				DbDataAdapter dataAdapter = @this.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = command;
				dataAdapter.Fill(ds);

				return ds;
			}
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText)
		{
			return @this.ExecuteDataSet(cmdText, null, CommandType.Text, null);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, DbTransaction transaction)
		{
			return @this.ExecuteDataSet(cmdText, null, CommandType.Text, transaction);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, CommandType commandType)
		{
			return @this.ExecuteDataSet(cmdText, null, commandType, null);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, CommandType commandType, DbTransaction transaction)
		{
			return @this.ExecuteDataSet(cmdText, null, commandType, transaction);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, DbParameter[] parameters)
		{
			return @this.ExecuteDataSet(cmdText, parameters, CommandType.Text, null);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
		{
			return @this.ExecuteDataSet(cmdText, parameters, CommandType.Text, transaction);
		}

		public static DataSet ExecuteDataSet(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, CommandType commandType)
		{
			return @this.ExecuteDataSet(cmdText, parameters, commandType, null);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
		{
			using (var command = @this.Connection.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;

				if (transaction != null)
				{
					command.Transaction = transaction;
				}

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				var ds = new DataSet();
				DbDataAdapter dataAdapter = @this.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = command;
				dataAdapter.Fill(ds);

				return ds.Tables.Count == 0 ? null : ds.Tables[0];
			}
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, Action<DbCommand> commandFactory)
		{
			using (var command = @this.Connection.CreateCommand())
			{
				commandFactory(command);

				var ds = new DataSet();
				DbDataAdapter dataAdapter = @this.SqlFactory.CreateDataAdapter();
				dataAdapter.SelectCommand = command;
				dataAdapter.Fill(ds);

				return ds.Tables[0];
			}
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText)
		{
			return @this.ExecuteDataTable(cmdText, null, CommandType.Text, null);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, DbTransaction transaction)
		{
			return @this.ExecuteDataTable(cmdText, null, CommandType.Text, transaction);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, CommandType commandType)
		{
			return @this.ExecuteDataTable(cmdText, null, commandType, null);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, CommandType commandType, DbTransaction transaction)
		{
			return @this.ExecuteDataTable(cmdText, null, commandType, transaction);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, DbParameter[] parameters)
		{
			return @this.ExecuteDataTable(cmdText, parameters, CommandType.Text, null);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
		{
			return @this.ExecuteDataTable(cmdText, parameters, CommandType.Text, transaction);
		}

		public static DataTable ExecuteDataTable(this DatabaseFactory @this, string cmdText, DbParameter[] parameters, CommandType commandType)
		{
			return @this.ExecuteDataTable(cmdText, parameters, commandType, null);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction) where T : new()
		{
			using (DbCommand command = @this.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;
				command.Transaction = transaction;

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (IDataReader reader = command.ExecuteReader())
				{
					return reader.ToEntities<T>();
				}
			}
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, Action<DbCommand> commandFactory) where T : new()
		{
			using (DbCommand command = @this.CreateCommand())
			{
				commandFactory(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					return reader.ToEntities<T>();
				}
			}
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, null, CommandType.Text, null);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, null, CommandType.Text, transaction);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, CommandType commandType) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, null, commandType, null);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, null, commandType, transaction);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, parameters, CommandType.Text, null);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, parameters, CommandType.Text, transaction);
		}

		public static IEnumerable<T> ExecuteEntities<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType) where T : new()
		{
			return @this.ExecuteEntities<T>(cmdText, parameters, commandType, null);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction) where T : new()
		{
			using (DbCommand command = @this.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;
				command.Transaction = transaction;

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (IDataReader reader = command.ExecuteReader())
				{
					reader.Read();
					return reader.ToEntity<T>();
				}
			}
		}

		public static T ExecuteEntity<T>(this DbConnection @this, Action<DbCommand> commandFactory) where T : new()
		{
			using (DbCommand command = @this.CreateCommand())
			{
				commandFactory(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					reader.Read();
					return reader.ToEntity<T>();
				}
			}
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, null, CommandType.Text, null);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, null, CommandType.Text, transaction);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, CommandType commandType) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, null, commandType, null);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, null, commandType, transaction);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, DbParameter[] parameters) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, parameters, CommandType.Text, null);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, parameters, CommandType.Text, transaction);
		}

		public static T ExecuteEntity<T>(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType) where T : new()
		{
			return @this.ExecuteEntity<T>(cmdText, parameters, commandType, null);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
		{
			using (DbCommand command = @this.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;
				command.Transaction = transaction;

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (IDataReader reader = command.ExecuteReader())
				{
					reader.Read();
					return reader.ToExpandoObject();
				}
			}
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, Action<DbCommand> commandFactory)
		{
			using (DbCommand command = @this.CreateCommand())
			{
				commandFactory(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					reader.Read();
					return reader.ToExpandoObject();
				}
			}
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText)
		{
			return @this.ExecuteExpandoObject(cmdText, null, CommandType.Text, null);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObject(cmdText, null, CommandType.Text, transaction);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, CommandType commandType)
		{
			return @this.ExecuteExpandoObject(cmdText, null, commandType, null);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObject(cmdText, null, commandType, transaction);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters)
		{
			return @this.ExecuteExpandoObject(cmdText, parameters, CommandType.Text, null);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObject(cmdText, parameters, CommandType.Text, transaction);
		}

		public static dynamic ExecuteExpandoObject(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType)
		{
			return @this.ExecuteExpandoObject(cmdText, parameters, commandType, null);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType, DbTransaction transaction)
		{
			using (DbCommand command = @this.CreateCommand())
			{
				command.CommandText = cmdText;
				command.CommandType = commandType;
				command.Transaction = transaction;

				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}

				using (IDataReader reader = command.ExecuteReader())
				{
					return reader.ToExpandoObjects();
				}
			}
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, Action<DbCommand> commandFactory)
		{
			using (DbCommand command = @this.CreateCommand())
			{
				commandFactory(command);

				using (IDataReader reader = command.ExecuteReader())
				{
					return reader.ToExpandoObjects();
				}
			}
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText)
		{
			return @this.ExecuteExpandoObjects(cmdText, null, CommandType.Text, null);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObjects(cmdText, null, CommandType.Text, transaction);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, CommandType commandType)
		{
			return @this.ExecuteExpandoObjects(cmdText, null, commandType, null);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, CommandType commandType, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObjects(cmdText, null, commandType, transaction);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, DbParameter[] parameters)
		{
			return @this.ExecuteExpandoObjects(cmdText, parameters, CommandType.Text, null);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, DbParameter[] parameters, DbTransaction transaction)
		{
			return @this.ExecuteExpandoObjects(cmdText, parameters, CommandType.Text, transaction);
		}

		public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbConnection @this, string cmdText, DbParameter[] parameters, CommandType commandType)
		{
			return @this.ExecuteExpandoObjects(cmdText, parameters, commandType, null);
		}

		public static string ParameterValueForSQL(this DbParameter @this)
		{
			object paramValue = @this.Value;

			if (paramValue == null)
			{
				return "NULL";
			}

			switch (@this.DbType)
			{
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Time:
				case DbType.Xml:
				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return $"'{paramValue.ToString().Replace("'", "''")}'";
				case DbType.Boolean:
					return (paramValue.ToBooleanOrDefault(false)) ? "1" : "0";
				case DbType.Decimal:
					return ((decimal)paramValue).ToString(CultureInfo.InvariantCulture).Replace("'", "''");
				case DbType.Double:
					return ((double)paramValue).ToString(CultureInfo.InvariantCulture).Replace("'", "''");
				default:
					return paramValue.ToString().Replace("'", "''");
			}
		}

		public static string CommandAsSql(this DbCommand @this, string providerName = "")
		{
			var sql = new StringBuilder();

			switch (@this.CommandType)
			{
				case CommandType.Text:
					@this.CommandAsSql_Text(sql, providerName);
					break;

				case CommandType.StoredProcedure:
					@this.CommandAsSql_StoredProcedure(sql, providerName);
					break;
			}

			return sql.ToString();
		}

		private static void CommandAsSql_Text(this DbCommand @this, StringBuilder sql, string providerName)
		{
			string query = @this.CommandText;
			string parameterFlag = "";

			if (providerName.IndexOf("SqlClient") > -1)
			{
				parameterFlag = "@";
			}
			else if (providerName.IndexOf("Oracle") > -1)
			{
				parameterFlag = ":";
			}
			else if (providerName.IndexOf("MySql") > -1)
			{
				parameterFlag = ":";
			}
			else if (providerName.IndexOf("Npgsql") > -1)
			{
				parameterFlag = "@";
			}
			else if (providerName.IndexOf("OleDb") > -1)
			{
				parameterFlag = "@";
			}
			else if (providerName.IndexOf("Odbc") > -1)
			{
				parameterFlag = "@";
			}
			else if (providerName.IndexOf("SQLite") > -1)
			{
				parameterFlag = "@";
			}

			foreach (DbParameter p in @this.Parameters)
			{
				query = Regex.Replace(query, "\\B" + parameterFlag + p.ParameterName + "\\b", p.ParameterValueForSQL()); //the first one is \B, the 2nd one is \b, since ParameterName starts with @ which is a non-word character in RegEx (see https://stackoverflow.com/a/2544661)
			}

			sql.AppendLine(query);
		}

		private static void CommandAsSql_StoredProcedure(this DbCommand @this, StringBuilder sql, string providerName)
		{
			if (providerName.IndexOf("SqlClient") > -1)
			{
				sql.AppendLine("declare @return_value int;");

				foreach (DbParameter sp in @this.Parameters)
				{
					if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
					{
						sql.Append("declare ").Append(sp.ParameterName).Append("\t").Append(sp.DbType.ToString()).Append("\t= ");

						sql.Append((sp.Direction == ParameterDirection.Output) ? "null" : sp.ParameterValueForSQL()).AppendLine(";");
					}
				}

				sql.Append("exec [").Append(@this.CommandText).AppendLine("]");

				bool FirstParam = true;
				foreach (DbParameter param in @this.Parameters)
				{
					if (param.Direction != ParameterDirection.ReturnValue)
					{
						sql.Append((FirstParam) ? "\t" : "\t, ");

						if (FirstParam)
							FirstParam = false;

						if (param.Direction == ParameterDirection.Input)
						{
							sql.Append(param.ParameterName).Append(" = ").AppendLine(param.ParameterValueForSQL());
						}
						else
						{
							sql.Append(param.ParameterName).Append(" = ").Append(param.ParameterName).AppendLine(" output");
						}
					}
				}
				sql.AppendLine(";");

				sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

				foreach (DbParameter sp in @this.Parameters)
				{
					if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
					{
						sql.Append("select '").Append(sp.ParameterName).Append("' = convert(varchar, ").Append(sp.ParameterName).AppendLine(");");
					}
				}
			}
			else
			{
				// 데이터 제공자에 적절한 스토어드 프로시져 작성
			}
		}
	}
}
