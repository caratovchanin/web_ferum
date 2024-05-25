using System;
using System.Collections.Generic;

namespace webFerum.Models.AppContext;

public partial class Article
{
    public int Id { get; set; }

    public string Head { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Text { get; set; } = null!;

    public string? Face { get; set; }
}
