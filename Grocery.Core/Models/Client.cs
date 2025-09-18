
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public int Id { get; set; }
        public Client(int id, string name, string emailAddress, string password) : base(id, name)
        {
            this.EmailAddress=emailAddress;
            this.Password=password;
            this.Id = id;
        }
    }
}
