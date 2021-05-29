using Qrame.CoreFX.Data.ExtensionMethod;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Qrame.CoreFX.Data.Profiler
{
    public class ProfilerFactory
    {
        private static Func<IAdoNetProfiler> _constructor;

        private static bool _initialized = false;
        private static readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        public static void Initialize(Type profilerType)
        {
            if (profilerType == null)
            {
                throw new ArgumentNullException(nameof(profilerType));
            }

            if (profilerType.GetInterfaces().All(x => x != typeof(IAdoNetProfiler)))
            {
                throw new ArgumentException($"The type must be {typeof(IAdoNetProfiler).FullName}.", nameof(profilerType));
            }

            _readerWriterLockSlim.ExecuteWithReadLock(() =>
            {
                if (_initialized)
                {
                    throw new InvalidOperationException("This factory class has already initialized.");
                }

                ProviderUtility.InitialzeDbProviderFactory();

                _constructor = Expression.Lambda<Func<IAdoNetProfiler>>(Expression.New(profilerType)).Compile()
                    ?? throw new InvalidOperationException("There is no default constructor. The profiler must have it.");

                _initialized = true;
            });
        }

        internal static IAdoNetProfiler GetProfiler()
        {
            return _readerWriterLockSlim.ExecuteWithWriteLock(() =>
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("This factory class has not initialized yet.");
                }

                return _constructor.Invoke();
            });
        }
    }
}