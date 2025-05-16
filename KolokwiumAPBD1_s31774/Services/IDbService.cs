using KolokwiumAPBD1_s31774.Models.DTOs;

namespace KolokwiumAPBD1_s31774.Services;

public interface IDbService
{
    Task<RegisterAppointmentDTO> GetAppointmentById(int AppointmentId);
    Task AddNewAppointment(int PatientId, RegisterAppointmentDTO registerAppointmentDto);
}