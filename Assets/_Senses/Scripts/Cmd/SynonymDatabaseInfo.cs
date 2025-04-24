using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu]
    public class SynonymDatabaseInfo : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, string[]> synonyms;

        public bool IsSynonymOf(string inputText, string synonymKey)
        {
            string[] lTokenized = inputText.Split(new char[] { ' ', '.', '?' });
            synonyms.TryGetValue(synonymKey, out string[] lSynonyms);

            return lTokenized.Any(word => string.Equals(word, synonymKey, System.StringComparison.OrdinalIgnoreCase) ||
                                  (lSynonyms != null && 
                                  lSynonyms.Any(synonym => string.Equals(synonym, word, System.StringComparison.OrdinalIgnoreCase))));
        }
    }
}