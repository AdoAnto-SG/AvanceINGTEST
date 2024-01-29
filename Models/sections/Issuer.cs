using Newtonsoft.Json;

namespace Integrador.Models.sections;

public class Issuer
{
	[JsonProperty("nit")]
	public string? Nit { get; set; }

	[JsonProperty("nrc")]
	public string? Nrc { get; set; }

	[JsonProperty("name")]
	public string? Name { get; set; }

	[JsonProperty("economic_activity")]
	public string? EconomicActivity { get; set; }

	[JsonProperty("phone")]
	public string? Phone { get; set; }

	[JsonProperty("email")]
	public string? Email { get; set; }

	[JsonProperty("country_id")]
	public int? CountryId { get; set; }

	[JsonProperty("department")]
	public string? Department { get; set; }

	[JsonProperty("municipality")]
	public string? Municipality { get; set; }

	[JsonProperty("address")]
	public string? Address { get; set; }
}