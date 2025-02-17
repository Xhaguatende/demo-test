// ----------------------------------------------------------------------------
//  <copyright file="TelemetryFilteringProcessor.cs" company="ZigZag Global">
//    Copyright (c) ZigZag Global. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------------

namespace ZZ.Circle.Gateway.Extensions;

using System.Diagnostics;
using System.Text.RegularExpressions;
using Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry;

public class TelemetryFilteringProcessor : BaseProcessor<Activity>
{
    private readonly Observability _observabilityConfiguration;

    public TelemetryFilteringProcessor(IOptions<Observability> observability)
    {
        _observabilityConfiguration = observability.Value;
    }

    public override void OnEnd(Activity activity)
    {
        if (IsFilteredEndpoint(activity.DisplayName))
        {
            activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
        }
    }

    private bool IsFilteredEndpoint(string displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            return false;
        }

        return _observabilityConfiguration.IgnoreRequestsForTelemetryMatching.Exists(
            x => Regex.IsMatch(
                displayName,
                x,
                RegexOptions.None,
                TimeSpan.FromMilliseconds(150)));
    }
}