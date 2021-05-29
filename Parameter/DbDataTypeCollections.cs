using System.Collections.Generic;

using Qrame.CoreFX.Collections;

namespace Qrame.CoreFX.Data.Parameter
{
    /// <summary>
    /// 닷넷 프레임워크 기반의 응용 프로그램에서, 한번의 데이터 소스 연결 상태에서, 여러개의 DbCommand 명령을 
    /// 수행하기 위한 DbDataType 클래스 컬렉션입니다. 인덱싱 키에는 쿼리 명령을 수행할 commandText이며,
    /// 인덱싱 값에는 쿼리 명령을 수행할 매개 변수를 정의 합니다.
    /// </summary>
    public class DbDataTypeCollections : KeyedList<string, List<DbDataType>>
    {
        /// <summary>
        /// DbCommand 명령을 수행하기 위한 commandText를 추가합니다. 일반적으로 매개 변수가 
        /// 필요없는 쿼리 명령을 수행할 ANSI SQL이거나 데이터 소스가 지원하는 procedureName명 입니다.
        /// </summary>
        /// <param name="parameterName">DbCommand 명령을 수행하기 위한 commandText입니다.</param>
        public DbDataTypeCollections(string parameterName)
        {
            Add(parameterName, null);
        }

        /// <summary>
        /// DbCommand 명령을 수행하기 위한 commandText와 범용 DbParameter를 구성 하기 위한, 매개 변수 클래스를 정의합니다.
        /// </summary>
        /// <param name="parameterName">DbCommand 명령을 수행하기 위한 commandText입니다.</param>
        /// <param name="parameterList">DbParameter를 구성 하기 위한, 매개 변수 클래스입니다.</param>
        public DbDataTypeCollections(string parameterName, List<DbDataType> parameterList)
        {
            Add(parameterName, parameterList);
        }
    }
}
