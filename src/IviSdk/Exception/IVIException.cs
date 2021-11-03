using Grpc.Core;

namespace Mythical.Game.IviSdkCSharp.Exception
{
    public class IVIException : System.Exception
    {
        public static readonly int UNPROCESSABLE_ENTITY = 422;
        public static readonly string HTTP_CODE_KEY = "HttpCode";

        public IVIException(IVIErrorCode code) : base()
        {
            this.Code = code;
        }

        public IVIException(string message, IVIErrorCode code) : base(message)
        {
            this.Code = code;
        }

        public IVIException(string message, System.Exception cause, IVIErrorCode code) : base(message, cause)
        {
            this.Code = code;
        }

        public IVIException(System.Exception ex, IVIErrorCode code)// : base(ex)
        {
            this.Code = code;
        }

        public IVIException(string message, System.Exception cause, bool enableSuppression, bool writableStackTrace, IVIErrorCode code)// : base(message, cause, enableSuppression, writableStackTrace)
        {
            this.Code = code;
        }

        public virtual IVIErrorCode GetCode()
        {
            return Code;
        }

        public IVIErrorCode Code { get; }

        public static IVIException FromGrpcException(RpcException exception)
        {
            return BuildIviException(exception.StatusCode, exception.Trailers, exception.Message);
        }
        
        private static IVIException BuildIviException(StatusCode statusCode, Grpc.Core.Metadata metadata, string message)
        {
        
            // GRPC Status doesn't handle all http codes, so check if one was added
            if (metadata != null)
            {
                // var httpCodeString = metadata[Metadata.Key];
                // if (string.IsNullOrWhiteSpace(httpCodeString))
                // {
                //     var iviErrorCode = FromStatusCode(int.Parse(httpCodeString));
                //     return LogError(message, iviErrorCode);
                // }
            }
        
            switch (statusCode)
            {
                case StatusCode.InvalidArgument:
                    return LogError(message, IVIErrorCode.INVALID_ARGUMENT);
                case StatusCode.NotFound:
                    return LogError(message, IVIErrorCode.NOT_FOUND);
                case StatusCode.PermissionDenied:
                    return LogError(message, IVIErrorCode.PERMISSION_DENIED);
                case StatusCode.Unimplemented:
                    return LogError(message, IVIErrorCode.UNIMPLEMENTED);
                case StatusCode.Unauthenticated:
                    return LogError(message, IVIErrorCode.UNAUTHENTICATED);
                case StatusCode.Unavailable:
                    return LogError(message, IVIErrorCode.UNAVAILABLE);
                case StatusCode.ResourceExhausted:
                    return LogError(message, IVIErrorCode.RESOURCE_EXHAUSTED);
                case StatusCode.Aborted:
                    return LogError(message, IVIErrorCode.ABORTED);
                case StatusCode.DeadlineExceeded:
                case StatusCode.FailedPrecondition:
                case StatusCode.OutOfRange:
                    return LogError(message, IVIErrorCode.BAD_REQUEST);
                case StatusCode.AlreadyExists:
                    return LogError(message, IVIErrorCode.CONFLICT);
                case StatusCode.DataLoss:
                case StatusCode.Internal:
                case StatusCode.Unknown:
                    return LogError(message, IVIErrorCode.SERVER_ERROR);
                default:
                    return LogError(message, IVIErrorCode.UNKNOWN_GRPC_ERROR);
            }
        }

        private static IVIException LogError(string message, IVIErrorCode code)
        {
            //_logger.l("gRPC error from IVI server: code: {} message: {}", code, message);

            return new IVIException(message, code);
        }

        private static IVIErrorCode FromHttpStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                // case HttpURLConnection.HTTP_BAD_REQUEST:
                //     return IVIErrorCode.BAD_REQUEST;
                // case HttpURLConnection.HTTP_UNAUTHORIZED:
                //     return IVIErrorCode.NOT_AUTHORIZED;
                // case HttpURLConnection.HTTP_FORBIDDEN:
                //     return IVIErrorCode.FORBIDDEN;
                // case HttpURLConnection.HTTP_NOT_FOUND:
                //     return IVIErrorCode.NOT_FOUND;
                // case HttpURLConnection.HTTP_CONFLICT:
                //     return IVIErrorCode.CONFLICT;
                // case HttpURLConnection.HTTP_CLIENT_TIMEOUT:
                //     return IVIErrorCode.TIMEOUT;
                // case UNPROCESSABLE_ENTITY:
                //     return IVIErrorCode.UNPROCESSABLE_ENTITY;
                default:
                    return IVIErrorCode.SERVER_ERROR;
            }
        }
    }
}