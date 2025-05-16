namespace KolokwiumAPBD1_s31774.Models.DTOs;

public class RegisterAppointmentDTO
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string PWZ { get; set; }
    public List<AppointmentServiceDTO> Services { get; set; }
}