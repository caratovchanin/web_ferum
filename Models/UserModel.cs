using webFerum.Models.AppContext;

namespace webFerum.Models
{
    public class UserModel
    {
        public string Name { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Number { get; set; } = null!;

        public string Email { get; set; }

        public string Password { get; set; } 

        public UserModel(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Lastname = user.Lastname;
            Number = user.Number;
            Email = user.Email;
        }
        public UserModel()
        {
        }
        public virtual User GetUser()
        {
            return new User()
            {
                Name = Name,
                Surname = Surname,
                Lastname = Lastname,
                Number = Number,
                Email = Email,
                Password = Password,
                RoleId = 1
            };
        }
    }

    public class UserModelRole : UserModel
    {
        public override User GetUser()
        {
            var user = base.GetUser();
            user.RoleId = RoleId;
            return user;
        }
        public UserModelRole(User user) 
            : base(user)
        { }

        public int RoleId { get; set; }
    }

    public class UserChached
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public class AuthUser
    {
        public string Email { get; set; } 
        public string Password { get; set; }

        public bool isValid { get { return Email != null && Password != null;}}
    }

}
