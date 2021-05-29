
using System.Data;

namespace Qrame.CoreFX.Data
{
    /// <summary>
    /// 공용 데이터베이스 타입을 지원하기 위한 데이터 타입입니다.
    /// 자세한 내용은 다음 정보를 참조하세요.
    /// 
    /// SqlDbType 열거형 http://msdn.microsoft.com/ko-kr/library/system.data.sqldbtype.aspx
    /// OracleDbType Enumeration Type http://download.oracle.com/docs/html/B28089_01/featOraCommand.htm 
    /// 
    /// 처음 DatabaseType를 설계 했을 때에는 모든 데이터베이스에서 지원하는 공용 데이터 타입만을 적용하려고 했으나,
    /// 다음과 같은 2가지 이유로 전용 데이터 타입을 적용 합니다.
    /// 1. DataAccessClient에서 공용 기능을 구현
    /// 2. 추후 추가되는 데이터베이스의 데이터 타입 지원 여부
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SqlDbType.Binary, OracleDbType.Raw에 대응하는 데이터 타입입니다.
        /// </summary>
        Binary,
        /// <summary>
        /// SqlDbType.Bit, OracleDbType.Char(오라클은 Boolean 타입을 지원하지 않습니다.)에 대응하는 데이터 타입입니다.
        /// </summary>
        Boolean,
        /// <summary>
        /// SqlDbType.TinyInt, OracleDbType.Byte에 대응하는 데이터 타입입니다.
        /// </summary>
        Byte,
        /// <summary>
        /// SqlDbType.Char, OracleDbType.Char에 대응하는 데이터 타입입니다.
        /// </summary>
        Char,
        /// <summary>
        /// SqlDbType.DateTime, OracleDbType.Date에 대응하는 데이터 타입입니다.
        /// </summary>
        Date,
        /// <summary>
        /// SqlDbType.Time에 대응하는 데이터 타입입니다.
        /// </summary>
        Time,
        /// <summary>
        /// SqlDbType.DateTime, OracleDbType.Date에 대응하는 데이터 타입입니다.
        /// </summary>
        DateTime,
        /// <summary>
        /// SqlDbType.Decimal, OracleDbType.Decimal에 대응하는 데이터 타입입니다.
        /// </summary>
        Decimal,
        /// <summary>
        /// OracleDbType.Double에 대응하는 데이터 타입입니다.
        /// </summary>
        Double,
        /// <summary>
        /// SqlDbType.Object에 대응하는 데이터 타입입니다.
        /// </summary>
        Object,
        /// <summary>
        /// SqlDbType.Single에 대응하는 데이터 타입입니다.
        /// </summary>
        Single,
        /// <summary>
        /// SqlDbType.Float, OracleDbType.BinaryFloat에 대응하는 데이터 타입입니다.
        /// </summary>
        Float,
        /// <summary>
        /// SqlDbType.BigInt, OracleDbType.Int64에 대응하는 데이터 타입입니다.
        /// </summary>
        Int64,
        /// <summary>
        /// SqlDbType.Int, OracleDbType.Int32에 대응하는 데이터 타입입니다.
        /// </summary>
        Int32,
        /// <summary>
        /// SqlDbType.SmallInt, OracleDbType.Int16에 대응하는 데이터 타입입니다.
        /// </summary>
        Int16,
        /// <summary>
        /// SqlDbType.BigInt, OracleDbType.Long에 대응하는 데이터 타입입니다.
        /// </summary>
        Long,
        /// <summary>
        /// OracleDbType.NChar에 대응하는 데이터 타입입니다.
        /// </summary>
        NChar,
        /// <summary>
        /// OracleDbType.NClob에 대응하는 데이터 타입입니다.
        /// </summary>
        NClob,
        /// <summary>
        /// SqlDbType.NText에 대응하는 데이터 타입입니다.
        /// </summary>
        NText,
        /// <summary>
        /// SqlDbType.NVarChar, OracleDbType.NVarchar2에 대응하는 데이터 타입입니다.
        /// </summary>
        NVarChar,
        /// <summary>
        /// SqlDbType.SmallMoney에 대응하는 데이터 타입입니다.
        /// </summary>
        Currency,
        /// <summary>
        /// SqlDbType.Text에 대응하는 데이터 타입입니다.
        /// </summary>
        Text,
        /// <summary>
        /// SqlDbType.Timestamp, OracleDbType.TimeStamp에 대응하는 데이터 타입입니다.
        /// </summary>
        Timestamp,
        /// <summary>
        /// SqlDbType.UniqueIdentifier에 대응하는 데이터 타입입니다.
        /// </summary>
        Guid,
        /// <summary>
        /// SqlDbType.VarBinary에 대응하는 데이터 타입입니다.
        /// </summary>
        VarBinary,
        /// <summary>
        /// SqlDbType.VarChar, OracleDbType.Varchar2에 대응하는 데이터 타입입니다.
        /// </summary>
        VarChar,
        /// <summary>
        /// SqlDbType.Xml, OracleDbType.XmlType에 대응하는 데이터 타입입니다.
        /// </summary>
        Xml,
        /// <summary>
        /// 데이터베이스에서 지원하지 않는 데이터 타입입니다.
        /// </summary>
        NotSupported
    }

    /// <summary>
    /// DatabaseType 클래스를 대상으로 동작하는 확장 메서드 클래스입니다.
    /// </summary>
    public static class DatabaseTypeExtensions
    {
        /// <summary>
        /// 공용 데이터베이스 타입을 DbType으로 반환합니다.
        /// </summary>
        /// <param name="DataType">공용 데이터베이스 타입입니다.</param>
        public static DbType ToDbType(this DatabaseType DataType)
        {
            switch (DataType)
            {
                case DatabaseType.Binary:
                    return DbType.Binary;
                case DatabaseType.Boolean:
                    return DbType.Boolean;
                case DatabaseType.Byte:
                    return DbType.Byte;
                case DatabaseType.Char:
                    return DbType.AnsiStringFixedLength;
                case DatabaseType.Date:
                    return DbType.Date;
                case DatabaseType.Time:
                    return DbType.Time;
                case DatabaseType.DateTime:
                    return DbType.DateTime;
                case DatabaseType.Decimal:
                    return DbType.Decimal;
                case DatabaseType.Float:
                    return DbType.Single;
                case DatabaseType.Int64:
                    return DbType.Int64;
                case DatabaseType.Int32:
                    return DbType.Int32;
                case DatabaseType.Int16:
                    return DbType.Int16;
                case DatabaseType.NChar:
                    return DbType.StringFixedLength;
                case DatabaseType.NText:
                    return DbType.String;
                case DatabaseType.NVarChar:
                    return DbType.String;
                case DatabaseType.Object:
                    return DbType.Object;
                case DatabaseType.Single:
                    return DbType.Single;
                case DatabaseType.Currency:
                    return DbType.Currency;
                case DatabaseType.Text:
                    return DbType.AnsiString;
                case DatabaseType.Timestamp:
                    return DbType.Time;
                case DatabaseType.Guid:
                    return DbType.Guid;
                case DatabaseType.VarBinary:
                    return DbType.Binary;
                case DatabaseType.VarChar:
                    return DbType.String;
                default:
                    return DbType.String;
            }
        }
    }

    //
    // 요약:
    //     .NET Framework 데이터 공급자의 필드, 속성 또는 Parameter 개체의 데이터 형식을 지정합니다.
    public enum Database_DbType
    {
        //
        // 요약:
        //     범위가 1문자에서 8,000문자까지인 비유니코드 문자의 가변 길이 스트림입니다.
        AnsiString = 0,
        //
        // 요약:
        //     범위가 1바이트에서 8,000바이트까지인 이진 데이터의 가변 길이 스트림입니다.
        Binary = 1,
        //
        // 요약:
        //     0에서 255 사이의 값을 갖는 8비트 부호 없는 정수입니다.
        Byte = 2,
        //
        // 요약:
        //     true 또는 false의 부울 값을 나타내는 단순 형식입니다.
        Boolean = 3,
        //
        // 요약:
        //     정확성이 통화 단위의 10000분의 1이고 범위가 -2 63(또는 -922,337,203,685,477.5808)에서 2 63 -1(또는
        //     +922,337,203,685,477.5807)까지인 통화 값입니다.
        Currency = 4,
        //
        // 요약:
        //     날짜 값을 나타내는 형식입니다.
        Date = 5,
        //
        // 요약:
        //     날짜 및 시간 값을 나타내는 형식입니다.
        DateTime = 6,
        //
        // 요약:
        //     1.0 x 10-28부터 약 7.9 x 1028까지 28-29개의 유효 자릿수를 가진 값을 나타내는 단순 형식입니다.
        Decimal = 7,
        //
        // 요약:
        //     약 5.0 x 10-324부터 1.7 x 10308까지 15-16자리의 정밀도를 가진 값을 나타내는 부동 소수점 형식입니다.
        Double = 8,
        //
        // 요약:
        //     GUID(Globally Unique IDentifier)입니다.
        Guid = 9,
        //
        // 요약:
        //     -32768과 32767 사이의 값을 가진 부호 있는 16비트 정수를 나타내는 정수 계열 형식입니다.
        Int16 = 10,
        //
        // 요약:
        //     -2147483648과 2147483647 사이의 값을 가진 부호 있는 32비트 정수를 나타내는 정수 계열 형식입니다.
        Int32 = 11,
        //
        // 요약:
        //     -9223372036854775808과 9223372036854775807 사이의 값을 가진 부호 있는 64비트 정수를 나타내는 정수 계열
        //     형식입니다.
        Int64 = 12,
        //
        // 요약:
        //     다른 DbType 값에 의해 명시적으로 나타나지 않은 참조 또는 값 형식을 나타내는 일반 형식입니다.
        Object = 13,
        //
        // 요약:
        //     -128과 127 사이의 값을 가진 부호 있는 8비트 정수를 나타내는 정수 계열 형식입니다.
        SByte = 14,
        //
        // 요약:
        //     약 1.5 x 10-45부터 3.4 x 1038까지 7자리의 정밀도를 가진 값을 나타내는 부동 소수점 형식입니다.
        Single = 15,
        //
        // 요약:
        //     유니코드 문자열을 나타내는 형식입니다.
        String = 16,
        //
        // 요약:
        //     SQL Server DateTime 값을 나타내는 형식입니다.SQL Server time 값을 사용하려면 System.Data.SqlDbType.Time을
        //     사용합니다.
        Time = 17,
        //
        // 요약:
        //     0과 65535 사이의 값을 가진 부호 없는 16비트 정수를 나타내는 정수 계열 형식입니다.
        UInt16 = 18,
        //
        // 요약:
        //     0과 4294967295 사이의 값을 가진 부호 없는 32비트 정수를 나타내는 정수 계열 형식입니다.
        UInt32 = 19,
        //
        // 요약:
        //     0과 18446744073709551615 사이의 값을 가진 부호 없는 64비트 정수를 나타내는 정수 계열 형식입니다.
        UInt64 = 20,
        //
        // 요약:
        //     가변 길이 숫자 값입니다.
        VarNumeric = 21,
        //
        // 요약:
        //     유니코드 문자가 아닌 고정 길이 스트림입니다.
        AnsiStringFixedLength = 22,
        //
        // 요약:
        //     유니코드 문자의 고정 길이 문자열입니다.
        StringFixedLength = 23,
        //
        // 요약:
        //     XML 문서나 단편의 구문 분석된 표현입니다.
        Xml = 25,
        //
        // 요약:
        //     날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time 값 범위는 00:00:00부터
        //     23:59:59.9999999까지이며 정확도는 100나노초입니다.
        DateTime2 = 26,
        //
        // 요약:
        //     표준 시간대를 고려한 날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time
        //     값 범위는 00:00:00부터 23:59:59.9999999까지이며 정확도는 100나노초입니다.표준 시간대 값의 범위는 -14:00에서 +14:00
        //     사이입니다.
        DateTimeOffset = 27
    }

    public enum Database_OracleDbType
    {
        BFile = 101,
        Blob = 102,
        Byte = 103,
        Char = 104,
        Clob = 105,
        Date = 106,
        Decimal = 107,
        Double = 108,
        Long = 109,
        LongRaw = 110,
        Int16 = 111,
        Int32 = 112,
        Int64 = 113,
        IntervalDS = 114,
        IntervalYM = 115,
        NClob = 116,
        NChar = 117,
        NVarchar2 = 119,
        Raw = 120,
        RefCursor = 121,
        Single = 122,
        TimeStamp = 123,
        TimeStampLTZ = 124,
        TimeStampTZ = 125,
        Varchar2 = 126,
        XmlType = 127,
        BinaryDouble = 132,
        BinaryFloat = 133,
        Boolean = 134
    }

    public enum Database_MySqlDbType
    {
        Decimal = 0,
        Byte = 1,
        Int16 = 2,
        Int32 = 3,
        Float = 4,
        Double = 5,
        Timestamp = 7,
        Int64 = 8,
        Int24 = 9,
        Date = 10,
        Time = 11,
        DateTime = 12,
        Datetime = 12,
        Year = 13,
        Newdate = 14,
        VarString = 15,
        Bit = 16,
        JSON = 245,
        NewDecimal = 246,
        Enum = 247,
        Set = 248,
        TinyBlob = 249,
        MediumBlob = 250,
        LongBlob = 251,
        Blob = 252,
        VarChar = 253,
        String = 254,
        Geometry = 255,
        UByte = 501,
        UInt16 = 502,
        UInt32 = 503,
        UInt64 = 508,
        UInt24 = 509,
        Binary = 600,
        VarBinary = 601,
        TinyText = 749,
        MediumText = 750,
        LongText = 751,
        Text = 752,
        Guid = 800
    }

    //
    // 요약:
    //     Represents a PostgreSQL data type that can be written or read to the database.
    //     Used in places such as Npgsql.NpgsqlParameter.NpgsqlDbType to unambiguously specify
    //     how to encode or decode values.
    //
    // 설명:
    //     See http://www.postgresql.org/docs/current/static/datatype.html
    public enum Database_NpgsqlDbType
    {
        //
        // 요약:
        //     Corresponds to the PostgreSQL "array" type, a variable-length multidimensional
        //     array of another type. This value must be combined with another value from NpgsqlTypes.NpgsqlDbType
        //     via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/arrays.html
        Array = int.MinValue,
        //
        // 요약:
        //     Corresponds to the PostgreSQL 8-byte "bigint" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Bigint = 1,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "boolean" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-boolean.html
        Boolean = 2,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "box" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Box = 3,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-binary.html
        Bytea = 4,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "circle" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Circle = 5,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "char(n)" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-character.html
        Char = 6,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "date" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        Date = 7,
        //
        // 요약:
        //     Corresponds to the PostgreSQL 8-byte floating-point "double" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Double = 8,
        //
        // 요약:
        //     Corresponds to the PostgreSQL 4-byte "integer" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Integer = 9,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "line" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Line = 10,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "lseg" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        LSeg = 11,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "money" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-money.html
        Money = 12,
        //
        // 요약:
        //     Corresponds to the PostgreSQL arbitrary-precision "numeric" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Numeric = 13,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "path" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Path = 14,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "point" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Point = 15,
        //
        // 요약:
        //     Corresponds to the PostgreSQL geometric "polygon" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-geometric.html
        Polygon = 16,
        //
        // 요약:
        //     Corresponds to the PostgreSQL floating-point "real" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Real = 17,
        //
        // 요약:
        //     Corresponds to the PostgreSQL 2-byte "smallint" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-numeric.html
        Smallint = 18,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "text" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-character.html
        Text = 19,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "time" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        Time = 20,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "timestamp" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        Timestamp = 21,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "varchar" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-character.html
        Varchar = 22,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "refcursor" type.
        Refcursor = 23,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "inet" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-net-types.html
        Inet = 24,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "bit" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-bit.html
        Bit = 25,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "timestamp with time zone" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        TimestampTZ = 26,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "timestamp with time zone" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        TimestampTz = 26,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "uuid" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-uuid.html
        Uuid = 27,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "xml" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-xml.html
        Xml = 28,
        //
        // 요약:
        //     Corresponds to the PostgreSQL internal "oidvector" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-oid.html
        Oidvector = 29,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "interval" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        Interval = 30,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "time with time zone" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        TimeTZ = 31,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "time with time zone" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        TimeTz = 31,
        //
        // 요약:
        //     Corresponds to the PostgreSQL internal "name" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-character.html
        Name = 32,
        //
        // 요약:
        //     Corresponds to the obsolete PostgreSQL "abstime" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-datetime.html
        Abstime = 33,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical
        //     address.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-net-types.html
        MacAddr = 34,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "json" type, a field storing JSON in text format.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-json.html
        Json = 35,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "jsonb" type, a field storing JSON in an optimized
        //     binary format.
        //
        // 설명:
        //     Supported since PostgreSQL 9.4. See http://www.postgresql.org/docs/current/static/datatype-json.html
        Jsonb = 36,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value
        //     pairs.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/hstore.html
        Hstore = 37,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "char" type.
        //
        // 설명:
        //     This is an internal field and should normally not be used for regular applications.
        //     See http://www.postgresql.org/docs/current/static/datatype-text.html
        InternalChar = 38,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "varbit" type, a field storing a variable-length
        //     string of bits.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-boolean.html
        Varbit = 39,
        //
        // 요약:
        //     A special value that can be used to send parameter values to the database without
        //     specifying their type, allowing the database to cast them to another value based
        //     on context. The value will be converted to a string and send as text.
        //
        // 설명:
        //     This value shouldn't ordinarily be used, and makes sense only when sending a
        //     data type unsupported by Npgsql.
        Unknown = 40,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "oid" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-oid.html
        Oid = 41,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-oid.html
        Xid = 42,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "cid" type, an internal command identifier.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-oid.html
        Cid = 43,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-net-types.html
        Cidr = 44,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "tsvector" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-textsearch.html
        TsVector = 45,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "tsquery" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-textsearch.html
        TsQuery = 46,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in
        //     the pg_type table.
        Regtype = 49,
        //
        // 요약:
        //     The geometry type for PostgreSQL spatial extension PostGIS.
        Geometry = 50,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "citext" type for the citext module.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/citext.html
        Citext = 51,
        //
        // 요약:
        //     Corresponds to the PostgreSQL internal "int2vector" type.
        Int2Vector = 52,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical
        //     location of a row within its table.
        Tid = 53,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte
        //     physical address.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-net-types.html
        MacAddr8 = 54,
        //
        // 요약:
        //     The geography (geodetic) type for PostgreSQL spatial extension PostGIS.
        Geography = 55,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "tsquery" type.
        //
        // 설명:
        //     See http://www.postgresql.org/docs/current/static/datatype-textsearch.html
        Regconfig = 56,
        //
        // 요약:
        //     Corresponds to the PostgreSQL "range" type, continuous range of values of specific
        //     type. This value must be combined with another value from NpgsqlTypes.NpgsqlDbType
        //     via a bit OR (e.g. NpgsqlDbType.Range | NpgsqlDbType.Integer)
        //
        // 설명:
        //     Supported since PostgreSQL 9.2. See http://www.postgresql.org/docs/9.2/static/rangetypes.html
        Range = 1073741824
    }

    //
    // 요약:
    //     .NET Framework 데이터 공급자의 필드, 속성 또는 Parameter 개체의 데이터 형식을 지정합니다.
    public enum Database_SQLiteType
    {
        //
        // 요약:
        //     범위가 1문자에서 8,000문자까지인 비유니코드 문자의 가변 길이 스트림입니다.
        AnsiString = 0,
        //
        // 요약:
        //     범위가 1바이트에서 8,000바이트까지인 이진 데이터의 가변 길이 스트림입니다.
        Binary = 1,
        //
        // 요약:
        //     0에서 255 사이의 값을 갖는 8비트 부호 없는 정수입니다.
        Byte = 2,
        //
        // 요약:
        //     true 또는 false의 부울 값을 나타내는 단순 형식입니다.
        Boolean = 3,
        //
        // 요약:
        //     정확성이 통화 단위의 10000분의 1이고 범위가 -2 63(또는 -922,337,203,685,477.5808)에서 2 63 -1(또는
        //     +922,337,203,685,477.5807)까지인 통화 값입니다.
        Currency = 4,
        //
        // 요약:
        //     날짜 값을 나타내는 형식입니다.
        Date = 5,
        //
        // 요약:
        //     날짜 및 시간 값을 나타내는 형식입니다.
        DateTime = 6,
        //
        // 요약:
        //     1.0 x 10-28부터 약 7.9 x 1028까지 28-29개의 유효 자릿수를 가진 값을 나타내는 단순 형식입니다.
        Decimal = 7,
        //
        // 요약:
        //     약 5.0 x 10-324부터 1.7 x 10308까지 15-16자리의 정밀도를 가진 값을 나타내는 부동 소수점 형식입니다.
        Double = 8,
        //
        // 요약:
        //     GUID(Globally Unique IDentifier)입니다.
        Guid = 9,
        //
        // 요약:
        //     -32768과 32767 사이의 값을 가진 부호 있는 16비트 정수를 나타내는 정수 계열 형식입니다.
        Int16 = 10,
        //
        // 요약:
        //     -2147483648과 2147483647 사이의 값을 가진 부호 있는 32비트 정수를 나타내는 정수 계열 형식입니다.
        Int32 = 11,
        //
        // 요약:
        //     -9223372036854775808과 9223372036854775807 사이의 값을 가진 부호 있는 64비트 정수를 나타내는 정수 계열
        //     형식입니다.
        Int64 = 12,
        //
        // 요약:
        //     다른 DbType 값에 의해 명시적으로 나타나지 않은 참조 또는 값 형식을 나타내는 일반 형식입니다.
        Object = 13,
        //
        // 요약:
        //     -128과 127 사이의 값을 가진 부호 있는 8비트 정수를 나타내는 정수 계열 형식입니다.
        SByte = 14,
        //
        // 요약:
        //     약 1.5 x 10-45부터 3.4 x 1038까지 7자리의 정밀도를 가진 값을 나타내는 부동 소수점 형식입니다.
        Single = 15,
        //
        // 요약:
        //     유니코드 문자열을 나타내는 형식입니다.
        String = 16,
        //
        // 요약:
        //     SQL Server DateTime 값을 나타내는 형식입니다.SQL Server time 값을 사용하려면 System.Data.SqlDbType.Time을
        //     사용합니다.
        Time = 17,
        //
        // 요약:
        //     0과 65535 사이의 값을 가진 부호 없는 16비트 정수를 나타내는 정수 계열 형식입니다.
        UInt16 = 18,
        //
        // 요약:
        //     0과 4294967295 사이의 값을 가진 부호 없는 32비트 정수를 나타내는 정수 계열 형식입니다.
        UInt32 = 19,
        //
        // 요약:
        //     0과 18446744073709551615 사이의 값을 가진 부호 없는 64비트 정수를 나타내는 정수 계열 형식입니다.
        UInt64 = 20,
        //
        // 요약:
        //     가변 길이 숫자 값입니다.
        VarNumeric = 21,
        //
        // 요약:
        //     유니코드 문자가 아닌 고정 길이 스트림입니다.
        AnsiStringFixedLength = 22,
        //
        // 요약:
        //     유니코드 문자의 고정 길이 문자열입니다.
        StringFixedLength = 23,
        //
        // 요약:
        //     XML 문서나 단편의 구문 분석된 표현입니다.
        Xml = 25,
        //
        // 요약:
        //     날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time 값 범위는 00:00:00부터
        //     23:59:59.9999999까지이며 정확도는 100나노초입니다.
        DateTime2 = 26,
        //
        // 요약:
        //     표준 시간대를 고려한 날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time
        //     값 범위는 00:00:00부터 23:59:59.9999999까지이며 정확도는 100나노초입니다.표준 시간대 값의 범위는 -14:00에서 +14:00
        //     사이입니다.
        DateTimeOffset = 27
    }

    //
    // 요약:
    //     System.Data.OleDb.OleDbParameter에 사용할 필드, 속성의 데이터 형식을 지정합니다.
    public enum Database_OleDbType
    {
        //
        // 요약:
        //     값(DBTYPE_EMPTY)이 없습니다.
        Empty = 0,
        //
        // 요약:
        //     부호 있는 16비트 정수(DBTYPE_I2)입니다.이는 System.Int16에 매핑합니다.
        SmallInt = 2,
        //
        // 요약:
        //     부호 있는 32비트 정수(DBTYPE_I4)입니다.이는 System.Int32에 매핑합니다.
        Integer = 3,
        //
        // 요약:
        //     -3.40E +38부터 3.40E +38 사이의 부동 소수점 숫자(DBTYPE_R4)입니다.이는 System.Single에 매핑합니다.
        Single = 4,
        //
        // 요약:
        //     -1.79E +308부터 1.79E +308 사이의 부동 소수점 숫자(DBTYPE_R8)입니다.이는 System.Double에 매핑합니다.
        Double = 5,
        //
        // 요약:
        //     -2 63(또는 -922,337,203,685,477.5808)과 2 63 -1(또는 +922,337,203,685,477.5807) 사이의
        //     범위를 가진 통화 값이며, 정확도는 통화 단위(DBTYPE_CY)의 1000분의 10까지입니다.이는 System.Decimal에 매핑합니다.
        Currency = 6,
        //
        // 요약:
        //     두 자릿수로 저장된 날짜 데이터(DBTYPE_DATE)입니다.정수 부분은 1899년 12월 30일 이후의 날짜 수이고, 소수 부분은 하루를
        //     분수로 표시한 수입니다.이는 System.DateTime에 매핑합니다.
        Date = 7,
        //
        // 요약:
        //     유니코드 문자의 null로 끝나는 문자열(DBTYPE_BSTR)입니다.이는 System.String에 매핑합니다.
        BSTR = 8,
        //
        // 요약:
        //     IDispatch 인터페이스에 대한 포인터(DBTYPE_IDISPATCH)입니다.이는 System.Object에 매핑합니다.
        IDispatch = 9,
        //
        // 요약:
        //     32비트 오류 코드(DBTYPE_ERROR)입니다.이는 System.Exception에 매핑합니다.
        Error = 10,
        //
        // 요약:
        //     부울 값(DBTYPE_BOOL)입니다.이는 System.Boolean에 매핑합니다.
        Boolean = 11,
        //
        // 요약:
        //     숫자, 문자열, 이진 또는 날짜 데이터와 함께 특수 값 Empty 및 Null을 포함하는 특수 데이터 형식(DBTYPE_VARIANT)입니다.다른
        //     지정이 없으면 이 형식으로 가정합니다.이는 System.Object에 매핑합니다.
        Variant = 12,
        //
        // 요약:
        //     IUnknown 인터페이스에 대한 포인터(DBTYPE_UNKNOWN)입니다.이는 System.Object에 매핑합니다.
        IUnknown = 13,
        //
        // 요약:
        //     -10 38 -1과 10 38 -1 사이의 고정된 정밀도 및 배율 숫자 값(DBTYPE_DECIMAL)입니다.이는 System.Decimal에
        //     매핑합니다.
        Decimal = 14,
        //
        // 요약:
        //     부호 있는 8비트 정수(DBTYPE_I1)입니다.이는 System.SByte에 매핑합니다.
        TinyInt = 16,
        //
        // 요약:
        //     부호 없는 8비트 정수(DBTYPE_UI1)입니다.이는 System.Byte에 매핑합니다.
        UnsignedTinyInt = 17,
        //
        // 요약:
        //     부호 없는 16비트 정수(DBTYPE_UI2)입니다.이는 System.UInt16에 매핑합니다.
        UnsignedSmallInt = 18,
        //
        // 요약:
        //     부호 없는 32비트 정수(DBTYPE_UI4)입니다.이는 System.UInt32에 매핑합니다.
        UnsignedInt = 19,
        //
        // 요약:
        //     부호 있는 64비트 정수(DBTYPE_I8)입니다.이는 System.Int64에 매핑합니다.
        BigInt = 20,
        //
        // 요약:
        //     부호 없는 64비트 정수(DBTYPE_UI8)입니다.이는 System.UInt64에 매핑합니다.
        UnsignedBigInt = 21,
        //
        // 요약:
        //     1601년 1월 1일 이후로 100나노초 간격의 숫자를 나타내는 부호 없는 64비트 정수(DBTYPE_FILETIME)입니다.이는 System.DateTime에
        //     매핑합니다.
        Filetime = 64,
        //
        // 요약:
        //     GUID(globally unique identifier)(DBTYPE_GUID)입니다.이는 System.Guid에 매핑합니다.
        Guid = 72,
        //
        // 요약:
        //     이진 데이터의 스트림(DBTYPE_BYTES)입니다.이는 System.Byte 형식의 System.Array에 매핑합니다.
        Binary = 128,
        //
        // 요약:
        //     문자열(DBTYPE_STR)입니다.이는 System.String에 매핑합니다.
        Char = 129,
        //
        // 요약:
        //     유니코드 문자의 null로 끝나는 스트림(DBTYPE_WSTR)입니다.이는 System.String에 매핑합니다.
        WChar = 130,
        //
        // 요약:
        //     고정된 정밀도와 배율이 있는 정확한 숫자 값(DBTYPE_NUMERIC)입니다.이는 System.Decimal에 매핑합니다.
        Numeric = 131,
        //
        // 요약:
        //     yyyymmdd 형식의 날짜 데이터(DBTYPE_DBDATE)입니다.이는 System.DateTime에 매핑합니다.
        DBDate = 133,
        //
        // 요약:
        //     hhmmss 형식의 시간 데이터(DBTYPE_DBTIME)입니다.이는 System.TimeSpan에 매핑합니다.
        DBTime = 134,
        //
        // 요약:
        //     yyyymmddhhmmss 형식의 날짜 및 시간 데이터(DBTYPE_DBTIMESTAMP)입니다.이는 System.DateTime에 매핑합니다.
        DBTimeStamp = 135,
        //
        // 요약:
        //     자동화 PROPVARIANT(DBTYPE_PROP_VARIANT)입니다.이는 System.Object에 매핑합니다.
        PropVariant = 138,
        //
        // 요약:
        //     가변 길이 숫자 값(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.Decimal에 매핑합니다.
        VarNumeric = 139,
        //
        // 요약:
        //     유니코드 아닌 문자의 가변 길이 스트림(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.String에
        //     매핑합니다.
        VarChar = 200,
        //
        // 요약:
        //     긴 문자열 값(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.String에 매핑합니다.
        LongVarChar = 201,
        //
        // 요약:
        //     유니코드 문자의 가변 길이를 갖는 null로 끝나는 스트림(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.String에
        //     매핑합니다.
        VarWChar = 202,
        //
        // 요약:
        //     긴 null로 끝나는 유니코드 문자열 값(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.String에
        //     매핑합니다.
        LongVarWChar = 203,
        //
        // 요약:
        //     이진 데이터의 가변 길이 스트림(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.Byte 형식의
        //     System.Array에 매핑합니다.
        VarBinary = 204,
        //
        // 요약:
        //     긴 이진 값(System.Data.OleDb.OleDbParameter 전용)입니다.이는 System.Byte 형식의 System.Array에
        //     매핑합니다.
        LongVarBinary = 205
    }

    //
    // 요약:
    //     System.Data.Odbc.OdbcParameter에 사용할 필드, 속성의 데이터 형식을 지정합니다.
    public enum Database_OdbcType
    {
        //
        // 요약:
        //     정밀도 19(부호 있는 숫자의 경우) 또는 20(부호 없는 숫자의 경우) 및 배율 0(부호 있는 숫자의 경우: ?2[63] <= n <=
        //     2[63] ? 1, 부호 없는 숫자의 경우:0 <= n <= 2[64] ? 1)인 정확한 숫자 값(SQL_BIGINT)입니다.이는 System.Int64에
        //     매핑합니다.
        BigInt = 1,
        //
        // 요약:
        //     이진 데이터의 스트림(SQL_BINARY)입니다.이는 System.Byte 형식의 System.Array에 매핑합니다.
        Binary = 2,
        //
        // 요약:
        //     한 비트 이진 데이터(SQL_BIT)입니다.이는 System.Boolean에 매핑합니다.
        Bit = 3,
        //
        // 요약:
        //     고정 길이 문자열(SQL_CHAR)입니다.이는 System.String에 매핑합니다.
        Char = 4,
        //
        // 요약:
        //     yyyymmddhhmmss 형식의 날짜 데이터(SQL_TYPE_TIMESTAMP)입니다.이는 System.DateTime에 매핑합니다.
        DateTime = 5,
        //
        // 요약:
        //     전체 자릿수가 p 이상이고 소수 자릿수가 s인 부호 있는 정확한 숫자 값입니다(1 <= p <= 15, s <= p).최대 전체 자릿수는
        //     드라이버에 따라 다릅니다(SQL_DECIMAL).이는 System.Decimal에 매핑합니다.
        Decimal = 6,
        //
        // 요약:
        //     1 <= p <= 15 및 s <= p인 정밀도 p와 배율 s를 가진 부호 있는 정확한 숫자 값(SQL_NUMERIC)입니다.이는 System.Decimal에
        //     매핑합니다.
        Numeric = 7,
        //
        // 요약:
        //     이진 정밀도 53(0 또는 절대값 10[?308] ~ 10[308])을 가진 부호 있는 숫자 근사값(SQL_DOUBLE)입니다.이는 System.Double에
        //     매핑합니다.
        Double = 8,
        //
        // 요약:
        //     가변 길이 이진 데이터입니다.최대 길이는 데이터 소스에 따라 다릅니다(SQL_LONGVARBINARY).이는 System.Byte 형식의
        //     System.Array에 매핑합니다.
        Image = 9,
        //
        // 요약:
        //     정밀도 10 및 배율 0(부호 있는 숫자의 경우: ?2[31] <= n <= 2[31] ? 1, 부호 없는 숫자의 경우: 0 <= n <=
        //     2[32] ? 1)인 정확한 숫자 값(SQL_INTEGER)입니다.이는 System.Int32에 매핑합니다.
        Int = 10,
        //
        // 요약:
        //     고정 문자열 길이의 유니코드 문자열(SQL_WCHAR)입니다.이는 System.String에 매핑합니다.
        NChar = 11,
        //
        // 요약:
        //     유니코드 가변 길이 문자 데이터입니다.최대 길이는 데이터 소스에 따라 다릅니다 (SQL_WLONGVARCHAR).이는 System.String에
        //     매핑합니다.
        NText = 12,
        //
        // 요약:
        //     유니코드 문자의 가변 길이 스트림(SQL_WVARCHAR)입니다.이는 System.String에 매핑합니다.
        NVarChar = 13,
        //
        // 요약:
        //     이진 정밀도 24(0 또는 절대값 10[-38] ~ 10[38])을 가진 부호 있는 숫자 근사값(SQL_REAL)입니다.이는 System.Single에
        //     매핑합니다.
        Real = 14,
        //
        // 요약:
        //     고정 길이의 GUID(SQL_GUID)입니다.이는 System.Guid에 매핑합니다.
        UniqueIdentifier = 15,
        //
        // 요약:
        //     yyyymmddhhmmss 형식의 날짜 및 시간 데이터(SQL_TYPE_TIMESTAMP)입니다.이는 System.DateTime에 매핑합니다.
        SmallDateTime = 16,
        //
        // 요약:
        //     정밀도 5 및 배율 0(부호 있는 숫자의 경우: ?32,768 <= n <= 32,767, 부호 없는 숫자의 경우: 0 <= n <= 65,535)인
        //     정확한 숫자 값(SQL_SMALLINT)입니다.이는 System.Int16에 매핑합니다.
        SmallInt = 17,
        //
        // 요약:
        //     가변 길이 문자 데이터입니다.최대 길이는 데이터 소스에 따라 다릅니다(SQL_LONGVARCHAR).이는 System.String에 매핑합니다.
        Text = 18,
        //
        // 요약:
        //     이진 데이터의 스트림(SQL_BINARY)입니다.이는 System.Byte 형식의 System.Array에 매핑합니다.
        Timestamp = 19,
        //
        // 요약:
        //     정밀도 3 및 배율 0(부호 있는 숫자의 경우: -128 <= n <= 127, 부호 없는 숫자의 경우: 0 <= n <= 255)인 정확한
        //     숫자 값(SQL_TINYINT)입니다.이는 System.Byte에 매핑합니다.
        TinyInt = 20,
        //
        // 요약:
        //     가변 길이 이진 데이터입니다.최대값은 사용자가 설정합니다(SQL_VARBINARY).이는 System.Byte 형식의 System.Array에
        //     매핑합니다.
        VarBinary = 21,
        //
        // 요약:
        //     가변 길이 스트림 문자열(SQL_CHAR)입니다.이는 System.String에 매핑합니다.
        VarChar = 22,
        //
        // 요약:
        //     yyyymmdd 형식의 날짜 데이터(SQL_TYPE_DATE)입니다.이는 System.DateTime에 매핑합니다.
        Date = 23,
        //
        // 요약:
        //     hhmmss 형식의 날짜 데이터(SQL_TYPE_TIMES)입니다.이는 System.DateTime에 매핑합니다.
        Time = 24
    }

    //
    // 요약:
    //     System.Data.SqlClient.SqlParameter에 사용할 필드, 속성의 SQL Server 데이터 형식을 지정합니다.
    public enum Database_SqlDbType
    {
        //
        // 요약:
        //     System.Int64.64비트 부호 있는 정수입니다.
        BigInt = 0,
        //
        // 요약:
        //     System.Byte 형식의 System.Array입니다.범위가 1바이트에서 8,000바이트까지인 이진 데이터의 고정 길이 스트림입니다.
        Binary = 1,
        //
        // 요약:
        //     System.Boolean.0, 1 또는 null일 수 있는 부호 없는 숫자 값입니다.
        Bit = 2,
        //
        // 요약:
        //     System.String.범위가 1자에서 8,000자까지이고 유니코드가 아닌 문자의 고정 길이 스트림입니다.
        Char = 3,
        //
        // 요약:
        //     System.DateTime.3.33밀리초의 정확성으로 값의 범위가 1753년 1월 1일에서 9999년 12월 31일까지인 날짜 및 시간
        //     데이터입니다.
        DateTime = 4,
        //
        // 요약:
        //     System.Decimal.-10 38 -1과 10 38 -1 사이의 고정 전체 자릿수 및 소수 자릿수 값입니다.
        Decimal = 5,
        //
        // 요약:
        //     System.Double.범위가 -1.79E +308에서 1.79E +308까지인 부동 소수점 숫자입니다.
        Float = 6,
        //
        // 요약:
        //     System.Byte 형식의 System.Array입니다.범위가 0바이트에서 2 31 -1(또는 2,147,483,647)바이트까지인 이진
        //     데이터의 가변 길이 스트림입니다.
        Image = 7,
        //
        // 요약:
        //     System.Int32.32비트 부호 있는 정수입니다.
        Int = 8,
        //
        // 요약:
        //     System.Decimal.정확성이 통화 단위의 10000분의 1이고 범위가 -2 63(또는 -922,337,203,685,477.5808)에서
        //     2 63 -1(또는 +922,337,203,685,477.5807)까지인 통화 값입니다.
        Money = 9,
        //
        // 요약:
        //     System.String.범위가 1자에서 4,000자까지인 유니코드 문자의 고정 길이 스트림입니다.
        NChar = 10,
        //
        // 요약:
        //     System.String.최대 길이가 2 30 - 1(또는 1,073,741,823)자인 유니코드 데이터의 가변 길이 스트림입니다.
        NText = 11,
        //
        // 요약:
        //     System.String.범위가 1자에서 4,000자까지인 유니코드 문자의 가변 길이 스트림입니다.문자열이 4,000자보다 더 큰 경우 암시적
        //     변환이 실패합니다.4,000자보다 더 긴 문자열로 작업할 경우 개체를 명시적으로 설정합니다.데이터 열이 nvarchar(max)일 경우 System.Data.SqlDbType.NVarChar를
        //     사용합니다.
        NVarChar = 12,
        //
        // 요약:
        //     System.Single.범위가 -3.40E +38에서 3.40E +38까지인 부동 소수점 숫자입니다.
        Real = 13,
        //
        // 요약:
        //     System.Guid.GUID(Globally Unique IDentifier)입니다.
        UniqueIdentifier = 14,
        //
        // 요약:
        //     System.DateTime.1분의 정확성으로 값의 범위가 1900년 1월 1일에서 2079년 6월 6일까지인 날짜 및 시간 데이터입니다.
        SmallDateTime = 15,
        //
        // 요약:
        //     System.Int16.16비트 부호 있는 정수입니다.
        SmallInt = 16,
        //
        // 요약:
        //     System.Decimal.통화 단위의 10000분의 1 정확성으로 범위가 -214,748.3648에서 +214,748.3647까지인 통화
        //     값입니다.
        SmallMoney = 17,
        //
        // 요약:
        //     System.String.최대 길이가 2 31 -1(또는 2,147,483,647)자이고 유니코드가 아닌 데이터의 가변 길이 스트림입니다.
        Text = 18,
        //
        // 요약:
        //     System.Byte 형식의 System.Array입니다.데이터베이스 내에서 고유한 자동 생성되는 이진 숫자입니다.timestamp는 일반적으로
        //     버전이 표시되는 테이블 행에 대한 메커니즘으로 사용됩니다.저장소 크기는 8바이트입니다.
        Timestamp = 19,
        //
        // 요약:
        //     System.Byte.8비트 부호 없는 정수입니다.
        TinyInt = 20,
        //
        // 요약:
        //     System.Byte 형식의 System.Array입니다.범위가 1바이트에서 8,000바이트까지인 이진 데이터의 가변 길이 스트림입니다.바이트
        //     배열이 8.000바이트보다 더 큰 경우 암시적 변환이 실패합니다.8.000바이트보다 더 큰 바이트 배열로 작업할 경우 개체를 명시적으로 설정합니다.
        VarBinary = 21,
        //
        // 요약:
        //     System.String.범위가 1문자에서 8,000문자까지인 비유니코드 문자의 가변 길이 스트림입니다.데이터 열이 varchar(max)일
        //     경우 System.Data.SqlDbType.VarChar를 사용합니다.
        VarChar = 22,
        //
        // 요약:
        //     System.Object.SQL Server 값 Empty 및 Null뿐만 아니라 숫자, 문자열, 이진 데이터 또는 날짜 데이터를 포함할
        //     수 있는 특수 데이터 형식으로 다른 데이터 형식이 선언되지 않으면 이 형식이 사용됩니다.
        Variant = 23,
        //
        // 요약:
        //     XML 값입니다.System.Data.SqlClient.SqlDataReader.GetValue(System.Int32) 메서드나 System.Data.SqlTypes.SqlXml.Value
        //     속성을 사용하여 XML을 문자열로 가져오거나 System.Data.SqlTypes.SqlXml.CreateReader 메서드를 호출하여 XML을
        //     System.Xml.XmlReader로 가져옵니다.
        Xml = 25,
        //
        // 요약:
        //     SQL Server 사용자 정의 형식(UDT)입니다.
        Udt = 29,
        //
        // 요약:
        //     테이블 반환 매개 변수에 들어 있는 구조적 데이터를 지정하기 위한 특수 데이터 형식입니다.
        Structured = 30,
        //
        // 요약:
        //     값 범위가 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이인 날짜 데이터입니다.
        Date = 31,
        //
        // 요약:
        //     24시간제 시간 데이터입니다.Time 값 범위는 00:00:00부터 23:59:59.9999999까지이며 정확도는 100나노초입니다.SQL
        //     Server time 값에 해당합니다.
        Time = 32,
        //
        // 요약:
        //     날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time 값 범위는 00:00:00부터
        //     23:59:59.9999999까지이며 정확도는 100나노초입니다.
        DateTime2 = 33,
        //
        // 요약:
        //     표준 시간대를 고려한 날짜 및 시간 데이터입니다.날짜 값 범위는 서기 1년 1월 1일에서 서기 9999년 12월 31일 사이입니다.Time
        //     값 범위는 00:00:00부터 23:59:59.9999999까지이며 정확도는 100나노초입니다.표준 시간대 값의 범위는 -14:00에서 +14:00
        //     사이입니다.
        DateTimeOffset = 34
    }
}
