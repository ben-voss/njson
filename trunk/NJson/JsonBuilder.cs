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

