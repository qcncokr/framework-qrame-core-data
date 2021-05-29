using System;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Collections;

using Qrame.CoreFX.Exceptions;
using Qrame.CoreFX.Patterns;
using Qrame.CoreFX.Configuration.Settings;

namespace Qrame.CoreFX.Data
{
    /// <summary>    
    /// 데이터베이스에 procedureName를 대상으로 매개 변수 정보를 DbParameter 클래스로 동적으로 생성하고 관리하는 클래스입니다.
    /// 데이터 소스에 따라 지원되지 않을 수 있습니다. 캐시 키는 (데이터 소스 명 : procedureName 명)입니다.
    /// </summary>
    public sealed class DbParameterCache
    {
        /// <summary>
        /// DbParameter를 캐시에 관리하는 해시 테이블입니다. 
        /// </summary>
        private static Hashtable parameterCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// DbParameter 객체의 참조 타입의 복사본을 반환 합니다. 
        /// </summary>
        /// <param name="parameters">DbParameter[] 객체입니다</param>
        /// <returns>DbParameter[] 객체입니다</returns>
        private static DbParameter[] CloneParameters(DbParameter[] parameters)
        {
            DbParameter[] discoveredParameters = new DbParameter[parameters.Length];

            parameters.CopyTo(discoveredParameters, 0);

            return discoveredParameters;
        }
       
        /// <summary>
        /// 캐시에서 DbParameter객체를 가져옵니다. 
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="procedureName">데이터베이스에서 지원하는 procedureName입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수 목록입니다.</returns>
        public static DbParameter[] GetCachedParameterSet(DataProviders dataProviders, string procedureName)
        {
            DbParameter[] cachedParameters = (DbParameter[])parameterCache[string.Concat(dataProviders.ToString(), ":", procedureName)];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }
        
        /// <summary>
        /// DbParameter 객체를 캐시로 관리합니다. 
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="procedureName">데이터베이스에서 지원하는 procedureName입니다.</param>
        /// <param name="parameters">DbCommand에 대한 매개 변수 목록입니다.</param>
        public static void CacheParameterSet(DataProviders dataProviders, string procedureName, params DbParameter[] parameters)
        {
            parameterCache[string.Concat(dataProviders.ToString(), ":", procedureName)] = parameters;
        }

        /// <summary>
        /// procedureName의 매개 변수 항목을 DbParameter[]로 반환합니다. 
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="connectionString">데이터베이스 연결 문자열입니다</param>
        /// <param name="procedureName">데이터베이스에서 지원하는 procedureName입니다.</param>
        /// <returns>DbCommand에 대한 매개 변수 목록입니다.</returns>
        public static DbParameter[] GetSpParameterSet(DataProviders dataProviders, string connectionString, string procedureName)
        {
            return GetSpParameterSet(dataProviders, connectionString, procedureName, false);
        }

        /// <summary>
        /// procedureName의 매개 변수 항목을 DbParameter[]로 반환합니다. 
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="connectionString">데이터베이스 연결 문자열입니다</param>
        /// <param name="procedureName">데이터베이스에서 지원하는 procedureName입니다.</param>
        /// <param name="outputParameter">output으로 결과를 가져올지 결정합니다</param>
        /// <returns>DbCommand에 대한 매개 변수 목록입니다.</returns>
        public static DbParameter[] GetSpParameterSet(DataProviders dataProviders, string connectionString, string procedureName, bool outputParameter)
        {
            string hashKey = string.Concat(dataProviders.ToString(), ":", procedureName, outputParameter == true ? ":OutputParameter" : "");

            DbParameter[] cachedParameters;

            cachedParameters = (DbParameter[])parameterCache[hashKey];

            DatabaseSetting setting = Singleton<DatabaseSetting>.Instance;
            
            if (setting.IsParameterCache == false)
            {
                cachedParameters = null;
            }

            if (cachedParameters == null)
            {
                cachedParameters = (DbParameter[])(parameterCache[hashKey] = DiscoverSpParameterSet(dataProviders, connectionString, procedureName, outputParameter));
            }

            return CloneParameters(cachedParameters);
        }

        /// <summary>
        /// procedureName의 매개 변수 항목을 DbParameter[]로 반환합니다. 
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="connectionString">데이터 소스 연결 문자열입니다.</param>
        /// <param name="procedureName">매개 변수 항목을 DbParameter[]로 반환할 procedureName명 입니다.</param>
        /// <param name="outputParameter">데이터베이스 제공자에 따라 반환되어지는 매개 변수 항목을 조정합니다.</param>
        /// <returns></returns>
        private static DbParameter[] DiscoverSpParameterSet(DataProviders dataProviders, string connectionString, string procedureName, bool outputParameter)
        {
            using (DatabaseFactory dbFactory = new DatabaseFactory(connectionString, dataProviders))
            using (DbCommand parameterCommand = dbFactory.Command)
            {
                dbFactory.ConnectionOpen();
                parameterCommand.CommandType = CommandType.StoredProcedure;
                parameterCommand.CommandText = procedureName;

                DeriveParameters(dbFactory.SqlFactory, parameterCommand);

                if (dataProviders == DataProviders.SqlServer && outputParameter == false)
                {
                    parameterCommand.Parameters.RemoveAt(0);
                }

                DbParameter[] DiscoveredParameters = new DbParameter[parameterCommand.Parameters.Count]; ;

                parameterCommand.Parameters.CopyTo(DiscoveredParameters, 0);
                parameterCommand.Parameters.Clear();
                return DiscoveredParameters;
            }
        }

        /// <summary>
        /// 데이터베이스에서 procedureName의 매개 변수 항목을 끌어옵니다.
        /// </summary>
        /// <param name="providerFactory">데이터 소스 클래스의 공급자 구현에 대한 인스턴스를 만드는 데 사용되는 메서드의 집합을 나타냅니다.</param>
        /// <param name="dbCommand">데이터 소스에 연결된 동안 실행되고 관계형 데이터베이스에 액세스하는 닷넷 프레임워크 데이터 공급자에 의해 구현되는 SQL문을 나타냅니다.</param>
        public static void DeriveParameters(DbProviderFactory providerFactory, IDbCommand dbCommand)
        {
            MethodInfo method;
            DbCommandBuilder commandBuilder;

            Type commandType;

            commandBuilder = providerFactory.CreateCommandBuilder();

            commandType = commandBuilder.GetType();
            method = commandType.GetMethod("DeriveParameters", BindingFlags.Public | BindingFlags.Static);

            if (method != null)
            {
                method.Invoke(null, new object[] { dbCommand });
            }
            else
            {
                ExceptionFactory.Register("ArgumentException", new WarningException());
                ExceptionFactory.Handle(string.Format("{0} 제공자에서 Stored Procedre의 매개 변수 정보를 가져오는 기능을 지원하지 않습니다.", providerFactory.GetType().Name), new ArgumentException());
            }
        }
    }
}
