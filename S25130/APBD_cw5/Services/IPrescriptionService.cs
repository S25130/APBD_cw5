using System.Threading.Tasks;
using APBD_cw5.DTOs;

namespace APBD_cw5.Services;

public interface IPrescriptionService {
    Task AddPrescriptionAsync(AddPrescriptionRequest request);
}