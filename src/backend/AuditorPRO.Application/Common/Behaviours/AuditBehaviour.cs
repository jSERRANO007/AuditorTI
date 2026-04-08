using AuditorPRO.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuditorPRO.Application.Common.Behaviours;

public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuditBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;

    public AuditBehaviour(ILogger<AuditBehaviour<TRequest, TResponse>> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("AuditorPRO Request: {Name} by {UserId} ({Email})",
            requestName, _currentUser.UserId, _currentUser.Email);

        var response = await next();

        _logger.LogInformation("AuditorPRO Request Completed: {Name}", requestName);
        return response;
    }
}
