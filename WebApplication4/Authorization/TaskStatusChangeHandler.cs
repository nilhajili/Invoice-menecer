using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Authorization;

public class TaskStatusChangeHandler
    : AuthorizationHandler<TaskStatusChangeRequirement, Invoice>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaskStatusChangeRequirement requirement,
        Invoice invoice)
    {
        if (context.User == null || invoice == null)
            return Task.CompletedTask;


        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Task.CompletedTask;

  
        if (invoice.CreatedByUserId.ToString() == userId)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        return Task.CompletedTask;
    }
}