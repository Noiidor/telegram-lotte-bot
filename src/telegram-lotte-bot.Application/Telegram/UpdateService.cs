using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.Application.Interfaces;
using telegram_lotte_bot.Domain.Telegram;

namespace telegram_lotte_bot.Application.Telegram
{
    public class UpdateService : IUpdateService
    {
        private readonly ILogger<UpdateService> _logger;
        private readonly IUpdateHandler _handler;
        private readonly ICommandService _commandService;

        public UpdateService(ILogger<UpdateService> logger, IUpdateHandler handler, ICommandService commandService)
        {
            _logger = logger;
            _commandService = commandService;
            _handler = handler;
        }

        public async Task StartReceivingUpdates(CancellationToken cancellationToken)
        {
            long offset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                List<MessageUpdate> updates = await _handler.GetUpdates(offset);

                updates.ForEach(e => offset = e.Id + 1);

                await _commandService.CheckUpdates(updates);
            }
        }
    }
}
