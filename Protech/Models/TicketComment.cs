using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class TicketComment
{
    public int IdTicketComment { get; set; }

    public int? IdTicket { get; set; }

    public int? IdUser { get; set; }

    public string? Comment { get; set; }

    public DateTime? Date { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
