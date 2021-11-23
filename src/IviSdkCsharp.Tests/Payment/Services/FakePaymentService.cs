using Grpc.Core;
using Ivi.Proto.Api.Payment;
using Ivi.Rpc.Api.Payment;
using System.Threading.Tasks;

namespace IviSdkCsharp.Tests.Payment.Services;

public class FakePaymentService : PaymentService.PaymentServiceBase
{
    private static FakePaymentService mock;

    public static void UseMock(FakePaymentService mock)
        => FakePaymentService.mock = mock;

    public override Task<PaymentMethodResponse> CreatePaymentMethod(CreatePaymentMethodRequest request, ServerCallContext context)
        => mock.CreatePaymentMethod(request, context);

    public override Task<DeletePaymentMethodResponse> DeletePaymentMethod(DeletePaymentMethodRequest request, ServerCallContext context)
        => mock.DeletePaymentMethod(request, context);

    public override Task<Token> GenerateClientToken(CreateTokenRequest request, ServerCallContext context)
        => mock.GenerateClientToken(request, context);

    public override Task<GetPaymentMethodResponse> GetPaymentMethods(GetPaymentMethodRequest request, ServerCallContext context)
        => mock.GetPaymentMethods(request, context);

    public override Task<PaymentMethodResponse> UpdatePaymentMethod(UpdatePaymentMethodRequest request, ServerCallContext context)
        => mock.UpdatePaymentMethod(request, context);
}