using Stripe.Checkout;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class StripeSessionService
{
    private readonly IConfiguration _configuration;

    public StripeSessionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Session CreateSession(
        int referenceId, // OrderHeaderId أو TicketId
        List<(string Name, decimal Price, int count)> items,
        string successPath,
        string cancelPath)
    {
        var domain = _configuration["Stripe:Domain"];

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = $"{domain}{successPath}?Id={referenceId}",
            CancelUrl = $"{domain}{cancelPath}",
        };

        foreach (var item in items)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name
                    }
                },
                Quantity = item.count
            });
        }

        var service = new SessionService();
        return service.Create(options);
    }
}
