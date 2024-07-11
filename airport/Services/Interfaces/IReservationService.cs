using airport.Models.Dto;

namespace airport.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<bool> createReservation(ReservationDto reservationDto, int userId);
        public Task<IEnumerable<object>> getReservationsByUser(int userId);
        public Task<bool> cancelReservationSoft(int id, int userId);
        public Task<bool> cancelReservationPer(int id, int userId);
    }
}
