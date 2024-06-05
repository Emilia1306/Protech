using System;
using System.Collections.Generic;

namespace Protech.Models;

public partial class BackupFile
{
    public int IdBackupFile { get; set; }

    public int? IdTicket { get; set; }

    public string? Name { get; set; }

    public virtual Ticket? IdTicketNavigation { get; set; }
}
