using System.ComponentModel.DataAnnotations;

namespace APBD_cw5.Models;

public class Doctor {
    [Key]
    public int IdDoctor { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}