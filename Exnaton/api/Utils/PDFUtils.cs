using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Exnaton.Exceptions;
using Exnaton.Models;
using Exnaton.Utils;
using Models.DTOs.MeasurementDataDTO;
using OxyPlot;
using OxyPlot.SkiaSharp;
using QuestPDF.Helpers;
using Serilog;
using QuestPDF.Infrastructure;

public static class PDFUtils
{
    public static void ExportPDF(List<MeasurementDataDTO> measurements, Serilog.ILogger logger)
    {
        try
        {
            logger.Error("@TEST: 1.");
            QuestPDF.Settings.License = LicenseType.Community;
            
            if (measurements == null || measurements.Count == 0 || measurements.Any(m => m == null))
                throw BusinessExceptions.InvalidMeasurementException(logger, "Measurements are required and cannot be null.");
            
            // Generate graph     
            logger.Error("@TEST: 2.");
            var plotModel = GraphUtils.CreateTimeSeriesGraph(measurements, logger);
            if (plotModel == null)
                throw BusinessExceptions.ServiceNotAvailableException(logger);

            logger.Error("@TEST: 3.");
            var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "chart.png");

            // Export the graph as PNG using SkiaSharp (cross-platform)
            var exporter = new PngExporter { Width = 600, Height = 400 };
            logger.Error("@TEST: 4.");
            using (var stream = File.Create(imageFilePath))
            {
                logger.Error("@TEST: 5.");
                exporter.Export(plotModel, stream);
                logger.Error("@TEST: 6.");
            }

            // Create the PDF document
            logger.Error("@TEST: 7.");
            Document.Create(container =>
            {
                logger.Error("@TEST: 8.");
                container.Page(page =>
                {
                    logger.Error("@TEST: 9.");
                    page.Margin(50);
                    var logoFullname = Path.Combine(Directory.GetCurrentDirectory(), "logo.png");
                    page.Background().Image(logoFullname).FitWidth();
                    logger.Error("@TEST: 10.");
                    page.Content().Column(column =>
                    {
                        logger.Error("@TEST: 11.");
                        column.Item().Text("Welcome to the Exnaton Monitoring Results Record.").Bold().FontSize(16);
                        column.Item().Text("Below is the graph showing the tracking samples recorded by the Exnaton machine/measurement units, along with some analysis details.");

                        // Add graph image to PDF with width specification
                        column.Item().Image(imageFilePath).FitWidth();
                        
                        column.Item().PaddingTop(15).Text("Analysis of the Data and Observations:");
                        column.Item().PaddingTop(2).Text("[Hypothesis about the nature of the data]:");
                        column.Item().PaddingTop(5).Text("1. The tracking period (T) is observed to be 15 minutes.");
                        column.Item().Text("2. We hypothesize that this data originates from a machine/module/measurement unit identified by a unique ID (muid). Additionally getting quality measurement details indicating that the mu measure the results. Additionally, the data appears to be related to energy measurements. Some encoded characters(\"0100021D00FF\") may indicate device information, sensor details, or specific measurement identifiers.");
                        column.Item().Text("3. From the graph, we observe that at the start of energy consumption or supply, the tracking object's measurement remained stable. However, after a few minutes, the values increased gradually before experiencing a sharp rise to peak levels.");
                        column.Item().Text("4. Really high-frequency measurements (around 2688 samples in February 2023), so the system needs to have zero time intervals for writing to the database.");
                        column.Item().Text("5. When grouping intervals, we observe a pattern. First, the metrics start from 0, then gradually increase. After some minutes, they increase dramatically until they reach a peak, then fluctuate, reaching higher peaks (sometimes). After a few minutes, the metrics return to zero again.");
                        column.Item().Text("6. The time it takes for these fluctuations to return to zero ranges between 400 and 546 minutes (this is not standardized).");
                        column.Item().Text("7. Based on the February samples we have, we assume that these represent the energy consumption of a machine or a location (because we don't know if the values represent currency or if they are sampled with a factor). The consumption occurs in periods, not constantly, but often and over specific intervals.");
                        column.Item().Text("8. The measurements also vary significantly between intervals, so it may be something that influences consumption during the month. It's not certain whether it's always a machine that is plugged in or something doing a specific job on its own (maybe it is also standalone solution like a solar panel and some times is cloudy or machine/module interpret by human actions).");
                        column.Item().Text("9. The different metrics show greater differences at points near zero. The largest difference is observed with the ID 1db7649e-9342-4e04-97c7-f0ebb88ed1f8, which holds zeros, while the ID 95ce3367-cbce-4a4d-bbe3-da082831d7bd fluctuates between zero and other values slightly above zero.");
                        
                        column.Item().PaddingTop(15).Text("[Bonus: Checking for Autocorrelation in the Time-Series Data]:");
                        column.Item().PaddingTop(5).Text("1. Regarding the fluctuations at the bottoms and tops: We don't know if the issue is with the sensor itself or the measurements. There is always fault tolerance, and we can adjust some filters for better fault prediction. However, we may need these fluctuations at both the bottom and top levels.");
                        
                        column.Item().PaddingTop(15).Text($"Created on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        column.Item().Text("Author: Kostas Makatsoris via Exnaton.").FontColor(QuestPDF.Infrastructure.Color.FromRGB(255, 0, 0));
                        column.Item().Text("Enjoy your Data Exploration Report! :)").FontColor(QuestPDF.Infrastructure.Color.FromRGB(255, 0, 0));
                        logger.Error("@TEST: 12.");
                    });
                    logger.Error("@TEST: 13.");
                });
                logger.Error("@TEST: 14.");
            }).GeneratePdf($"exploitation_results_{measurements?.FirstOrDefault()?.Tags?.Muid}_{measurements?.Count}.pdf");
            logger.Error("@TEST: 15.");
            // Clean up image after PDF creation
            File.Delete(imageFilePath);
            logger.Error("@TEST: 16.");
        }
        catch (Exception ex)
        {
            logger.Error($"@TEST: 0. ex?.Message={JsonSerializer.Serialize(ex?.Message)} and \n\n ex?.InnerException?.Message={JsonSerializer.Serialize(ex?.InnerException?.Message)}");
            throw BusinessExceptions.ServiceNotAvailableException(logger, logMsg: ex?.Message);
        }
    }
}
