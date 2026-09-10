using System;
using System.Collections.Generic;

namespace Devsu.AccountMovement.Infrastructure.Persistence.Entities;

public partial class Account
{
    public long AccountId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public decimal InitialBalance { get; set; }

    public bool Status { get; set; }

    public long CustomerId { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
