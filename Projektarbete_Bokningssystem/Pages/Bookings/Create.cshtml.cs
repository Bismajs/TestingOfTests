using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projektarbete_Bokningssystem.Data;
using Projektarbete_Bokningssystem.Models;

namespace Projektarbete_Bokningssystem.Pages.Bookings
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Booking Booking { get; set; }

        public SelectList RoomList { get; set; }

        public void OnGet()
        {
            // Kolla om det finns n�gra studierum i databasen
            if (!_context.StudyRooms.Any())
            {
                // Om inte, skapa tre grundl�ggande rum
                var rooms = new List<StudyRoom>
            {
                new StudyRoom { Name = "Studierum 1" },
                new StudyRoom { Name = "Studierum 2" },
                new StudyRoom { Name = "Studierum 3" }
            };

                _context.StudyRooms.AddRange(rooms);
                _context.SaveChanges();
            }

            // H�mta rum f�r dropdown-listan
            RoomList = new SelectList(_context.StudyRooms, "Id", "Name");

            // S�tt dagens datum som standard
            Booking = new Booking
            {
                BookingDate = DateTime.Today
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RoomList = new SelectList(_context.StudyRooms, "Id", "Name");
                return Page();
            }

            // H�mta inloggad anv�ndare
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Omdirigera till inloggning
            }

            // S�tt anv�ndare f�r bokningen
            Booking.UserId = user.Id;
            Booking.CreatedAt = DateTime.Now;
            Booking.Status = BookingStatus.Confirmed;

            // Kontrollera om rummet redan �r bokat det valda datumet
            var existingBooking = await _context.Bookings
                .Where(b => b.StudyRoomId == Booking.StudyRoomId &&
                        b.BookingDate.Date == Booking.BookingDate.Date &&
                        b.Status == BookingStatus.Confirmed)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                ModelState.AddModelError(string.Empty, "Detta rum �r redan bokat f�r det valda datumet.");
                RoomList = new SelectList(_context.StudyRooms, "Id", "Name");
                return Page();
            }

            // Spara bokningen
            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { message = "Bokning skapad framg�ngsrikt!" });
        }
    }
}

