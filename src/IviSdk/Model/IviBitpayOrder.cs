using System.Collections.Generic;
namespace Mythical.Game.IviSdkCSharp.Model;

public class IviBitpayOrder : IIviPaymentProviderOrder
{
    public IviBitpayOrder()
    {
    }

    public Dictionary<string, object>? Invoice { get; set; }
}