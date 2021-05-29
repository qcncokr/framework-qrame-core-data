using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace Qrame.CoreFX.Data.Profiler
{
    public class ProfilerDbCommand : DbCommand
    {
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IAdoNetProfiler _profiler;
        private static readonly Hashtable _bindByNameGetCache = new Hashtable();
        private static readonly Hashtable _bindByNameSetCache = new Hashtable();

        public override string CommandText
        {
            get { return WrappedCommand.CommandText; }
            set { WrappedCommand.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return WrappedCommand.CommandTimeout; }
            set { WrappedCommand.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return WrappedCommand.CommandType; }
            set
            {
                if (value != CommandType.Text &&
                    value != CommandType.StoredProcedure &&
                    value != CommandType.TableDirect)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                WrappedCommand.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
            set
            {
                _connection = value;

                var adoNetProfilerDbConnection = value as ProfilerDbConnection;

                WrappedCommand.Connection = (adoNetProfilerDbConnection == null)
                    ? value
                    : adoNetProfilerDbConnection.WrappedConnection;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { 
                return WrappedCommand.Parameters; 
            }
        }

        protected override DbTransaction DbTransaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;

                var adoNetProfilerDbTransaction = value as ProfilerDbTransaction;

                WrappedCommand.Transaction = (adoNetProfilerDbTransaction == null)
                    ? value
                    : adoNetProfilerDbTransaction.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get { return WrappedCommand.DesignTimeVisible; }
            set { WrappedCommand.DesignTimeVisible = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return WrappedCommand.UpdatedRowSource; }
            set { WrappedCommand.UpdatedRowSource = value; }
        }

        public DbCommand WrappedCommand { get; private set; }

        public bool BindByName
        {
            get
            {
                var cache = GetBindByNameGetAction(WrappedCommand.GetType());

                return cache != null ? cache.Invoke(WrappedCommand) : false;
            }
            set
            {
                var cache = GetBindByNameSetAction(WrappedCommand.GetType());

                if (cache != null)
                {
                    cache.Invoke(WrappedCommand, value);
                }
            }
        }

        internal ProfilerDbCommand(DbCommand command, DbConnection connection, IAdoNetProfiler profiler)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            WrappedCommand = command;

            _connection = connection;
            _profiler = profiler;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteReader(behavior);
            }

            _profiler.OnExecuteReaderStart(this);

            try
            {
                var dbReader = WrappedCommand.ExecuteReader(behavior);

                return new ProfilerDbDataReader(dbReader, _profiler);
            }
            catch (Exception exception)
            {
                _profiler.OnCommandError(this, exception);

                throw exception;
            }
        }

        public override int ExecuteNonQuery()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteNonQuery();
            }

            _profiler.OnExecuteNonQueryStart(this);

            var result = default(int?);

            try
            {
                result = WrappedCommand.ExecuteNonQuery();

                return result.Value;
            }
            catch (Exception exception)
            {
                _profiler.OnCommandError(this, exception);

                throw exception;
            }
            finally
            {
                _profiler.OnExecuteNonQueryFinish(this, result ?? 0);
            }
        }

        public override object ExecuteScalar()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteScalar();
            }

            _profiler.OnExecuteScalarStart(this);

            object result = null;

            try
            {
                result = WrappedCommand.ExecuteScalar();

                return result;
            }
            catch (Exception exception)
            {
                _profiler.OnCommandError(this, exception);

                throw exception;
            }
            finally
            {
                _profiler.OnExecuteScalarFinish(this, result);
            }
        }

        public override void Cancel()
        {
            WrappedCommand.Cancel();
        }

        public override void Prepare()
        {
            WrappedCommand.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return WrappedCommand.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WrappedCommand?.Dispose();
            }

            WrappedCommand = null;

            base.Dispose(disposing);
        }

        private Func<DbCommand, bool> GetBindByNameGetAction(Type commandType)
        {
            lock (_bindByNameGetCache)
            {
                if (_bindByNameGetCache[commandType] is Func<DbCommand, bool> cache)
                {
                    return cache;
                }

                var property = commandType
                    .GetTypeInfo()
                    .GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null
                    && property.CanRead
                    && property.PropertyType == typeof(bool)
                    && property.GetIndexParameters().Length == 0
                    && property.GetGetMethod() != null)
                {
                    var target = Expression.Parameter(typeof(DbCommand), "target");
                    var prop   = Expression.PropertyOrField(Expression.Convert(target, commandType), "BindByName");

                    var action = Expression.Lambda<Func<DbCommand, bool>>(Expression.Convert(prop, typeof(bool)), target).Compile();

                    _bindByNameGetCache.Add(commandType, action);

                    return action;
                }
            }

            return null;
        }

        private Action<DbCommand, bool> GetBindByNameSetAction(Type commandType)
        {
            lock (_bindByNameSetCache)
            {
                if (_bindByNameSetCache[commandType] is Action<DbCommand, bool> cache)
                {
                    return cache;
                }

                var property = commandType
                    .GetTypeInfo()
                    .GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
                
                if (property != null
                    && property.CanWrite
                    && property.PropertyType == typeof(bool)
                    && property.GetIndexParameters().Length == 0
                    && property.GetSetMethod() != null)
                {
                    var target = Expression.Parameter(typeof(DbCommand), "target");
                    var value  = Expression.Parameter(typeof(bool), "value");

                    var left  = Expression.PropertyOrField(Expression.Convert(target, commandType), "BindByName");
                    var right = Expression.Convert(value, left.Type);

                    var action = Expression.Lambda<Action<DbCommand, bool>>(Expression.Assign(left, right), target, value).Compile();

                    _bindByNameSetCache.Add(commandType, action);

                    return action;
                }
            }

            return null;
        }
    }
}
