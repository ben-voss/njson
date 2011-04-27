//
// NJson - JSON Library for .Net.
//    Copyright (C) 2011 Ben Voß
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// Portions of this program are based on the following sources:
// 
// RFC-4627 as described at the following URL:
//
// 		http://www.ietf.org/rfc/rfc4627.txt?number=4627
//
// JSON.org
//		http://www.json.org
//		http://www.json.org/example.html
//
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
			
			return ParseObject(JsonReader.Create(reader));
		}
		
		public static object Parse(JsonReader reader) {
			if (reader == null)
				throw new ArgumentNullException("reader", "The value specified for the 'reader' argument must not be null.");

			return ParseObject(reader);
		}
		
		// Parses an object
		// c contains the first character of the object.  The reader is positioned to read the next character
		private static object ParseObject(JsonReader reader) {
			switch (reader.Type) {
				case JsonType.Object:
					return ParseDictionary(reader);
			
				case JsonType.Array:
					return ParseArray(reader);
				
				case JsonType.String:
					return reader.ReadValueAsString();
					
				case JsonType.Number:
					return reader.ReadValueAsDecimal();
				
				case JsonType.Boolean:
					return reader.ReadValueAsBool();
	
				case JsonType.Null:
					return reader.ReadNull();
			}
			
			throw new Exception();
		}

		// Parse a dictionary
        // The function enters with c equal to the dictionary open bracket '{'
        // and the reader positioned at the next character.
		// The function returns the list with c set to the first character after the
		// close bracket of the dictionary and the reader positioned to the next character.
		private static IDictionary<String, Object> ParseDictionary(JsonReader reader) {
			IDictionary<String, object> dictionary = new Dictionary<String, object>();
			
			reader.ReadObject();
			
			while (reader.Type == JsonType.String) {
				String name = reader.ReadName();
				object value = ParseObject(reader);
				dictionary.Add(name, value);
			}
			
			reader.ReadEnd();
			
			return dictionary;			
		}

		// Parse an array
        // The function enters with c equal to the array open bracket '['
        // and the reader positioned at the next character.
		// The function returns the list with c set to the first character after the
		// close bracket of the array and the reader positioned to the next character.
		private static IList<Object> ParseArray(JsonReader reader) {
			IList<Object> array = new List<Object>();
			
			reader.ReadArray();
			
			while (reader.IsValue) {
				array.Add(ParseObject(reader));
			}
			
			reader.ReadEnd();
			
			return array;
		}
				
	}
}

