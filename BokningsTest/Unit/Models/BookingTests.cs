using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projektarbete_Bokningssystem.Models;



namespace BokningsTest.Unit.Models
{
    public class BookingTests
    {
        //Enhetstester flr att kontrollera att standardvärdena sätts korrekt i Booking-klassen.
        [Fact]
        public void Booking_DefaultValues_AreSetCorrectly()
        {
            // Skapar ett nytt objekt av Booking för att testa standardvärdena. ARRANGE
            var booking = new Booking();

            // Verifierar att standardvärdena är korrekta. ASSERT
            Assert.Equal(BookingStatus.Confirmed, booking.Status); // Standardvärde för Status är Confirmed
            Assert.True((DateTime.Now - booking.CreatedAt).TotalSeconds < 1); // bokningen skapades nyss


        }

        // Enhetstest för att kontrollera att egenskaperna i Booking-klassen kan tilldelas värden.
        [Fact]
        public void Booking_Properties_CanBeAssigned()
        {
            // Skapa testdata för en användare och ett studierum som krävs för bokningen. ARRANGE
            var user = new Microsoft.AspNetCore.Identity.IdentityUser { UserName = "testuser" };
            var studyRoom = new StudyRoom { Id = 1, Name = "Room A" };

            //Skapa ett nytt objekt av Booking och tilldela egenskaper.
            var booking = new Booking
            {
                Id = 1,
                UserId = "user123",
                User = user,
                StudyRoomId = studyRoom.Id,
                StudyRoom = studyRoom,
                BookingDate = new DateTime(2025, 5, 1),
                CreatedAt = DateTime.Now,
                Status = BookingStatus.Cancelled
            };

            // Verifiera att egenskaperna har tilldelats korrekt. ASSERT
            Assert.Equal(1, booking.Id);
            Assert.Equal("user123", booking.UserId);
            Assert.Equal(user, booking.User);
            Assert.Equal(1, booking.StudyRoomId);
            Assert.Equal(studyRoom, booking.StudyRoom);
            Assert.Equal(new DateTime(2025, 5, 1), booking.BookingDate);
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }
    }
}