using Newtonsoft.Json.Linq;
using Raven.Client;
using System.Linq;

namespace Foogenda.Framework
{
	public static class DocumentSessionExtensions
	{
		public static IQueryable<T> Filter<T>(this IDocumentSession source, string json)
		{
			var query = Parse(json);
			if (string.IsNullOrEmpty(query))
				return source.Query<T>();
			else
				return source.Advanced.LuceneQuery<T>().Where(query).AsQueryable();
		}

		private static string Parse(string json)
		{
			if (string.IsNullOrEmpty(json))
				return json;

			var jObject = JObject.Parse(json);
			var result = string.Empty;
			var and = string.Empty;
			foreach (var pair in jObject)
			{
				result += and;
				result += pair.Key.First().ToString().ToUpper() + pair.Key.Substring(1);
				result += ":";
				result += pair.Value.ToString();
				and = " AND ";
			}

			return result;
		}
	}
}
