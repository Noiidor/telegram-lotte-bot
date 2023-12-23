using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telegram_lotte_bot.DTO.Telegram;
using telegram_lotte_bot.Logic;

namespace telegram_lotte_bot.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ILogger _logger;
        private readonly UpdateHandler _handler;
        private readonly ICommandService _commandService;

        public UpdateService(ILogger logger, UpdateHandler handler, CommandService commandService)
        {
            _logger = logger;
            _handler = handler;
            _commandService = commandService;
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
