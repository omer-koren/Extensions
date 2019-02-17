using System;
using System.IO;
using Koren.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Koren.Extensions.Configuration
{

    public static class JsonFileConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path)
        {
            return AddJsonFile(builder, provider: null, path: path, optional: false, reloadOnChange: false, expandEnvironmentVariables: true);
        }

        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false,expandEnvironmentVariables : true);
        }

        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange, expandEnvironmentVariables: true);
        }

        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange,bool expandEnvironmentVariables)
        {
            return AddJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange, expandEnvironmentVariables : expandEnvironmentVariables);
        }


        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange,bool expandEnvironmentVariables)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"Invalid file path {path}");
            }

            return builder.AddJsonFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ExpandEnvironmentVariables = expandEnvironmentVariables;

                s.ResolveFileProvider();
            });
        }

        public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, Action<JsonFileConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}