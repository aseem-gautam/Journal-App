using JournalApp.Models;
using System.Text;
using Markdig;

namespace JournalApp.Services
{
    public class ExportService
    {
        private readonly JournalService _journalService;

        public ExportService(JournalService journalService)
        {
            _journalService = journalService;
        }

        // Export entries as PDF (we'll use QuestPDF later, for now just prepare the data)
        public async Task<string> ExportToPdfAsync(DateTime startDate, DateTime endDate, string filePath)
        {
            try
            {
                var entries = await _journalService.GetEntriesByDateRangeAsync(startDate, endDate);

                if (!entries.Any())
                {
                    return string.Empty;
                }

                // For now, we'll export as HTML which can be converted to PDF
                // We'll implement QuestPDF in the next step
                var html = GenerateHtmlForExport(entries, startDate, endDate);

                var htmlPath = Path.Combine(filePath, $"journal_export_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                await File.WriteAllTextAsync(htmlPath, html);

                return htmlPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to PDF: {ex.Message}");
                return string.Empty;
            }
        }

        // Export entries as Markdown
        public async Task<string> ExportToMarkdownAsync(DateTime startDate, DateTime endDate, string filePath)
        {
            try
            {
                var entries = await _journalService.GetEntriesByDateRangeAsync(startDate, endDate);

                if (!entries.Any())
                {
                    return string.Empty;
                }

                var markdown = GenerateMarkdownForExport(entries, startDate, endDate);

                var mdPath = Path.Combine(filePath, $"journal_export_{DateTime.Now:yyyyMMdd_HHmmss}.md");
                await File.WriteAllTextAsync(mdPath, markdown);

                return mdPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to Markdown: {ex.Message}");
                return string.Empty;
            }
        }

        // Export entries as plain text
        public async Task<string> ExportToTextAsync(DateTime startDate, DateTime endDate, string filePath)
        {
            try
            {
                var entries = await _journalService.GetEntriesByDateRangeAsync(startDate, endDate);

                if (!entries.Any())
                {
                    return string.Empty;
                }

                var text = GenerateTextForExport(entries, startDate, endDate);

                var txtPath = Path.Combine(filePath, $"journal_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                await File.WriteAllTextAsync(txtPath, text);

                return txtPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to text: {ex.Message}");
                return string.Empty;
            }
        }

        private string GenerateHtmlForExport(List<JournalEntry> entries, DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head>");
            sb.AppendLine("<meta charset='UTF-8'>");
            sb.AppendLine("<title>Journal Export</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 800px; margin: 40px auto; padding: 20px; }");
            sb.AppendLine("h1 { color: #333; border-bottom: 3px solid #4CAF50; padding-bottom: 10px; }");
            sb.AppendLine(".entry { margin-bottom: 40px; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }");
            sb.AppendLine(".entry-date { color: #666; font-size: 14px; }");
            sb.AppendLine(".entry-title { color: #333; font-size: 24px; margin: 10px 0; }");
            sb.AppendLine(".entry-mood { display: inline-block; padding: 5px 10px; background: #f0f0f0; border-radius: 5px; margin: 5px 5px 5px 0; }");
            sb.AppendLine(".entry-tag { display: inline-block; padding: 3px 8px; background: #e3f2fd; border-radius: 3px; margin: 3px; font-size: 12px; }");
            sb.AppendLine(".entry-content { margin-top: 15px; line-height: 1.6; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head><body>");

            sb.AppendLine($"<h1>Journal Export</h1>");
            sb.AppendLine($"<p><strong>Date Range:</strong> {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}</p>");
            sb.AppendLine($"<p><strong>Total Entries:</strong> {entries.Count}</p>");
            sb.AppendLine("<hr>");

            foreach (var entry in entries)
            {
                sb.AppendLine("<div class='entry'>");
                sb.AppendLine($"<div class='entry-date'>{entry.Date:dddd, MMMM dd, yyyy}</div>");
                sb.AppendLine($"<h2 class='entry-title'>{entry.Title}</h2>");

                // Moods
                sb.AppendLine("<div>");
                sb.AppendLine($"<span class='entry-mood'>{entry.PrimaryMood.Icon} {entry.PrimaryMood.Name}</span>");
                foreach (var secondaryMood in entry.SecondaryMoods)
                {
                    sb.AppendLine($"<span class='entry-mood'>{secondaryMood.Mood.Icon} {secondaryMood.Mood.Name}</span>");
                }
                sb.AppendLine("</div>");

                // Tags
                if (entry.Tags.Any())
                {
                    sb.AppendLine("<div style='margin-top: 10px;'>");
                    foreach (var tag in entry.Tags)
                    {
                        sb.AppendLine($"<span class='entry-tag'>#{tag.Tag.Name}</span>");
                    }
                    sb.AppendLine("</div>");
                }

                // Content (convert markdown to HTML)
                var htmlContent = Markdown.ToHtml(entry.Content);
                sb.AppendLine($"<div class='entry-content'>{htmlContent}</div>");

                sb.AppendLine("</div>");
            }

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private string GenerateMarkdownForExport(List<JournalEntry> entries, DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Journal Export");
            sb.AppendLine();
            sb.AppendLine($"**Date Range:** {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}");
            sb.AppendLine($"**Total Entries:** {entries.Count}");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            foreach (var entry in entries)
            {
                sb.AppendLine($"## {entry.Date:dddd, MMMM dd, yyyy}");
                sb.AppendLine();
                sb.AppendLine($"### {entry.Title}");
                sb.AppendLine();

                // Moods
                sb.Append($"**Mood:** {entry.PrimaryMood.Icon} {entry.PrimaryMood.Name}");
                if (entry.SecondaryMoods.Any())
                {
                    foreach (var mood in entry.SecondaryMoods)
                    {
                        sb.Append($", {mood.Mood.Icon} {mood.Mood.Name}");
                    }
                }
                sb.AppendLine();
                sb.AppendLine();

                // Tags
                if (entry.Tags.Any())
                {
                    sb.Append("**Tags:** ");
                    sb.AppendLine(string.Join(", ", entry.Tags.Select(t => $"#{t.Tag.Name}")));
                    sb.AppendLine();
                }

                // Content
                sb.AppendLine(entry.Content);
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GenerateTextForExport(List<JournalEntry> entries, DateTime startDate, DateTime endDate)
        {
            var sb = new StringBuilder();
            sb.AppendLine("JOURNAL EXPORT");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();
            sb.AppendLine($"Date Range: {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}");
            sb.AppendLine($"Total Entries: {entries.Count}");
            sb.AppendLine();
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            foreach (var entry in entries)
            {
                sb.AppendLine($"Date: {entry.Date:dddd, MMMM dd, yyyy}");
                sb.AppendLine($"Title: {entry.Title}");
                sb.AppendLine();

                sb.Append($"Mood: {entry.PrimaryMood.Name}");
                if (entry.SecondaryMoods.Any())
                {
                    foreach (var mood in entry.SecondaryMoods)
                    {
                        sb.Append($", {mood.Mood.Name}");
                    }
                }
                sb.AppendLine();

                if (entry.Tags.Any())
                {
                    sb.AppendLine($"Tags: {string.Join(", ", entry.Tags.Select(t => t.Tag.Name))}");
                }

                sb.AppendLine();
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine(entry.Content);
                sb.AppendLine("-".PadRight(80, '-'));
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}