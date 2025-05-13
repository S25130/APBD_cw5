using APBD_cw5.Data;
using APBD_cw5.DTOs;
using APBD_cw5.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_cw5.Services;

public class PrescriptionService : IPrescriptionService {
    private readonly AppDbContext _context;

    public PrescriptionService(AppDbContext context) {
        _context = context;
    }

    public async Task AddPrescriptionAsync(AddPrescriptionRequest request) {
        if (request.DueDate < request.Date)
            throw new ArgumentException("DueDate must be greater than or equal to Date.");

        if (request.Medicaments.Count > 10)
            throw new ArgumentException("Prescription cannot contain more than 10 medicaments.");

        var existingMedicaments = await _context.Medicaments
            .Where(m => request.Medicaments.Select(med => med.IdMedicament).Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        if (existingMedicaments.Count != request.Medicaments.Count)
            throw new ArgumentException("One or more medicaments do not exist.");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p =>
                p.FirstName == request.Patient.FirstName &&
                p.LastName == request.Patient.LastName &&
                p.BirthDate == request.Patient.BirthDate);

        if (patient == null) {
            patient = new Patient {
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                BirthDate = request.Patient.BirthDate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(); // potrzebne do uzyskania IdPatient
        }

        var doctor = await _context.Doctors.FindAsync(request.Doctor.IdDoctor);
        if (doctor == null)
            throw new ArgumentException("Doctor not found.");

        var prescription = new Prescription {
            Date = request.Date,
            DueDate = request.DueDate,
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Description = m.Description
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
    }
}
