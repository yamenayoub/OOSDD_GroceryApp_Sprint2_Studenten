using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientService _clientService;
        public AuthService(IClientService clientService)
        {
            _clientService = clientService;
        }
        public Client? Login(string email, string password)
        {
            //Vraag de klantgegevens [Client] op die je zoekt met het opgegeven emailadres

            Client? RetrievedClient = this._clientService.Get(email);

            // Als klant niet gevonden, retourneer null.
            if (RetrievedClient == null)
            {
                return null;
            }

            //Als je een klant gevonden hebt controleer dan of het password matcht --> PasswordHelper.VerifyPassword(password, passwordFromClient)
            if (!PasswordHelper.VerifyPassword(password, RetrievedClient.Password))
            {
                return null;
            }

            //Als alles klopt dan klantgegveens teruggeven, anders null
            return RetrievedClient;
        }
    }
}
