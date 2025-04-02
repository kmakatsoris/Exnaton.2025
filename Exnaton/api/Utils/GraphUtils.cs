using Exnaton.Exceptions;
using Models.DTOs.MeasurementDataDTO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Exnaton.Utils;

public static class GraphUtils
{
    public static PlotModel CreateTimeSeriesGraph(List<MeasurementDataDTO> measurements, Serilog.ILogger logger)
    {
        try
        {
            if (measurements == null || measurements.Count == 0 || measurements.Any(m => m == null))
                return null;
            
            var plotModel = new PlotModel { Title = "Energy Measurement Over Time" };

            // Create a LineSeries to hold the measurement data
            var series = new LineSeries
            {
                Title = "Energy",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle
            };

            // Add data points to the series
            DateTime referenceTime = measurements.First().Timestamp;
            foreach (var measurement in measurements)
            {
                if (measurement?.IndicationsRTU?.FirstOrDefault() == null || measurement?.IndicationsRTU?.FirstOrDefault().Value == null)
                    continue;
                double normaliseTime = (referenceTime - measurement.Timestamp).TotalMinutes;
                series.Points.Add(new DataPoint(normaliseTime, measurement.IndicationsRTU.FirstOrDefault().Value));
            }

            // Add the series to the plot model
            plotModel.Series.Add(series);

            return plotModel;
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(logger, logMsg: ex?.Message);
        }
    }
}