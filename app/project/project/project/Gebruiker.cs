using System;
using System.Collections.Generic;

namespace project.project;

public partial class Gebruiker
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public string WachtwoordHash { get; set; } = null!;

    public string UniekeCode { get; set; } = null!;

    public DateTime? TijdstipGeactiveerd { get; set; }
}
