using Exnaton.Models.Requests;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Interfaces;

public interface IDataService
{
    /// <summary>
    /// Fetches measurement data from an external source based on the provided request
    /// and stores it in the database.
    /// </summary>
    /// <param name="request">The request containing filtering criteria such as MUID, measurement type, time range, and limits.</param>
    /// <param name="enReturnResponse">Option to return the response from the external service got and filtered based on the request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidMeasurementException">Thrown when the request is invalid.</exception>
    /// <exception cref="ExternalServiceNotAvailableException">Thrown when the response is invalid or when face HttpRequestException or JsonException or other unhandled exception.</exception>
    Task<List<MeasurementDataDTO>> FetchAndStoreDataAsync(ReadMeasurementsRequest request, bool enReturnResponse = false);

    /// <summary>
    /// Reads measurement data from the database based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing filtering criteria such as measurement type, limit, start date, and stop date.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a list of <see cref="MeasurementDataDTO"/> objects 
    /// or null if no data is found.
    /// </returns>
    /// <exception cref="BusinessException">Thrown when the request contains invalid parameters.</exception>
    Task<List<MeasurementDataDTO>?> ReadDataAsync(ReadMeasurementsRequest request);
}