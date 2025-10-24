using System.Security.Cryptography;
using System.Text;

namespace BSC.API.utils
{
	public class Utils
	{
		public static string ComputeHash(string input)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
			return Convert.ToBase64String(bytes);
		}
	}
}
