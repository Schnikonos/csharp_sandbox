using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.AggregatorEvents
{
    public class AEEventSender
    {
        private readonly IMediator _mediator;

        public AEEventSender(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendEvent(string Message)
        {
            await _mediator.Publish(new AEEventArgs(Message));
        }
    }
}
