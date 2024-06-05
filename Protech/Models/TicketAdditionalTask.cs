using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class TicketAdditionalTask
{
    public int IdTicketAdditionalTask { get; set; }

    public int? IdTicket { get; set; }

    public int? IdEmployee { get; set; }

    public string? Description { get; set; }

    public bool? Finished { get; set; }

    public virtual User? IdEmployeeNavigation { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }
}
