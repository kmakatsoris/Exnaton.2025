using Exnaton.Models.Entities;
using Exnaton.Models.Requests;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Interfaces.Repositories;

public interface IMeasurementsRepository
{
    List<MeasurementDataDTO>? Read(ReadMeasurementsRequest request);
    Task<List<MeasurementDataEntity>> AddAsync(List<MeasurementDataDTO> measurementData);
}