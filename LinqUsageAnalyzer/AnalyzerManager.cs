using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqUsageAnalyzer.DAL;
using LinqUsageAnalyzer.Interfaces;
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
            "mono",
            "mobile-samples",
            "xamarin-forms-samples",
            "coreclr",
            "roslyn",
            "PowerShell",
            "CodeHub",
            "SpaceEngineers",
            "RestSharp",
            "Mvc",
            "SparkleShare",
            "msbuild",
            "MonoGame"
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

                    if (_statisticsRepository.RepositoryExist(repository.Name))
                    {
                        continue;
                    }

                    try
                    {
                        _log.Log(LogLevel.Info, "Starting analysis for {0}", repository.Name);
                        var statistics = await AnalizeProjectAsync(repository);

                        _log.Debug("Found {0} LINQ Operators", statistics.Counters.LinqOperatorUsed.Sum(pair => pair.Value));
                        if (statistics.AnalyzedModels > 0)
                        {
                            await _statisticsRepository.SaveAsync(statistics);
                        }
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
            _log.Log(LogLevel.Trace, "-- Downloading repository code --");
            var downloaded =
                await _codeRepository.DownloadSourceRepositoryCodeAsync(repository, ProjectTempDownloadDirectory);

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
}