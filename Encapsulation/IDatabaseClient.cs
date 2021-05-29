using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Qrame.CoreFX.Data.Parameter;

namespace Qrame.CoreFX.Data
{
    /// <summary>
    /// DatabaseFactory 클래스의 기능을 업무에 따라 재개발(데코레이터 패턴) 할 경우, 재개발 할 Database Client 클래스에서 
    /// 제공 해야 할 기능들을 정의합니다.
    /// </summary>
    public interface IDatabaseClient : IDisposable
    {
        /// <summary>
        /// DbDataType 타입으로 데이터베이스에 procedureName 명령 수행시 매개 변수를 데이터베이스로 부터 가져와 구성 할 건지, 직접 구성한 매개 변수를 사용 할 건지 설정합니다.
        /// </summary>
        bool IsDeriveParameters { get; set; }
        
        /// <summary>
        /// 공용 데이터베이스 타입을 System.Data.DbType 데이터 형식으로 반환합니다.
        /// </summary>
        /// <param name="dataType">공용 데이터베이스 타입을 지원하기 위한 데이터 타입입니다.</param>
        /// <returns>연결 중인 데이터베이스 제공자에서 지원 하는 매개 변수에 사용할 필드, 속성의 데이터 형식입니다.</returns>
        dynamic ToCommonDbType(DatabaseType dataType);

        /// <summary>
        /// 공용 데이터베이스 타입을 연결 중인 데이터베이스 제공자에서 지원 하는 데이터 형식으로 반환합니다.
        /// </summary>
        /// <param name="dataType">공용 데이터베이스 타입을 지원하기 위한 데이터 타입입니다.</param>
        /// <returns>연결 중인 데이터베이스 제공자에서 지원 하는 매개 변수에 사용할 필드, 속성의 데이터 형식입니다.</returns>
        dynamic ToConnectDbType(DatabaseType dataType);

        /// <summary>
        /// 데이터베이스 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 DbParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">DbParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">DbParameter 개체의 매개 변수 값입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        DbParameter CreateParameter(dynamic toDbType, string parameterName, object value);

        /// <summary>
        /// 데이터베이스 제공자에 명령을 수행하기 위한 DbCommand에 매개 변수를 작성합니다.
        /// </summary>
        /// <param name="toDbType">닷넷 프레임워크 데이터 공급자의 필드, 속성 또는 DbParameter 개체의 데이터 형식을 지정합니다.</param>
        /// <param name="parameterName">DbParameter 개체의 매개 변수 이름입니다.</param>
        /// <param name="value">DbParameter 개체의 매개 변수 값입니다.</param>
        /// <param name="parameterDirection">DbParameter 개체의 매개 변수의 형식입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수입니다.</returns>
        DbParameter CreateParameter(dynamic toDbType, string parameterName, object value, ParameterDirection parameterDirection);
        
        /// <summary>
        /// 데이터베이스 제공자에 SQL문을 수행 하며, 명령 문자열을 해석하는 방법을 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 SQL 입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법입니다.</param>
        /// <returns>SQL 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string commandText, CommandType dbCommandType);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbDataType> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다. 결과값으로 DataSet을 반환 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>procedureName 결과를 DataSet 타입으로 반환합니다.</returns>
        DataSet ExecuteDataSet(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);
        
        /// <summary>
        /// 데이터베이스 제공자에 SQL문을 수행 하며, 명령 문자열을 해석하는 방법을 지정합니다. 결과값으로 DbDataReader을 반환 합니다.
        /// DbDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 SQL 입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법입니다.</param>
        /// <returns>SQL 결과를 DbDataReader 타입으로 반환합니다.</returns>
        DbDataReader ExecuteReader(string commandText, CommandType dbCommandType);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 결과값으로 DbDataReader을 반환 합니다.
        /// DbDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <returns>procedureName 결과를 DbDataReader 타입으로 반환합니다.</returns>
        DbDataReader ExecuteReader(string procedureName, List<DbParameter> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 문자열로 순서대로 지정합니다. 결과값으로 DbDataReader을 반환 합니다.
        /// DbDataReader 사용이 끝난 후에는 SqlServerClient 인스턴스의 Dispose() 메서드를 명시적으로 호출해야 Database 연결을 해제 합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <returns>procedureName 결과를 DbDataReader 타입으로 반환합니다.</returns>
        DbDataReader ExecuteReader(string procedureName, List<DbDataType> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 SQL문을 수행 하며, 명령 문자열을 해석하는 방법을 지정합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 SQL 입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string commandText, CommandType dbCommandType);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbParameter> parameters);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, out DbCommand outputDbCommand);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbParameter 컬렉션으로 지정합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbDataType> parameters);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState);

        /// <summary>        
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, procedureName 매개 변수를 제네릭 DbDataType 컬렉션으로 지정합니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        int ExecuteNonQuery(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 SQL문을 수행 하며, 명령 문자열을 해석하는 방법을 지정합니다. 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="commandText">쿼리 명령을 수행할 SQL 입니다.</param>
        /// <param name="dbCommandType">명령 문자열을 해석하는 방법입니다.</param>
        /// <returns>영향 받는 행의 수입니다.</returns>
        object ExecuteScalar(string commandText, CommandType dbCommandType);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbParameter> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbParameter> parameters, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbParameter 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbParameter> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbDataType> parameters);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다. 명령 수행후 Database 연결을 닫습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbDataType> parameters, out DbCommand outputDbCommand);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState);

        /// <summary>
        /// 데이터베이스 제공자에 procedureName문을 수행 하며, 반환된 결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다. 다른 모든 열과 행은 무시됩니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 수행할 procedureName명 입니다.</param>
        /// <param name="parameters">procedureName의 매개 변수를 구성할, 제네릭 DbDataType 컬렉션입니다.</param>
        /// <param name="connectionState">명령 수행후 Database 연결 상태를 지정할 connectionState 열거자 입니다.</param>
        /// <param name="outputDbCommand">쿼리 명령을 수행후 Output 매개 변수로 넘어오는 값을 저장하는 타입입니다.</param>
        /// <returns>결과 집합의 첫 번째 행의 첫 번째 열을 반환합니다.</returns>
        object ExecuteScalar(string procedureName, List<DbDataType> parameters, ExecutingConnectionState connectionState, out DbCommand outputDbCommand);

        /// <summary>
        /// 제네릭 DbParameter 컬렉션에 등록된 정보로, 데이터베이스 제공자에 명령을 수행할 SQL 문자열을 반환하며, 데이터베이스 제공자에 아무런 작업을 수행하지 않습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 생성할 procedureName명 입니다.</param>
        /// <param name="parameters">제네릭 DbParameter 컬렉션 객체 입니다</param>
        /// <returns>DatabaseFactory에 명령을 수행할 SQL 문자열 입니다.</returns>
        string ExecuteCommandText(string procedureName, List<DbParameter> parameters);

        /// <summary>
        /// 제네릭 DbDataType 컬렉션에 등록된 정보로, 데이터베이스 제공자에 명령을 수행할 SQL 문자열을 반환하며, 데이터베이스 제공자에 아무런 작업을 수행하지 않습니다.
        /// </summary>
        /// <param name="procedureName">쿼리 명령을 생성할 procedureName명 입니다.</param>
        /// <param name="parameters">제네릭 DbDataType 컬렉션 객체 입니다</param>
        /// <returns>DatabaseFactory에 명령을 수행할 SQL 문자열 입니다.</returns>
        string ExecuteCommandText(string procedureName, List<DbDataType> parameters);

        /// <summary>
        /// 명령 수행시 트랜잭션 상에서 동작하도록 데이터 연결을 설정합니다. 이 설정을 사용할 경우 CommitTransaction() 또는 RollbackTransaction()이 명시적으로 호출 되어야 합니다.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 데이터베이스 트랜잭션을 커밋합니다.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 데이터베이스 트랜잭션을 롤백합니다.
        /// </summary>
        void RollbackTransaction();
    }
}
