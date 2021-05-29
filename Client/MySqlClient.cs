using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
using MySql.Data.MySqlClient;

namespace Qrame.CoreFX.Data.Client
{
    /// <summary>    
    /// Winform, Webform에서 공용으로 사용하는 Multi DBMS 데이터베이스 조작을 구현하기 위해 설계되었습니다.
    /// 
    /// 주요 기능으로 다음과 같습니다.
    /// <code>
    ///   using (MySqlClient dbClient = new MySqlClient())
    ///   {
    ///       List&lt;MySqlParameter&gt; parameters = new List&lt;MySqlParameter&gt;();
    ///
    ///       parameters.Add(dbClient.CreateParameter(MySqlDbType.VarChar, "DicKey", "ChangeSetup"));
    ///
    ///       using (DataSet result = dbClient.ExecuteDataSet("GetSys_Dictionary", parameters))
    ///       {
    ///           // ......
    ///       }
    ///   }
    /// </code>
    /// </summary>
    public sealed class MySqlClient : IDisposable
    {
        /// <summary>
        /// MySQL 데이터 베이스 연결문자열입니다.
        /// </summary>
        private string connectionString;

        /// <summary>
        /// MySQL 데이터 베이스 연결문자열입니다.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// MySQL 닷넷 프레임워크의 데이터 공급자의 기능을 동일 하게 제공하는 Factory 클래스입니다.
        /// </summary>
        private DatabaseFactory databaseFactory;

        /// <summary>
        /// MySQL 닷넷 프레임워크의 데이터 공급자의 기능을 동일 하게 제공하는 Factory 클래스입니다.
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
        public MySqlClient()
        {
            DatabaseSetting config = Singleton<DatabaseSetting>.Instance;
            connectionString = config.ConnectionString;

            if (config.IsConnectionStringEncryption == true)
            {
                Decryptor cryptography = new Decryptor(Encryption.Des);
                Encoding ascii = new ASCIIEncoding();
                connectionString = Convert.ToBase64String(cryptography.Decrypt(connectionString.ToBytes(ascii), config.DecryptionKey.ToBytes(ascii)));
            }

            databaseFactory = new DatabaseFactory(connectionString, DataProviders.MySQL);
            databaseFactory.Command.CommandTimeout = databaseFactory.Connection.ConnectionTimeout;
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 연결 문자열에 따라 데이터 소스에 연결 구성을 설정합니다.
        /// </summary>
        /// <param name="connectionString">데이터베이스 연결 문자열입니다</param>
        public MySqlClient(string connectionString)
        {
            ConnectionString = connectionString;

            databaseFactory = new DatabaseFactory(connectionString, DataProviders.MySQL);
            databaseFactory.Command.CommandTimeout = databaseFactory.Connection.ConnectionTimeout;
        }

        /// <summary>
        /// 현재 연결에 대한 통계 수집이 활성화됩니다. 연결에 대한 통계 수집은 MySql.ManagedDataAccess.Client.MySqlConnection 만 지원 합니다.
        /// 이외의 연결 객체에 대해서는 통계 수집을 수행하지 않습니다.
        /// </summary>
        public void StatisticsEnabled()
        {
            databaseFactory.StatisticsEnabled();
        }

        /// <summary>
        /// 메서드가 호출된 시점에서 통계 수집을 반환 합니다.
        /// </summary>
        /// <returns>메서드가 호출된 시점에서 통계의 이름 값 쌍 컬렉션을 반환합니다. MySqlConnection 이외의 연결 객체에 대해서는 null값을 반환 합니다.</returns>
        public IDictionary RetrieveStatistics()
        {
            return databaseFactory.RetrieveStatistics();
        }

        /// <summary>
        /// SQL Server 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 MySqlParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">MySqlParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">MySqlParameter 개체의 매개 변수 값입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        public MySqlParameter CreateParameter(MySqlDbType toDbType, string parameterName, object value)
        {
            return CreateParameter(toDbType, parameterName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// SQL Server 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 MySqlParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">MySqlParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">MySqlParameter 개체의 매개 변수 값입니다.</param>
        /// <param name="direction">MySqlParameter 개체의 매개 변수의 형식입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        public MySqlParameter CreateParameter(MySqlDbType toDbType, string parameterName, object value, ParameterDirection direction)
        {
            MySqlParameter parameter = databaseFactory.Command.CreateParameter() as MySqlParameter;

            if (parameter == null)
            {
                ExceptionFactory.Register("Exception", new WarningException());
                ExceptionFactory.Handle("MySqlClient 공급자에서 MySqlParameter 개체를 생성하지 못했습니다.", new Exception());
            }

            parameter.MySqlDbType = toDbType;
            parameter.Direction = direction;

            parameter.ParameterName = parameterName;
            parameter.Value = value;

            return parameter;
        }

        /// <summary>
        /// 제네릭 MySqlParameter 컬렉션에 등록된 정보로, SQL Server 제공자에게 명령을 수행할 SQL 문자열을 반환하며, 데이터베이스 제공자에게 아무런 작업을 수행하지 않습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 생성할 procedureName명 입니다.</param>
        /// <param name="parameters">제네릭 MySqlParameter 컬렉션 객체 입니다</param>
        /// <returns>DatabaseFactory에 명령을 수행할 SQL 문자열 입니다.</returns>
        public string ExecuteCommandText(string procedureName, List<MySqlParameter> parameters)
        {
            string CommandParameters = "";

            if (parameters.Count == 0)
            {
                return string.Concat("exec ", procedureName, ";");
            }

            if (isDeriveParameters == true)
            {
                MySqlParameter[] parameterSet = GetSpParameterSet(procedureName);

                foreach (MySqlParameter parameter in parameterSet)
                {
                    if (SetDbParameterData(parameter, parameters) == true)
                    {
                        CommandParameters += string.Concat(parameter.ParameterName, "='", parameter.Value.ToString().Replace("'", "''"), "', ");
                    }
                }
            }
            else
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType dbCommandType, List<MySqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteProcedureFmtOnly(string procedureName, CommandType dbCommandType, List<MySqlParameter> parameters, ExecutingConnectionState connectionState)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<MySqlParameter> parameters)
        {
            return ExecuteDataSet(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteDataSet(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 procedureName문을 수행 하여 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public DataSet ExecuteDataSet(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState, out MySqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            using (DataSet result = databaseFactory.ExecuteDataSet(procedureName, CommandType.StoredProcedure, connectionState))
            {
                outputDbCommand = databaseFactory.OutputCommand as MySqlCommand;
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        public int ExecuteNonQuery(string commandText, CommandType dbCommandType, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<MySqlParameter> parameters)
        {
            return ExecuteNonQuery(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL 문을 실행합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>commandText 결과를 DataSet 타입으로 반환합니다.</returns>
        public int ExecuteNonQuery(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState, out MySqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            int result = databaseFactory.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, connectionState);
            outputDbCommand = databaseFactory.OutputCommand as MySqlCommand;
            
            return result;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 MySqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public MySqlDataReader ExecuteReader(string commandText, CommandType dbCommandType)
        {
            return ExecuteReader(commandText, dbCommandType, null) as MySqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 MySqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public MySqlDataReader ExecuteReader(string commandText, CommandType dbCommandType, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }
            
            return databaseFactory.ExecuteReader(commandText, dbCommandType, ExecutingConnectionState.CloseOnExit) as MySqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 MySqlDataReader를 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public MySqlDataReader ExecuteReader(string procedureName, List<MySqlParameter> parameters)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteReader(procedureName, CommandType.StoredProcedure, ExecutingConnectionState.CloseOnExit) as MySqlDataReader;
        }

        /// <summary>
        /// SQL Server 제공자에 commandText문을 수행 하여 PocoMapping을 반환 합니다. ANSI SQL 또는 T-SQL을 사용합니다.
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public T ExecutePocoMapping<T>(string commandText, List<MySqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            T results = default(T);
            if (parameters != null)
            { 
                foreach (MySqlParameter parameter in parameters)
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
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public List<T> ExecutePocoMappings<T>(string commandText, List<MySqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            List<T> results = null;
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// MySqlDataReader 사용이 끝난 후에는 MySqlClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 ANSI SQL 또는 T-SQL 문자열입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법을 지정합니다.</param>
        /// <returns>commandText 결과를 MySqlDataReader 타입으로 반환합니다.</returns>
        public List<dynamic> ExecuteDynamic(string commandText, List<MySqlParameter> parameters, CommandType dbCommandType = CommandType.StoredProcedure)
        {
            List<dynamic> results = new List<dynamic>();
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string commandText, CommandType dbCommandType, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
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
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<MySqlParameter> parameters)
        {
            return ExecuteScalar(procedureName, parameters, ExecutingConnectionState.CloseOnExit);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState)
        {
            SetDbFactoryCommand(procedureName, parameters);

            return databaseFactory.ExecuteScalar(procedureName, CommandType.StoredProcedure, connectionState);
        }

        /// <summary>
        /// SQL Server 제공자에 SQL문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>        
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        public object ExecuteScalar(string procedureName, List<MySqlParameter> parameters, ExecutingConnectionState connectionState, out MySqlCommand outputDbCommand)
        {
            SetDbFactoryCommand(procedureName, parameters);

            databaseFactory.IsOutputParameter = true;
            object result = databaseFactory.ExecuteScalar(procedureName, CommandType.StoredProcedure, connectionState);
            outputDbCommand = databaseFactory.OutputCommand as MySqlCommand;
            return result;
        }

        /// <summary>
        /// 지정된 procedureName문의 MySqlParameter 컬렉션을 반환합니다.
        /// </summary>
        /// <param name="procedureName">MySqlParameter 컬렉션을 반환할 procedureName명 입니다.</param>
        /// <returns>procedureName문의 MySqlParameter 컬렉션입니다.</returns>
        private MySqlParameter[] GetSpParameterSet(string procedureName)
        {
            DbParameter[] result = DbParameterCache.GetSpParameterSet(DataProviders.MySQL, connectionString, procedureName);

            MySqlParameter[] parameters = new MySqlParameter[result.Length];

            for (int i = 0; i < result.Length; i++)
            {
                parameters[i] = result[i] as MySqlParameter;
            }

            return parameters;
        }

        /// <summary>
        /// Factory의 DatabaseCommand객체에 MySqlParameter를 구성합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 MySqlParameter 컬렉션입니다.</param>
        private void SetDbFactoryCommand(string procedureName, List<MySqlParameter> parameters)
        {
            if (isDeriveParameters == true)
            {
                if (parameters != null)
                {
                    MySqlParameter[] parameterSet = GetSpParameterSet(procedureName);

                    foreach (MySqlParameter parameter in parameterSet)
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
                foreach (MySqlParameter parameter in parameters)
                {
                    databaseFactory.AddParameter(parameter);
                }
            }
        }

        /// <summary>
        /// 제네릭 MySqlParameter 컬렉션의 데이터를 기준으로, MySqlParameter에 값을 입력 합니다.
        /// </summary>
        /// <param name="parameter">parameter 타입입니다.</param>
        /// <param name="ListParameters">MySqlParameter 컬렉션 타입입니다.</param>
        /// <returns>ListParameters 컬렉션에 parameter와 동일한 키가 있으면 true를, 아니면 false를 반환합니다.</returns>
        private bool SetDbParameterData(MySqlParameter parameter, List<MySqlParameter> ListParameters)
        {
            bool isMatchingParameter = false;
            object dbValue = null;

            var result = from p in ListParameters
                         where p.ParameterName.Equals(parameter.ParameterName, StringComparison.CurrentCultureIgnoreCase)
                         select p;

            if (result.Count() > 0)
            {
                MySqlParameter listParameter = null;
                foreach (MySqlParameter nvp in result)
                {
                    listParameter = nvp;
                    break;
                }

                dbValue = listParameter.Value;
                isMatchingParameter = true;
            }
            else
            {
                switch (parameter.MySqlDbType)
                {
                    case MySqlDbType.Decimal:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Byte:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.Int16:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Int24:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Int32:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Int64:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Float:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Double:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Timestamp:
                        dbValue = DateTime.Now;
                        break;
                    case MySqlDbType.Date:
                        dbValue = DateTime.Now;
                        break;
                    case MySqlDbType.Time:
                        dbValue = DateTime.Now;
                        break;
                    case MySqlDbType.DateTime:
                        dbValue = DateTime.Now;
                        break;
                    case MySqlDbType.Year:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Newdate:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.VarString:
                        dbValue = "";
                        break;
                    case MySqlDbType.Bit:
                        dbValue = false;
                        break;
                    case MySqlDbType.JSON:
                        dbValue = "";
                        break;
                    case MySqlDbType.NewDecimal:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Enum:
                        dbValue = "";
                        break;
                    case MySqlDbType.Set:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.TinyBlob:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.MediumBlob:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.LongBlob:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.Blob:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.VarChar:
                        dbValue = "";
                        break;
                    case MySqlDbType.String:
                        dbValue = "";
                        break;
                    case MySqlDbType.Geometry:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.UByte:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.UInt16:
                        dbValue = 0;
                        break;
                    case MySqlDbType.UInt24:
                        dbValue = 0;
                        break;
                    case MySqlDbType.UInt32:
                        dbValue = 0;
                        break;
                    case MySqlDbType.UInt64:
                        dbValue = 0;
                        break;
                    case MySqlDbType.Binary:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.VarBinary:
                        dbValue = DBNull.Value;
                        break;
                    case MySqlDbType.TinyText:
                        dbValue = "";
                        break;
                    case MySqlDbType.MediumText:
                        dbValue = "";
                        break;
                    case MySqlDbType.LongText:
                        dbValue = "";
                        break;
                    case MySqlDbType.Text:
                        dbValue = "";
                        break;
                    case MySqlDbType.Guid:
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
        /// MySqlClient에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// MySqlClient의 리소스를 선택적으로 해제합니다.
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
