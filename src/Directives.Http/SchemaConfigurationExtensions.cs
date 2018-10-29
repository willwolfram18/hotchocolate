using System;

namespace HotChocolate.Directives.Http
{
    public static class SchemaConfigurationExtensions
    {
        public static ISchemaConfiguration RegisterHttpDirectives(
            this ISchemaConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration
                .RegisterHttpGetDirectiveCore()
                .RegisterUrlDecodeDirectiveCore()
                .RegisterUrlEncodeDirectiveCore();
        }

        #region RegisterHttpGetDirective

        public static ISchemaConfiguration RegisterHttpGetDirective(
            this ISchemaConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration.RegisterHttpGetDirectiveCore();
        }

        private static ISchemaConfiguration RegisterHttpGetDirectiveCore(
            this ISchemaConfiguration configuration)
        {
            configuration.RegisterDataLoader<HttpGetDataLoader>();
            configuration.RegisterDirective<HttpGetDirectiveType>();

            return configuration;
        }

        #endregion

        #region RegisterUrlDecodeDirective

        public static ISchemaConfiguration RegisterUrlDecodeDirective(
            this ISchemaConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration.RegisterUrlDecodeDirectiveCore();
        }

        private static ISchemaConfiguration RegisterUrlDecodeDirectiveCore(
            this ISchemaConfiguration configuration)
        {
            configuration.RegisterDirective<UrlDecodeDirectiveType>();

            return configuration;
        }

        #endregion

        #region RegisterUrlEncodeDirective

        public static ISchemaConfiguration RegisterUrlEncodeDirective(
            this ISchemaConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration.RegisterUrlEncodeDirectiveCore();
        }

        private static ISchemaConfiguration RegisterUrlEncodeDirectiveCore(
            this ISchemaConfiguration configuration)
        {
            configuration.RegisterDirective<UrlEncodeDirectiveType>();

            return configuration;
        }

        #endregion
    }
}
