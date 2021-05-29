using MySql.Data.MySqlClient;

using Npgsql;

using Oracle.ManagedDataAccess.Client;

using Qrame.CoreFX.Configuration.Settings;
using Qrame.CoreFX.Exceptions;
using Qrame.CoreFX.Patterns;

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Qrame.CoreFX.Data
{
	/// <summary>
	/// 닷넷 프레임워크 기반의 응용 프로그램에서 DatabaseFactory를 이용 하여 SQL Server, ORACLE, OLE DB를, 및 ODBC 용
	/// 닷넷 프레임워크의 데이터 공급자의 기능을 동일 하게 제공 하도록 설계되었습니다.
	/// 
	/// 주요 기능으로 다음과 같습니다.
	/// </summary>
	public class DatabaseFactory : IDisposable
	{
		/// <summary>
		/// 데이터 베이스 연결문자열입니다.
		/// </summary>
		private string connectionString;

		/// <summary>
		/// 데이터 베이스 연결문자열입니다.
		/// </summary>
		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		/// <summary>
		/// 데이터베이스에 대한 연결을 나타냅니다.
		/// </summary>
		private DbConnection databaseConnection;

		public DbConnection Connection
		{
			get
			{
				return databaseConnection;
			}
		}

		/// <summary>
		/// 명령을 나타내는 데이터베이스 관련 클래스의 기본 클래스입니다.
		/// </summary>
		private DbCommand databaseCommand;

		/// <summary>
		/// 명령을 나타내는 데이터베이스 관련 클래스의 기본 클래스를 가져옵니다.
		/// </summary>
		public DbCommand Command
		{
			get
			{
				return databaseCommand;
			}
		}

		/// <summary>
		/// 연결 중인 데이터 소스 열거자입니다.
		/// </summary>
		private DataProviders connectionProvider;

		/// <summary>
		/// 연결 중인 데이터 소스 열거자를 가져옵니다.
		/// </summary>
		public DataProviders ConnectionProvider
		{
			get
			{
				return connectionProvider;
			}
		}

		/// <summary>
		/// 쿼리 명령을 수행후 Output 매개 변수를 outputCommand 객체에 저장하는 옵션입니다.
		/// </summary>
		public bool IsOutputParameter = false;

		/// <summary>
		/// 쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장합니다.
		/// </summary>
		private DbCommand outputCommand;

		/// <summary>
		/// 쿼리 명령을 수행후 반환 된 OutputCommand 객체를 가져옵니다.
		/// </summary>
		public DbCommand OutputCommand
		{
			get { return outputCommand; }
		}

		/// <summary>
		/// 닷넷 리소스가 반환되었는지 확인합니다.
		/// </summary>
		private bool isBaseDisposedResources = false;

		/// <summary>
		/// 데이터 소스 클래스의 공급자 구현에 대한 인스턴스를 만드는 데 사용되는 메서드의 집합을 나타냅니다.
		/// </summary>
		public DbProviderFactory SqlFactory = null;

		/// <summary>
		/// 인스턴스 생성시, 응용 프로그램 구성에서 정의한 연결 문자열과 응용 프로그램 구성에서 정의한 기본 제공자에 따라 데이터 소스에 연결 구성을 설정합니다.
		/// </summary>
		public DatabaseFactory()
		{
			DatabaseSetting setting = Singleton<DatabaseSetting>.Instance;
			DataProviders provider = (DataProviders)Enum.Parse(typeof(DataProviders), setting.DataProvider);

			InitializeDatabaseFactory(setting.ConnectionString, provider);
		}

		/// <summary>
		/// 인스턴스 생성시, 지정한 연결 문자열과 응용 프로그램 구성에서 정의한 기본 제공자에 따라 데이터 소스에 연결 구성을 설정합니다.
		/// </summary>
		/// <param name="ConnectionString">Database 연결 문자열입니다.</param>
		public DatabaseFactory(string connectionString)
		{
			DatabaseSetting setting = Singleton<DatabaseSetting>.Instance;
			setting.ConnectionString = connectionString;

			InitializeDatabaseFactory(setting.ConnectionString, DataProviders.SqlServer);
		}

		/// <summary>
		/// 인스턴스 생성시, 지정된 데이터 소스 연결 문자열과 데이터 제공자에 따라 데이터 소스에 연결 구성을 설정합니다.
		/// </summary>
		/// <param name="connectionString">데이터 소스 연결 문자열입니다.</param>
		/// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
		public DatabaseFactory(string connectionString, DataProviders dataProviders = DataProviders.SqlServer)
		{
			InitializeDatabaseFactory(connectionString, dataProviders);
		}

		/// <summary>
		/// 인스턴스 생성시, 지정된 데이터 소스 연결 문자열과 데이터 제공자에 따라 데이터 소스에 연결 구성을 설정합니다.
		/// </summary>
		/// <param name="ConnectionString">데이터 소스 연결 문자열입니다.</param>
		/// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
		private void InitializeDatabaseFactory(string connectionString, DataProviders dataProviders)
		{
			ConnectionString = connectionString;

			switch (dataProviders)
			{
				case DataProviders.SqlServer:
					SqlFactory = SqlClientFactory.Instance;
					break;
				case DataProviders.Oracle:
					SqlFactory = OracleClientFactory.Instance;
					break;
				case DataProviders.MySQL:
					SqlFactory = MySqlClientFactory.Instance;
					break;
				case DataProviders.PostgreSQL:
					SqlFactory = NpgsqlFactory.Instance;
					break;
				case DataProviders.SQLite:
					SqlFactory = SQLiteFactory.Instance;
					break;
			}

			connectionProvider = dataProviders;
			databaseConnection = SqlFactory.CreateConnection();
			databaseCommand = SqlFactory.CreateCommand();
			if (dataProviders == DataProviders.Oracle)
			{
				((OracleCommand)databaseCommand).BindByName = true;
			}

			outputCommand = SqlFactory.CreateCommand();
			if (dataProviders == DataProviders.Oracle)
			{
				((OracleCommand)outputCommand).BindByName = true;
			}

			databaseConnection.ConnectionString = ConnectionString;
			databaseCommand.Connection = databaseConnection;
		}

		/// <summary>
		/// 매개 변수명과 값으로 DbCommand 객체에 DbParameter를 추가합니다.
		/// </summary>
		/// <param name="parameterName">매개 변수를 구성할 명입니다.</param>
		/// <param name="parameterValue">매개 변수를 구성할 값입니다.</param>
		/// <returns>DbCommand 객체에 DbParameter 컬렉션의 총 개수를 반환합니다.</returns>
		public int AddParameter(string parameterName, object parameterValue)
		{
			DbParameter parameter = SqlFactory.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.Value = parameterValue;

			return databaseCommand.Parameters.Add(parameter);
		}

		/// <summary>
		/// DbCommand 객체에 DbParameter를 추가합니다.
		/// </summary>
		/// <param name="parameter">매개 변수를 구성할 DbParameter 객체입니다.</param>
		/// <returns>DbCommand 객체에 DbParameter 컬렉션의 총 개수를 반환합니다.</returns>
		public int AddParameter(DbParameter parameter)
		{
			return databaseCommand.Parameters.Add(parameter);
		}

		/// <summary>
		/// 현재 연결에 대한 통계 수집이 활성화됩니다. 연결에 대한 통계 수집은 System.Data.SqlClient.SqlConnection 만 지원 합니다.
		/// 이외의 연결 객체에 대해서는 통계 수집을 수행하지 않습니다.
		/// </summary>
		public void StatisticsEnabled()
		{
			if (databaseConnection is System.Data.SqlClient.SqlConnection)
			{
				((System.Data.SqlClient.SqlConnection)databaseConnection).StatisticsEnabled = true;
			}
		}

		/// <summary>
		/// 메서드가 호출된 시점에서 통계 수집을 반환 합니다.
		/// </summary>
		/// <returns>메서드가 호출된 시점에서 통계의 이름 값 쌍 컬렉션을 반환합니다. SqlConnection 이외의 연결 객체에 대해서는 null값을 반환 합니다.</returns>
		public IDictionary RetrieveStatistics()
		{
			IDictionary statistics = null;
			if (databaseConnection is System.Data.SqlClient.SqlConnection)
			{
				statistics = ((System.Data.SqlClient.SqlConnection)databaseConnection).RetrieveStatistics();
			}

			return statistics;
		}

		/// <summary>
		/// 현재 데이터 소스에 적용된 연결 설정을 이용 하여 데이터 연결을 엽니다.
		/// </summary>
		internal void ConnectionOpen()
		{
			DatabaseReConnection();
		}

		/// <summary>
		/// 데이터베이스 연결이 닫혔으면 재연결을 시도 합니다.
		/// </summary>
		private void DatabaseReConnection()
		{
			if (databaseConnection.State == System.Data.ConnectionState.Closed)
			{
				if (databaseConnection.ConnectionString.Length == 0)
				{
					databaseConnection.ConnectionString = connectionString;
				}

				databaseConnection.Open();
			}
		}

		/// <summary>
		/// 명령 수행시 트랜잭션 상에서 동작하도록 데이터 연결을 설정합니다. 이 설정을 사용할 경우 CommitTransaction() 또는 RollbackTransaction()이 명시적으로 호출 되어야 합니다.
		/// </summary>
		public DbTransaction BeginTransaction()
		{
			DatabaseReConnection();

			databaseCommand.Transaction = databaseConnection.BeginTransaction();
			return databaseCommand.Transaction;
		}

		/// <summary>
		/// 데이터베이스 트랜잭션을 커밋합니다.
		/// </summary>
		public void CommitTransaction()
		{
			if (databaseCommand.Transaction != null)
			{
				databaseCommand.Transaction.Commit();
			}

			databaseConnection.Close();
		}

		/// <summary>
		/// 데이터베이스 트랜잭션을 롤백합니다.
		/// </summary>
		public void RollbackTransaction()
		{
			if (databaseCommand.Transaction != null)
			{
				databaseCommand.Transaction.Rollback();
			}

			databaseConnection.Close();
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열입니다.</param>
		/// <returns>영향 받는 행의 수입니다.</returns>
		public int ExecuteNonQuery(string commandText)
		{
			return ExecuteNonQuery(commandText, CommandType.Text, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열입니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>영향 받는 행의 수입니다.</returns>
		public int ExecuteNonQuery(string commandText, ExecutingConnectionState connectionState)
		{
			return ExecuteNonQuery(commandText, CommandType.Text, connectionState);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 CommandType 열거자에 따라 SQL문 또는 procedureName를 수행합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>영향 받는 행의 수입니다.</returns>
		public int ExecuteNonQuery(string commandText, CommandType dbCommandType)
		{
			return ExecuteNonQuery(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 CommandType 열거자에 따라 SQL문 또는 procedureName를 수행합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>영향 받는 행의 수입니다.</returns>
		public int ExecuteNonQuery(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
		{
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			int result = -1;
			try
			{
				DatabaseReConnection();

				result = databaseCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				DatabaseExceptionHandle(exception);
			}
			finally
			{
				SetOutputParameter();

				databaseCommand.Parameters.Clear();

				if (connectionState == ExecutingConnectionState.CloseOnExit)
				{
					databaseConnection.Close();
				}
			}

			return result;
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열입니다.</param>
		/// <returns>object입니다.</returns>
		public object ExecuteScalar(string commandText)
		{
			return ExecuteScalar(commandText, CommandType.Text, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>object입니다.</returns>
		public object ExecuteScalar(string commandText, CommandType dbCommandType)
		{
			return ExecuteScalar(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>object입니다.</returns>
		public object ExecuteScalar(string commandText, ExecutingConnectionState connectionState)
		{
			return ExecuteScalar(commandText, CommandType.Text, connectionState);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>object입니다.</returns>
		public object ExecuteScalar(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
		{
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			object result = null;
			try
			{
				DatabaseReConnection();

				result = databaseCommand.ExecuteScalar();
			}
			catch (Exception exception)
			{
				DatabaseExceptionHandle(exception);
			}
			finally
			{
				SetOutputParameter();

				databaseCommand.Parameters.Clear();

				if (connectionState == ExecutingConnectionState.CloseOnExit)
				{
					databaseConnection.Close();
				}
			}

			return result;
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 앞으로만 이동 가능한 데이터 스트림을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <returns>데이터 소스에서 앞으로만 이동 가능한 행 스트림입니다.</returns>
		public DbDataReader ExecuteReader(string commandText)
		{
			return ExecuteReader(commandText, CommandType.Text, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 앞으로만 이동 가능한 데이터 스트림을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>데이터 소스에서 앞으로만 이동 가능한 행 스트림입니다.</returns>
		public DbDataReader ExecuteReader(string commandText, CommandType dbCommandType)
		{
			return ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 앞으로만 이동 가능한 데이터 스트림을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>데이터 소스에서 앞으로만 이동 가능한 행 스트림입니다.</returns>
		public DbDataReader ExecuteReader(string commandText, CommandType dbCommandType, CommandBehavior commandBehavior)
		{
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			DbDataReader dataReader = null;
			try
			{
				DatabaseReConnection();

				dataReader = databaseCommand.ExecuteReader(commandBehavior);
			}
			catch (Exception exception)
			{
				DatabaseExceptionHandle(exception);
			}
			finally
			{
				SetOutputParameter();

				databaseCommand.Parameters.Clear();
			}

			return dataReader;
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 앞으로만 이동 가능한 데이터 스트림을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>데이터 소스에서 앞으로만 이동 가능한 행 스트림입니다.</returns>
		public DbDataReader ExecuteReader(string commandText, ExecutingConnectionState connectionState)
		{
			return ExecuteReader(commandText, CommandType.Text, connectionState);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 앞으로만 이동 가능한 데이터 스트림을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>데이터 소스에서 앞으로만 이동 가능한 행 스트림입니다.</returns>
		public DbDataReader ExecuteReader(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
		{
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			DbDataReader dataReader = null;
			try
			{
				DatabaseReConnection();

				if (connectionState == ExecutingConnectionState.CloseOnExit)
				{
					dataReader = databaseCommand.ExecuteReader(CommandBehavior.CloseConnection);
				}
				else
				{
					dataReader = databaseCommand.ExecuteReader();
				}

			}
			catch (Exception exception)
			{
				DatabaseExceptionHandle(exception);
			}
			finally
			{
				SetOutputParameter();

				databaseCommand.Parameters.Clear();
			}

			return dataReader;
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataSet을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열입니다.</param>
		/// <returns>메모리 내의 데이터 캐시를 나타냅니다.</returns>
		public DataSet ExecuteDataSet(string commandText)
		{
			return ExecuteDataSet(commandText, CommandType.Text, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataSet을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>메모리 내의 데이터 캐시를 나타냅니다.</returns>
		public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType)
		{
			return ExecuteDataSet(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataSet을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>메모리 내의 데이터 캐시를 나타냅니다.</returns>
		public DataSet ExecuteDataSet(string commandText, ExecutingConnectionState connectionState)
		{
			return ExecuteDataSet(commandText, CommandType.Text, connectionState);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataSet을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>메모리 내의 데이터 캐시를 나타냅니다.</returns>
		public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
		{
			DbDataAdapter dataAdapter = SqlFactory.CreateDataAdapter();
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			dataAdapter.SelectCommand = databaseCommand;

			using (DataSet result = new DataSet())
			{
				try
				{
					DatabaseReConnection();

					dataAdapter.Fill(result);
				}
				catch (Exception exception)
				{
					DatabaseExceptionHandle(exception);
				}
				finally
				{
					SetOutputParameter();

					databaseCommand.Parameters.Clear();

					if (connectionState == ExecutingConnectionState.CloseOnExit)
					{
						if (databaseConnection.State == System.Data.ConnectionState.Open)
						{
							databaseConnection.Close();
						}
					}
				}

				return result;
			}
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataTable을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열입니다.</param>
		/// <returns>메모리에 있는 데이터로 구성된 하나의 테이블을 나타냅니다.</returns>
		public DataTable ExecuteDataTable(string commandText)
		{
			return ExecuteDataTable(commandText, CommandType.Text, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataTable을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <returns>메모리에 있는 데이터로 구성된 하나의 테이블을 나타냅니다.</returns>
		public DataTable ExecuteDataTable(string commandText, CommandType dbCommandType)
		{
			return ExecuteDataTable(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataTable을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>메모리에 있는 데이터로 구성된 하나의 테이블을 나타냅니다.</returns>
		public DataTable ExecuteDataTable(string commandText, ExecutingConnectionState connectionState)
		{
			return ExecuteDataTable(commandText, CommandType.Text, connectionState);
		}

		/// <summary>
		/// 데이터베이스 제공자에게 명령문을 수행 하여 DataTable을 반환 합니다.
		/// </summary>
		/// <param name="commandText">데이터베이스에서 지원하는 SQL 문자열 또는 procedureName입니다.</param>
		/// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
		/// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
		/// <returns>메모리에 있는 데이터로 구성된 하나의 테이블을 나타냅니다.</returns>
		public DataTable ExecuteDataTable(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
		{
			DbDataAdapter dataAdapter = SqlFactory.CreateDataAdapter();
			databaseCommand.CommandText = commandText;
			databaseCommand.CommandType = dbCommandType;

			dataAdapter.SelectCommand = databaseCommand;

			using (DataTable result = new DataTable())
			{
				try
				{
					DatabaseReConnection();

					dataAdapter.Fill(result);
				}
				catch (Exception exception)
				{
				}
				finally
				{
					SetOutputParameter();

					databaseCommand.Parameters.Clear();

					if (connectionState == ExecutingConnectionState.CloseOnExit)
					{
						if (databaseConnection.State == System.Data.ConnectionState.Open)
						{
							databaseConnection.Close();
						}
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Output 매개변수 옵션에 따라 outputCommand 객체를 구성합니다.
		/// </summary>
		private void SetOutputParameter()
		{
			if (IsOutputParameter == true)
			{
				outputCommand.Parameters.Clear();
				foreach (DbParameter dbParameter in databaseCommand.Parameters)
				{
					DbParameter parameter = SqlFactory.CreateParameter();
					parameter.ParameterName = dbParameter.ParameterName;
					parameter.Value = dbParameter.Value;

					outputCommand.Parameters.Add(parameter);
				}

				IsOutputParameter = false;
			}
		}

		/// <summary>
		/// Database 명령 수행중 에러가 발생 했을 경우 처리를 담당합니다.
		/// </summary>
		/// <param name="e">Exception입니다.</param>
		private void DatabaseExceptionHandle(Exception e)
		{
			ExceptionFactory.Register("DataException", new ErrorException());
			ExceptionFactory.Handle("Database 명령 수행중 에러가 발생 했습니다.", e);

			Dispose();
			throw e;
		}

		/// <summary>
		/// 소멸자가 호출된 시점에선 Dispose가 호출 되지 않으므로 Managed 리소스는 해제 하지않습니다.
		/// </summary>
		~DatabaseFactory()
		{
			Dispose(false);
		}

		/// <summary>
		/// DatabaseFactory에서 사용하는 리소스(메모리 제외)를 삭제합니다.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);

			// GC에서 소멸자를 호출하지 않도록 지정합니다.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// DatabaseFactory의 리소스를 선택적으로 해제합니다.
		/// </summary>
		/// <param name="isFromDispose">명시적으로 Dispose 호출이 되었는지 구분합니다.</param>
		protected virtual void Dispose(bool isFromDispose)
		{
			if (isBaseDisposedResources == false)
			{
				if (isFromDispose == true)
				{
					databaseCommand.Dispose();
					outputCommand.Dispose();

					if (databaseConnection.State == System.Data.ConnectionState.Open)
					{
						databaseConnection.Close();
					}

					databaseConnection.Dispose();
				}

				isBaseDisposedResources = true;
			}
		}
	}
}
