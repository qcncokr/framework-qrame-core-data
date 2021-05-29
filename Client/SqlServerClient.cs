using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

using Qrame.CoreFX;
using Qrame.CoreFX.Exceptions;
using Qrame.CoreFX.Patterns;
using Qrame.CoreFX.Cryptography;
using Qrame.CoreFX.ExtensionMethod;
using Qrame.CoreFX.Data;
using Qrame.CoreFX.Data.Parameter;
using Qrame.CoreFX.Configuration.Settings;
using System.Dynamic;

namespace Qrame.CoreFX.Data.Client
{
    /// <summary>    
    /// Winform, Webform에서 공용으로 사용하는 Multi DBMS 데이터베이스 조작을 구현하기 위해 설계되었습니다.
    /// 
    /// 주요 기능으로 다음과 같습니다.
    /// <code>
    ///   using (SqlServerClient dbClient = new SqlServerClient())
    ///   {
    ///       List&lt;SqlParameter&gt; parameters = new List&lt;SqlParameter&gt;();
    ///
    ///       parameters.Add(dbClient.CreateParameter(SqlDbType.VarChar, "DicKey", "ChangeSetup"));
    ///
    ///       using (DataSet result = dbClient.ExecuteDataSet("GetSys_Dictionary", parameters))
    ///       {
    ///           // ......
    ///       }
    ///   }
    /// </code>
    /// </summary>
    public sealed class SqlServerClient : IDisposable
    {
        /// <summary>
        /// SqlServer 데이터 베이스 연결문자열입니다.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// SqlServer 데이터 베이스 연결문자열입니다.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// SqlServer 닷넷 프레임워크의 데이터 공급자의 기능을 동일 하게 제공하는 Factory 클래스입니다.
        /// </summary>
        private DatabaseFactory databaseFactory;

        /// <summary>
        /// SqlServer 닷넷 프레임워크의 데이터 공급자의 기능을 동일 하게 제공하는 Factory 클래스입니다.
        /// </summary>
        public DatabaseFactory DbFactory
        {
            get
            {
                return databaseFactory;
            }
        }
        /// <summary>
        /// 리소스가 반환되었는지 확인합니다.
        /// </summary>
        private bool isDisposedResources = false;

        /// <summary>
        /// procedureName 명령 수행시 매개 변수를 데이터베이스로 부터 가져와 구성 할 건지, 직접 구성한 매개 변수를 사용 할 건지 설정합니다.
        /// </summary>
        public bool isDeriveParameters = false;

        /// <summary>
        /// procedureName 명령 수행시 매개 변수를 데이터베이스로 부터 가져와 구성 할 건지, 직접 구성한 매개 변수를 사용 할 건지 설정합니다.
        /// </summary>
        public bool IsDeriveParameters
        {
            get { return isDeriveParameters; }
            set { isDeriveParameters = value; }
        }

        /// <summary>
        /// 인스턴스 생성시, 응용 프로그램 구성에서 정의한 연결 문자열에 따라 연결 구성을 설정합니다.
        /// </summary>
        public SqlServerClient()
        {
            DatabaseSetting config = Singleton<DatabaseSetting>.Instance;
            connectionString = config.ConnectionString;

            if (config.IsConnectionStringEncryption == true)
            {
                Decryptor cryptography = new Decryptor(Encryption.Des);
                Encoding ascii = new ASCIIEncoding();
                connectionString = Convert.ToBase64String(cryptography.Decrypt(connectionString.ToBytes(ascii), config.DecryptionKey.ToBytes(ascii)));
            }

            databaseFactory = new DatabaseFactory(connectionString, DataProviders.SqlServer);
            databaseFactory.Command.CommandTimeout = databaseFactory.Connection.ConnectionTimeout;
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 연결 문자열에 따라 데이터 소스에 연결 구성을 설정합니다.
        /// </summary>
        /// <param name="connectionString">데이터베이스 연결 문자열입니다</param>
        public SqlServerClient(string connectionString)
        {
            ConnectionString = connectionString;

            databaseFactory = new DatabaseFactory(connectionString, DataProviders.SqlServer);
            databaseFactory.Command.CommandTimeout = databaseFactory.Connection.ConnectionTimeout;
        }

        /// <summary>
        /// 현재 연결에 대한 통계 수집이 활성화됩니다. 연결에 대한 통계 수집은 System.Data.SqlClient.SqlConnection 만 지원 합니다.
        /// 이외의 연결 객체에 대해서는 통계 수집을 수행하지 않습니다.
        /// </summary>
        public void StatisticsEnabled()
        {
            databaseFactory.StatisticsEnabled();
        }

        /// <summary>
        /// 메서드가 호출된 시점에서 통계 수집을 반환 합니다.
        /// </summary>
        /// <returns>메서드가 호출된 시점에서 통계의 이름 값 쌍 컬렉션을 반환합니다. SqlConnection 이외의 연결 객체에 대해서는 null값을 반환 합니다.</returns>
        public IDictionary RetrieveStatistics()
        {
            return databaseFactory.RetrieveStatistics();
        }

        /// <summary>
        /// SQL Server 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 SqlParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">SqlParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">SqlParameter 개체의 매개 변수 값입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        public SqlParameter CreateParameter(SqlDbType toDbType, string parameterName, object value)
        {
            return CreateParameter(toDbType, parameterName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// SQL Server 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 SqlParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">SqlParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">SqlParameter 개체의 매개 변수 값입니다.</param>
        /// <param name="direction">SqlParameter 개체의 매개 변수의 형식입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        public SqlParameter CreateParameter(SqlDbType toDbType, string parameterName, object value, ParameterDirection direction)
        {
            SqlParameter parameter = databaseFactory.Command.CreateParameter() as SqlParameter;

            if (parameter == null)
            {
                ExceptionFactory.Register("Exception", new WarningException());
                ExceptionFactory.Handle("SqlServerClient 공급자에서 SqlParameter 개체를 생성하지 못했습니다.", new Exception());
            }

            parameter.SqlDbType = toDbType;
            parameter.Direction = direction;

            parameter.ParameterName = parameterName;
            parameter.Value = value;

            return parameter;
        }

        /// <summary>
        /// 제네릭 SqlParameter 컬렉션에 등록된 정보로, SQL Server 제공자에게 명령을 수행할 SQL 문자열을 반환하며, 데이터베이스 제공자에게 아무런 작업을 수행하지 않습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 생성할 procedureName명 입니다.</param>
        /// <param name="parameters">제네릭 SqlParameter 컬렉션 객체 입니다</param>
        /// <returns>DatabaseFactory에 명령을 수행할 SQL 문자열 입니다.</returns>
        public string ExecuteCommandText(string procedureName, List<SqlParameter> parameters)
        {
            string CommandParameters = "";

            if (parameters.Count == 0)
            {
                return string.Concat("exec ", procedureName, ";");
            }

            if (isDeriveParameters == true)
            {
                SqlParameter[] parameterSet = GetSpParameterSet(procedureName);

                foreach (SqlParameter parameter in parameterSet)
                {
                    if (SetDbParameterData(parameter, parameters) == true)
                    {
                        CommandParameters += string.Concat(parameter.ParameterName, "='", parameter.Value.ToString().Replace("'", "''"), "', ");
                    }
                }
            }
            else
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if (parameter.ParameterName.IndexOf("@") > -1)
                    {
                        CommandParameters += string.Concat(parameter.ParameterName, "='", parameter.Value.ToString().Replace("'", "''"), "', ");
                    }
                    else
                    {
                        CommandParameters += string.Concat("@", parameter.ParameterName, "='", parameter.Value.ToString().Replace("'", "''"), "', ");
                    }
                }
            }

            if (CommandParameters.Length > 0)
            {
                CommandParameters = CommandParameters.Substring(0, CommandParameters.Length - 2);
            }

            return string.Concat("exec ", procedureName, " ", CommandParameters, ";");
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 DataSet을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType)
        {
            return databaseFactory.ExecuteDataSet(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 DataSet을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, ExecutingConnectionState connectionState)
        {
            return databaseFactory.ExecuteDataSet(commandText, dbCommandType, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 DataSet을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, List<SqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            return databaseFactory.ExecuteDataSet(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 DataSet을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, List<SqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            return databaseFactory.ExecuteDataSet(commandText, dbCommandType, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 결과 스키마 DataSet을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteProcedureFmtOnly(string procedureName, CommandType dbCommandType, List<SqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            string parameterText = " ";
            foreach (DbParameter parameter in databaseFactory.Command.Parameters)
            {
                parameterText += string.Concat(parameter.ParameterName, "='", parameter.Value.ToString().Replace("'", "''"), "',");
            }

            if (string.IsNullOrEmpty(parameterText) == false)
            {
                parameterText = parameterText.Left(parameterText.Length - 1);
            }

            return databaseFactory.ExecuteDataSet("SET FMTONLY ON;EXEC " + procedureName + parameterText + ";SET FMTONLY OFF;", dbCommandType, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 DataSet을 반환 합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<SqlParameter> parameters)
        {
            return ExecuteDataSet(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteDataSet(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState, out SqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            using (DataSet result = databaseFactory.ExecuteDataSet(procedureName, CommandType.StoredProcedure, connectionState))
            {
                outputDbCommand = databaseFactory.OutputCommand as SqlCommand;
                return result;
            }
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        public int ExecuteNonQuery(string commandText, CommandType dbCommandType)
        {
            return databaseFactory.ExecuteNonQuery(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        public int ExecuteNonQuery(string commandText, CommandType dbCommandType, List<SqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            return databaseFactory.ExecuteNonQuery(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<SqlParameter> parameters)
        {
            return ExecuteNonQuery(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState, out SqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            int result = databaseFactory.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, connectionState);
            outputDbCommand = databaseFactory.OutputCommand as SqlCommand;
            return result;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 SqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public SqlDataReader ExecuteReader(string commandText, CommandType dbCommandType)
        {
            return ExecuteReader(commandText, dbCommandType, null) as SqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 SqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public SqlDataReader ExecuteReader(string commandText, CommandType dbCommandType, List<SqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            return databaseFactory.ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit) as SqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 SqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public SqlDataReader ExecuteReader(string procedureName, List<SqlParameter> parameters)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteReader(procedureName, CommandType.StoredProcedure, ExecutingConnectionState.CloseOnExit) as SqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 PocoMapping을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public T ExecutePocoMapping<T>(string commandText, List<SqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            T results = default(T);
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            using (var reader = databaseFactory.ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    results = Activator.CreateInstance<T>();

                    List<string> columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }


                    reader.ToObject(columnNames, results);
                }
            }

            return results;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 PocoMapping을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public List<T> ExecutePocoMappings<T>(string commandText, List<SqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            List<T> results = null;
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            using (var reader = databaseFactory.ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit))
            {
                if (reader.HasRows)
                {
                    results = reader.ToObjectList<T>();
                }
            }

            return results;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 dynamic 컬렉션을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// SqlDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 SqlDataReader 타입으로 반환합니다.</returns>
        public List<dynamic> ExecuteDynamic(string commandText, List<SqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            List<dynamic> results = new List<dynamic>();
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            using (var reader = databaseFactory.ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit))
            {
                var schemaTable = reader.GetSchemaTable();

                List<string> columnNames = new List<string>();
                foreach (DataRow row in schemaTable.Rows)
                {
                    columnNames.Add(row["ColumnName"].ToString());
                }

                while (reader.Read())
                {
                    var data = new ExpandoObject() as IDictionary<string, Object>;
                    foreach (string columnName in columnNames)
                    {
                        var val = reader[columnName];
                        data.Add(columnName, Convert.IsDBNull(val) ? null : val);
                    }

                    results.Add((ExpandoObject)data);
                }
            }

            return results;
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string commandText, CommandType dbCommandType)
        {
            return databaseFactory.ExecuteScalar(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string commandText, CommandType dbCommandType, List<SqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }

            return databaseFactory.ExecuteScalar(commandText, dbCommandType);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
        {
            return ExecuteScalar(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteScalar(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<SqlParameter> parameters, ExecutingConnectionState connectionState, out SqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            object result = databaseFactory.ExecuteScalar(procedureName, CommandType.StoredProcedure, connectionState);
            outputDbCommand = databaseFactory.OutputCommand as SqlCommand;
            return result;
        }

        /// <summary>
        /// 지정된 procedureName문의 SqlParameter 컬렉션을 반환합니다.
        /// </summary>
        /// <param name="procedureName">SqlParameter 컬렉션을 반환할 procedureName명 입니다.</param>
        /// <returns>procedureName문의 SqlParameter 컬렉션입니다.</returns>
        private SqlParameter[] GetSpParameterSet(string procedureName)
        {
            DbParameter[] result = DbParameterCache.GetSpParameterSet(DataProviders.SqlServer, connectionString, procedureName);

            SqlParameter[] parameters = new SqlParameter[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                parameters[i] = result[i] as SqlParameter;
            }

            return parameters;
        }

        /// <summary>
        /// Factory의 DatabaseCommand객체에 SqlParameter를 구성합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 SqlParameter 컬렉션입니다.</param>
        private void SetDbFactoryCommand(string procedureName, List<SqlParameter> parameters)
        {
            if (isDeriveParameters == true)
            {
                if (parameters != null)
                {
                    SqlParameter[] parameterSet = GetSpParameterSet(procedureName);

                    foreach (SqlParameter parameter in parameterSet)
                    {
                        if (SetDbParameterData(parameter, parameters) == true)
                        {
                            databaseFactory.AddParameter(parameter);
                        }
                    }
                }
            }
            else
            {
                foreach (SqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }
        }

        /// <summary>
        /// 제네릭 SqlParameter 컬렉션의 데이터를 기준으로, SqlParameter에 값을 입력 합니다.
        /// </summary>
        /// <param name="parameter">parameter 타입입니다.</param>
        /// <param name="ListParameters">SqlParameter 컬렉션 타입입니다.</param>
        /// <returns>ListParameters 컬렉션에 parameter와 동일한 키가 있으면 true를, 아니면 false를 반환합니다.</returns>
        private bool SetDbParameterData(SqlParameter parameter, List<SqlParameter> ListParameters)
        {
            bool isMatchingParameter = false;
            object dbValue = null;

            var result = from p in ListParameters
                         where p.ParameterName.Equals(parameter.ParameterName, StringComparison.CurrentCultureIgnoreCase)
                         select p;

            if (result.Count() > 0)
            {
                SqlParameter listParameter = null;
                foreach (SqlParameter nvp in result)
                {
                    listParameter = nvp;
                    break;
                }

                dbValue = listParameter.Value;
                isMatchingParameter = true;
            }
            else
            {
                switch (parameter.SqlDbType)
                {
                    case SqlDbType.BigInt:
                        dbValue = 0;
                        break;
                    case SqlDbType.Binary:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.Bit:
                        dbValue = false;
                        break;
                    case SqlDbType.Char:
                        dbValue = "".ToCharArray();
                        break;
                    case SqlDbType.Date:
                        dbValue = DateTime.Now;
                        break;
                    case SqlDbType.DateTime:
                        dbValue = DateTime.Now;
                        break;
                    case SqlDbType.DateTime2:
                        dbValue = DateTime.Now;
                        break;
                    case SqlDbType.DateTimeOffset:
                        dbValue = DateTime.Now;
                        break;
                    case SqlDbType.Decimal:
                        dbValue = 0;
                        break;
                    case SqlDbType.Float:
                        dbValue = 0;
                        break;
                    case SqlDbType.Image:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.Int:
                        dbValue = 0;
                        break;
                    case SqlDbType.Money:
                        dbValue = 0;
                        break;
                    case SqlDbType.NChar:
                        dbValue = "";
                        break;
                    case SqlDbType.NText:
                        dbValue = "";
                        break;
                    case SqlDbType.NVarChar:
                        dbValue = "";
                        break;
                    case SqlDbType.Real:
                        dbValue = 0;
                        break;
                    case SqlDbType.SmallDateTime:
                        dbValue = DateTime.Now;
                        break;
                    case SqlDbType.SmallInt:
                        dbValue = 0;
                        break;
                    case SqlDbType.SmallMoney:
                        dbValue = 0;
                        break;
                    case SqlDbType.Structured:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.Text:
                        dbValue = "";
                        break;
                    case SqlDbType.Time:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.Timestamp:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.TinyInt:
                        dbValue = 0;
                        break;
                    case SqlDbType.Udt:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.UniqueIdentifier:
                        dbValue = Guid.NewGuid();
                        break;
                    case SqlDbType.VarBinary:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.VarChar:
                        dbValue = "";
                        break;
                    case SqlDbType.Variant:
                        dbValue = DBNull.Value;
                        break;
                    case SqlDbType.Xml:
                        dbValue = "";
                        break;
                    default:
                        dbValue = DBNull.Value;
                        break;
                }

                isMatchingParameter = false;
            }

            parameter.Value = dbValue;
            return isMatchingParameter;
        }

        /// <summary>
        /// 명령 수행시 트랜잭션 상에서 동작하도록 데이터 연결을 설정합니다. 이 설정을 사용할 경우 CommitTransaction() 또는 RollbackTransaction()이 명시적으로 호출 되어야 합니다.
        /// CommitTransaction 이 호출 되지 않고 데이터 연결이 닫힐 경우 Rollback으로 처리됩니다. 메서드 호출시 연결이 자동으로 닫히는 옵션을 확인하세요.
        /// </summary>
        public void BeginTransaction()
        {
            databaseFactory.BeginTransaction();
        }

        /// <summary>
        /// 데이터베이스 트랜잭션을 커밋합니다.
        /// </summary>
        public void CommitTransaction()
        {
            databaseFactory.CommitTransaction();
        }

        /// <summary>
        /// 데이터베이스 트랜잭션을 롤백합니다.
        /// </summary>
        public void RollbackTransaction()
        {
            databaseFactory.RollbackTransaction();
        }

        /// <summary>
        /// SqlServerClient에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// SqlServerClient의 리소스를 선택적으로 해제합니다.
        /// </summary>
        /// <param name="isFromDispose">명시적으로 Dispose 호출이 되었는지 구분합니다.</param>
        private void Dispose(bool isFromDispose)
        {
            if (isDisposedResources == false)
            {
                if (isFromDispose)
                {
                    if (databaseFactory != null)
                    {
                        databaseFactory.Dispose();
                    }

                    GC.SuppressFinalize(this);
                }

                isDisposedResources = true;
            }
        }
    }
}
