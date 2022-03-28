using Microsoft.AspNetCore.Mvc;
using YABC.DTO;
using YABC.Models;
using YABC.ViewModels;

namespace YABC.Services;

public interface IBlockService
{   
    string CalculateHash(Block block);
    Task<bool> IsBlockValid(long id);
    Task<List<BlockDTO>> GetBlocks();
    Task<BlockDTO?> GetBlock(int id);
    Task<BlockDTO> AddBlock(CreateBlockViewModel block);
    Task<(byte[], string)> GetImage(int id);
}
