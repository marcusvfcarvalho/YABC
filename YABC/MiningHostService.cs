using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;
using YABC;
using YABC.Data;
using YABC.DTO;
using YABC.Services;

public class MiningHostService : IHostedService, IDisposable
{

   private readonly YABCContext? _context;

    private IServiceScopeFactory _scopedFactory;

    private IServiceScope? _scope;

    private readonly ILogger<MiningHostService> _logger;

    private readonly IHubContext<BlockHub>? _hubContext;
    private readonly IBlockService? _blockService;

    public MiningHostService(IServiceScopeFactory scopedFactory, ILogger<MiningHostService> logger)
    {
        _scopedFactory = scopedFactory;
        _logger = logger;

        _scope = _scopedFactory.CreateScope();
        _context = _scope.ServiceProvider.GetService<YABCContext>();
        _hubContext = _scope.ServiceProvider.GetService<IHubContext<BlockHub>>();
        _blockService = _scope.ServiceProvider.GetService<IBlockService>(); ;
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            do
            {
                YABC.Models.Block? block = _context?.Block.Where(x => x.Hash256 == String.Empty)
                .OrderBy(x => x.Id)
                .FirstOrDefault();

                if (block == null)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    if (block.Id == 1)
                    {
                        block.PreviousHash256 = new string('0', 64);
                    }
                    else
                    {
                        var previousBlock = _context?.Block.Where(x => x.Id < block.Id && x.Hash256 != String.Empty).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (previousBlock != null)
                        {
                            block.PreviousHash256 = previousBlock.Hash256;
                        }
                    }

                    int numberOfThreads = 10;
                    int latestGroup = 1;
                    do
                    {
                        List<int> nonces = new();
                        for (int nonce = latestGroup; nonce < latestGroup + numberOfThreads; nonce++)
                        {
                            nonces.Add(nonce);
                        }
                        latestGroup += numberOfThreads;

                        _ = Parallel.ForEach(nonces, async n =>
                          {
                              block.Nonce = n;
                              var newHash = _blockService?.CalculateHash(block) ?? "";
                              if (newHash.StartsWith("0000"))
                              {
                                  block.Hash256 = newHash;
                                  block.Nonce = n;
                                  _logger.LogInformation("Found Hash for Block {blockId}", block.Id);
                                  await _hubContext.Clients.All.SendAsync("ReceiveNotification", new NotificationMessage()
                                  {
                                      MessageType = MessageType.Mined,
                                      Block = new BlockDTO()
                                      {
                                          Id = block.Id,
                                          CreationDateTime = block.CreationDateTime,
                                          Hash256 = block.Hash256,
                                          PreviousHash256 = block.PreviousHash256,
                                          Name = block.Name,
                                          Nonce = block.Nonce,
                                          PersonId = block.PersonId
                                      }
                                  });
                              }
                          });
                        if (block.Hash256 != String.Empty)
                        {
                            break;
                        }

                    } while (true);

                    _context?.SaveChanges();

                }
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            } while (true);
        },cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}