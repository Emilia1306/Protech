using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class UserCategory
{
    public int IdUserCategory { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
