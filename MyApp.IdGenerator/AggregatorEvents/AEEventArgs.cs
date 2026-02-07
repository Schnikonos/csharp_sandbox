using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.IdGenerator.AggregatorEvents
{
    public record AEEventArgs(string Message) : INotification;

}
