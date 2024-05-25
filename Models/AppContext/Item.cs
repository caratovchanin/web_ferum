using System;
using System.Collections.Generic;

namespace webFerum.Models.AppContext;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Img { get; set; }

    public int Cost { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
