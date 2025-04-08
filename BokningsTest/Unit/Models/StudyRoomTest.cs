using Projektarbete_Bokningssystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BokningsTest.Unit.Models
{
    public class StudyRoomTests
    {
        // Enhetstest för att kontrollera att standardvärdena sätts korrekt i StudyRoom-klassen.
        [Fact]
        public void StudyRoom_Properties_CanBeAssigned()
        {
            // Skapa ett nytt StudyRoom-objekt och tilldela värden till dess egenskaper. ARRANGE
            var room = new StudyRoom
            {
                Id = 1,
                Name = "Room A",
                Bookings = new List<Booking>()
            };

            // Verifiera att egenskaperna har tilldelats korrekt. ASSERT
            Assert.Equal(1, room.Id);
            Assert.Equal("Room A", room.Name);
            Assert.NotNull(room.Bookings);
        }

        // Enhetstest för att kontrollera att ett StudyRoom-objekt kan innehålla flera bokningar.
        [Fact]
        public void StudyRoom_CanContainMultipleBookings()
        {
            // Skapa 2 nya Booking objekt. ARRANGE
            var booking1 = new Booking { Id = 1 };
            var booking2 = new Booking { Id = 2 };

            // Skapa ett nytt StudyRoom och tilldela en lista med bokningar.
            var room = new StudyRoom
            {
                Bookings = new List<Booking> { booking1, booking2 }
            };

            // Verifiera att studierummet kan hålla två bokningar. ASSERT
            Assert.Equal(2, room.Bookings.Count);
        }
    }
}
