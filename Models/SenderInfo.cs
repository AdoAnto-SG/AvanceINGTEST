namespace Integrador.Models;

public sealed class SenderInfo
{
	public string Name { get; set; } = string.Empty;
	public string? TradeName { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string NIT { get; set; } = string.Empty;
	public string NRC { get; set; } = string.Empty;
	public string AddressComplement { get; set; } = string.Empty;
	public string Municipality { get; set; } = string.Empty;
	public string Department { get; set; } = string.Empty;
	public string EconomicActivity { get; set; } = string.Empty;
	public string EconomicActivityCode { get; set; } = string.Empty;
	public string? EstablishmentCode { get; set; } 
	public string? PointOfSaleCode { get; set; } 
	public string? MHEstablishmentCode { get; set; }
	public string? MHPointOfSaleCode { get; set; }
}
