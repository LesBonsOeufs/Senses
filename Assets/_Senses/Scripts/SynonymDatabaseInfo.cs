using AYellowpaper.SerializedCollections;
using System.Linq;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu]
    public class SynonymDatabaseInfo : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, string[]> synonyms;

        public bool ContainsSynonymOf(string inputText, string synonymKey)
        {
            string[] lTokenized = inputText.Split(new char[] { ' ', '.', '?' });
            string[] lSynonyms = synonyms[synonymKey];
            return lTokenized.Any(word => string.Equals(word, synonymKey, System.StringComparison.OrdinalIgnoreCase) || 
            synonyms[synonymKey].Any(synonym => string.Equals(synonym, word, System.StringComparison.OrdinalIgnoreCase)));
        }
    }
}