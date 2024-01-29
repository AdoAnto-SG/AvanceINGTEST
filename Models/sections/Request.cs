using FindexMapper.Core.Entities;
using Newtonsoft.Json;

namespace Integrador.Models.sections;

public class Request
{
	[JsonProperty("issuer")]
	public Issuer? Issuer { get; set; }

	[JsonProperty("receiver")]
	public Receiver? Receiver { get; set; }

	[JsonProperty("description")]
	public string? Description { get; set; }

	[JsonProperty("way_to_pay")]
	public string? WayToPay { get; set; }

	[JsonProperty("payment_method")]
	public string? PaymentMethod { get; set; }

	[JsonProperty("payment_date")]
	public string? PaymentDate { get; set; }

	[JsonProperty("time_quantity")]
	public int? TimeQuantity { get; set; }

	[JsonProperty("reference")]
	public string? Reference { get; set; }

	[JsonProperty("status")]
	public string? Status { get; set; }

	[JsonProperty("item")]
	public object? Item { get; set; }

	[JsonProperty("unit_of_measurement")]
	public string? UnitOfMeasurement { get; set; }

	[JsonProperty("remission_type")]
	public object? RemissionType { get; set; }

	[JsonProperty("billing_place")]
	public string? BillingPlace { get; set; }

	[JsonProperty("cancellation_reason")]
	public decimal? CancellationReason { get; set; }

	[JsonProperty("applied_taxes")]
	public string? AppliedTaxes { get; set; }

	[JsonProperty("arrears")]
	public object? Arrears { get; set; }

	[JsonProperty("totalIva")]
	public decimal TotalIva { get; set; }

	[JsonProperty("totalunitario")]
	public decimal Totalunitario { get; set; }

	[JsonProperty("total")]
	public decimal Total { get; set; }

    [JsonProperty("quantity")]
    public decimal Quantity { get; set; }
}