// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Koren.Extensions.Configuration.Json
{
    internal class JsonConfigurationParser
    {
        private JsonConfigurationParser(bool expandEnvVars) {
            _expandEnvVars = expandEnvVars;
        }

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private readonly bool _expandEnvVars;
        private string _currentPath;
        private EnvironmentVariableExpander _expander = new EnvironmentVariableExpander();

        private JsonTextReader _reader;

        public static IDictionary<string, string> Parse(Stream input,bool expandEnvVars)
        {
            try
            {
                return new JsonConfigurationParser(expandEnvVars).ParseStream(input);
            }
            catch (JsonReaderException e)
            {
                string errorLine = string.Empty;
                if (input.CanSeek)
                {
                    input.Seek(0, SeekOrigin.Begin);

                    IEnumerable<string> fileContent;
                    using (var streamReader = new StreamReader(input))
                    {
                        fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(e, fileContent);
                    }
                }

                throw new FormatException($"Error parsing json line {e.LineNumber} position {e.LinePosition}", e);
            }
        }


        private static string RetrieveErrorContext(JsonReaderException e, IEnumerable<string> fileContent)
        {
            string errorLine = null;
            if (e.LineNumber >= 2)
            {
                var errorContext = fileContent.Skip(e.LineNumber - 2).Take(2).ToList();
                // Handle situations when the line number reported is out of bounds
                if (errorContext.Count() >= 2)
                {
                    errorLine = errorContext[0].Trim() + Environment.NewLine + errorContext[1].Trim();
                }
            }
            if (string.IsNullOrEmpty(errorLine))
            {
                var possibleLineContent = fileContent.Skip(e.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }
            return errorLine;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }

        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();
            _reader = new JsonTextReader(new StreamReader(input));
            _reader.DateParseHandling = DateParseHandling.None;

            var jsonConfig = JObject.Load(_reader);

            VisitJObject(jsonConfig);

            return _data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;

                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token.Value<JValue>());
                    break;

                default:
                    throw new FormatException($"Unsupported token type {_reader.TokenType.ToString()} at {_reader.Path}:{_reader.LineNumber}:{_reader.LinePosition}");

                    
            }
        }

        private void VisitArray(JArray array)
        {
            for (int index = 0; index < array.Count; index++)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JValue data)
        {
            var key = _currentPath;

            if (_data.ContainsKey(key))
            {
                throw new FormatException("Duplicate Key");
            }

            var value = data.ToString(CultureInfo.InvariantCulture);

            value = _expander.Expand(value);

            _data[key] = value;
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
