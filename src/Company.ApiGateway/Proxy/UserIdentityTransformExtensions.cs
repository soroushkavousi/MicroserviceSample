using System.Globalization;
using Company.Shared.Extensions;
using Company.Shared.Identity;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Company.ApiGateway.Proxy;

public static class UserIdentityTransformExtensions
{
    public static TransformBuilderContext AddUserIdentityHeader(this TransformBuilderContext builder)
    {
        builder.AddRequestTransform(context =>
        {
            context.ForwardAuthenticatedUserId();
            return ValueTask.CompletedTask;
        });
        return builder;
    }

    public static void ForwardAuthenticatedUserId(this RequestTransformContext context)
    {
        context.ProxyRequest.Headers.Remove(UserIdentityHeaders.UserId);

        if (!context.HttpContext.User.TryGetUserId(out long userId))
            return;

        context.ProxyRequest.Headers.TryAddWithoutValidation(
            UserIdentityHeaders.UserId,
            userId.ToString(CultureInfo.InvariantCulture));
    }
}