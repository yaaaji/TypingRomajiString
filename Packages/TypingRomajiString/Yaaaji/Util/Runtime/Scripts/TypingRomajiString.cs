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

		private bool m_isChangeSelectIndex = false;
		//public bool isChangeRomajiString => currentParts?.isChangeSelectIndex ?? false;
		public bool isChangeRomajiString => m_isChangeSelectIndex;

		class RomajiParts{
			public string kana = "";
			public List<string> romajiList = new ();
			public int kanaIndex = 0;
			public int kanaLength => kana.Length;
			public bool isComplete => kanaIndex >= kanaLength;
			public int selectIndex = 0;
			public bool isChangeSelectIndex = false;
			public int inputIndex = 0;
			public string inputHistory = "";
			public bool isN => romajiList[selectIndex] == "n";
			public bool isNWait => isN && inputIndex == 1;

			public RomajiParts(int index,string kana,List<string> romajiList)
			{
				this.kana = kana;
				this.romajiList = romajiList;
			}

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
				for(int idx = 0 ; idx < romajiList.Count; idx++)
				{
					if (idx == selectIndex) continue;
					var r = romajiList[idx];
					if (r.Length <= inputIndex) continue;
					if (r[inputIndex] == c)
					{
						isChangeSelectIndex = true;
						selectIndex = idx;
						inputHistory += c;
						inputIndex++;
						isValid = true;
						// みつけた.
						//Dev.Log($"searchOtherRomaji: Find {inputHistory} selectIndex: {selectIndex} inputIndex: {inputIndex}");
						break;
					}
				}
				return isValid;
			}


			public int UpdateInput(string inputRomaji,RomajiParts nextParts = null)
			{
				isChangeSelectIndex = false;
				if (string.IsNullOrEmpty(inputRomaji)) return 0;

				//Dev.Log($"Update kana: {kana}, select:{romajiList[selectIndex]} inputHistory: {inputHistory}, kanaIndex: {kanaIndex}, selectIndex: {selectIndex}, inputIndex: {inputIndex}");

				// 入力されたローマ字からselectIndexが変わるか調べる.
				// 無効な文字列が見つかったらhistroyの追加をやめる.
				// それ以外はhistoryに追加する.
				// "ん"の時だけ特殊な処理になる(n単独の場合次の単語の先頭が来たら次に行ける)
				bool isValid = false;
				int prevIndex = inputIndex;
				foreach(var c in inputRomaji)
				{
					if (selectIndex >= romajiList.Count) break;
					var romaji = romajiList[selectIndex];
					// "ん"の時は次の文字を見て次のローマ字に行く.
					if ( isNWait )
					{
						//Dev.Log($"isNWait: {inputHistory} c:{c}");

						// 2個目のnか次のパーツの頭文字だったら次のローマ字に行く.
						// nnなら完了.
						if ( c == 'n' )
						{
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
							// nextPartsの先頭を1文字処理してはいけない.
							return 1;//nextParts.UpdateInput(c.ToString(),null);
						}
						// それ以外は受け付けない.
						break;
					}

					if (romaji.Length <= inputIndex) break;

					if (romaji[inputIndex] == c)
					{
						inputHistory += c;
						inputIndex++;
						isValid = true;
					}
					// 今のローマ字候補に無い場合.
					else
					{
						// 他のリストにあるか調べる.
						isValid = searchOtherRomaji(c,isValid);
					}
					if (!isValid)
					{
						// 無効な文字列が見つかったらhistoryの追加をやめる.
						break;
					}
				}
				if ( isValid ){
					// inputHistoryの文字列をかなに変換する.
					var historyKana = inputHistory.ToHiragana(isTruncate: true);
					// かなの長さが変わったらselectIndexを更新する.
					kanaIndex = historyKana.Length;

					//Dev.Log($"kana: {kana}, select:{romajiList[selectIndex]} inputHistory: {inputHistory}, kanaIndex: {kanaIndex}, selectIndex: {selectIndex}, inputIndex: {inputIndex}");

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
				if (romajiPartsIndex < 0 || romajiPartsIndex >= _romajiList.Count) return null;
				if (romajiPartsIndex+1 >= _romajiList.Count) return null;
				return _romajiList[romajiPartsIndex+1];
			}
		}

		string getFixedRomajiString(int index)
		{
			if ( index < 0 ) return "";
			index = Mathf.Min(index, _romajiList.Count-1);

			// 0から指定されたindexまでのPartsをselectIndex,inputIndexを考慮して結合する
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
			// 指定されたindexから最後までのPartsをselectIndex,inputIndexを考慮して結合する
			var result = "";
			var parts = _romajiList[index];
			result += parts.romajiList[parts.selectIndex].Substring(parts.inputIndex);
			for (var i = index+1; i < _romajiList.Count; i++)
			{
				parts = _romajiList[i];
				result += parts.romajiList[parts.selectIndex];
			}
			return result;
		}

		// 追加インプットされたローマ字リストからfixedとleftを更新する
		public int UpdateInputString(string inputRomaji){
			m_isChangeSelectIndex = false;

			if ( string.IsNullOrEmpty(inputRomaji) ) return 0;

			if ( isComplete ) return 0;

			//
			var curParts = currentParts;
			if ( curParts == null ) return 0;

			// 入力が尽きるまで進める.
			int totalAcceptCount = 0;
			while(true){
				int acceptCount = curParts.UpdateInput(inputRomaji,nextParts);
				totalAcceptCount += acceptCount;
				// 一文字も受け入れられなかったら終了.
				if ( acceptCount <= 0 ) break;

				// 文字列変更フラグを立てる.
				m_isChangeSelectIndex |= curParts.isChangeSelectIndex;

				// 
				if ( curParts.isComplete ){
					//Dev.Log($"Complete: {curParts.inputHistory} acceptCount: {acceptCount} inputRomaji: {inputRomaji}");
					inputRomaji = inputRomaji.Substring(acceptCount);
					//Dev.Log($"Complete Update: {inputRomaji}");

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
				Debug.Log($"kana: {parts.kana}, romajiList: {string.Join(",", parts.romajiList)}, kanaIndex: {parts.kanaIndex}, selectIndex: {parts.selectIndex}, inputIndex: {parts.inputIndex}, inputHistory: {parts.inputHistory}");
			}

		}
	}
}
