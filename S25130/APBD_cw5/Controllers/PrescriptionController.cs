namespace APBD_cw5.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using APBD_cw5.DTOs;
using APBD_cw5.Services;


[ApiController]
[Route("api/prescriptions")]
public class PrescriptionController : ControllerBase {
    private readonly IPrescriptionService _service;
    public PrescriptionController(IPrescriptionService service) {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequest request) {
        try {
            await _service.AddPrescriptionAsync(request);
            return Ok();
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
    }
}
