using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using Projektarbete_Bokningssystem.Data;
using Projektarbete_Bokningssystem.Pages.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BokningsTest.Unit.Pages
{
    public class CreateModelTests
    {
        [Fact]
        public async Task OnGetAsync_SetsBookingDate_ToToday()
        {
            //skapar en "fejk" användare ARRANGE
            var user = new IdentityUser { Id = "user123" };
            //mock av UserManager, låtsas att den returnerar en användare.
            var userManager = MockUserManager(user);

            //skapa en in-memory databas som ersätter riktiga databasen för testet
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);


            //Skapa sidan vi vill testa, med "fejk" objekt
            var pageModel = new CreateModel(context, userManager.Object);

            // Simulera en inloggad användare i HttpContext
            pageModel.PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }, "mock"))
                }
            };

            // Kör metoden OnGetAsync ACT
            var result = await pageModel.OnGetAsync();

            // Kolla att dagens daturm har satrs ASSERT
            Assert.Equal(DateTime.Today, pageModel.Booking.BookingDate);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsChallenge_WhenUserIsNull()
        {
            // Skapa UserManager som alltid returnerar null (inte inloggad) ARRANGE
            var userManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null
            );
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

            //sKAPA TESTDATABAS
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            // Skapa en CreateModel med mockade beroenden 
            var pageModel = new CreateModel(context, userManager.Object)
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Kör OnGetAsync metoden ACT
            var result = await pageModel.OnGetAsync();

            //kolla att resultatet är en ChallengeResult (dvs. "du måste logga in") ASSERT
            Assert.IsType<ChallengeResult>(result);
        }

        //Hjälpmetod som skapar en fejkad UserManager som returnerar en inloggad användarR
        private Mock<UserManager<IdentityUser>> MockUserManager(IdentityUser fakeUser)
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(fakeUser);
            mgr.Setup(x => x.IsInRoleAsync(fakeUser, "Admin")).ReturnsAsync(false);

            return mgr;
        }

    }
}
