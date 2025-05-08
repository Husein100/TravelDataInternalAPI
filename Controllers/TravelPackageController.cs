//using Microsoft.AspNetCore.Mvc;

//namespace TravelDataInternalAPI.Controllers
//{
//    [ApiController]
//    [Route("api/packages")]
//    public class TravelPackageController : ControllerBase
//    {


//        public TravelPackageController(ITravelPackageRepository repository)
//        {
//            _repository = repository;
//        }

//        // Gem en ny rejsepakke
//        [HttpPost]
//        public async Task<IActionResult> CreateTravelPackage([FromBody] TravelPackage package)
//        {
//            if (package == null)
//            {
//                return BadRequest("Invalid travel package.");
//            }

//            // Gem pakken i databasen
//            await _repository.SaveAsync(package);

//            // Returnér en succesrespons
//            return CreatedAtAction(nameof(GetTravelPackage), new { id = package.Id }, package);
//        }

//        // Hent en rejsepakke
//        [HttpGet("{id}")]
//        public async Task<ActionResult<TravelPackage>> GetTravelPackage(int id)
//        {
//            var package = await _repository.GetByIdAsync(id);
//            if (package == null)
//            {
//                return NotFound();
//            }
//            return package;
//        }
//    }

//}
