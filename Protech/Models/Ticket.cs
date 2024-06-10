using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class Ticket
{
    public int IdTicket { get; set; }

    public int? IdUser { get; set; }

    public int? IdEmployee { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Priority { get; set; }

    public string? State { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<BackupFile> BackupFiles { get; set; } = new List<BackupFile>();

    public virtual User? IdEmployeeNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<TicketAdditionalTask> TicketAdditionalTasks { get; set; } = new List<TicketAdditionalTask>();

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
}
