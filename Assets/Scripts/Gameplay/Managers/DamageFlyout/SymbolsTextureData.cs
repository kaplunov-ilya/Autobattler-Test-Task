using System.Collections.Generic;
using UnityEngine;

namespace UI.Gameplay.Views.DamageFlyout
{
    public sealed class SymbolsTextureData
    {
        private const int LengthLine = 10;

        private readonly Dictionary<char, Vector2> _charsDict;
        
        public SymbolsTextureData()
        {
            var chars = new List<char>()
            {
                ' ', '!', '*',',','-','.','/',':',';','<','=','>','?',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 
                'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z',
                '[','\\',']','^','_','$','+',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            };
            
            _charsDict = new Dictionary<char, Vector2>();
            for (var i = 0; i < chars.Count; i++)
            {
                var c = char.ToLowerInvariant(chars[i]);
                if (_charsDict.ContainsKey(c)) continue;
                
                var uv = new Vector2(i % LengthLine, (LengthLine - 1) - i / LengthLine);
                _charsDict.Add(c, uv);
            }
        }

        public Vector2 GetTextTextureCoordinates(char c)
        {
            c = char.ToLowerInvariant(c);
            return _charsDict!.TryGetValue(c, out var texCord) ? texCord : Vector2.zero;
        }
    }
}