using Newtonsoft.Json;

namespace Integrador.Models.sections;

public class Receiver
{
	[JsonProperty("document_type")]
	public int? DocumentType { get; set; }

	[JsonProperty("document")]
	public string? Document { get; set; }

	[JsonProperty("fullname")]
	public string? Fullname { get; set; }

	[JsonProperty("economic_activity")]
	public string? EconomicActivity { get; set; }

	[JsonProperty("country_id")]
	public int? CountryId { get; set; }

	[JsonProperty("department")]
	public string? Department { get; set; }

	[JsonProperty("municipality")]
	public string? Municipality { get; set; }

	[JsonProperty("address")]
	public string? Address { get; set; }

	[JsonProperty("phone")]
	public string? Phone { get; set; }

	[JsonProperty("fiscal_phone")]
	public string? FiscalPhone { get; set; }

	[JsonProperty("email")]
	public string? Email { get; set; }

	[JsonProperty("fiscal_email")]
	public string? FiscalEmail { get; set; }
}