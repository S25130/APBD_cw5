namespace APBD_cw5.DTO;

public class AddPrescriptionRequest {
    public PatientDto Patient { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
    public List<PrescriptionMedicamentDto> Medicaments { get; set; } = new();
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}

public class PatientDto {
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
}

public class DoctorDto {
    public int IdDoctor { get; set; }
}

public class PrescriptionMedicamentDto {
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public string Description { get; set; } = null!;
}
