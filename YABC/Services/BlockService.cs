using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using YABC.Data;
using YABC.DTO;
using YABC.Models;
using YABC.ViewModels;

namespace YABC.Services;

public class BlockService : IBlockService
{
    private readonly ILogger<MiningHostService> _logger;
    private readonly YABCContext _context;


    private const int difficult = 4;

    public BlockService(ILogger<MiningHostService> logger, YABCContext context)
    {
        _logger = logger;
        _context = context;
    }

    public string CalculateHash(Block block)
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(block.Nonce);
            writer.Write(block.Id);
            writer.Write(block.PreviousHash256);
            writer.Write(block.Name);
            writer.Write(block.CreationDateTime.ToString("yyyyMMddhhmmss"));
            writer.Write(block.Image);
            writer.Write((int)block.PersonId);
        }
        stream.Flush();
        byte[] bytes = stream.GetBuffer();
        var newHash = ComputeSha256Hash(bytes);
        return newHash;        
    }


    static string ComputeSha256Hash(byte[] rawData)
    {
        using var sha256Hash = SHA256.Create();
        byte[]? bytes = sha256Hash.ComputeHash(rawData);

        StringBuilder? builder = new();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }

    public async Task<bool> IsBlockValid(long id)
    {
        if (id>1)
        {
            var blocks =await _context.Block.Where(x => x.Id==id || x.Id==id-1).ToListAsync();
            var prevBlock = blocks[0]; 
            var block = blocks[1];

            if (prevBlock.Hash256!=block.PreviousHash256 || !block.PreviousHash256.StartsWith(new String('0', difficult)))
            {
                return false;
            }

            var hash = CalculateHash(block);
            if (!hash.StartsWith(new String('0', difficult)))
            {
                return false;
            }

            return true;
        } else
        {
            var block = await _context.Block.Where(x => x.Id == id).FirstAsync();

            if (block.PreviousHash256 != new String('0', 64))
            {
                return false;
            }

            var hash = CalculateHash(block);
            if (!hash.StartsWith(new String('0', difficult)))
            {
                return false;
            }

            return true;
        }
    }

    public async Task<List<BlockDTO>> GetBlocks()
    {
        return await _context.Block.OrderByDescending(x => x.Id).Select(x => new BlockDTO()
        {
            CreationDateTime = x.CreationDateTime,
            Hash256 = x.Hash256,
            Id = x.Id,
            Name = x.Name,
            Nonce = x.Nonce,
            PersonId = x.PersonId,
            PreviousHash256 = x.PreviousHash256
        }).ToListAsync();
    }

    public async Task<BlockDTO?> GetBlock(int id)
    {
        var block = await _context.Block.FindAsync(id);

        if (block == null)
        {
            return null;
        }

        return new BlockDTO()
        {
            Id = block.Id,
            CreationDateTime = block.CreationDateTime,
            Hash256 = block.Hash256,
            PreviousHash256 = block.PreviousHash256,
            Name = block.Name,
            Nonce = block.Nonce,
            PersonId = block.PersonId
        };
    }

    public async Task<BlockDTO> AddBlock(CreateBlockViewModel block)
    {
        Block newBlock = new()
        {
            Name = block.Name ?? "",
            Image = Encoding.ASCII.GetBytes(block.Image ?? ""),
            PersonId = block.PersonId ?? 0
        };

        _context.Block.Add(newBlock);

        await _context.SaveChangesAsync();

        var newBlockDTO = new BlockDTO()
        {
            Id = newBlock.Id,
            CreationDateTime = newBlock.CreationDateTime,
            Hash256 = newBlock.Hash256,
            PreviousHash256 = newBlock.PreviousHash256,
            Name = block.Name ?? "",
            Nonce = newBlock.Nonce,
            PersonId = newBlock.PersonId
        };

        return newBlockDTO;        
    }

    public async Task<(byte[], string)> GetImage(int id)
    {
        var block = await _context.Block.FindAsync(id);
        if (block != null)
        {
            string initial = Encoding.ASCII.GetString(block.Image, 0, 30);
            string mime = initial[5..initial.IndexOf(";")];

            string base64 = Encoding.ASCII.GetString(block.Image, 0, block.Image.Length)[(initial.IndexOf(";")+8)..];
            return (Convert.FromBase64String(base64), mime);
        }
        else
        {
            return (Array.Empty<byte>(), string.Empty);   
        }
    }
}
