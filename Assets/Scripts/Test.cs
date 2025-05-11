using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yaaaji.Util;

public class Test : MonoBehaviour
{
	public TMP_Text kanjiMapText;
	public TMP_Text kanaKanjiMapText;
	public TMP_Text displayText;
	public TMP_Text kanaText;
	public TMP_Text romajiText;

	public TypingRomajiString typingString;

	public List<int> kanjiMap = null;
	public List<int> kanaToDisplayMap = null;

	void setText( string display, string text )
	{
		typingString = new TypingRomajiString( text );
		displayText.text = display;
		kanaText.text = typingString.kanaText;
		romajiText.text = typingString.romajiModified;
		kanjiMap = display.CreateKanjiToKanaMap( typingString.kanaText );
		kanaToDisplayMap = new List<int>();
		var mapText = string.Join( ",", kanjiMap );
		Debug.Log( $"kanjiMap: {mapText}" );
		int srcIndex = 0;
		int displayIndex = 0;
		foreach( var v in kanjiMap ){
			var kanaCount = v;
			var displayChara = display[displayIndex];
			for ( int i = 0; i < kanaCount; i++ )
			{
				var srcChara = typingString.kanaText[srcIndex];
				Debug.Log($"  > {srcIndex}[{srcChara}]->{displayIndex}[{displayChara}]");
				if ( i >= kanaCount - 1 )
				{
					// 最後の文字に反応するようにする.
					kanaToDisplayMap.Add( displayIndex );
				}
				else
				{
					// 途中の文字は前回の表示位置を設定する.
					kanaToDisplayMap.Add( displayIndex-1 );
				}
				srcIndex++;
			}
			displayIndex++;
		}
		kanjiMapText.text = mapText;
		kanaKanjiMapText.text = string.Join( ",", kanaToDisplayMap );
	}

	IEnumerator wait(float time)
	{
		yield return new WaitForSeconds( time );
	}

	IEnumerator textTest(string display,string text)
	{
		setText( display, text );
		while(true)
		{
			yield return null;
			var input = Input.inputString;
			var acceptCount = typingString.UpdateInputString( input );
			if( acceptCount > 0 )
			{
				Debug.Log( $"input: {input}" );
				Debug.Log( $"acceptCount: {acceptCount}" );
				Debug.Log( $"romaji: {typingString.romaji}" );
				Debug.Log( $"kanaText: {typingString.kanaText}" );
				Debug.Log( $"romajiModified: {typingString.romajiModified}" );
				Debug.Log( $"isChangeRomaji: {typingString.isChangeRomajiString}" );

				// きりつめてひらがなに変換する.
				// アルファベットがあるとひらがな変換時にマッピングが狂う
				var inputIndex = typingString.romajiLeftMapIndex;
				if (  inputIndex >= display.Length )
				{
					// ひらがな→漢字マップを使って、漢字の部分をひらがなに変換する.
					displayText.text = $"<color=#7f7f7f>{display}</color>";
				}
				else
				{
					// 漢字→ひらがなマップを使って、漢字を対応させる.
					inputIndex = kanaToDisplayMap[inputIndex];
					// 最初は負の値が入っている.
					if ( inputIndex < 0 )
					{
						// ひらがな→漢字マップを使って、漢字の部分をひらがなに変換する.
						displayText.text = $"{display}";
					}
					else
					{
						// 漢字→ひらがなマップを使って、漢字を対応させる.
						displayText.text = $"<color=#7f7f7f>{display.Substring(0, inputIndex)}</color><color=white>{display.Substring(inputIndex)}</color>";
					}
				}
				kanaText.text = typingString.kanaText;
				romajiText.text = typingString.romajiModified;
			}
			if( typingString.isComplete ) break;
		}

		Debug.Log( "complete" );
	}


	IEnumerator Test00()
	{
		// 漢字→ひらがなマップの作成テスト.
		yield return textTest( "四十路マン登場","よそじまんとうじょう" );
		yield return textTest( "hatomune混じったやつ","hatomuneまじったやつ");
		yield return textTest( "天気ですね","てんきですね" );
		yield return textTest( "壊しちゃった","こわしちゃった" );
	}


	// Start is called before the first frame update
	IEnumerator Start()
	{
		yield return wait( 1.0f );
		yield return Test00();
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
