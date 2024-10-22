using System.Diagnostics.CodeAnalysis;

namespace Orders.Api.Events;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
// ReSharper disable InconsistentNaming
public sealed record ExternalPaymentApproved(string tid, DateTime approved_at);
// ReSharper restore InconsistentNaming
