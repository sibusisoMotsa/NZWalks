using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper, IRegionRepository regionRepository, IWalkDifficultyRepository walkDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalks()
        {
            //Fetch data fo=rom DB - domain walks
            var walksDomain = await walkRepository.GetAllAsync();

            //Convert doamin walks to DTO walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);

            //return respons
            return Ok(walksDTO);

        }

        [HttpGet]

        [Route("{id:Guid}")]
        [ActionName("GetWalkById")]

        public async Task<IActionResult> GetWalkById(Guid id)
        {

            var walksDomain = await walkRepository.GetAsync(id);

            // if(walksDomain == null)
            //{
            //  return NotFound();
            //}

            var walksDTO = mapper.Map<Models.DTO.Walk>(walksDomain);

            return Ok(walksDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalk([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {
            //Validate incoming request
            if(!(await ValidateAddWalk(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }
            //Covert DTO to Domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            //Pass domain object to repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            //Convert the Domain object bact to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };

            //Send DTO response back to client
            return CreatedAtAction(nameof(AddWalk), new { id = walkDTO.Id }, walkDTO);


        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateWalk([FromRoute]Guid id,[FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //Validate incoming request
            if(!(await ValidateUpdateWalk(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }
            //Convert DTO to Domain model
            var walk = new Models.Domain.Walk()
            {
                Name = updateWalkRequest.Name,
                Length = updateWalkRequest.Length,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId,
            };
            //Update walk using Repo
            walk = await walkRepository.UpdateAsync(id, walk);
            //If null then not found
            if (walk == null)
            {
                return NotFound();
            }
            //Convert domain back to DTO
            var walkDTO = new Models.DTO.Walk()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId,
            };
            //Return ok response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteWalk(Guid id)
        {
            //call repository to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);
            if(walkDomain == null)
            {
                return NotFound("walk not found!");
            }

            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        }
        #region Private Methods
        private async Task<bool> ValidateAddWalk(Models.DTO.AddWalkRequest addWalkRequest)
        {
            if(addWalkRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest), $"Add walk data required");
                return false;
            }

            if (string.IsNullOrEmpty(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name), $"{nameof(addWalkRequest.Name)} Please add the name, remove any whitespace");
            }

            if(addWalkRequest.Length <= 0)
            {
                ModelState.AddModelError(nameof(addWalkRequest.Length), $"{nameof(addWalkRequest.Length)} Please add a length greater than zero");
            }

            var region = await regionRepository.GetAsync(addWalkRequest.RegionId);

            if (region == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId), $"{nameof(addWalkRequest.RegionId)} is invalid!");
                
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);

            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId), $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid!");

            }

            if(ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        private async Task<bool> ValidateUpdateWalk(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest), $"Add walk data required");
                return false;
            }

            if (string.IsNullOrEmpty(updateWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Name), $"{nameof(updateWalkRequest.Name)} Please add the name, remove any whitespace");
            }

            if (updateWalkRequest.Length <= 0)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Length), $"{nameof(updateWalkRequest.Length)} Please add a length greater than zero");
            }

            var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);

            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId), $"{nameof(updateWalkRequest.RegionId)} is invalid!");

            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);

            if (walkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId), $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid!");

            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
