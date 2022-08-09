// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Infrastructure.Middleware
{
    public class LogMiddleware<TEvent> : IMiddleware<TEvent>
        where TEvent : notnull, IEvent
    {
        private readonly ILogger<LogMiddleware<TEvent>> _logger;

        public LogMiddleware(ILogger<LogMiddleware<TEvent>> logger)
        {
            _logger = logger;
        }

        public bool SupportRecursive => true;

        public async Task HandleAsync(TEvent action, EventHandlerDelegate next)
        {
            var typeName = action.GetType().FullName;

            _logger.LogInformation("----- command {CommandType}", typeName);

            await next();
        }
    }
}