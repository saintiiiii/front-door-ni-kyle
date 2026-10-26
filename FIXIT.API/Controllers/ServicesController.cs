using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FIXIT.API.Controllers;
[ApiController, Route("api/services")]
public class ServicesController(FIXITDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List(CancellationToken ct)=>Ok(await db.Services.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.Name).Select(x=>new{x.Id,x.Name,x.Description,x.BasePrice}).ToListAsync(ct));
    [HttpGet("areas")] public async Task<IActionResult> Areas()=>Ok(await db.ServiceAreas.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.CityMunicipality).Select(x=>new{x.Id,x.CityMunicipality,x.Province}).ToListAsync());
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id)=>await db.Services.AsNoTracking().Where(x=>x.Id==id&&x.IsActive).Select(x=>new{x.Id,x.Name,x.Description,x.BasePrice}).FirstOrDefaultAsync() is{} s?Ok(s):NotFound();
}
