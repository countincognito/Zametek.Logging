using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using System;

namespace Zametek.Utility.Logging
{
    public static class ServiceCollectionExtensions
    {
        #region Fields

        private static LogTypes s_DefaultLogTypes = LogProxy.DefaultLogTypes;

        #endregion

        #region Public Members

        public static IServiceCollection ActivateLogTypes(
            this IServiceCollection services,
            LogTypes logTypes)
        {
            s_DefaultLogTypes |= logTypes;
            return services;
        }

        public static IServiceCollection DeactivateLogTypes(
            this IServiceCollection services,
            LogTypes logTypes)
        {
            s_DefaultLogTypes &= ~logTypes;
            return services;
        }

        #region Singletons

        public static IServiceCollection TryAddSingletonWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Type implementationType)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddSingleton(implementationType);
            collection.TryAddSingleton(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationType));
            return collection;
        }

        public static IServiceCollection TryAddSingletonWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddSingleton(implementationFactory);
            collection.TryAddSingleton(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationFactory));
            return collection;
        }

        public static IServiceCollection TryAddSingletonWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddSingleton<TImplementation>();
            collection.TryAddSingleton(provider => provider.CreateProxy<TService, TImplementation>());
            return collection;
        }

        public static IServiceCollection TryAddSingletonWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {

            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddSingleton<TImplementation>();
            collection.TryAddSingleton(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));
            return collection;
        }

        #endregion

        #region Scoped

        public static IServiceCollection TryAddScopedWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Type implementationType)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddScoped(implementationType);
            collection.TryAddScoped(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationType));
            return collection;
        }

        public static IServiceCollection TryAddScopedWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddScoped(implementationFactory);
            collection.TryAddScoped(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationFactory));
            return collection;
        }

        public static IServiceCollection TryAddScopedWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddScoped<TImplementation>();
            collection.TryAddScoped(provider => provider.CreateProxy<TService, TImplementation>());
            return collection;
        }

        public static IServiceCollection TryAddScopedWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {

            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddScoped<TImplementation>();
            collection.TryAddScoped(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));
            return collection;
        }

        #endregion

        #region Transient

        public static IServiceCollection TryAddTransientWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Type implementationType)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddTransient(implementationType);
            collection.TryAddTransient(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationType));
            return collection;
        }

        public static IServiceCollection TryAddTransientWithLogProxy(
            this IServiceCollection collection,
            Type interfaceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            interfaceType.ThrowIfNotInterface();

            collection.TryAddTransient(implementationFactory);
            collection.TryAddTransient(
                interfaceType,
                provider => provider.CreateProxy(interfaceType, implementationFactory));
            return collection;
        }

        public static IServiceCollection TryAddTransientWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddTransient<TImplementation>();
            collection.TryAddTransient(provider => provider.CreateProxy<TService, TImplementation>());
            return collection;
        }

        public static IServiceCollection TryAddTransientWithLogProxy<TService, TImplementation>(
            this IServiceCollection collection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {

            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            typeof(TService).ThrowIfNotInterface();

            collection.TryAddTransient<TImplementation>();
            collection.TryAddTransient(provider => provider.CreateProxy<TService, TImplementation>(implementationFactory));
            return collection;
        }

        #endregion

        #endregion

        #region Private Members

        private static object CreateProxy(
            this IServiceProvider serviceProvider,
            Type interfaceType,
            Type implementationType)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            interfaceType.ThrowIfNotInterface();

            object service = serviceProvider.GetRequiredService(implementationType);
            ILogger logger = serviceProvider.GetRequiredService<ILogger>().ForContext(implementationType);
            return LogProxy.Create(interfaceType, service, logger, s_DefaultLogTypes);
        }

        private static object CreateProxy(
            this IServiceProvider serviceProvider,
            Type interfaceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (implementationFactory is null)
            {
                throw new ArgumentNullException(nameof(implementationFactory));
            }
            interfaceType.ThrowIfNotInterface();

            object service = implementationFactory(serviceProvider);
            Type implementationType = service.GetType();
            ILogger logger = serviceProvider.GetRequiredService<ILogger>().ForContext(implementationType);
            return LogProxy.Create(interfaceType, service, logger, s_DefaultLogTypes);
        }

        private static TService CreateProxy<TService, TImplementation>(this IServiceProvider serviceProvider)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            typeof(TService).ThrowIfNotInterface();

            TImplementation service = serviceProvider.GetRequiredService<TImplementation>();
            ILogger logger = serviceProvider.GetRequiredService<ILogger>().ForContext<TImplementation>();
            return LogProxy.Create<TService>(service, logger, s_DefaultLogTypes);
        }

        private static TService CreateProxy<TService, TImplementation>(
            this IServiceProvider serviceProvider,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            typeof(TService).ThrowIfNotInterface();

            TImplementation service = implementationFactory(serviceProvider);
            ILogger logger = serviceProvider.GetRequiredService<ILogger>().ForContext<TImplementation>();
            return LogProxy.Create<TService>(service, logger, s_DefaultLogTypes);
        }

        #endregion
    }
}
