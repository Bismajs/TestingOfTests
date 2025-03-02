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

        // Property f�r att lagra bokningsdata f�r kalendern
        public List<object> BookingEvents { get; set; }

        public SelectList RoomList { get; set; } //Bindas till en dropdown meny

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

            // H�mta bokningar fr�n databasen f�r kalendern
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

        public async Task<IActionResult> OnPostAsync()
        {
            // H�mta inloggad anv�ndare
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // S�tt anv�ndare f�r bokningen INNAN modelltillst�ndsvalidering
            Booking.UserId = user.Id;
            Booking.CreatedAt = DateTime.Now;
            Booking.Status = BookingStatus.Confirmed;

            // Ta bort validering f�r navigeringsegenskaper som kommer att hanteras automatiskt
            ModelState.Remove("Booking.User");
            ModelState.Remove("Booking.StudyRoom");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {
                        Property = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                TempData["DebugMessage"] = $"ModelState.IsValid �r false. Fel: {System.Text.Json.JsonSerializer.Serialize(errors)}";

                RoomList = new SelectList(_context.StudyRooms, "Id", "Name");
                return Page();
            }

            // Kontrollera om rummet redan �r bokat
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

            try
            {
                _context.Bookings.Add(Booking);
                await _context.SaveChangesAsync();
                TempData["DebugMessage"] = "Bokning sparad framg�ngsrikt";
                return RedirectToPage("./Index", new { message = "Bokning skapad framg�ngsrikt!" });
            }
            catch (Exception ex)
            {
                TempData["DebugMessage"] = "Fel vid sparande: " + ex.Message;
                ModelState.AddModelError(string.Empty, "Ett fel uppstod n�r bokningen skulle sparas.");
                RoomList = new SelectList(_context.StudyRooms, "Id", "Name");
                return Page();
            }
        }
    }
}


