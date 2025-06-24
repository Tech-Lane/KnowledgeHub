using KnowledgeHubV2.Data;
using KnowledgeHubV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace KnowledgeHubV2.Services
{
    public class ReferenceProcessingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ReferenceProcessingService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<string> ProcessContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return "";
            }

            var regex = new Regex(@"\{\{(.+?)\}\}");
            var matches = regex.Matches(content);

            var processedContent = content;

            foreach (Match match in matches)
            {
                var replacement = await GetReplacementValue(match.Groups[1].Value.Trim());
                processedContent = processedContent.Replace(match.Value, replacement);
            }

            return processedContent;
        }

        private async Task<string> GetReplacementValue(string reference)
        {
            var parts = reference.Split('.');
            if (parts.Length != 3)
            {
                return $"(Invalid Reference: {reference})";
            }

            var tableName = parts[0];
            var columnName = parts[1];
            var aggregateType = parts[2];

            using var context = await _dbContextFactory.CreateDbContextAsync();
            var table = await context.NoteTables
                .Include(t => t.Columns)
                .FirstOrDefaultAsync(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (table == null)
            {
                return $"(Table '{tableName}' not found)";
            }

            var column = table.Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (column == null)
            {
                return $"(Column '{columnName}' not found in table '{tableName}')";
            }

            if (column.DataType != ColumnDataType.Number && aggregateType.ToLower() != "count")
            {
                return $"(Cannot perform aggregate '{aggregateType}' on non-numeric column)";
            }
            
            var rows = table.Rows;

            if (!rows.Any())
            {
                return "0";
            }

            try
            {
                switch (aggregateType.ToLower())
                {
                    case "sum":
                        var sum = rows.Sum(r => {
                            if (r.TryGetValue(column.Name, out var val) && val != null)
                            {
                                if (val is JsonElement je && je.ValueKind == JsonValueKind.Number && je.TryGetDecimal(out var dec)) return dec;
                                if (decimal.TryParse(val.ToString(), out var result)) return result;
                            }
                            return 0;
                        });
                        return sum.ToString("0.##");

                    case "count":
                        return rows.Count.ToString();

                    case "avg":
                         var avg = rows.Average(r => {
                            if (r.TryGetValue(column.Name, out var val) && val != null)
                            {
                                if (val is JsonElement je && je.ValueKind == JsonValueKind.Number && je.TryGetDecimal(out var dec)) return dec;
                                if (decimal.TryParse(val.ToString(), out var result)) return result;
                            }
                            return 0;
                        });
                        return avg.ToString("0.##");

                    default:
                        return $"(Unknown aggregate '{aggregateType}')";
                }
            }
            catch (Exception ex)
            {
                return $"(Error processing aggregate: {ex.Message})";
            }
        }
    }
} 