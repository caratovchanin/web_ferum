using System;
using System.Collections.Generic;

namespace webFerum.Models.AppContext;

public partial class Client
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdItem { get; set; }

    public string? Message { get; set; }

    public virtual Item IdItemNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
