using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LinqUsageAnalyzer.Analyzers.QueryLinqAnalyzers;
using LinqUsageAnalyzer.Extensions;
using LinqUsageAnalyzer.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LinqUsageAnalyzer.Analyzers
{
    class QueryLinqAnalyzer : IFileAnalyzer
    {
        private readonly SemanticModel _semanticModel;
        private readonly SyntaxNode _root;
        private static readonly Dictionary<SyntaxKind, IQueryClauseAnalyzer> _clauseActionsDictionary = new Dictionary<SyntaxKind, IQueryClauseAnalyzer>
        {
            { SyntaxKind.FromClause, new FromClause()},
            { SyntaxKind.GroupClause, new GroupClause()},
            { SyntaxKind.JoinClause, new JoinClause()},
            { SyntaxKind.LetClause, new LetClause()},
            { SyntaxKind.OrderByClause, new OrderByClause()},
            { SyntaxKind.SelectClause, new SelectClause()},
            { SyntaxKind.WhereClause, new WhereClause()}
        };

        public QueryLinqAnalyzer(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _root = semanticModel.SyntaxTree.GetRoot();
        }

        public bool HasRelevantLinqQueries()
        {
            return GetSqlLinqNodes().Any();
        }

        public RepositoryCounters Analyze()
        {
            var result = new RepositoryCounters();

            foreach (var sqlExpressionSyntax in GetSqlLinqNodes())
            {
                var body = sqlExpressionSyntax.Body;
                QueryContinuationSyntax continuation;
                var clauses = body.Clauses;
                AnalyzeClause(sqlExpressionSyntax.FromClause, result);

                do
                {
                    foreach (var clause in clauses)
                    {
                        AnalyzeClause(clause, result);
                    }

                    AnalyzeClause(body.SelectOrGroup, result);
                    continuation = body.Continuation;

                    if (continuation == null)
                        continue;

                    AddToResult(continuation.IntoKeyword, result);
                    body = continuation.Body;
                    clauses = body.Clauses;

                } while (continuation != null);
            }

            return result;
        }

        private IEnumerable<QueryExpressionSyntax> GetSqlLinqNodes()
        {
            return _root.DescendantNodes()
                .OfType<QueryExpressionSyntax>();
        }

        private void AnalyzeClause(SyntaxNode clause, RepositoryCounters result)
        {
            if (!_clauseActionsDictionary.ContainsKey(clause.Kind()))
                return;

            _clauseActionsDictionary[clause.Kind()].AddToStatistics(clause, result, AddToResult);
        }

        private void AddToResult(SyntaxToken syntaxToken, RepositoryCounters result)
        {
            if (string.IsNullOrEmpty(syntaxToken.ValueText))
                return;

            result.Add(LinqKind.Query, syntaxToken.ValueText);
        }
    }
}
