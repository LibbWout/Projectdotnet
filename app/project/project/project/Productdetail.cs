using System;
using System.Collections.Generic;

namespace project.project;

public partial class Productdetail
{
    public int ProductId { get; set; }

    public DateTime Tijdstip { get; set; }

    public string Naam { get; set; } = null!;

    public float Prijs { get; set; }

    public string Producttype { get; set; } = null!;
}
