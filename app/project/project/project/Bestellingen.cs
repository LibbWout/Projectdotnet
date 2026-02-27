using System;
using System.Collections.Generic;

namespace project.project;

public partial class Bestellingen
{
    public int Id { get; set; }

    public int GebruikerId { get; set; }

    public DateTime TijdstipBesteld { get; set; }

    public string Status { get; set; } = null!;
}
