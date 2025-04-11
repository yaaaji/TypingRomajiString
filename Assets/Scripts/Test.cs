using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Yaaaji.Util;

public class Test : MonoBehaviour
{

	public TMP_Text kanaText;
	public TMP_Text romajiText;

	public TypingRomajiString typingString;

	void setText( string text )
	{
		typingString = new TypingRomajiString( text );
		kanaText.text = typingString.kanaText;
		romajiText.text = typingString.romajiModified;
	}

	IEnumerator wait(float time)
	{
		yield return new WaitForSeconds( time );
	}

	IEnumerator textTest(string text)
	{
		setText( text );
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
				kanaText.text = typingString.kanaText;
				romajiText.text = typingString.romajiModified;
			}
			if( typingString.isComplete ) break;
		}

		Debug.Log( "complete" );
	}


	IEnumerator Test00()
	{
		yield return textTest( "こわしちゃった" );
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
