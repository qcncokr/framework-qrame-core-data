using System;
using System.Transactions;

namespace Qrame.CoreFX.Data
{
    /// <summary>
    /// 닷넷 프레임워크의 데이터 공급자의 기능 수행시 Transaction Promotion을 지원하기 위한 
    /// TransactionScope 클래스의 데코레이터를 제공합니다. 이 클래스는 상속될 수 없습니다.
    /// 
    /// 참고자료 : http://www.simpleisbest.net/articles/996.aspx
    /// </summary>
    public sealed class DatabaseTransaction : IDisposable
    {
        /// <summary>
        /// Transaction Promotion 기능을 제공하는 객체입니다. 
        /// </summary>
        private TransactionScope transactionScope;

        /// <summary>
        /// 데이터베이스 DbConnection 참조입니다.
        /// </summary>
        private Transaction transaction;

        /// <summary>
        /// TransactionScope 객체에 추가 옵션을 부여합니다.
        /// </summary>
        private TransactionScopeOption transactionScopeOption;

        /// <summary>
        /// 데이터베이스 DbConnection에 Transaction 수행시 Timeout 정보를 가지고 있는 시간 간격입니다.
        /// </summary>
        private TimeSpan transactionScopeTimeout;

        /// <summary>
        /// 데이터베이스 DbConnection에 Transaction 옵션을 설정합니다.
        /// </summary>
        private TransactionOptions transactionOptions;

        /// <summary>
        /// COM+와 분산 Transaction을 지원해야 하는 옵션입니다.
        /// </summary>
        private EnterpriseServicesInteropOption interopOption;

        /// <summary>
        /// Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.
        /// </summary>
        private DataProviders dataProviders;

        /// <summary>
        /// 데이터 소스가 Transaction을 지원하는지 확인합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <returns>데이터 소스가 Transaction을 지원하는지 여부를 반환합니다.</returns>
        private bool IsSupportTransaction(DataProviders providers)
        {
            bool result = false;
            switch (providers)
            {
                case DataProviders.SqlServer:
                    result = true;
                    break;
                case DataProviders.Oracle:
                    result = true;
                    break;
                case DataProviders.MySQL:
                    result = true;
                    break;
                case DataProviders.PostgreSQL:
                    result = true;
                    break;
                case DataProviders.SQLite:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            dataProviders = providers;
            return result;
        }

        /// <summary>
        /// 인스턴스 생성시, Transaction Promotion을 설정합니다. Transaction을 지원하지 않는 데이터 소스일 경우 에러가 발생 할 수 있습니다.
        /// </summary>
        public DatabaseTransaction()
        {
            transactionScope = new TransactionScope();
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        public DatabaseTransaction(DataProviders dataProviders)
        {
            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope();
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="transactionToUse"></param>
        public DatabaseTransaction(DataProviders dataProviders, Transaction transactionToUse)
        {
            transaction = transactionToUse;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transaction);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="scopeOption">Provides additional options for creating a transaction scope.</param>
        public DatabaseTransaction(DataProviders dataProviders, TransactionScopeOption scopeOption)
        {
            transactionScopeOption = scopeOption;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transactionScopeOption);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="transactionToUse">Represents a transaction.</param>
        /// <param name="scopeTimeout">Transaction 시간 간격을 나타냅니다.</param>
        public DatabaseTransaction(DataProviders dataProviders, Transaction transactionToUse, TimeSpan scopeTimeout)
        {
            transaction = transactionToUse;
            scopeTimeout = scopeTimeout;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transaction, scopeTimeout);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="scopeOption">Provides additional options for creating a transaction scope.</param>
        /// <param name="scopeTimeout">Transaction 시간 간격을 나타냅니다.</param>
        public DatabaseTransaction(DataProviders dataProviders, TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            transactionScopeOption = scopeOption;
            transactionScopeTimeout = scopeTimeout;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transactionScopeOption, scopeTimeout);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="scopeOption">Provides additional options for creating a transaction scope.</param>
        /// <param name="TransactionOptions">Contains additional information that specifies transaction behaviors.</param>
        public DatabaseTransaction(DataProviders dataProviders, TransactionScopeOption scopeOption, TransactionOptions TransactionOptions)
        {
            transactionScopeOption = scopeOption;
            transactionOptions = TransactionOptions;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transactionScopeOption, transactionOptions);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="TransactionToUse">Represents a transaction.</param>
        /// <param name="scopeTimeout">Transaction 시간 간격을 나타냅니다.</param>
        /// <param name="InteropOption"></param>
        public DatabaseTransaction(DataProviders dataProviders, Transaction TransactionToUse, TimeSpan scopeTimeout, EnterpriseServicesInteropOption InteropOption)
        {
            transaction = TransactionToUse;
            transactionScopeTimeout = scopeTimeout;
            interopOption = InteropOption;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transaction, scopeTimeout, interopOption);
            }
        }

        /// <summary>
        /// 인스턴스 생성시, 지정한 데이터 소스에 Transaction Promotion을 설정합니다.
        /// </summary>
        /// <param name="dataProviders">Qrame.CoreFX.Data에서 지원하는 데이터 소스 열거자입니다.</param>
        /// <param name="scopeOption">Provides additional options for creating a transaction scope.</param>
        /// <param name="TransactionOptions">Contains additional information that specifies transaction behaviors.</param>
        /// <param name="InteropOption">Specifies how distributed transactions interact with COM+ transactions.</param>
        public DatabaseTransaction(DataProviders dataProviders, TransactionScopeOption scopeOption, TransactionOptions TransactionOptions, EnterpriseServicesInteropOption InteropOption)
        {
            transactionScopeOption = scopeOption;
            transactionOptions = TransactionOptions;
            interopOption = InteropOption;

            if (IsSupportTransaction(dataProviders) == true)
            {
                transactionScope = new TransactionScope(transactionScopeOption, transactionOptions, interopOption);
            }
        }

        /// <summary>
        /// Transaction을 Commit합니다. 이 메소드를 호출 하지 않고 Transaction을 종료하면 Rollback됩니다.
        /// </summary>
        public void Complete()
        {
            if (transactionScope != null)
            {
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// DatabaseTransaction에서 사용하는 리소스(메모리 제외)를 삭제합니다.
        /// </summary>
        public void Dispose()
        {
            if (transactionScope != null)
            {
                transactionScope.Dispose();
            }
        }
    }
}
