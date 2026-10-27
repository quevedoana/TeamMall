using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeaMall.Models;
using TeaMall.Services;

namespace TeaMall.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
public async Task<IActionResult> ObtenerFechaActualizacion(
    [FromServices] GoogleDriveService googleDriveService)
{
    try
    {
        var fecha = await googleDriveService
            .ObtenerUltimaModificacionAsync();

        return Json(new
        {
            success = true,
            fecha = fecha?.ToString("O")
        });
    }
    catch (Exception ex)
    {
        // Registrar el detalle técnico en el servidor.
        Console.Error.WriteLine(
            $"Error consultando Google Drive: {ex}");

        return StatusCode(502, new
        {
            success = false,
            mensaje = "No se pudo consultar la fecha de actualización."
        });
    }
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
