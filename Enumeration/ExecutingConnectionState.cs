

namespace Qrame.CoreFX.Data
{
    /// <summary>
    /// DbCommand 명령 수행후 데이터 소스 연결 상태에 대한 열거자입니다.
    /// </summary>
    public enum ExecutingConnectionState
    {
        /// <summary>
        /// 데이터 원본에 명령 수행후 데이터 소스 연결을 닫지 않고 유지합니다.
        /// </summary>
        KeepOpen,
        /// <summary>
        /// 데이터 원본에 명령 수행후 데이터 소스 연결을 닫습니다.
        /// </summary>
        CloseOnExit
    }
}
