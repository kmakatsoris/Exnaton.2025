namespace Exnaton.Exceptions;

public static class BusinessExceptions
{
    private static readonly string ExternalServiceNotAvailable = "We are sorry some external services are not currently available, we will contact with them resolving the issues. Please try again later.";
    private static readonly string ServiceNotAvailable = "We are sorry the service is not currently available. Please try again later.";
    private static readonly string NotMeasurementFound = "Not entity found.";
    private static readonly string InvalidMeasurement = "Invalid measurement data provided.";
    // Append..

    public static Exception ExternalServiceNotAvailableException(Serilog.ILogger logger, string logMsg = "") => ErrorThrow(logger, msg: ExternalServiceNotAvailable, logMsg: logMsg);
    public static Exception ServiceNotAvailableException(Serilog.ILogger logger, string logMsg = "") => ErrorThrow(logger, msg: ServiceNotAvailable, logMsg: logMsg);
    public static Exception NotEntityFoundException(Serilog.ILogger logger, string logMsg = "") => ErrorThrow(logger, msg: NotMeasurementFound, logMsg: logMsg);
    public static Exception InvalidMeasurementException(Serilog.ILogger logger, string reason, string logMsg = "") => ErrorThrow(logger, msg: InvalidMeasurement, extMsg: reason, logMsg: logMsg);
    // Append..

    #region Private Class Methods

    private static Exception ErrorThrow(Serilog.ILogger logger, 
        string msg = "We are sorry, the service is not currently available. Please try again later.",
        string extMsg = "",
        string logMsg = "",
        bool enLog = true)
    {
        if (logger != null && enLog)
            logger.Error(msg+extMsg+"\n\n"+logMsg);
        
        return new Exception(msg+extMsg);
    }
    
    private static Exception WarningThrow(Serilog.ILogger logger, 
        string msg = "We are sorry, the service is not currently available. Please try again later.",
        string extMsg = "",
        string logMsg = "",
        bool enLog = true)
    {
        if (logger != null && enLog)
            logger.Warning(msg+extMsg+"\n\n"+logMsg);
        
        return new Exception(msg+extMsg);
    }

    #endregion
}