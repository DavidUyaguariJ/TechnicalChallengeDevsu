using System;
using System.Collections.Generic;

namespace Devsu.PersonCustomer.Infrastructure.Persistence.Entities;

public partial class Customer
{
    public long ClientId { get; set; }

    public string Name { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int Age { get; set; }

    public string Identification { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Status { get; set; }
}
