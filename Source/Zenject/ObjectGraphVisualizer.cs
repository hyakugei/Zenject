using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree.Zenject
{
    // Responsibilities:
    // - Output a file specifying the full object graph for a given root dependency
    // - This file uses the DOT language with can be fed into GraphViz to generate an image
    // - http://www.graphviz.org/
    public static class ObjectGraphVisualizer
    {
        public static void OutputObjectGraphToFile(DiContainer container, string outputPath)
        {
            // Output the entire object graph to file
            var graph = container.CalculateObjectGraph<IDependencyRoot>();

            var resultStr = "digraph { \n";

            foreach (var entry in graph)
            {
                // ignore these to clean up the graph
                if (entry.Key == typeof(EntryPointInitializer) || entry.Key == typeof(KernelInitializer))
                {
                    continue;
                }

                foreach (var dependencyType in entry.Value)
                {
                    // ignore factory dependency to clean up graph
                    if (dependencyType == typeof(DiContainer))
                    {
                        continue;
                    }

                    resultStr += GetFormattedTypeName(entry.Key) + " -> " + GetFormattedTypeName(dependencyType) + "; \n";
                }
            }

            resultStr += " }";

            System.IO.File.WriteAllText(outputPath, resultStr);
        }

        static string GetTypeNameWithGenericArguments(Type type)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return type.Name;
            }

            var genericArguments = type.GetGenericArguments();
            var typeDefinition = type.Name;
            var unmangledName = typeDefinition.Substring(0, typeDefinition.IndexOf("`"));
            return unmangledName + "<" + String.Join(",", genericArguments.Select(GetTypeNameWithGenericArguments).ToArray()) + ">";
        }

        static string GetFormattedTypeName(Type type)
        {
            var str = GetTypeNameWithGenericArguments(type);

            // GraphViz does not read names with < and > characters so replace them:
            str = str.Replace("<", "_");
            str = str.Replace(">", "");

            return str;
        }
    }
}

