namespace CurrencyConverter.Data;

using System.Text.Json;

public class ExchangeRateService
{
	public readonly List<string> Currencies = new List<string>() { 
											"EUR",
											"USD",
											"BRL",
											"CAD",
											"JPY",
											"GBP",
											"CHF",
											"NZD",
											"CNY",
											"HKD",
											"AUD" };

	private readonly string[] Symbols = { 	"€­­­­­ ­ ­­­", 
											"$­ ­ ­­­",
											"R$­ ­", 
											"CA$", 
											"¥­ ­ ­­­", 
											"£­ ­ ­­­", 
											"₣­ ­ ­­­", 
											"NZ$", 
											"CN¥", 
											"HK$",
											"A$­ ­"};
	private const string BASE_URL = "https://api.exchangerate.host/convert";
	public async Task<Rate[]?> GetRates(string from, double amount)
	{
		if(!Currencies.Contains(from))
			return null;

		using var httpClient = new HttpClient();
		var rates = new List<Rate>();

		foreach(var currency in Currencies )
		{
			if(from == currency)
				continue;
			
			using var request = new HttpRequestMessage(new HttpMethod("GET"), $"{BASE_URL}?from={from}&to={currency}");
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var obj = JsonSerializer.Deserialize<ExchangeRateResponse>(content);

			var symbolIndex = Currencies.IndexOf(currency);
			rates.Add(new Rate
			{
				Code = currency,
				Symbol = Symbols[symbolIndex],
				Value = obj.info.rate,
				Total = Math.Round(amount * obj.info.rate, 6)
			});
		}
		
		return rates.ToArray();
	}
}