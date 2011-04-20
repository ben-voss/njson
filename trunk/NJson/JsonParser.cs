using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace NJson
{
	public class JsonParser
	{
		private JsonParser ()
		{
		}
		
		public static object Parse(string jsonString) {
			if (jsonString == null)
				throw new ArgumentNullException("jsonString", "The value specified for the 'jsonString' argument must not be null.");
			
			return Parse(new StringReader(jsonString));			
		}
		
		public static object Parse(TextReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader", "The value specified for the 'reader' argument must not be null.");
			
			char c = (char)reader.Read();
			return ParseObject(reader, ref c);
		}
		
		// Parses an object
		// c contains the first character of the object.  The reader is positioned to read the next character
		private static object ParseObject(TextReader reader, ref char c) {
			ParseWhiteSpace(reader, ref c);
			
			if (c == '{')
				return ParseDictionary(reader, ref c);				

			if (c == '[')
				return ParseArray(reader, ref c);
			
			// Literal String
			if (c == '"')
				return ParseString(reader, ref c);

			// Literal Number
			if ((c >= '0' && c <= '9') || (c == '-'))
				return ParseNumber(reader, ref c);
							
			// Lteral true
			if (c == 't' &&
			    reader.Read() == 'r' &&
			    reader.Read() == 'u' &&
			    reader.Read() == 'e')
			{
				c = (char)reader.Read();
				return true;
			}
			
			// Literal false
			if (c == 'f' &&
			    reader.Read() == 'a' &&
			    reader.Read() == 'l' &&
			    reader.Read() == 's' &&
			    reader.Read() == 'e')
			{
				c = (char)reader.Read();
				return true;
			}
			
			// Literal null
			if (c == 'n' &&
			    reader.Read() == 'u' &&
			    reader.Read() == 'l' &&
			    reader.Read() == 'l')
			{
				c = (char)reader.Read();
				return null;
			}
			
			if (c == 0xffff)
				throw new Exception("Unexpected end of stream.");
						
			throw new Exception("Unexpected char '" + c + "'.");
		}

		// Parse a dictionary
        // The function enters with c equal to the dictionary open bracket '{'
        // and the reader positioned at the next character.
		// The function returns the list with c set to the first character after the
		// close bracket of the dictionary and the reader positioned to the next character.
		private static IDictionary<String, Object> ParseDictionary(TextReader reader, ref char c) {
			c = (char)reader.Read();
			if (c == 0xffff)
				throw new Exception("Unexpected end of stream.");

			ParseWhiteSpace(reader, ref c);

			IDictionary<String, object> dictionary = new Dictionary<String, object>();

			while (c != '}') {	
				// Parse the key
				if (c != '"')
					throw new Exception("Expected start of string found '" + c + "' instead.");

				String name = ParseString(reader, ref c);
			
				// Parse the value
				ParseWhiteSpace(reader, ref c);
			
				if (c != ':')
					throw new Exception("Expected ':' after name");
				
				c = (char)reader.Read();
				if (c == 0xffff)
					throw new Exception("Unexpected end of stream.");

				object value = ParseObject(reader, ref c);
				
				// Add to the dictionary
				dictionary.Add(name, value);
			
				// Consume the comma if there is one
				ParseWhiteSpace(reader, ref c);
				if (c == ',')
				{
					c = (char)reader.Read();
					if (c == 0xffff)
						throw new Exception("Unexpected end of stream.");
				}

				ParseWhiteSpace(reader, ref c);
			}
			
			c = (char)reader.Read();
			
			return dictionary;			
		}

		// Parse an array
        // The function enters with c equal to the array open bracket '['
        // and the reader positioned at the next character.
		// The function returns the list with c set to the first character after the
		// close bracket of the array and the reader positioned to the next character.
		private static IList<Object> ParseArray(TextReader reader, ref char c) {
			// Read the first character to advance past the open bracket
			c = (char)reader.Read();
			if (c == 0xffff)
				throw new Exception("Unexpected end of stream.");
			
			ParseWhiteSpace(reader, ref c);
			
			IList<Object> array = new List<Object>();
			
			while (c != ']') {
				array.Add(ParseObject(reader, ref c));
				
				ParseWhiteSpace(reader, ref c);
				
				if (c == 0xffff)
					throw new Exception("Unexpected end of stream.");

				if (c == ',')
				{
					c = (char)reader.Read();
					if (c == 0xffff)
						throw new Exception("Unexpected end of stream.");
				}
				
				ParseWhiteSpace(reader, ref c);				
			}
			
			c = (char)reader.Read();
			
			return array;
		}
				
		// Parse a string
        // The function enters with c equal to the string open quote '"'
        // and the reader positioned at the next character.
		// The function returns the parsed string and sets c equal to the first
		// character after the closing quote and the reader posittioned at the
		// next character
		private static string ParseString(TextReader reader, ref char c) {						
			StringBuilder builder = new StringBuilder();
			c = (char)reader.Read();
			while (c != '"') {
				
				if (c == '\\')
				{
					c = (char)reader.Read();
					if (c == 0xffff)
						throw new Exception("Unexpected end of stream.");
					
					if (c == 'n')
						builder.Append('\n');
					else if (c == 'r')
						builder.Append('\r');
					else if (c == 't')
						builder.Append('\t');
					else if (c == 'b')
						builder.Append('\b');
					else if (c == 'f')
						builder.Append('\f');
					else if (c == '\\')
						builder.Append('\\');
					else if (c == '/')
						builder.Append('/');
					else if (c == '"')
						builder.Append('"');
					else if (c == 'u')
					{
						char[] chars = new char[4];
						reader.Read(chars, 0, 4);
						
						uint unicode;
						if (!UInt32.TryParse(new String(chars), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out unicode))
							throw new Exception();
						
						builder.Append(Char.ConvertFromUtf32((int)unicode));
					}
				} else
					builder.Append(c);
					
				c = (char)reader.Read();				
				if (c == 0xffff)
					throw new Exception("Unexpected end of stream");
			}
			
			c = (char)reader.Read();
			
			return builder.ToString();
		}
		
		// Parse a number (Decimal)
        // The function enters with c equal to the first digit of the number (0-9) or the decimal point
        // and the reader positioned at the next character.
		// The function returns the parsed Decimal and sets c equal to the first
		// character after the last digit and the reader positioned at the
		// next character
		private static Decimal ParseNumber(TextReader reader, ref char c) {
			StringBuilder builder = new StringBuilder();

			do {
				builder.Append(c);
				c = (char)reader.Read();
			} while ((c >= '0' && c <= '9') || (c == 'e') || (c == 'E') || (c == '-') || (c == '+') || (c == '.'));
			
			Decimal number;
			if (!Decimal.TryParse(builder.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out number))
				throw new Exception("Unable to parse '" + builder + "' into a Decimal.");
			
			return number;
		}
	
		// Consumes white space (tabs, spaces, new lines)
		// The function enters with a character that can be anything.  If the character is not
		// white space the function returns immediately.  Otherwise, it will consume characters
		// until a non-white space character is encounter.
		// The function returns with c set to the first non-white space character and the reader
		// positioned at the next character
		private static void ParseWhiteSpace(TextReader reader, ref char c) {
			while (Char.IsWhiteSpace(c))
				c = (char)reader.Read();
		}
	}
}

