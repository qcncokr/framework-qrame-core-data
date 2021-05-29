using System;
using System.Data.Common;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Qrame.CoreFX.Data.Profiler
{
	public abstract class ProfilerProviderFactory : DbProviderFactory
	{
	}

	public class ProfilerProviderFactory<TProviderFactory> : ProfilerProviderFactory, IServiceProvider
		where TProviderFactory : DbProviderFactory
	{
		public static readonly ProfilerProviderFactory<TProviderFactory> Instance = new ProfilerProviderFactory<TProviderFactory>();

		public TProviderFactory WrappedProviderFactory { get; }

		public override bool CanCreateDataSourceEnumerator => WrappedProviderFactory.CanCreateDataSourceEnumerator;

		public ProfilerProviderFactory()
		{
			var field = typeof(TProviderFactory).GetField("Instance", BindingFlags.Public | BindingFlags.Static);

			if (field == null)
			{
				throw new NotSupportedException("Provider doesn't have Instance property.");
			}

			WrappedProviderFactory = (TProviderFactory)field.GetValue(null);
		}

		public override DbCommand CreateCommand()
		{
			var command = WrappedProviderFactory.CreateCommand();
			var connection = (ProfilerDbConnection)WrappedProviderFactory.CreateConnection();
			var profiler = connection.Profiler;

			return new ProfilerDbCommand(command, connection, profiler);
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return WrappedProviderFactory.CreateCommandBuilder();
		}

		public override DbConnection CreateConnection()
		{
			var connection = WrappedProviderFactory.CreateConnection();

			return new ProfilerDbConnection(connection);
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return WrappedProviderFactory.CreateConnectionStringBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return WrappedProviderFactory.CreateDataAdapter();
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return WrappedProviderFactory.CreateDataSourceEnumerator();
		}

		public override DbParameter CreateParameter()
		{
			return WrappedProviderFactory.CreateParameter();
		}

		public object GetService(Type serviceType)
		{
			if (serviceType == GetType())
			{
				return WrappedProviderFactory;
			}

			var service = ((IServiceProvider)WrappedProviderFactory).GetService(serviceType);

			return service;
		}
	}
}
