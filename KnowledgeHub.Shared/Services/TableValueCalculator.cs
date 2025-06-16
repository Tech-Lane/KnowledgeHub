using KnowledgeHub.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Services
{
    public static class TableValueCalculator
    {
        public static Task<string> Calculate(Table table, string columnName, string aggregation)
        {
            try
            {
                var columns = JsonSerializer.Deserialize<List<ColumnDefinition>>(table.ColumnsAsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ColumnDefinition>();
                var column = columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                if (column == null)
                {
                    return Task.FromResult($"[Column '{columnName}' not found]");
                }

                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(table.DataAsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Dictionary<string, object>>();
                if (!data.Any())
                {
                    return Task.FromResult("0");
                }

                var values = new List<double>();
                if (column.DataType == ColumnDataType.Number)
                {
                    foreach (var row in data)
                    {
                        if (row.TryGetValue(columnName, out var cellValue) && double.TryParse(cellValue.ToString(), out var numericValue))
                        {
                            values.Add(numericValue);
                        }
                    }
                }

                return Task.FromResult(aggregation switch
                {
                    "sum" => values.Sum().ToString("N2"),
                    "count" => values.Count.ToString(),
                    "avg" => values.Average().ToString("N2"),
                    "min" => values.Min().ToString("N2"),
                    "max" => values.Max().ToString("N2"),
                    _ => $"[Unknown aggregation: {aggregation}]",
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating table value: {ex.Message}");
                return Task.FromResult("[Calculation Error]");
            }
        }
    }
} 