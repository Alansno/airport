using airport.Context;
using airport.Models;
using airport.Models.Dto;
using airport.Services.Interfaces;
using airport.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace airport.Services
{
    public class ReservationService: IReservationService
    {
        private readonly AirportContext _airportContext;
        private readonly IFlightService _flightService;
        private readonly string _connectionString;
        public ReservationService(AirportContext airportContext, IFlightService flightService, IConfiguration configuration)
        {
            _airportContext = airportContext;
            _flightService = flightService;
            _connectionString = configuration.GetConnectionString("Connection");
        }

        public async Task<bool> cancelReservationPer(int id, int userId)
        {
            try
            {
                var reservation = await _airportContext.Reservations.Where(r => r.Id == id).Where(r => r.UserId == userId).FirstOrDefaultAsync();
                if (reservation == null) return false;

                _airportContext.Remove(reservation);
                await _airportContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> cancelReservationSoft(int id, int userId)
        {
            try
            {
                var reservation = await _airportContext.Reservations.Where(r => r.Id == id).Where(r => r.UserId == userId).FirstOrDefaultAsync();
                if (reservation == null) return false;

                reservation.isDeleted = true;
                reservation.DeletedAt = DateTime.UtcNow;

                _airportContext.Update(reservation);
                return true;
            }   
            catch (DbUpdateException ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> createReservation(ReservationDto reservationDto, int userId)
        {
            using (var transaction = _airportContext.Database.BeginTransaction())
            {
                try
                {
                    var flight = await _airportContext.Flights
                    .FromSqlInterpolated($"SELECT * FROM Flights WITH (UPDLOCK) WHERE Id = {reservationDto.FlightId}")
                    .FirstOrDefaultAsync();

                    var reservations = await _flightService.getReservationsById(reservationDto.FlightId);

                    if (reservations.Reservations < reservations.Capacity && reservations.Status == "Programado")
                    {
                        var reservation = new Reservation
                        {
                            DateReservation = DateTime.UtcNow,
                            StateReservation = "Satisfactorio",
                            UserId = userId,
                            FlightId = reservationDto.FlightId,
                            PlanId = reservationDto.PlanId,
                        };

                        await _airportContext.Reservations.AddAsync(reservation);
                        await _airportContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        throw new ArgumentException("Ya no hay espacio para reservar este vuelo");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<object>> getReservationsByUser(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT r.DateReservation, r.StateReservation, f.Destination, p.TypePlan FROM Reservations r JOIN Flights f ON r.FlightId = f.Id JOIN Plans p ON r.PlanId = p.Id WHERE r.UserId = @userId;";
                    var result = await connection.QueryAsync(sql, new { userId });

                    return result;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
