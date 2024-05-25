using System;
using System.Collections.Generic;

namespace webFerum.Models.AppContext;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Number { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Role Role { get; set; } = null!;
}
