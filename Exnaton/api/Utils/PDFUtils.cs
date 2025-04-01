using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.IO;
using Exnaton.Utils;
using Models.DTOs.MeasurementDataDTO;
using OxyPlot.WindowsForms;

public static class PDFUtils
{
    public static void ExportPDF(List<MeasurementDataDTO> measurements, Serilog.ILogger logger)
    {
        // Generate graph
        var plotModel = GraphUtils.CreateTimeSeriesGraph(measurements, logger);

        // Create the file path to save the graph
        var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "chart.png");

        // Export the graph as PNG using OxyPlot.Core's PngExporter
        PngExporter.Export(plotModel, imageFilePath, 600, 400); // Width and Height of the image

        // Create the PDF document
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Content().Column(column =>
                {
                    column.Item().Text("Task A - Data Exploration").Bold().FontSize(16);
                    column.Item().Text("Explore the data and group it by different time intervals.");
                    
                    // Add graph image to PDF with width specification
                    column.Item().Image(imageFilePath).FitWidth();

                    column.Item().Text("Explain what you see/what the data represents.");
                    column.Item().Text("Come up with a hypothesis on what kind of data you are looking at.");
                    column.Item().Text("Bonus: Check for any autocorrelation within the time-series data.");
                    
                    // Adding additional info like timestamp, author, title, etc.
                    column.Item().Text("Created on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    column.Item().Text("Author: [Your Name]");
                    column.Item().Text("Title: Data Exploration Report");
                });
            });
        }).GeneratePdf("output.pdf");

        // Clean up image after PDF creation
        File.Delete(imageFilePath);
    }
}
