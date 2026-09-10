using System;
using System.Collections.Generic;

namespace Devsu.AccountMovement.Infrastructure.Persistence.Entities;

public partial class Movement
{
    public long MovementId { get; set; }

    public DateTime MovementDate { get; set; }

    public string MovementType { get; set; } = null!;

    public decimal Value { get; set; }

    public decimal Balance { get; set; }

    public long AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
