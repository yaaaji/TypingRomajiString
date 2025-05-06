using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yaaaji.Util
{
	// タイピング用のローマ字のバリエーションを管理するクラス.
	public class TypingRomajiString
	{
		string srcHiragana = "";

		string fixedRomaji = "";

		string leftRomaji = "";

		Color _fixedColor = Color.gray;
		public Color fixedColor {
			get => _fixedColor;
			set {
				_fixedColor = value;
				_fixedColorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(_fixedColor)}>";
			}
		}
		string _fixedColorTag;
		public string fixedColorTag => _fixedColorTag??=$"<color=#{ColorUtility.ToHtmlStringRGB(_fixedColor)}>";

		Color _leftColor = Color.white;
		public Color leftColor {
			get => _leftColor;
			set {
				_leftColor = value;
				_leftColorTag = $"<color=#{ColorUtility.ToHtmlStringRGB(_leftColor)}>";
			}
		}
		string _leftColorTag;
		public string leftColorTag => _leftColorTag??=$"<color=#{ColorUtility.ToHtmlStringRGB(_leftColor)}>";

		public bool isComplete => romajiPartsIndex >= _romajiList.Count;

		public string kanaText => srcHiragana;

		// 全体のローマ字を取得するプロパティ
		public string romaji {
			get{
				return fixedRomaji + leftRomaji;
			}
		}

		public string romajiModified {
			get{
				return $"{fixedColorTag}{fixedRomaji}</color>{leftColorTag}{leftRomaji}</color>";
			}
		}

		private bool m_isChangeString = false;
		public bool isChangeRomajiString => m_isChangeString;

		public class Node{
			public char c = ' ';
			public int level = 0;
			public int index = 0;
			public bool isDefault = true;
			public string str = null;
			public List<Node> nextList = null;
			public Dictionary<char,Node> nextDic;
			public bool isEnd => nextDic == null || nextDic.Count <= 0;

			public Node(char c,int level,int index,bool isDefault = true)
			{
				this.c = c;
				this.level = level;
				this.index = index;
				this.isDefault = isDefault;
			}

			public string endString{
				get{
					// 末端のノードまで移動する.
					if (isEnd) return str;
					var node = nextList[0];
					// 0番目のノードを優先する.
					while( node != null && !node.isEnd )
					{
						node = node.nextList[0];
					}
					if ( node == null ) return"";
					return node.str;
				}
			}

			public Node AddNext(char nextC,int index)
			{
				if (nextDic == null) nextDic = new ();
				if (nextList == null) nextList = new ();
				var n = FindNext(nextC);
				// 新規追加の場合.
				if ( n == null) {
					var node = new Node(nextC,level+1,index, nextList.Count == 0);
					nextDic.Add(node.c,node);
					nextList.Add(node);
					return node;
				}
				//既存の場合は普通にNodeを返す.
				return n;
			}

			public Node FindNext(char c)
			{
				if (nextDic == null) return null;
				nextDic.TryGetValue(c,out var node);
				//if (nextList == null) return null;
				return node;
			}
		}

		class RomajiParts{
			public string kana = "";
			public List<string> romajiList = new ();
			public Node rootNode = new Node(' ',-1,0,true);
			Node currentNode = null;
			public bool isNoKana =false;
			public int kanaIndex = 0;
			public int kanaLength => kana.Length;
			public bool isComplete => kanaIndex >= kanaLength;
			public bool isNComplete => isComplete && isNWait;
			public bool isChangeString = false;
			public int inputIndex = 0;
			public string inputHistory = "";
			public bool isN => kana == "ん";
			public bool isNWait => isN && inputHistory=="n" && inputIndex == 1;
			public string activeRomaji {
				get{
					if ( isN ){
						if ( inputIndex > 0 )
						{
							if ( inputHistory[0] == 'n' ){
								return inputHistory;
							}
							else{
								return currentNode.endString; // xnが返る.
							}
						}
						else{
							return "n";
						}
					}
					return currentNode.endString;
				}
			}

			public bool CheckKana(char c){
				// ひらがな以外はfalseを返す.
				// ひらがなはtrueを返す.
				// ひらがなはUnicodeで0x3040-0x309Fの範囲にある.
				if (c >= 0x3040 && c <= 0x309F) return true;
				return false;
			}

			public RomajiParts(int index,string kana,List<string> romajiList)
			{
				this.kana = kana;
				this.romajiList = romajiList;
				// かな変換できない文字は別の処理にする必要がある.
				this.isNoKana = !CheckKana(kana[0]);
				// romajiListからcharに分解した木構造を作る.
				// 先頭の文字をrootNodeに追加する.
				//Debug.Log($"new RomajiParts: {kana} list={string.Join(",", romajiList)}");
				for (int i = 0; i < romajiList.Count; i++)
				{
					var r = romajiList[i];
					if (r.Length <= 0) continue;
					var node = rootNode.AddNext(r[0],i);
					// 2文字目以降を追加する.
					for(int c = 1; c < r.Length; c++)
					{
						node = node.AddNext(r[c],i);
					}
					// 末端のノードに最終系を追加する.
					node.str = r;
				}
				currentNode = rootNode;
				//var tree = DumpNode(currentNode);
				//Debug.Log(tree);
			}

			// s
			// +i
			// +h
			//  +i
			// c
			//  +i
			// のような表示ツリー構造を作る.
			string DumpNode(Node node){
				int level = node.level-1<0 ? 0 : node.level;
				string plus = node.level-1<0? "" : "+";
				var result = $"{new string(' ', level)}{plus}{node.c}\n";
				if (node.isEnd) return result;
				foreach(var kv in node.nextDic)
				{
					var n = kv.Value;
					result += DumpNode(n);
				}
				return result;
			}

			// 現在のノードの文字列を取得する.
			public string NodeString { get{
				if (currentNode == null) return "";
				return currentNode.endString;
			}}

			// 先頭1文字をチェックする.
			// nの打ち切り用に使う.
			bool topCaharaCheck(char roman)
			{
				foreach(var r in romajiList)
				{
					if (r[0] == roman) return true;
				}
				return false;
			}

			bool searchOtherRomaji(char c,bool isValid)
			{
				// 他のリストにあるか調べる.
				// その箇所だけ調べるとおかしな組み合わせになる事があるのでフラグと比べる.
				var next = currentNode.FindNext(c);
				if ( next != null )
				{
					//Debug.Log($"searchOtherRomaji: {inputHistory} c:{c} idx:{inputIndex}");
					isChangeString = true;
					inputHistory += c;
					inputIndex++;
					// 次のノードに移動する.
					currentNode = next;
					isValid = true;
				}
				else
				{
					//Debug.Log($"searchOtherRomaji: {inputHistory} c:{c} idx:{inputIndex} not found.");
				}

				return isValid;
			}


			public int UpdateInput(string inputRomaji,RomajiParts nextParts = null)
			{
				isChangeString = false;
				if (string.IsNullOrEmpty(inputRomaji)) return 0;

				//Dev.Log($"Update kana: {kana}, select:{activeRomaji} inputHistory: {inputHistory}, kanaIndex: {kanaIndex}, inputIndex: {inputIndex}");

				// 入力されたローマ字から文字列が変わるか調べる.
				// 無効な文字列が見つかったらhistroyの追加をやめる.
				// それ以外はhistoryに追加する.
				// "ん"の時だけ特殊な処理になる(n単独の場合次の単語の先頭が来たら次に行ける)
				bool isValid = false;
				int prevIndex = inputIndex;
				foreach(var c in inputRomaji)
				{
					var romaji = activeRomaji;
					// "ん"の時は次の文字を見て次のローマ字に行く.
					if ( isNWait )
					{
						//Debug.Log($"UpdateInput.isNWait: {inputHistory} c:{c} next:{nextParts?.activeRomaji} idx:{inputIndex}");

						// 2個目のnか次のパーツの頭文字だったら次のローマ字に行く.
						// nnなら完了.
						if ( c == 'n' )
						{
							// 文字の変更が入る.
							isValid = searchOtherRomaji(c,isValid);
							kanaIndex = 1;
							return 1;
						}
						// 次のローマ字がyで先頭が該当するなら何もしない(nyの時)
						else if ( c == 'y' )
						{
							break;
						}
						// 次のローマ字の先頭だったら一旦終わらせる（パラメータの更新は無し)
						else if ( nextParts != null && nextParts.topCaharaCheck(c) )
						{
							isValid = true;
							kanaIndex = 1;
							// nextPartsの先頭を1文字処理してはいけない(indexだけ変える.).
							//Debug.Log($"UpdateInput isNWait アクセプト: {inputHistory} c:{c} next:{nextParts?.activeRomaji} idx:{inputIndex}");
							return 0;
						}
						// それ以外は受け付けない.
						break;
					}

					if (romaji.Length <= inputIndex) break;

					var next = currentNode.FindNext(c);
					if ( next != null )
					{
						isChangeString = !next.isDefault; // デフォルト以外に変更があったらtrueにする.
						inputHistory += c;
						inputIndex++;
						currentNode = next;
						isValid = true;
					}
					// 見つからなかった
					else
					{
						isValid = false;

						// 見つからない場合はn待ちか次のノードに行く
					}
					if ( !isValid )
					{
						// 無効な文字列か終了まで行ったらhistoryの追加をやめる.
						break;
					}
				}
				if ( isValid ){
					// nの時だけはvalid扱いにしない.
					if ( isNWait ){
						// n待ちの時はkanaIndexを更新しない（確定していないため).
					}
					else if ( isNoKana ){
						// かなが無い場合はindexを更新しない.
						kanaIndex = inputIndex;
					}
					else{
						// inputHistoryの文字列をかなに変換する.
						var historyKana = inputHistory.ToHiragana(isTruncate: true);
						// かなの長さが変わったらかなのindexを更新する.
						kanaIndex = historyKana.Length;
					}
					// 入力文字を何文字使ったか調べる.
					return inputIndex - prevIndex;
				}
				return 0;
			}
		}

		List<RomajiParts> _romajiList = new ();
		int romajiPartsIndex = 0;

		RomajiParts currentParts {
			get{
				if (romajiPartsIndex < 0 || romajiPartsIndex >= _romajiList.Count) return null;
				return _romajiList[romajiPartsIndex];
			}
		}
		RomajiParts nextParts {
			get{
				if (romajiPartsIndex < 0 || romajiPartsIndex+1 >= _romajiList.Count) return null;
				return _romajiList[romajiPartsIndex+1];
			}
		}

		string getFixedRomajiString(int index)
		{
			if ( index < 0 ) return "";
			index = Mathf.Min(index, _romajiList.Count-1);

			// 0から指定されたindexまでのPartsをinputIndexを考慮して結合する
			var result = "";
			for (var i = 0; i < index+1; i++)
			{
				var parts = _romajiList[i];
				//Dev.Log($"parts[{i}]: {parts.inputHistory}");
				result += parts.inputHistory;
			}
			return result;
		}

		string getLeftRomajiString(int index)
		{
			if (index < 0 || index >= _romajiList.Count) return "";
			// 指定されたindexから最後までのPartsをinputIndexを考慮して結合する
			var result = "";
			var parts = _romajiList[index];
			result += parts.activeRomaji.Substring(parts.inputIndex);
			for (var i = index+1; i < _romajiList.Count; i++)
			{
				parts = _romajiList[i];
				result += parts.activeRomaji;
			}
			return result;
		}

		// 追加インプットされたローマ字リストからfixedとleftを更新する
		public int UpdateInputString(string inputRomaji){
			m_isChangeString = false;

			if ( string.IsNullOrEmpty(inputRomaji) ) return 0;

			if ( isComplete ) return 0;

			//

			// 入力が尽きるまで進める.
			int totalAcceptCount = 0;
			while(true){
				var curParts = currentParts;
				if ( curParts == null ) return 0;
				int acceptCount = curParts.UpdateInput(inputRomaji,nextParts);
				totalAcceptCount += acceptCount;
				// 一文字も受け入れられなかったら終了.
				if ( !curParts.isNComplete && acceptCount <= 0 ) break;

				// 文字列変更フラグを立てる.
				m_isChangeString |= curParts.isChangeString;

				// 
				if ( curParts.isComplete ){
					inputRomaji = inputRomaji.Substring(acceptCount);

					// 完了したら次のPartsに進む.
					romajiPartsIndex++;

					if ( isComplete ){
						//completeSubject.OnNext(true);
						break;
					}

					if ( inputRomaji.Length <= 0 ){
						// 入力が無くなったら終了.
						break;
					}
				}
				else{
					// 完了してない場合以降は不要なinputなので以降無視して終了する.
					break;
				}
			}

			// fixedRomajiとleftRomajiを更新する.
			if ( totalAcceptCount >= 0 ){
				fixedRomaji = getFixedRomajiString(romajiPartsIndex);
				leftRomaji = getLeftRomajiString(romajiPartsIndex);
				//Dev.Log( $"fixedRomaji: {fixedRomaji}, leftRomaji: {leftRomaji}, totalAcceptCount: {totalAcceptCount}");
			}
			return totalAcceptCount;
		}

		public TypingRomajiString(string hiragana)
		{
			// hiraganaを分解して、_hiraganaListに追加する
			srcHiragana = hiragana;

			var kanaIndex = 0;
			// _hiraganaListの各要素に対して、ローマ字のバリエーションを取得する
			foreach (var kanaRomanPair in hiragana.ConvertHiraganaToRomaji())
			{
				var (kana,romajiList) = kanaRomanPair;
				var parts = new RomajiParts(kanaIndex, kana, romajiList);
				_romajiList.Add( parts );
				kanaIndex = parts.kanaLength;
			}

			fixedRomaji = "";
			leftRomaji = getLeftRomajiString(0);
			Dump();
		}

		void Dump(){
			Debug.Log($"srcHiragana: {srcHiragana}");
			Debug.Log($"fixedRomaji: {fixedRomaji}");
			Debug.Log($"leftRomaji: {leftRomaji}");
			Debug.Log($"romajiPartsIndex: {romajiPartsIndex}");
			foreach(var parts in _romajiList){
				Debug.Log($"kana: {parts.kana}, romajiList: {string.Join(",", parts.romajiList)}, kanaIndex: {parts.kanaIndex}, inputIndex: {parts.inputIndex}, inputHistory: {parts.inputHistory}");
			}

		}
	}
}
