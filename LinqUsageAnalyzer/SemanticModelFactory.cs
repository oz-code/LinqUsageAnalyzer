﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;

namespace LinqUsageAnalyzer
{
    public class SemanticModelFactory
    {
        private readonly ILogger _log;
        private readonly HashSet<string> _ignoredFiles = new HashSet<string>
        {
            "AssemblyInfo.cs", ".NETFramework,Version=v4.6.1.AssemblyAttributes.cs"
        };

        public SemanticModelFactory()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<SemanticModel> CreateSemanticModels(string projectFolder)
        {
            return Observable.Create<SemanticModel>(
                async obs =>await GetSematicModelAsync(projectFolder, obs))
                .Distinct(model => model.SyntaxTree.FilePath)
                .ToEnumerable();
        }

        private async Task GetSematicModelAsync(string projectFolder, IObserver<SemanticModel> obs)
        {
            var solutionFiles = FindSolutionFilesInFolder(projectFolder);

            foreach (var solutionFile in solutionFiles)
            {
                try
                {
                    _log.Log(LogLevel.Debug, "Analyzing solution {0}", solutionFile);
                    var solution = await MSBuildWorkspace.Create().OpenSolutionAsync(solutionFile);

                    foreach (var project in solution.Projects)
                    {
                        _log.Log(LogLevel.Debug, "Analyzing Project {0}", project.Name);

                        foreach (var document in project.Documents)
                        {
                            if (_ignoredFiles.Contains(document.Name))
                            {
                                continue;
                            }

                            if (!document.SupportsSyntaxTree || !document.SupportsSemanticModel)
                            {
                                continue;
                            }

                            try
                            {
                                var semanticModel = await document.GetSemanticModelAsync();
                                obs.OnNext(semanticModel);
                            }
                            catch (Exception exc)
                            {
                                _log.Log(LogLevel.Error, exc, "Failed creating semantic model for project {0}", project.Name);
                            }

                        }
                    }
                }
                catch (Exception exc)
                {
                   _log.Log(LogLevel.Error, exc, "Failed creating semantic model for solution {0}", solutionFile);
                }
            }

            obs.OnCompleted();
        }

        private IEnumerable<string> FindSolutionFilesInFolder(string projectFolder)
        {
            return Directory.GetFiles(projectFolder, "*.sln", SearchOption.AllDirectories);
        }
    }
}
