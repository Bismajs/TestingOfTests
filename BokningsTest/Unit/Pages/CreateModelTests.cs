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
            // Arrange
            var user = new IdentityUser { Id = "user123" };
            var userManager = MockUserManager(user);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new CreateModel(context, userManager.Object);

            // Simulera en inloggad användare
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

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.Equal(DateTime.Today, pageModel.Booking.BookingDate);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsChallenge_WhenUserIsNull()
        {
            // Arrange
            var userManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null
            );
            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new CreateModel(context, userManager.Object)
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            Assert.IsType<ChallengeResult>(result);
        }

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
