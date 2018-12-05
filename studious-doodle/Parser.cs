using System;
using System.Collections.Generic;
using System.Dynamic;

namespace studious_doodle
{
	internal class Parser
	{
		internal dynamic document = new ExpandoObject();

		private Lexer Lexer;

		private Lexer.Token currentToken;
		private int currentIndex = -1;

		public Parser(string source)
		{
			Lexer = new Lexer(source);
			Parse();
		}

		private void Error(string message)
		{
			throw new Exception($"[Parser]: {message}");
		}

		private void Next()
		{
			currentIndex++;

			if (currentIndex == Lexer.Tokens.Count)
				return;

			currentToken = Lexer.Tokens[currentIndex];
		}

		private bool EOS()
		{
			return currentIndex == Lexer.Tokens.Count;
		}

		private Lexer.Token Expect(Lexer.TokenType tokenType)
		{
			if (currentToken.Type != tokenType)
				Error($"Expected {tokenType}, got {currentToken.Type}");

			var retToken = currentToken;
			Next();
			return retToken;
		}

		private void Parse()
		{
			if (Lexer.Tokens.Count < 2)
				Error("Invalid JSON structure");

			Next();

			if (currentToken.Type == Lexer.TokenType.BrkCurlOpen)
			{
				document.root = ReadObject();
			}
			else if (currentToken.Type == Lexer.TokenType.BrkSquareOpen)
			{
				document.root = ReadArray();
			}
			else
			{
				Error("Invalid JSON structure");
			}
		}

		private IDictionary<string, object> ReadObject()
		{
			Expect(Lexer.TokenType.BrkCurlOpen);

			var dict = new ExpandoObject() as IDictionary<string, object>;

			while (currentToken.Type != Lexer.TokenType.BrkCurlClose)
			{
				if (dict.Count > 0)
				{
					Expect(Lexer.TokenType.Comma);
				}

				var key = (string)Expect(Lexer.TokenType.String).Value;
				Expect(Lexer.TokenType.Colon);
				var value = ReadValue();

				dict.Add(key, value);
			}

			Expect(Lexer.TokenType.BrkCurlClose);

			return dict;
		}

		private IList<object> ReadArray()
		{
			Expect(Lexer.TokenType.BrkSquareOpen);

			var array = new List<object>();

			while (currentToken.Type != Lexer.TokenType.BrkSquareClose)
			{
				if (array.Count > 0)
				{
					Expect(Lexer.TokenType.Comma);
				}

				array.Add(ReadValue());
			}

			Expect(Lexer.TokenType.BrkSquareClose);

			return array;
		}

		private object ReadValue()
		{
			object ret = null;

			switch (currentToken.Type)
			{
				case Lexer.TokenType.BrkCurlOpen:
					return ReadObject();

				case Lexer.TokenType.BrkSquareOpen:
					return ReadArray();

				case Lexer.TokenType.Number:
					ret = (double)currentToken.Value;
					break;

				case Lexer.TokenType.String:
					ret = (string)currentToken.Value;
					break;

				case Lexer.TokenType.True:
					ret = true;
					break;

				case Lexer.TokenType.False:
					ret = false;
					break;

				case Lexer.TokenType.Null:
					ret = null;
					break;

				default:
					throw new Exception($"Expected value type, got {currentToken.Type}");
			}

			Next();
			return ret;
		}
	}
}
