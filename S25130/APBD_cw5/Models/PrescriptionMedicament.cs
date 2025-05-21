using System.ComponentModel.DataAnnotations;

namespace APBD_cw5.Models;

public class PrescriptionMedicament {
    [Key]
    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; } = null!;

    public int IdMedicament { get; set; }
    public Medicament Medicament { get; set; } = null!;

    public int Dose { get; set; }
    public string Description { get; set; } = null!;
}
