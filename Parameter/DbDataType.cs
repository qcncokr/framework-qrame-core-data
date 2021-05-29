
using Qrame.CoreFX;

namespace Qrame.CoreFX.Data.Parameter
{
    /// <summary>
    /// 닷넷 프레임워크 기반의 응용 프로그램에서 데이터 명령을 수행하기 위해 범용 DbParameter를 구성 하기 위한, 매개 변수 클래스를 정의합니다.
    /// </summary>
    public class DbDataType : BaseEntity
    {
        /// <summary>
        /// 매개 변수 키 입니다.
        /// </summary>
        private string parameterName = "";

        /// <summary>
        /// 매개 변수 값 입니다.
        /// </summary>
        private string parameterValue = "";

        /// <summary>
        /// 공용 데이터베이스 타입을 지원하기 위한 열거자입니다.
        /// </summary>
        private DatabaseType dataType = DatabaseType.NotSupported;

        /// <summary>
        /// 매개 변수 키를 가져오거나, 설정합니다.
        /// </summary>
        public string ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }

        /// <summary>
        /// 매개 변수 값을 가져오거나, 설정합니다.
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        /// <summary>
        /// 공용 데이터베이스 타입을 지원하기 위한 열거자입니다. 
        /// </summary>
        public DatabaseType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        /// <summary>
        /// 매개 변수 키와 값으로 DbParameter를 구성 하기 위한 생성자입니다.
        /// </summary>
        /// <param name="parameterName">매개 변수 값 입니다.</param>
        /// <param name="parameterValue">매개 변수 키 입니다.</param>        
        /// <param name="dataType">공용 데이터베이스 타입을 지원하기 위한 열거자입니다. </param>
        public DbDataType(string parameterName, string parameterValue, DatabaseType dataType)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
            DataType = dataType;
        }
    }
}
