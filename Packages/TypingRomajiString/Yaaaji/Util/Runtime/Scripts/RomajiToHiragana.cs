using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Yaaaji.Util
{
	public static class RomajiToHiragana
	{
		private static readonly Dictionary<string, string> romajiToHiraganaTable = new Dictionary<string, string>
		{
			{ "a", "あ" }, { "i", "い" }, { "u", "う" }, { "e", "え" }, { "o", "お" },
			{ "ba", "ば" }, { "bi", "び" }, { "bu", "ぶ" }, { "be", "べ" }, { "bo", "ぼ" },
			{ "ca", "か" }, { "ci", "し" }, { "cu", "く" }, { "ce", "せ" }, { "co", "こ" },
			{ "da", "だ" }, { "di", "ぢ" }, { "du", "づ" }, { "de", "で" }, { "do", "ど" },
			{ "fa", "ふぁ" }, { "fi", "ふぃ" }, { "fu", "ふ" }, { "fe", "ふぇ" }, { "fo", "ふぉ" },
			{ "ga", "が" }, { "gi", "ぎ" }, { "gu", "ぐ" }, { "ge", "げ" }, { "go", "ご" },
			{ "ha", "は" }, { "hi", "ひ" }, { "hu", "ふ" }, { "he", "へ" }, { "ho", "ほ" },
			{ "ja", "じゃ" }, { "ji", "じ" }, { "ju", "じゅ" }, { "je", "じぇ" }, { "jo", "じょ" },
			{ "ka", "か" }, { "ki", "き" }, { "ku", "く" }, { "ke", "け" }, { "ko", "こ" },
			{ "la", "ぁ" }, { "li", "ぃ" }, { "lu", "ぅ" }, { "le", "ぇ" }, { "lo", "ぉ" },
			{ "ma", "ま" }, { "mi", "み" }, { "mu", "む" }, { "me", "め" }, { "mo", "も" },
			{ "na", "な" }, { "ni", "に" }, { "nu", "ぬ" }, { "ne", "ね" }, { "no", "の" },
			{ "pa", "ぱ" }, { "pi", "ぴ" }, { "pu", "ぷ" }, { "pe", "ぺ" }, { "po", "ぽ" },
			{ "qa", "くぁ" }, { "qi", "くぃ" }, { "qu", "く" }, { "qe", "くぇ" }, { "qo", "くぉ" },
			{ "ra", "ら" }, { "ri", "り" }, { "ru", "る" }, { "re", "れ" }, { "ro", "ろ" },
			{ "sa", "さ" }, { "si", "し" }, { "su", "す" }, { "se", "せ" }, { "so", "そ" },
			{ "ta", "た" }, { "ti", "ち" }, { "tu", "つ" }, { "te", "て" }, { "to", "と" },
			{ "va", "ゔぁ" }, { "vi", "ゔぃ" }, { "vu", "ゔ" }, { "ve", "ゔぇ" }, { "vo", "ゔぉ" },
			{ "wa", "わ" }, { "wi", "うぃ" }, { "wu", "う" }, { "we", "うぇ" }, { "wo", "を" },
			{ "xa", "ぁ" }, { "xi", "ぃ" }, { "xu", "ぅ" }, { "xe", "ぇ" }, { "xo", "ぉ" },
			{ "ya", "や" }, { "yi", "い" }, { "yu", "ゆ" }, { "ye", "いぇ" }, { "yo", "よ" },
			{ "za", "ざ" }, { "zi", "じ" }, { "zu", "ず" }, { "ze", "ぜ" }, { "zo", "ぞ" },
			{ "nn", "ん" },{ "xn", "ん" },
			{ "-", "ー" },

			{ "kya", "きゃ" }, { "kyi","きぃ" }, { "kyu", "きゅ" },  {"kye","きぇ" }, { "kyo", "きょ" },
			{ "gya", "ぎゃ" }, { "gyi", "ぎぃ" }, { "gyu", "ぎゅ" }, { "gye", "ぎぇ" }, { "gyo", "ぎょ" },
			{ "sha", "しゃ" }, { "shi", "し"  }, { "shu", "しゅ" },  {"she","しぇ"}, { "sho", "しょ" },
			{ "sya", "しゃ" }, { "syi", "しぃ" }, { "syu", "しゅ" }, {"sye","しぇ"}, { "syo", "しょ" },
			{ "dha", "でゃ" }, { "dhi", "でぃ" }, { "dhu", "でゅ" }, { "dhe", "でぇ" }, { "dho", "でょ" },
			{ "cha", "ちゃ" }, { "chi", "ち" }, { "chu", "ちゅ" }, { "che", "ちぇ" }, { "cho", "ちょ" },
			{ "tsa", "つぁ" }, { "tsi", "つぃ" }, { "tsu", "つ" }, { "tse", "つぇ" }, { "tso", "つぉ" },
			{ "tya", "ちゃ" }, { "tyi", "ちぃ" }, { "tyu", "ちゅ" },{ "tye", "ちぇ" },  { "tyo", "ちょ" },
			{ "nya", "にゃ" }, { "nyi", "にぃ" }, { "nyu", "にゅ" }, { "nye", "にぇ" }, { "nyo", "にょ" },
			{ "hya", "ひゃ" }, { "hyi", "ひぃ" }, { "hyu", "ひゅ" }, { "hye", "ひぇ" }, { "hyo", "ひょ" },
			{ "mya", "みゃ" }, { "myi", "みぃ" }, { "myu", "みゅ" }, { "mye", "みぇ" }, { "myo", "みょ" },
			{ "rya", "りゃ" }, { "ryi", "りぃ" }, { "ryu", "りゅ" }, { "rye", "りぇ" }, { "ryo", "りょ" },
			{ "bya", "びゃ" }, { "byi", "びぃ" }, { "byu", "びゅ" }, { "bye", "びぇ" }, { "byo", "びょ" },
			{ "pya", "ぴゃ" }, { "pyi", "ぴぃ" }, { "pyu", "ぴゅ" }, { "pye", "ぴぇ" }, { "pyo", "ぴょ" },
			{ "lya", "ゃ" }, { "lyi", "ぃ" }, { "lyu", "ゅ" }, { "lye", "ぇ" }, { "lyo", "ょ" },
			{ "xya", "ゃ" }, { "xyi", "ぃ" }, { "xyu", "ゅ" }, { "xye", "ぇ" }, { "xyo", "ょ" },
			{ "ltu", "っ" }, { "xtu", "っ" }
		};

		private static readonly List<string> nRomajiList = new List<string>
		{
			"nn", "xn", 
		};

		private static Dictionary<string, List<string>> _hiraganaToRomajiTable = null;
		private static Dictionary<string, List<string>> hiraganaToRomajiTable => _hiraganaToRomajiTable??=createHiraganaToRomajiTable();
		
		private static Dictionary<string, List<string>> createHiraganaToRomajiTable()
		{
			Dictionary<string, List<string>> table = new Dictionary<string, List<string>>();
			foreach (var pair in romajiToHiraganaTable)
			{
				if (!table.ContainsKey(pair.Value))
				{
					table[pair.Value] = new List<string>();
				}
				table[pair.Value].Add(pair.Key);
			}
			if (!table.ContainsKey("ん"))
			{
				table["ん"] = new List<string>();
			}
			table["ん"].Add("n");
			return table;
		}

		static string cToRomaji(string c,bool useRandom = false){
			var result = "";
			hiraganaToRomajiTable.TryGetValue(c, out var list);
			if ( list != null && list.Count>0 )
			{
				if ( useRandom ){
					result = list[Random.Range(0,list.Count)];
				}
				else{
					result = list[0];
				}
			}
			return result;
		}

		static List<string> cToRomajiList(string c){
			var result = new List<string>();
			hiraganaToRomajiTable.TryGetValue(c, out var list);
			if ( list != null && list.Count>0 )
			{
				result = list;
			}
			return result;
		}

		/// <summary>
		///  入力された1文字のひらがなのローマ字候補を取得する.
		/// </summary>
		/// <param name="c"></param>
		/// <param name=""></param>
		/// <returns>テーブルの中身を返しているので編集してはいけない</returns>
		public static List<string> ToRomajiCandidate(this string c){
			hiraganaToRomajiTable.TryGetValue(c, out var list);
			if ( list != null && list.Count>0 )
			{
				return list;
			}
			return null;
		}

		public static string ToHiragana(this string input,bool isTruncate = false)
		{
			return ConvertRomajiToHiragana(input, isTruncate);
		}

		public static string ToRomaji(this string input, bool useRandom = false)
		{
			return ConvertHiraganaToRomaji(input, useRandom);
		}

		static List<string> combinationCombine(List<string> list1, List<string> list2) {
			var result = new List<string>();

			foreach (var s1 in list1) {
				foreach (var s2 in list2) {
					result.Add(s1 + s2);
				}
			}

			return result;
		}

		static IEnumerable<Tuple<string,List<string>>> convertHiraganaToRomaji(this string input)
		{
			int length = input.Length;
			bool prevN = false;
			for (int i = 0; i < length;)
			{
				// Handle double consonants by waiting for the next vowel
				var c = input[i];
				var cStr = c.ToString();
				bool isLast = (i + 1) >= length;
				
				if ( c == 'っ' )
				{
					// 最後の文字の場合はltu,xtuにする.
					if ( isLast )
					{
						yield return new("っ",cToRomajiList("っ"));
						i++;
						continue;
					}
					// 次の文字が子音の場合は、その子音を重ねる.
					else{
						var nc = input[i+1];
						if ( IsVowel(nc) || nc == 'っ' || IsNagyou(nc) ){
							yield return new("っ",cToRomajiList("っ"));
							i++;
							continue;
						}
						else
						{
							// 先頭の子音を重ねる.
							var nextRomajiList = cToRomajiList(nc.ToString());

							// 先頭1文字を重ねた文字列に変換する.
							var romajiList = nextRomajiList.Select(s => s.Substring(0, 1)+s).ToList();

							// っ○の組み合わせを作る.
							var comb = combinationCombine(cToRomajiList("っ"), nextRomajiList);

							romajiList.AddRange(comb);

							yield return new($"っ{nc}",romajiList);
							i+=2;
							continue;
						}
					}
				}
				else if ( c == 'ん' )
				{
					// 次が母音かNもしくは最後の文字の場合はnn,xnにする.
					if ( ((i+1) < length && (IsVowel(input[i+1]) || IsNagyou(input[i+1]) || IsYagyou(input[i+1]) ) )
						|| (i+1) >= length )
					{
						yield return new("ん",nRomajiList);
						i++;
						continue;
					}
					else {
						// 次の文字を頭文字を一文字重ね、次の文字は子音だけにする.
						yield return new("ん",cToRomajiList("ん"));
						i++;
						continue;
					}
				}
				else{
					var findRomaji = false;
					// 2文字,1文字の順にチェック.
					// 可能性を全て列挙する必要があるため、breakしない.
					var romajiComb = new List<string>();
					var nextI = i;
					//for (int j = 2; j > 0; j--)
					// 2文字組み合わせチェック.
					if ( i + 1 < length )
					{
						string sub = input.Substring(i, 2);
						var romajiList = cToRomajiList(sub);
						if ( romajiList.Count > 0 )
						{
							nextI = i+2;
							findRomaji = true;
							romajiComb.AddRange(romajiList);

							// 2文字組み合わせが見つかった時は、1文字ずつも調べて組み合わせる.
							var romajiList2 = cToRomajiList(sub.Substring(0, 1));
							var romajiList3 = cToRomajiList(sub.Substring(1, 1));
							var comb = combinationCombine(romajiList2, romajiList3);
							romajiComb.AddRange(comb);

							yield return new ( sub, romajiComb );
						}
					}

					// 2文字の組み合わせが見つからない場合1文字組み合わせチェック.
					if ( !findRomaji )
					{
						var romajiList = cToRomajiList(cStr);
						if ( romajiList.Count > 0 )
						{
							romajiComb.AddRange(romajiList);
							nextI = i+1;
							findRomaji = true;
							yield return new ( cStr, romajiComb );
						}
					}

					if ( !findRomaji )
					{
						// 変換できなかった場合は、そのまま追加.
						yield return new ( c.ToString(), new List<string>{ c.ToString() });
						nextI++;
					}
					else
					{
						// LINQで先頭1文字だけにして、重複を削除する.
						//romajiComb = romajiComb.Select(s => s.Substring(0, 1)).Distinct().ToList();
						//yield return new ( c.ToString(), romajiComb );
					}
					i = nextI;
				}
			}
		}

		public static IEnumerable<Tuple<string,List<string>>> ConvertHiraganaToRomaji(this string input)
		{
			foreach(var pair in convertHiraganaToRomaji(input))
			{
				var romajiList = pair.Item2;
				romajiList.Sort((a, b) => {
					if ( a[0] == 'c' && b[0] != 'c' ) return 1;
					return a.Length.CompareTo(b.Length);
				});
				yield return pair;
			}
		}

		public static string ConvertHiraganaToRomaji(this string input, bool useRandom = false){
			string result = "";
			foreach(var pair in ConvertHiraganaToRomaji(input))
			{
				result += pair.Item2[0];
			}
			return result;
			/*
			string nextRomaji = "";

			for (int i = 0; i < length;)
			{
				// Handle double consonants by waiting for the next vowel
				var c = input[i];
				if ( c == 'っ' )
				{
					// 最後の文字の場合はランダムでltu,xtuにする.
					if ( (i+1) >= length )
					{
						result += cToRomaji("っ",useRandom);
						i++;
						continue;
					}
					// 次の文字が子音の場合は、その子音を重ねる.
					else{
						var nc = input[i+1];
						if ( IsVowel(nc) || nc == 'っ' || IsNagyou(nc) ){
							result += cToRomaji("っ",useRandom);
							i++;
							continue;
						}
						else
						{
							// 先頭の子音を重ねる.
							nextRomaji = cToRomaji(nc.ToString(),useRandom);
							result += nextRomaji.Substring(0,1);
							i++;
							continue;
						}
					}
				}
				else if ( c == 'ん' )
				{
					// 次が母音かNもしくは最後の文字の場合はnnにする.
					if ( ((i+1) < length && (IsVowel(input[i+1]) || IsNagyou(input[i+1])||IsYagyou(input[i+1])))
						|| (i+1) >= length )
					{
						result += "nn";
						i++;
						continue;
					}

					result += "n";
					i++;
					continue;
				}
				else{
					var findRomaji = false;
					for (int j = 2; j > 0; j--) // Try matching 3-letter, then 2-letter, then 1-letter combinations
					{
						if (i + j <= length) // substringで1文字少なくなるので<=にする.
						{
							string sub = input.Substring(i, j);
							var romaji = cToRomaji(sub,useRandom);
							if ( !string.IsNullOrEmpty(romaji) )
							{
								result += romaji;
								i += j;
								findRomaji = true;
								break;
							}
						}
					}
					if ( !findRomaji ){
						result += c;
						i++;
					}
				}
			}
			return result;
			*/
		}

		public static string ConvertRomajiToHiragana(string input, bool isTruncate = false)
		{
			string result = "";
			int length = input.Length;

			for (int i = 0; i < length;)
			{
				string hiragana = null;

				// Handle double consonants by waiting for the next vowel
				if ( i + 1 < length && IsConsonant(input[i+1]) )
				{
					if ( input[i] == 'n' ){
						result += "ん";
						i++;
						continue;
					}
					else
					if ( input[i] == input[i + 1] )
					{
						if ( i + 2 < length )
						{
							result += "っ";
							i++;
							continue;
						}
					}
				}

				// 3文字先までチェック.
				for (int j = 3; j > 0; j--) // Try matching 3-letter, then 2-letter, then 1-letter combinations
				{
					if (i + j > length) continue;

					string sub = input.Substring(i, j).ToLower();
					if (romajiToHiraganaTable.TryGetValue(sub, out hiragana))
					{
						//Dev.Log($"<color=green>Find</color>[{i}-{i+j-1}]:{sub}->{hiragana}");
						result += hiragana;
						i += j;
						break;
					}
				}

				// 変換できなかった場合は、そのまま追加.
				if (hiragana == null)
				{
					if ( isTruncate )
					{
						//Dev.Log($"<color=red>Truncate</color>[{i}]:{input[i]}:{result} {input}");
						return result; // 切り詰め時は即座に返る.
					}
					// If no match found, append the current character as-is
					result += input[i];
					i++;
				}
			}
			return result;
		}

		private static bool IsYagyou(char c)
		{
			return "やゆよ".IndexOf(c) >= 0;
		}

		private static bool IsNagyou(char c)
		{
			return "なにぬねのん".IndexOf(c) >= 0;
		}

		private static bool IsVowel(char c)
		{
			return "あいうえお".IndexOf(c) >= 0;
		}

		private static bool IsConsonant(char c)
		{
			return "bcdfghjklmpqrstvwxyz".IndexOf(c) >= 0;
		}

		// ひらがなの文字列を元の文字列に変換するためのマップを作成
		static bool IsHiragana(char c)
		{
			return c >= '\u3040' && c <= '\u309F';
		}

		// 元の文字列を「ひらがな」かどうかで連続部分（セグメント）に分割する
		// 各セグメントは (text, isHiragana, startIndex) のタプルで表す
		static List<(string text, bool isHiragana, int startIndex)> SegmentOriginal(string original)
		{
			var segments = new List<(string, bool, int)>();
			if (string.IsNullOrEmpty(original))
				return segments;

			int segStart = 0;
			bool currentIsHiragana = IsHiragana(original[0]);
			for (int i = 1; i < original.Length; i++)
			{
				bool charIsHiragana = IsHiragana(original[i]);
				if (charIsHiragana != currentIsHiragana)
				{
					segments.Add((original.Substring(segStart, i - segStart), currentIsHiragana, segStart));
					segStart = i;
					currentIsHiragana = charIsHiragana;
				}
			}
			segments.Add((original.Substring(segStart), currentIsHiragana, segStart));
			return segments;
		}

		// 基本漢字の範囲（U+4E00～U+9FFF）
		public static bool IsKanji(char c)
		{
			return c >= 0x4E00 && c <= 0x9FFF;
		}

		/// <summary>
		/// 文章とふりがな文字列から、1文字ずつの対応（ふりがな側の文字数）を List<int> として作成します。
		/// 戻り値のリストは、文章の各文字に対して何文字分のふりがなが対応するかを示します。
		/// 例）「漢字」を「かんじ」とした場合、ブロックは2文字、ふりがなは3文字なので、
		///     余りが1となり、先頭の漢字に2文字、次の漢字に1文字が対応する [2, 1] となります。
		/// ひらがななどの非漢字は1対1対応とします。
		/// </summary>
		public static List<int> CreateKanjiToKanaMap(this string sentence, string furigana)
		{
			List<int> mapping = new List<int>();
			int i = 0, j = 0;
			
			while (i < sentence.Length)
			{
				// 非漢字（ひらがな、カタカナ、記号など）は1対1対応とする
				if (!IsKanji(sentence[i]))
				{
					mapping.Add(1);
					i++;
					j++;
				}
				else
				{
					// 漢字ブロックを抽出
					int blockStart = i;
					while (i < sentence.Length && IsKanji(sentence[i]))
					{
						i++;
					}
					int kanjiCount = i - blockStart;
					
					// ブロックに対応するふりがな部分を取得
					// 次の文字（ブロック外の文字）を区切り文字として利用できれば、その位置までをふりがな読みとする
					int readingLength = 0;
					if (i < sentence.Length)
					{
						char delim = sentence[i];
						int nextCommonIndex = furigana.IndexOf(delim, j);
						if (nextCommonIndex == -1)
						{
							readingLength = furigana.Length - j;
						}
						else
						{
							readingLength = nextCommonIndex - j;
						}
					}
					else
					{
						// 文章末尾の場合は、残り全体を使用
						readingLength = furigana.Length - j;
					}
					
					// 漢字ブロック内の各文字へ、ふりがな文字数を均等に割り当て
					int baseCount = readingLength / kanjiCount;
					int extra = readingLength % kanjiCount;
					for (int k = 0; k < kanjiCount; k++)
					{
						int countForChar = baseCount + (k < extra ? 1 : 0);
						mapping.Add(countForChar);
					}
					j += readingLength;
				}
			}
			
			return mapping;
		}

		public static List<int> CreateKanaToKanjiMap(this string sentence, string furigana)
		{
			var kanjiToKana = CreateKanjiToKanaMap(sentence, furigana);
			// 逆にマップする.
			var kanaToKanji = new List<int>();
			int kanaIndex = 0;
			for (int i = 0; i < kanjiToKana.Count; i++)
			{
				for (int j = 0; j < kanjiToKana[i]; j++)
				{
					kanaToKanji.Add(i);
					kanaIndex++;
				}
			}
			return kanaToKanji;
		}
	}
}
