using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zadanie1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //1. Gdy argument nie został przekazany
            if (args.Length == 0)
            {
                throw new ArgumentNullException();
            }
            
            //2. Gdy argument, który nie jest poprawnym adresem URL
            if (!Uri.IsWellFormedUriString(args[0], UriKind.Absolute))
            {
                throw new ArgumentException("Niepoprawny URL");
            }
            
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync(args[0]);
            
            //3. Gdy podczas pobierania strony wystąpi błąd
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Błąd w czasie pobierania strony");
            }
            
            //5. Gdy zostały znalezione adresy email, powinny zostać wyświetlone na konsoli
            var content = result.Content;
            var text = content.ReadAsStringAsync().Result;
            var regex = new Regex(@"(?<=\s|^)([a-z0-9-.])+@+([a-z0-9\.]*)");
            
            var matchedText = regex.Matches(text);
            var distinctData = matchedText.Distinct();
            
            foreach (var st in distinctData)
            {
                Console.WriteLine(st);
            }

            //4. Gdy nie znaleziono żadnych adresów email
            if (matchedText.Count == 0)
            {
                throw new Exception("Nie znaleziono adresów email");
            }

        }
    }
}
