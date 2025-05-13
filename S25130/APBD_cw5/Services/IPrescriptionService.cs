namespace APBD_cw5.Services;

public interface IPrescriptionService {
    Task AddPrescriptionAsync(AddPrescriptionRequest request);
}
