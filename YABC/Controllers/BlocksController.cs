#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using YABC.Data;
using YABC.DTO;
using YABC.Models;
using YABC.Services;
using YABC.ViewModels;

namespace YABC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly IHubContext<BlockHub> _hubContext;
        private readonly IBlockService _blockService;


        public BlocksController(YABCContext context, IHubContext<BlockHub> hubContext, IBlockService blockService)
        {
            _hubContext = hubContext;
            _blockService = blockService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlockDTO>>> GetBlock()
        {
            return await _blockService.GetBlocks();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlockDTO>> GetBlock(int id)
        {
            var block = await _blockService.GetBlock(id);
            if (block == null)
            {
                return NotFound();
            }
            else 
            { 
                return Ok(block); 
            } 
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult<bool>> GetBlockStatus(int id)
        {
            var status = await _blockService.IsBlockValid(id);
            return Ok(status);
        }

        [HttpPut("{id}")]
        public IActionResult PutBlock(int id, Block block)
        {
            return StatusCode(501);
        }

        [HttpGet("{id}/image")]
        public async Task<ActionResult<string>> GetImage(int id)
        {
            (var content, string mime) = await _blockService.GetImage(id);
            return File(content, mime);
        }

        [HttpPost]
        public async Task<ActionResult<Block>> PostBlock(CreateBlockViewModel block)
        {
            var newBlockDTO = await _blockService.AddBlock(block);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new NotificationMessage()
            {
                MessageType = MessageType.Created,
                Block = new BlockDTO()
                {
                    Id = newBlockDTO.Id,
                    CreationDateTime = newBlockDTO.CreationDateTime,
                    Hash256 = newBlockDTO.Hash256,
                    PreviousHash256 = newBlockDTO.PreviousHash256,
                    Name = newBlockDTO.Name,
                    Nonce = newBlockDTO.Nonce,
                    PersonId = newBlockDTO.PersonId
                }
            });

            return CreatedAtAction("GetBlock", new { id = newBlockDTO.Id }, newBlockDTO);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBlock(int id)
        {
            return StatusCode(501);
        }
    }
}
