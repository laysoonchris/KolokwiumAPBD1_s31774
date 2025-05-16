using System.Data.Common;
using KolokwiumAPBD1_s31774.Exceptions;
using KolokwiumAPBD1_s31774.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace KolokwiumAPBD1_s31774.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }

    // public async Task<RegisterAppointmentDTO> GetAppointmentById(int AppointmentId)
    // {
    //     var query =
    //         @"SELECT first_name, last_name, r.rental_id, rental_date, return_date, s.name, ri.price_at_rental, m.title
    //         FROM Rental r
    //         JOIN Customer c ON r.customer_id = c.customer_id
    //         JOIN Status s ON r.status_id = s.status_id
    //         JOIN Rental_Item ri ON ri.rental_id = r.rental_id
    //         JOIN Movie m ON m.movie_id = ri.movie_id
    //         WHERE r.customer_id = @customerId;";
    //
    //     await using SqlConnection connection = new SqlConnection(_connectionString);
    //     await using SqlCommand command = new SqlCommand();
    //
    //     command.Connection = connection;
    //     command.CommandText = query;
    //     await connection.OpenAsync();
    //
    //     command.Parameters.AddWithValue("@AppointmentId", AppointmentId);
    //     var reader = await command.ExecuteReaderAsync();
    //
    //     AppointmentDTO? rentals = null;
    //
    //     while (await reader.ReadAsync())
    //     {
    //         if (rentals is null)
    //         {
    //             rentals = new AppointmentDTO
    //             {
    //                 Date = reader.GetDateTime(0),
    //                 Patient = reader.getPatientDTO(1),
    //                 Doctor = reader.
    //                 Services = new List<AppointmentServiceDTO>()
    //             };
    //         }
    //
    //             int rentalId = reader.GetInt32(2);
    //             
    //             var rental = rentals.Rentals.FirstOrDefault(e => e.Id.Equals(rentalId));
    //             if (rental is null)
    //             {
    //                 rental = new RentalDetailsDto()
    //                 {
    //                     Id = rentalId,
    //                     RentalDate = reader.GetDateTime(3),
    //                     ReturnDate = await reader.IsDBNullAsync(4) ? null : reader.GetDateTime(4),
    //                     Status = reader.GetString(5),
    //                     Movies = new List<RentedMovieDto>()
    //                 };
    //                 rentals.Rentals.Add(rental);
    //             }
    //             rental.Movies.Add(new RentedMovieDto()
    //             {
    //                 Title = reader.GetString(7),
    //                 PriceAtRental = reader.GetDecimal(6),
    //             });
    //             
    //         }       
    //         
    //         if (rentals is null)
    //         {
    //             throw new NotFoundException("No rentals found for the specified customer.");
    //         }
    //         
    //         return rentals;
    //         return new RegisterAppointmentDTO();
    //
    //     }
    // }


    public Task<RegisterAppointmentDTO> GetAppointmentById(int AppointmentId)
    {
        throw new NotImplementedException();
    }

    public async Task AddNewAppointment(int PatientId, RegisterAppointmentDTO registerAppointmentDto)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.Parameters.Clear();
            command.CommandText = "SELECT 1 FROM Patient WHERE patient_id = @PatientId;";
            command.Parameters.AddWithValue("@PatientId", PatientId);

            var patientIdRes = await command.ExecuteScalarAsync();
            if (patientIdRes is null)
                throw new NotFoundException($"Patient with ID - {PatientId} - not found.");

            command.Parameters.Clear();
            command.CommandText =
                @"INSERT INTO Appointment
            VALUES(@AppointmentId, @PatientId, @PWZ, @Services);";

            command.Parameters.AddWithValue("@AppointmentId", registerAppointmentDto.AppointmentId);
            command.Parameters.AddWithValue("@PatientId", PatientId);
            command.Parameters.AddWithValue("@PWZ", registerAppointmentDto.PWZ);
            command.Parameters.AddWithValue("@Services", registerAppointmentDto.Services);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                throw new ConflictException("An appointment with the same ID already exists.");
            }


            foreach (var service in registerAppointmentDto.Services)
            {
                command.Parameters.Clear();
                command.CommandText = "SELECT service_id FROM Service WHERE name = @ServiceName;";
                command.Parameters.AddWithValue("@ServieName", service.Name);

                var serviceId = await command.ExecuteScalarAsync();
                if (serviceId is null)
                    throw new NotFoundException($"Service - {service.Name} - not found.");

                command.Parameters.Clear();
                command.CommandText =
                    @"INSERT INTO Service
                        VALUES(@ServiceId, @Name, @BaseFee);";

                command.Parameters.AddWithValue("@ServiceId", serviceId);

                command.Parameters.AddWithValue("@Name", service.Name);

                command.Parameters.AddWithValue("@BaseFee", service.BaseFee);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

    