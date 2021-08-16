using System;
using System.Threading.Tasks;
using Cards.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Cards.Grpc
{
    public class CardsInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (ArgumentException argumentException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, argumentException.ToString()));
            }
            catch (MongoUnavailableException mongoUnavailableException)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, mongoUnavailableException.ToString()));
            }
            catch (Exception exception)
            {
                throw new RpcException(new Status(StatusCode.Unknown, exception.ToString()));
            }
        }
    }
}