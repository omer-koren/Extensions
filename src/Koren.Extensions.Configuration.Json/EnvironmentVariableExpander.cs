// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Koren.Extensions.Configuration.Json
{
    internal class EnvironmentVariableExpander
    {
        private const string SQUARE_BRACES_REGEX = @"(?<prefix>[^\[]|^)\[(?<var>[^\[\]]*?)\](?<suffix>[^\]]|$)";
        private const string CURLY_BRACES_REGEX =  @"(?<prefix>[^\{]|^)\{(?<var>[^\{\}]*?)\}(?<suffix>[^\}]|$)";

        public string Expand(string s)
        {
            s = Regex.Replace(s,SQUARE_BRACES_REGEX , (match) =>
            {

                var prefix = match.Groups["prefix"].Value;
                var suffix = match.Groups["suffix"].Value;

                var replacementGroup = match.Groups["var"].Value;

                var er = ExpandEnvironmentVariables(replacementGroup);

                if (er.Success)
                    return prefix + er.Expanded + suffix;

                return prefix + suffix;
            });

            s = s.Replace("[[", "[")
                 .Replace("]]", "]");

            var expandResult = ExpandEnvironmentVariables(s);

            if (!expandResult.Success)
                throw new Exception($"Undefined environment variables '{string.Join(",", expandResult.FailedVariable)}'");

            return expandResult.Expanded;
        }

        class ExpandResult
        {
            public bool Success { get; set; }
            public string Expanded { get; set; }
            public string[] FailedVariable { get; set; }
        }

        private ExpandResult ExpandEnvironmentVariables(string name)
        {
            var failed = new List<string>();

            var expanded = Regex.Replace(name, CURLY_BRACES_REGEX
                        , match =>
                        {
                            var prefix = match.Groups["prefix"].Value;
                            var variable = match.Groups["var"].Value;
                            var suffix = match.Groups["suffix"].Value;

                            var env = Environment.GetEnvironmentVariable(variable);

                            if (string.IsNullOrWhiteSpace(env))
                            {
                                failed.Add(variable);
                                return prefix + suffix;
                            }

                            return prefix + env + suffix;
                        });

            expanded = expanded.Replace("{{", "{")
                               .Replace("}}", "}");

            var result = new ExpandResult
            {
                Expanded = expanded,
                Success = failed.Count == 0,
                FailedVariable = failed.ToArray()
            };

            return result;
        }
    }
}
