using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class User
{
    public int IdUser { get; set; }

    public int? IdUserCategory { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Cellphone { get; set; }

    public string? Password { get; set; }

    public bool? ChangePassword { get; set; }

    public string? CompanyName { get; set; }

    public string? JobPosition { get; set; }

    public virtual UserCategory? IdUserCategoryNavigation { get; set; }

    public virtual ICollection<TicketAdditionalTask> TicketAdditionalTasks { get; set; } = new List<TicketAdditionalTask>();

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    public virtual ICollection<Ticket> TicketIdEmployeeNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketIdUserNavigations { get; set; } = new List<Ticket>();
}
