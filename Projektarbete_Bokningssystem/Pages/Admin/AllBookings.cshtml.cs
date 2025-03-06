using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projektarbete_Bokningssystem.Data;
using Projektarbete_Bokningssystem.Models;

namespace Projektarbete_Bokningssystem.Pages.Admin
{
        [Authorize]
        public class AllBookingsModel : PageModel
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;

            public AllBookingsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public IList<Booking> Bookings { get; set; }

            [TempData]
            public string StatusMessage { get; set; }

            public async Task<IActionResult> OnGetAsync()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge(); // Omdirigera till inloggning
                }

                // H�mta anv�ndarens bokningar
                Bookings = await _context.Bookings
                    .Include(b => b.StudyRoom)
                    .Include(b => b.User)
                    .Where(b => b.Status == BookingStatus.Confirmed)
                    .OrderBy(b => b.BookingDate)
                    .ToListAsync();

                // L�s in TempData om det finns
                if (TempData["Message"] != null)
                {
                    StatusMessage = TempData["Message"].ToString();
                }

                return Page();
            }

            public async Task<IActionResult> OnPostAsync(int id)
            {
                //Hitta anv�ndaren
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                // Hitta alla bokningar
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    return NotFound();
                }

                try
                {
                    // Ta bort bokningen fr�n databasen
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync(); // Spara �ndringarna till databasen

                    StatusMessage = "Bokningen har tagits bort.";
                }
                catch (Exception ex)
                {
                    // Hantera eventuella fel, exempelvis logga
                    StatusMessage = "Ett fel uppstod n�r bokningen skulle tas bort.";
                }

                return RedirectToPage();
            }
        }
    }