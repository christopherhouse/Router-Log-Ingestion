using System;
using Microsoft.Extensions.Logging;

namespace RouterLogIngestion.Functions;

public class GeocodeIpAddressActivity
{
    private readonly ILogger<GeocodeIpAddressActivity> _logger;

    public GeocodeIpAddressActivity(ILogger<GeocodeIpAddressActivity> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


}