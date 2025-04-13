using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;

namespace BokningsTest.GUI
{
    public class HomePageTests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        //Setup
        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false }); 
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        // Rensa upp
        public async Task DisposeAsync()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        //Test att hemsidaan visar "Välkommen" och knapparna "Logga in" och "Registrera dig"
        [Fact]
        public async Task HomePage_Should_DisplayWelcomeAndButtons()
        {
            await _page.GotoAsync("https://localhost:7086");

            // Gå till startsidan ARRANGE
            var logoutButton = await _page.Locator("text=Logga ut").ElementHandleAsync();
            if (logoutButton != null)
            {
                //Om användaren är inloggad, logga ut
                await logoutButton.ClickAsync();

                //Vänta på att användaren loggas ut och att inloggningssidan visas
                await _page.WaitForURLAsync("**/Identity/Account/Login");
                await _page.GotoAsync("https://localhost:7086");
            }

            //Kontrollera att startsidan innehåller "Välkommen"
            var content = await _page.ContentAsync();
            Assert.Contains("Välkommen", content);

            //Kontrollera att knappen "Logga in" finns
            var loginBtn = await _page.Locator("text=Logga in").ElementHandleAsync();
            Assert.NotNull(loginBtn);

            //Kontrollera att knappen "Registrera dig" finns
            var registerBtn = await _page.Locator("text=Registrera dig").ElementHandleAsync();
            Assert.NotNull(registerBtn);
        }

    }
    
}
