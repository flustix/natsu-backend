using Natsu.Backend.Models;

namespace Natsu.Backend.Utils;

public class SearchFilter
{
    private List<string> terms { get; } = new();

    public SearchFilter(string query)
    {
        var term = "";
        var inQuote = false;

        foreach (var c in query)
        {
            switch (c)
            {
                case ' ' when !inQuote:
                    finishTerm();
                    break;

                case '"':
                    finishTerm();
                    inQuote = !inQuote;
                    break;

                default:
                    term += c;
                    break;
            }
        }

        terms.Add(term);
        terms.RemoveAll(string.IsNullOrWhiteSpace);

        void finishTerm()
        {
            terms.Add(term);
            term = "";
        }
    }

    public bool Match(TaggedFile file)
    {
        return terms.All(w =>
        {
            var word = w;
            var invert = false;

            if (word.StartsWith('!'))
            {
                word = word[1..];
                invert = true;
            }

            return file.Description.Contains(word) == !invert;
        });
    }
}
