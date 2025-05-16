using System.ComponentModel.DataAnnotations;

namespace KolokwiumAPBD1_s31774.Models.DTOs;

public class AppointmentDTO
{
    public DateTime Date { get; set; }
    public PatientDTO Patient { get; set; }
    public DoctorDTO Doctor { get; set; }
    public List<AppointmentServiceDTO> Services { get; set; }
}

public class PatientDTO
{
    [MaxLength(100)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class DoctorDTO
{
    [MaxLength(100)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    [MaxLength(7)]
    public string PWZ { get; set; }
}

public class AppointmentServiceDTO
{    
    public int ServiceId { get; set; }
    public string Name { get; set; }
    public decimal BaseFee { get; set; }
    public decimal ServiceFee { get; set; }

}

