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

        // Property för att lagra bokningsdata för kalendern
        public List<object> BookingEvents { get; set; }

        public SelectList RoomList { get; set; } //Bindas till en dropdown meny

        public void LoadBookingData() // I am a
        {
            // Hämta rum för dropdown-listan
            RoomList = new SelectList(_context.StudyRooms, "Id", "Name");

            // Hämta bokningar för kalendern
            var bookings = _context.Bookings
                .Include(b => b.StudyRoom)
                .Where(b => b.Status == BookingStatus.Confirmed)
                .ToList();

            // Konvertera till FullCalendar-format
            BookingEvents = bookings.Select(b => new
            {
                id = b.Id,
                title = b.StudyRoom.Name + " (Bokat)",
                start = b.BookingDate.ToString("yyyy-MM-dd"),
                roomId = b.StudyRoomId,
                allDay = true
            }).ToList<object>();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Kontrollera om användaren är admin och omdirigera om det behövs
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToPage("/Admin/AdminCreate");
            }

            // Sätt dagens datum som standard
            Booking = new Booking
            {
                BookingDate = DateTime.Today
            };

            // Ladda bokningsdata
            LoadBookingData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Hämta inloggad användare
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Kontrollera om användaren redan har 3 bokningar
            var userBookingsCount = await _context.Bookings
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Confirmed)
                .CountAsync();

            if (userBookingsCount >= 3)
            {
                ModelState.AddModelError(string.Empty, "Du har redan 3 bokningar, ta bort en bokning innan du gör en ny");
                // Ladda om bokningsdata så att kalendern visas korrekt
                LoadBookingData();
                return Page();
            }

            // Sätt användare för bokningen INNAN validering av formulär
            Booking.UserId = user.Id;
            Booking.CreatedAt = DateTime.Now;
            Booking.Status = BookingStatus.Confirmed;

            // Ta bort validering för egenskaper som kommer att hanteras automatiskt
            ModelState.Remove("Booking.User");
            ModelState.Remove("Booking.StudyRoom");


            // Kontrollera om rummet redan är bokat
            var existingBooking = await _context.Bookings
                .Where(b => b.StudyRoomId == Booking.StudyRoomId &&
                        b.BookingDate.Date == Booking.BookingDate.Date &&
                        b.Status == BookingStatus.Confirmed)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                ModelState.AddModelError(string.Empty, "Detta rum är redan bokat för det valda datumet.");

                // Ladda om bokningsdata så att kalendern visas korrekt
                LoadBookingData();
                
                return Page();
            }

            try
            {
                _context.Bookings.Add(Booking);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Bokning sparad!";
                TempData["MessageType"] = "success";
                return RedirectToPage("/Bookings/MyBookings");
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                ViewData["Message"] = "Det uppstod ett fel vid bokningen.";
                ViewData["MessageType"] = "danger";

                // Ladda om bokningsdata så att kalendern visas korrekt
                LoadBookingData();

                return Page();
            }
        }
    }
}


