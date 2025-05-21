namespace APBD_cw5.Tests;

using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using APBD_cw5.Data;
using APBD_cw5.DTOs;
using APBD_cw5.Models;
using APBD_cw5.Services;

public class PrescriptionServiceTests {
    private AppDbContext GetDbContext(string dbName) {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    private AddPrescriptionRequest GetValidRequest() => new AddPrescriptionRequest {
        Patient = new PatientDto {
            FirstName = "Jan",
            LastName = "Kowalski",
            BirthDate = new DateTime(1990, 1, 1)
        },
        Doctor = new DoctorDto {
            IdDoctor = 1
        },
        Date = DateTime.Today,
        DueDate = DateTime.Today.AddDays(1),
        Medicaments = new List<PrescriptionMedicamentDto> {
            new PrescriptionMedicamentDto {
                IdMedicament = 1,
                Dose = 10,
                Description = "Test desc"
            }
        }
    };

    [Fact]
    public async Task AddPrescriptionAsync_AddsNewPatient_WhenNotExists() {
        var db = GetDbContext(nameof(AddPrescriptionAsync_AddsNewPatient_WhenNotExists));
        db.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Lek", LastName = "arz", Email = "d@x.com" });
        db.Medicaments.Add(new Medicament { IdMedicament = 1, Name = "Apap", Description = "pain", Type = "pill" });
        await db.SaveChangesAsync();

        var service = new PrescriptionService(db);
        var request = GetValidRequest();

        await service.AddPrescriptionAsync(request);

        Assert.Single(db.Patients);
        Assert.Single(db.Prescriptions);
    }

    [Fact]
    public async Task AddPrescriptionAsync_Throws_WhenDueDateBeforeDate() {
        var db = GetDbContext(nameof(AddPrescriptionAsync_Throws_WhenDueDateBeforeDate));
        var service = new PrescriptionService(db);
        var request = GetValidRequest();
        request.DueDate = request.Date.AddDays(-1);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddPrescriptionAsync(request));
        Assert.Equal("DueDate must be greater than or equal to Date.", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_Throws_WhenMoreThan10Medicaments() {
        var db = GetDbContext(nameof(AddPrescriptionAsync_Throws_WhenMoreThan10Medicaments));
        db.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Lek", LastName = "arz", Email = "d@x.com" });
        for (int i = 1; i <= 11; i++) {
            db.Medicaments.Add(new Medicament { IdMedicament = i, Name = $"M{i}", Description = "desc", Type = "pill" });
        }
        await db.SaveChangesAsync();

        var service = new PrescriptionService(db);
        var request = GetValidRequest();
        request.Medicaments = new List<PrescriptionMedicamentDto>();
        for (int i = 1; i <= 11; i++) {
            request.Medicaments.Add(new PrescriptionMedicamentDto {
                IdMedicament = i, Dose = 1, Description = "desc"
            });
        }

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddPrescriptionAsync(request));
        Assert.Equal("Prescription cannot contain more than 10 medicaments.", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_Throws_WhenMedicamentDoesNotExist() {
        var db = GetDbContext(nameof(AddPrescriptionAsync_Throws_WhenMedicamentDoesNotExist));
        db.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Lek", LastName = "arz", Email = "d@x.com" });
        await db.SaveChangesAsync();

        var service = new PrescriptionService(db);
        var request = GetValidRequest(); // Lek ID 1 – brak w bazie

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddPrescriptionAsync(request));
        Assert.Equal("One or more medicaments do not exist.", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_Throws_WhenDoctorDoesNotExist() {
        var db = GetDbContext(nameof(AddPrescriptionAsync_Throws_WhenDoctorDoesNotExist));
        db.Medicaments.Add(new Medicament { IdMedicament = 1, Name = "Apap", Description = "pain", Type = "pill" });
        await db.SaveChangesAsync();

        var service = new PrescriptionService(db);
        var request = GetValidRequest(); // Lekarz ID 1 – brak w bazie

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddPrescriptionAsync(request));
        Assert.Equal("Doctor not found.", ex.Message);
    }
}
