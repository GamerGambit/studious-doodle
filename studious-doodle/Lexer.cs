using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace studious_doodle
{
	public class Lexer
	{
		public enum TokenType
		{
			Number,
			String,
			True,
			False,
			Null,
			BrkCurlOpen,
			BrkCurlClose,
			BrkSquareOpen,
			BrkSquareClose,
			Colon,
			Comma
		}

		public struct Token
		{
			public TokenType Type;
			public object Value;
		}

		public List<Token> Tokens { get; } = new List<Token>();

		private string source;
		private int currentIndex = -1;
		private char currentChar = '\0';
		private int currentRow = 1;
		private int currentColumn = 0;

		public Lexer(string source)
		{
			this.source = source;
			Read();
		}

		private void Error(string message)
		{
			throw new Exception($"[Lexer:{currentRow}:{currentColumn}] {message}");
		}

		private void Next()
		{
			currentIndex++;
			if (currentIndex == source.Length)
				return;

			currentChar = source[currentIndex];
			currentColumn++;
		}

		private bool EOS()
		{
			return currentIndex >= source.Length;
		}

		private void ReadString()
		{
			var sb = new StringBuilder();

			bool terminated = false;
			while (!EOS())
			{
				if (currentChar == '"')
				{
					terminated = true;
					Next();
					break;
				}

				if (currentChar == '\\')
				{
					Next();

					switch (currentChar)
					{
						case '"':
							Next();
							sb.Append('"');
							break;

						case '\\':
							Next();
							sb.Append('\\');
							break;

						case 'b':
							Next();
							sb.Append('\b');
							break;

						case 'n':
							Next();
							sb.Append('\n');
							break;

						case 'r':
							Next();
							sb.Append('\r');
							break;

						case 't':
							Next();
							sb.Append('\t');
							break;

						case 'u':
							{
								Next();

								for (var i = 0; i < 4; ++i)
								{
									if (!currentChar.IsHex())
										Error("Invalid unicode character");

									Next();
								}

								var codepoint = int.Parse(source.Substring(currentIndex - 4, 4), NumberStyles.HexNumber);
								sb.Append(char.ConvertFromUtf32(codepoint));

								break;
							}

						default:
							Error("Invalid escape sequence");
							break;
					}
				}
				else
				{
					sb.Append(currentChar);
					Next();
				}
			}

			if (!terminated)
				Error("Expected end of string, got end of input.");

			Tokens.Add(new Token()
			{
				Type = TokenType.String,
				Value = sb.ToString()
			});
		}

		private void ReadNumber()
		{
			var sb = new StringBuilder();
			while (!EOS())
			{
				if (char.IsNumber(currentChar) || currentChar == 'e' || currentChar == 'E' || currentChar == '.')
				{
					sb.Append(currentChar);
					Next();
				}
				else
				{
					break;
				}
			}

			if (!double.TryParse(sb.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, new CultureInfo("en-US"), out var result))
				Error("Invalid number format");

			Tokens.Add(new Token()
			{
				Type = TokenType.Number,
				Value = result
			});
		}

		private void Read()
		{
			if (source.Length < 2)
				Error("Source must be at least 2 characters");

			Next();

			while (!EOS())
			{
				switch (currentChar)
				{
					case '\r':
						Next();
						break;

					case '\n':
						currentColumn = 1;
						currentRow++;
						Next();
						break;

					case ' ':
					case '\t':
						currentColumn++;
						Next();
						break;

					case '[':
						Tokens.Add(new Token()
						{
							Type = TokenType.BrkSquareOpen
						});
						Next();
						break;

					case ']':
						Tokens.Add(new Token()
						{
							Type = TokenType.BrkSquareClose
						});
						Next();
						break;

					case '{':
						Tokens.Add(new Token()
						{
							Type = TokenType.BrkCurlOpen
						});
						Next();
						break;

					case '}':
						Tokens.Add(new Token()
						{
							Type = TokenType.BrkCurlClose
						});
						Next();
						break;

					case ':':
						Tokens.Add(new Token()
						{
							Type = TokenType.Colon
						});
						Next();
						break;

					case ',':
						Tokens.Add(new Token()
						{
							Type = TokenType.Comma
						});
						Next();
						break;

					case '"':
						Next();
						ReadString();
						break;

					default:
						{
							if (char.IsNumber(currentChar))
							{
								ReadNumber();
							}
							else
							{
								var sb = new StringBuilder();
								while (!EOS() && char.IsLetter(currentChar))
								{
									sb.Append(currentChar);
									Next();
								}

								var str = sb.ToString();
								if (str == "true")
								{
									Tokens.Add(new Token()
									{
										Type = TokenType.True
									});
								}
								else if (str == "false")
								{
									Tokens.Add(new Token()
									{
										Type = TokenType.False
									});
								}
								else if (str == "null")
								{
									Tokens.Add(new Token()
									{
										Type = TokenType.Null
									});
								}
								else
									Error("Invalid JSON sequence");
							}

							break;
						}
				}
			}
		}
	}
}
