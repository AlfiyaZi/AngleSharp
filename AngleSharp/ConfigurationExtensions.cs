﻿namespace AngleSharp
{
    using AngleSharp.Dom.Css;
    using AngleSharp.Events;
    using AngleSharp.Events.Default;
    using AngleSharp.Extensions;
    using AngleSharp.Network;
    using AngleSharp.Network.Default;
    using AngleSharp.Services;
    using AngleSharp.Services.Default;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// A set of useful extensions for Configuration (or derived) objects.
    /// </summary>
    [DebuggerStepThrough]
    public static class ConfigurationExtensions
    {
        #region General

        /// <summary>
        /// Returns a new configuration that includes the given service.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="service">The service to register.</param>
        /// <returns>The new instance with the service.</returns>
        public static Configuration With(this IConfiguration configuration, IService service)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var services = configuration.Services.Concat(service);
            return new Configuration(services, configuration.Events, configuration.Culture);
        }

        /// <summary>
        /// Returns a new configuration that uses the culture with the provided
        /// name.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="cultureName">The culture to set.</param>
        /// <returns>The new instance with the culture being set.</returns>
        public static Configuration SetCulture(this IConfiguration configuration, String cultureName)
        {
            if (cultureName == null)
            {
                throw new ArgumentNullException("cultureName");
            }
            
            return configuration.SetCulture(new CultureInfo(cultureName));
        }

        /// <summary>
        /// Returns a new configuration that uses the given culture. Providing
        /// null will reset the culture to the default one.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="culture">The culture to set.</param>
        /// <returns>The new instance with the culture being set.</returns>
        public static Configuration SetCulture(this IConfiguration configuration, CultureInfo culture)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return new Configuration(configuration.Services, configuration.Events, culture);
        }

        /// <summary>
        /// Returns a new configuration that uses the given event aggregator.
        /// Providing null will reset the events to the default one.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="events">The event aggregator to set.</param>
        /// <returns>The new instance with the events being set.</returns>
        public static Configuration SetEvents(this IConfiguration configuration, IEventAggregator events)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return new Configuration(configuration.Services, events, configuration.Culture);
        }

        /// <summary>
        /// Uses the currently active event aggregator or integrates a simple
        /// default aggregator to add the subscriber.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="subscriber">The subscriber to add.</param>
        /// <returns>The current instance, or a new instance.</returns>
        public static IConfiguration SetHandler<T>(this IConfiguration configuration, ISubscriber<T> subscriber)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (configuration.Events == null)
            {
                configuration = configuration.SetEvents(new SimpleEventAggregator());
            }

            configuration.Events.Subscribe(subscriber);
            return configuration;
        }

        #endregion

        #region Styling

        /// <summary>
        /// Registers the default styling service with a new CSS style engine
        /// to retrieve, if no other styling service has been registered yet.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="setup">Optional setup for the style engine.</param>
        /// <returns>The new instance with the service.</returns>
        public static IConfiguration WithCss(this IConfiguration configuration, Action<CssStyleEngine> setup = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            
            if (!configuration.GetServices<IStylingService>().Any())
            {
                var service = new StylingService();
                var engine = new CssStyleEngine();

                if (setup != null)
                {
                    setup(engine);
                }

                service.Register(engine);
                return configuration.With(service);
            }

            return configuration;
        }

        #endregion

        #region Loading Resources

        /// <summary>
        /// Registers the default loader service, if no other loader has been
        /// registered yet.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="setup">Optional setup for the loader service.</param>
        /// <param name="requesters">Optional requesters to use.</param>
        /// <returns>The new instance with the service.</returns>
        public static IConfiguration WithDefaultLoader(this IConfiguration configuration, Action<LoaderService> setup = null, IEnumerable<IRequester> requesters = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.GetServices<ILoaderService>().Any())
            {
                if (requesters == null)
                {
                    requesters = new IRequester[] { new HttpRequester(), new DataRequester() };
                }

                var service = new LoaderService(requesters);

                if (setup != null)
                {
                    setup(service);
                }
                
                return configuration.With(service);
            }

            return configuration;
        }

        #endregion

        #region Setting Encoding

        /// <summary>
        /// Registeres the default encoding determination algorithm, as
        /// specified by the W3C.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <returns>The new instance with the service.</returns>
        public static IConfiguration WithLocaleBasedEncoding(this IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentException("configuration");
            }

            if (!configuration.GetServices<IEncodingService>().Any())
            {
                var service = new LocaleEncodingService();
                return configuration.With(service);
            }

            return configuration;
        }

        #endregion

        #region Cookies

        /// <summary>
        /// Registers the default cookie service if no other cookie service has
        /// been registered yet.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <returns>The new instance with the service.</returns>
        public static IConfiguration WithCookies(this IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.GetServices<ICookieService>().Any())
            {
                var service = new MemoryCookieService();
                return configuration.With(service);
            }

            return configuration;
        }

        #endregion
    }
}
