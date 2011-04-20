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
using System.IO;
using System.Collections.Generic;

namespace NJson
{
	public class JsonBuilder
	{
		public JsonBuilder ()
		{			
		}
		
		public void Build(TextWriter writer, Object obj) {
			if (obj == null)
				Build(writer);
			else if (obj is IEnumerable<Object>)
				Build(writer, (IEnumerable<Object>)obj);
			else if (obj is IEnumerable<KeyValuePair<String, Object>>)
				Build(writer, (IEnumerable<KeyValuePair<String, Object>>)obj);
			else if (obj is String)
				Build(writer, (String)obj);
			else if (obj is Decimal)
				Build(writer, (Decimal)obj);
			else if (obj is bool)
				Build(writer, (bool)obj);
			else
				throw new Exception("Unknown type");
		}
		
		public void Build(TextWriter writer, IEnumerable<KeyValuePair<String, Object>> objectStructure) {
			writer.Write('{');
			
			IEnumerator<KeyValuePair<String, Object>> enumerator = objectStructure.GetEnumerator();
			if (enumerator.MoveNext())
			{
				KeyValuePair<String, Object> pair = enumerator.Current;
				Build(writer, pair.Key);
				writer.Write(':');
				Build(writer, pair.Value);				
			}
			
			while (enumerator.MoveNext()) {
				KeyValuePair<String, Object> pair = enumerator.Current;
				writer.Write(',');
				Build(writer, pair.Key);
				writer.Write(':');
				Build(writer, pair.Value);
			}
			
			writer.Write('}');			
		}
		
		public void Build(TextWriter writer, IEnumerable<Object> arrayStructure) {
			writer.Write('[');
			
			IEnumerator<Object> enumerator = arrayStructure.GetEnumerator();
			if (enumerator.MoveNext())
				Build(writer, enumerator.Current);
							
			while (enumerator.MoveNext()) {
				writer.Write(',');
				Build(writer, enumerator.Current);
			}
			
			writer.Write(']');
		}
		
		public void Build(TextWriter writer, String stringValue) {
			writer.Write('"');
			
			for (int i = 0; i < stringValue.Length; i++)
			{
				char c = stringValue[i];
				
				if (c == '"')
					writer.Write("\\\"");
				else if (c == '\r')
					writer.Write("\\r");
				else if (c == '\n')
					writer.Write("\\n");
				else if (c == '\t')
					writer.Write("\\t");
				else if (c == '\b')
					writer.Write("\\b");
				else if (c == '\f')
					writer.Write("\\f");
				else if (c >= 32 && c < 126)
				{
					writer.Write("\\u");
					writer.Write(Convert.ToString((int)c, 16).PadLeft(4, '0'));
				} else 
					writer.Write(c);
			}

			writer.Write('"');
		}
		
		public void Build(TextWriter writer, Decimal decimalValue) {
			writer.Write(decimalValue.ToString());
		}
		
		public void Build(TextWriter writer, bool boolValue) {
			if (boolValue)
				writer.Write("true");
			else
				writer.Write("false");
		}
		
		public void Build(TextWriter writer) {
			writer.Write("null");
		}
	}
}

