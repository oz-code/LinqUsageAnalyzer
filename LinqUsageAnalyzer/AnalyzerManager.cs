using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqUsageAnalyzer.Analyzers;
using LinqUsageAnalyzer.DAL;
using LinqUsageAnalyzer.Extensions;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using NLog;
using Octokit;

namespace LinqUsageAnalyzer
{
    public class AnalyzerManager
    {
        private static readonly HashSet<string> IgnoredRepositories = new HashSet<string>
        {
            "dapper-dot-net",
            "corefx",
            "monodroid-samples",
            "shadowsocks-windows",
            "mono",
            "mobile-samples",
            "xamarin-forms-samples",
            "coreclr",
            "roslyn",
            "SignalR",
            "PowerShell",
            "CodeHub",
            "EntityFramework",
            "SpaceEngineers",
            "ServiceStack",
            "RestSharp",
            "Mvc"
        };

        private readonly ICodeRepository _codeRepository;
        private readonly FileEngine _fileEngine;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly SemanticModelFactory _semanticModelFactory;
        private readonly SemanticModelAnalyzer _semanticModelAnalyzer;
        private readonly ILogger _log;

        internal string ProjectTempDownloadDirectory => Path.Combine(Path.GetTempPath(), "OzCodeLinqAnalyzer");

        public AnalyzerManager(ICodeRepository codeRepository, FileEngine fileEngine, IStatisticsRepository statisticsRepository)
        {
            _log = LogManager.GetCurrentClassLogger();

            _semanticModelFactory = new SemanticModelFactory();
            _semanticModelAnalyzer = new SemanticModelAnalyzer();

            _codeRepository = codeRepository;
            _fileEngine = fileEngine;
            _statisticsRepository = statisticsRepository;
        }

        public async Task StartAsync(int maxProjects)
        {
            var downloadedRepositories = 0;
            var currentPage = 1;
            while (downloadedRepositories < maxProjects)
            {
                var result = await _codeRepository.FindCSharpRepositoriesAsync(currentPage++);

                foreach (var repository in result.Items)
                {
                    if (IgnoredRepositories.Contains(repository.Name))
                    {
                        continue;
                    }

                    try
                    {
                        _log.Log(LogLevel.Info, "Starting analysis for {0}", repository.Name);

                        var statistics = await AnalizeProjectAsync(repository);

                        _log.Debug("Found {0} LINQ Operators", statistics.Counters.LinqOperatorUsed.Sum(pair => pair.Value));
                        // TODO: save to DB
                        _statisticsRepository.Save(statistics);
                    }
                    catch (Exception exc)
                    {
                        _log.Log(LogLevel.Error, exc, "Failed analyzing repository {0}", repository.Name);
                    }
                    finally
                    {
                        downloadedRepositories++;
                    }
                }
            }
        }

        private async Task<RepositoryStatistics> AnalizeProjectAsync(Repository repository)
        {
            var fixedRepository = await _codeRepository.GetRepositoryAsync(repository);
            _log.Log(LogLevel.Trace, "-- Downloading repository code --");
            var downloaded =
                await _codeRepository.DownloadSourceRepositoryCodeAsync(fixedRepository, ProjectTempDownloadDirectory);

            _log.Log(LogLevel.Trace, "-- Extracting archive --");

            var projectFolder = _fileEngine.Extract(downloaded);

            var statistics = new RepositoryStatistics(repository);

            foreach (var semanticModel in _semanticModelFactory.CreateSemanticModels(projectFolder))
            {
                _semanticModelAnalyzer.Analyze(semanticModel, statistics);
            }

            return statistics;
        }
    }

    public class SemanticModelAnalyzer
    {
        private readonly ILogger _log;

        public SemanticModelAnalyzer()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public void Analyze(SemanticModel semanticModel, RepositoryStatistics statistics)
        {
            statistics.LinesOfCode = semanticModel.GetNumberOfLines();

            AnalizeLinq(new QueryLinqAnalyzer(semanticModel), statistics);
            AnalizeLinq(new FluentLinqAnalyzer(semanticModel), statistics);
        }

        private void AnalizeLinq(IFileAnalyzer analyzer, RepositoryStatistics result)
        {
            if (analyzer.HasRelevantLinqQueries())
            {
                var counters = analyzer.Analyze();

                _log.Log(LogLevel.Trace, "-- Found {0} LINQ Operators --", counters.LinqOperatorUsed.Count);

                result.Counters.Append(counters);
            }
        }
    }
}