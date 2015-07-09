using System.Linq;
namespace Core
{
    public static class Helper
    {
        public static string RemoveIrrelevantTerms(this string text)
        {
            var terms = new string[]
            {
                "que","da","do","de","na","no","o","ó","a","à","á","e","é","i","í","u","ú","em","dia"           
            };

            return string.Join(" ", text.Split(' ').Except(terms));
        }
    }
}
