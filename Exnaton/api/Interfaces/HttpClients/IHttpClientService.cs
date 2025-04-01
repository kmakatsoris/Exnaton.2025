using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Interfaces.HttpClients;

public interface IHttpClientService
{
    Task<MeasurementDataWrapperDTO?> FetchDataAsync(Guid muid);
}