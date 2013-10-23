//
// NJson - JSON Library for .Net.
//    Copyright (C) 2011-2013 Ben Voß
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
using System.Text;
using System.Collections.Generic;

namespace NJson
{
	public class JsonBuilder
	{
		private JsonBuilder ()
		{			
		}
		
		public static void Build(StringBuilder builder, object obj) {
			if (builder == null)
				throw new ArgumentNullException("builder", "The value specified for the 'builder' argument must not be null.");
			
			Build(JsonWriter.Create(builder), obj);						
		}
		
		public static void Build(TextWriter writer, object obj) {
			if (writer == null)
				throw new ArgumentNullException("writer", "The value specified for the 'writer' argument must not be null.");
			
			Build(JsonWriter.Create(writer), obj);			
		}
		
		public static void Build(Stream stream, object obj) {
			if (stream == null)
				throw new ArgumentNullException("stream", "The value specified for the 'stream' argument must not be null.");
			
			Build(JsonWriter.Create(stream), obj);
		}
		
		public static void Build(String file, object obj) {
			if (file == null)
				throw new ArgumentNullException("file", "The value specified for the 'file' argument must not be null.");
			
			Build(JsonWriter.Create(file), obj);			
		}
		
		public static void Build(JsonWriter writer, Object obj) {
			if (writer == null)
				throw new ArgumentNullException("writer", "The value specified for the 'writer' argument must not be null.");
			
			if (obj == null)
				writer.WriteNullValue();
			else if (obj is IEnumerable<Object>)
			{
				writer.WriteStartArray();

				foreach (Object entry in (IEnumerable<Object>)obj)
					Build(writer, entry);
			
				writer.WriteEnd();
			}
			else if (obj is IEnumerable<KeyValuePair<String, Object>>)
			{
				writer.WriteStartObject();

				foreach (KeyValuePair<String, Object> pair in (IEnumerable<KeyValuePair<String, Object>>)obj)
				{
					writer.WriteName(pair.Key);
					Build(writer, pair.Value);
				}
			
				writer.WriteEnd();
			}
			else if (obj is String)
				writer.WriteValue((String)obj);
			else if (obj is Decimal)
				writer.WriteValue((Decimal)obj);
			else if (obj is bool)
				writer.WriteValue((bool)obj);
			else
				throw new Exception("Unknown type");
		}
	}
}
