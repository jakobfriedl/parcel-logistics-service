using FH.ParcelLogistics.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace FH.ParcelLogistics.DataAccess.Sql;

public class WebhookRepository : IWebhookRepository
{
    private readonly DbContext _context;
    private readonly ILogger<IWebhookRepository> _logger;

    public WebhookRepository(DbContext context, ILogger<IWebhookRepository> logger){
        _context = context;
        _logger = logger;
    }
}