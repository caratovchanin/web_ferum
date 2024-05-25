using System;
using System.Collections.Generic;

namespace webFerum.Models.AppContext;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Policy { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
