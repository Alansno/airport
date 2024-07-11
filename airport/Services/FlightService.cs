using airport.Context;
using airport.Hubs;
using airport.Models;
using airport.Models.Dto;
using airport.Models.Querys;
using airport.Services.Interfaces;
using airport.Utils;
using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;

namespace airport.Services
{
    public class FlightService : IFlightService
    {
        private readonly AirportContext _airportContext;
        private readonly string _connectionString;
        private readonly IHubContext<FlightHub> _hubContext;
        public FlightService(AirportContext airportContext, IConfiguration configuration, IHubContext<FlightHub> hubContext)
        {
            _airportContext = airportContext;
            _connectionString = configuration.GetConnectionString("Connection");
            _hubContext = hubContext;
        }

        public async Task<bool> createFlight(FlightDto flightdto)
        {
            DateTime utcDateDeparture = Utilities.convertToUtc(flightdto.DateDeparture, flightdto.Timezone);
            DateTime utcArrivalDate = Utilities.convertToUtc(flightdto.ArrivalDate, flightdto.Timezone);

            var flight = new Flight
            {
                Origin = flightdto.Origin,
                Destination = flightdto.Destination,
                DateDeparture = utcDateDeparture,
                ArrivalDate = utcArrivalDate,
                Airline = flightdto.Airline,
                Capacity = flightdto.Capacity,
                numFlight = flightdto.numFlight,
                Status = "Programado"
            };

            try
            {
                await _airportContext.Flights.AddAsync(flight);
                await _airportContext.SaveChangesAsync();

                var currentFligs = await currentFlights(flightdto.currentDate, flightdto.Timezone);
                await _hubContext.Clients.All.SendAsync("flights", currentFligs);
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<IEnumerable<object>> currentFlights(DateTime dateUser, string timezoneId)
        {
            try
            {

                // Obtener la zona horaria del usuario
                TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

                // Convertir la fecha local del usuario a la zona horaria local del usuario
                DateTime userLocalDateWithoutTime = TimeZoneInfo.ConvertTime(dateUser, userTimeZone).Date;

                DateTime userLocalDatePlusOne = userLocalDateWithoutTime.AddDays(1);

                // Consulta SQL para obtener vuelos actuales en base a la fecha local del usuario
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sql = @"SELECT Id, Origin, Destination, DateDeparture, ArrivalDate, Status, Airline 
                        FROM Flights 
                        WHERE CONVERT(date, SWITCHOFFSET(DateDeparture, DATEPART(TzOffset, DateDeparture))) = @UserLocalDateWithoutTime
                        OR CONVERT(date, SWITCHOFFSET(DateDeparture, DATEPART(TzOffset, DateDeparture))) = @UserLocalDatePlusOne";

                    var result = await connection.QueryAsync(sql, new { UserLocalDateWithoutTime = userLocalDateWithoutTime, UserLocalDatePlusOne = userLocalDatePlusOne });

                    // Convertir DateDeparture de UTC a la zona horaria del usuario
                    foreach (var flight in result)
                    {
                        flight.DateDeparture = TimeZoneInfo.ConvertTimeFromUtc(flight.DateDeparture, userTimeZone);
                    }

                    // Filtrar los vuelos donde DateDeparture sea igual a la fecha local del usuario
                    var filteredFlights = result.Where(f => f.DateDeparture.Date == userLocalDateWithoutTime);
                    return filteredFlights;
                }
                }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> deleteFlight(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> findFlightById(int id)
        {
            var flight = await (from f in _airportContext.Flights
                                join p in _airportContext.Plans on f.Id equals p.FlightId
                                where f.Id == id
                                select new {
                                    id = f.Id,
                                    origin = f.Origin,
                                    destination = f.Destination,
                                    dateDeparture = f.DateDeparture,
                                    arrivalDate = f.ArrivalDate,
                                    status = f.Status,
                                    airline = f.Airline,
                                    planId = p.Id,
                                    typePlan = p.TypePlan,
                                    price = p.Price,
                                    description = p.Description
                                }
                                ).ToListAsync();
            if (flight.Count == 0)
            {
            throw new ArgumentException("No existe el vuelo");
                
            }
            return flight;
        }

        public async Task<FlightReservations> getReservationsById(int id)
        {
            try
            {
                var reservations = await(from f in _airportContext.Flights
                                         join r in _airportContext.Reservations on f.Id equals r.FlightId into reservationGroup
                                         where f.Id == id
                                         select new FlightReservations
                                         {
                                             Capacity = f.Capacity,
                                             Status = f.Status,
                                             Reservations = reservationGroup.Count()
                                         }
                                         ).FirstOrDefaultAsync();

                return reservations ?? throw new ArgumentException("No se encontraron registros del vuelo");
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener reservaciones por ID", ex);
            }
        }

        public Task<bool> updateFlight(FlightDto flightdto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
