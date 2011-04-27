using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace NJson
{
	public class JsonReader
	{
		
		private enum ScopeType {
			Object,
			Array
		}
		
		private class Scope {
			public readonly ScopeType ScopeType;
			public bool FirstValueRead;
			public bool NameRead;
			
			public Scope(ScopeType scopeType) {
				ScopeType = scopeType;
			}
		}
		
		private readonly Stack<Scope> _scopeStack = new Stack<Scope>();
		
		private TextReader _reader;
		
		private char _c;
		
		private JsonReader (TextReader reader)
		{
			_reader = reader;
			_c = (char)reader.Read();
			ParseWhiteSpace();
		}
				
		public static JsonReader Create(String file) {
			if (file == null)
				throw new ArgumentNullException("file", "The value specified for the 'file' argument must not be null.");
			
			return Create(File.OpenText(file));
		}
		
		public static JsonReader Create(Stream stream) {
			if (stream == null)
				throw new ArgumentNullException("stream", "The value specified for the 'stream' argument must not be null.");

			return Create(new StreamReader(stream));
		}
		
		public static JsonReader Create(TextReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader", "The value specified for the 'reader' argument must not be null.");

			return new JsonReader(reader);
		}
		
		public JsonType Type {
			get {
				if (_c == '{')
					return JsonType.Object;
				
				if (_c == '[')
					return JsonType.Array;
				
				if (_c == '"')
					return JsonType.String;
				
				if (_c == 'f' || _c == 't')
					return JsonType.Boolean;
				
				if (_c == 'n')
					return JsonType.Null;
				
				return JsonType.Number;
			}    
		}
		
		public bool IsValue {
			get {
				return _c != '}' && _c != ']';		
			}
		}
		
		public void ReadObject() {
			if (_scopeStack.Count > 0)
				ValidateValue();
				
			if (_c != '{')
				throw new Exception("Expected '{'.");

			_c = (char)_reader.Read();
			ParseWhiteSpace();
			
			_scopeStack.Push(new Scope(ScopeType.Object));
		}
		
		public void ReadArray() {
			if (_scopeStack.Count > 0)
				ValidateValue();

			if (_c != '[')
				throw new Exception("Expected '['.");

			_c = (char)_reader.Read();
			ParseWhiteSpace();
			
			_scopeStack.Push(new Scope(ScopeType.Array));
		}
		
		public void ReadEnd() {
			if (_scopeStack.Count == 0)
				throw new Exception("Scope stack empty");
			
			Scope scope = _scopeStack.Pop();
			if (scope.ScopeType == ScopeType.Array) {
				if (_c != ']')
					throw new Exception("Expected ']'.");
			} else if (scope.ScopeType == ScopeType.Object) {
				if (_c != '}')
					throw new Exception("Expected '}'.");
			}
			
			if (_scopeStack.Count > 0) {
				_c = (char)_reader.Read();
				ParseWhiteSpace();
			
				ParseComma();
			}
		}
		
		public String ReadName() {
			if (_scopeStack.Count == 0)
				throw new Exception("Cannot read a name when there is no scope.");
			
			Scope scope = _scopeStack.Peek();
			
			if (scope.ScopeType != JsonReader.ScopeType.Object)
				throw new Exception("Cannot read a name when the scope is not an object.");
			
			if (scope.NameRead)
				throw new Exception("Cannot read a name when one has already been read.");
			
			String result = ParseString();
			
			scope.NameRead = true;
			
			if (_c != ':')
				throw new Exception("Expected ':'.");
			
			_c = (char)_reader.Read();
			ParseWhiteSpace();
			
			return result;
		}
		
		public String ReadValueAsString() {
			ValidateValue();
			
			String result = ParseString();
			
			ParseComma();
			
			return result;
		}
		
		public Decimal ReadValueAsDecimal() {
			ValidateValue();
			
			Decimal result = ParseNumber();
			
			ParseComma();
			
			return result;
		}
		
		public bool ReadValueAsBool() {
			ValidateValue();
			
			bool result = ParseBool();
			
			ParseComma();
			
			return result;	
		}
		
		private void ValidateValue() {
			if (_scopeStack.Count > 0) {
				Scope scope = _scopeStack.Peek();
			
				if ((scope.ScopeType == ScopeType.Object) && (!scope.NameRead))
					throw new Exception();
			}
		}
		
		private void ParseComma() {
			if (_scopeStack.Count > 0) {
				Scope scope = _scopeStack.Peek();
			
				if ((scope.ScopeType == ScopeType.Object) && (_c != '}') && (_c != ','))
					throw new Exception("Expected ',' or '}'");
				
				if ((scope.ScopeType == ScopeType.Array) && (_c != ']') && (_c != ','))
					throw new Exception("Expected ',' or ']'");
			
				if (_c == ',') {
					_c = (char)_reader.Read();
					ParseWhiteSpace();
				}
			
				scope.NameRead = false;
			}
		}
		
		private bool ParseBool() {
			if (_c == 't')
			{
				ConsumeExpected('r');
				ConsumeExpected('u');
				ConsumeExpected('e');
				_c = (char)_reader.Read();

				ParseWhiteSpace();

				return true;
			} else {
				ConsumeExpected('a');
				ConsumeExpected('l');
				ConsumeExpected('s');
				ConsumeExpected('e');
				_c = (char)_reader.Read();
			
				ParseWhiteSpace();
				
				return false;
			}
		}
		
		private void ConsumeExpected(Char expected) {
			_c = (char)_reader.Read();
			if (_c != expected)
				throw new Exception("Expected '" + expected + "'.");
		}
		
		// Parse a string
        // The function enters with c equal to the string open quote '"'
        // and the reader positioned at the next character.
		// The function returns the parsed string and sets c equal to the first
		// character after the closing quote and the reader posittioned at the
		// next character
		private string ParseString() {						
			StringBuilder builder = new StringBuilder();
			_c = (char)_reader.Read();
			while (_c != '"') {
				
				if (_c == '\\')
				{
					_c = (char)_reader.Read();
					if (_c == 0xffff)
						throw new Exception("Unexpected end of stream.");
					
					if (_c == 'n')
						builder.Append('\n');
					else if (_c == 'r')
						builder.Append('\r');
					else if (_c == 't')
						builder.Append('\t');
					else if (_c == 'b')
						builder.Append('\b');
					else if (_c == 'f')
						builder.Append('\f');
					else if (_c == '\\')
						builder.Append('\\');
					else if (_c == '/')
						builder.Append('/');
					else if (_c == '"')
						builder.Append('"');
					else if (_c == 'u')
					{
						char[] chars = new char[4];
						_reader.Read(chars, 0, 4);
						
						uint unicode;
						if (!UInt32.TryParse(new String(chars), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out unicode))
							throw new Exception();
						
						builder.Append(Char.ConvertFromUtf32((int)unicode));
					}
				} else
					builder.Append(_c);
					
				_c = (char)_reader.Read();				
				if (_c == 0xffff)
					throw new Exception("Unexpected end of stream");
			}
			
			_c = (char)_reader.Read();
			ParseWhiteSpace();

			return builder.ToString();
		}
		
		// Parse a number (Decimal)
        // The function enters with c equal to the first digit of the number (0-9) or the decimal point
        // and the reader positioned at the next character.
		// The function returns the parsed Decimal and sets c equal to the first
		// character after the last digit and the reader positioned at the
		// next character
		private Decimal ParseNumber() {
			StringBuilder builder = new StringBuilder();

			do {
				builder.Append(_c);
				_c = (char)_reader.Read();
			} while ((_c >= '0' && _c <= '9') || (_c == 'e') || (_c == 'E') || (_c == '-') || (_c == '+') || (_c == '.'));
			
			Decimal number;
			if (!Decimal.TryParse(builder.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out number))
				throw new Exception("Unable to parse '" + builder + "' into a Decimal.");
			
			ParseWhiteSpace();
			
			return number;
		}
		
		public Object ReadNull() {
			ConsumeExpected('u');
			ConsumeExpected('l');
			ConsumeExpected('l');			
			_c = (char)_reader.Read();
			ParseWhiteSpace();
			
			ParseComma();
			
			return null;
		}
		
		// Consumes white space (tabs, spaces, new lines)
		// The function enters with a character that can be anything.  If the character is not
		// white space the function returns immediately.  Otherwise, it will consume characters
		// until a non-white space character is encounter.
		// The function returns with c set to the first non-white space character and the reader
		// positioned at the next character
		private void ParseWhiteSpace() {
			while (Char.IsWhiteSpace(_c))
				_c = (char)_reader.Read();
		}
	}
}

