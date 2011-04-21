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
using System.Text;
using System.Collections.Generic;

namespace NJson
{
	public class JsonWriter
	{
		private enum ScopeType {
			Object,
			Array
		}
		
		private class Scope {
			public readonly ScopeType ScopeType;
			public bool FirstValueWritten;
			public bool NameWritten;
			
			public Scope(ScopeType scopeType) {
				ScopeType = scopeType;
			}
		}
		
		private readonly Stack<Scope> _scopeStack = new Stack<Scope>();
		private readonly TextWriter _writer;
		
		private JsonWriter (TextWriter writer)
		{
			_writer = writer;
		}
			                            
		public static JsonWriter Create(StringBuilder builder) {
			return Create(new System.IO.StringWriter(builder));
		}
			
		public static JsonWriter Create(TextWriter writer) {
			return new JsonWriter(writer);
		}
			
		public static JsonWriter Create(Stream stream) {
			return Create(new StreamWriter(stream));
		}
			
		public static JsonWriter Create(String file) {
			return Create(File.CreateText(file));
		}
		
		/// <summary>
		/// Closes the writer
		/// </summary>
		public void Close() {
			_writer.Close();
		}
		
		/// <summary>
		/// Clears all buffers and causes any buffered data to be written to the underlying stream.
		/// </summary>
		public void Flush() {
			_writer.Flush();
		}
		
		/// <summary>
		/// Writes the start of an object
		/// </summary>
		public void WriteStartObject() {
			if (_scopeStack.Count > 0) {
				Scope scope = _scopeStack.Peek();
				if ((scope.ScopeType == ScopeType.Object) && (!scope.NameWritten))
					throw new Exception("Must write a name before creating a nested object.");
			}
			
			_scopeStack.Push(new Scope(ScopeType.Object));
			_writer.Write('{');
		}
		
		/// <summary>
		/// Writes the start of an array.
		/// </summary>
		public void WriteStartArray() {
			if (_scopeStack.Count > 0) {
				Scope scope = _scopeStack.Peek();
				if ((scope.ScopeType == ScopeType.Object) && (!scope.NameWritten))
					throw new Exception("Must write a name before creating a nested array.");
			}

			_scopeStack.Push(new Scope(ScopeType.Array));
			_writer.Write('[');
		}
		
		/// <summary>
		/// Closes an open object or array.
		/// </summary>
		public void WriteEnd() {
			if (_scopeStack.Count == 0)
				throw new Exception();
			
			Scope scope = _scopeStack.Pop();
			if (scope.ScopeType == ScopeType.Object)
				_writer.Write('}');
			else
				_writer.Write(']');
		}

		/// <summary>
		/// Closes open objects and arrays
		/// </summary>
		public void WriteEndDocument() {
			while (_scopeStack.Count > 0)
				WriteEnd();
		}

		/// <summary>
		/// Writes the name of a key-value pair in an object
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> string containing the name to write.
		/// </param>
		public void WriteName(String name) {
			if (_scopeStack.Count == 0)
				throw new Exception("Names can only be written in object containers.");
			
			Scope scope = _scopeStack.Peek();
			
			if (scope.ScopeType != ScopeType.Object)
				throw new Exception("Names can only be written for object types");
			
			if (scope.NameWritten)
				throw new Exception("Name cannot be written more than once without a value written between.");
			else
				scope.NameWritten = true;
			
			if (scope.FirstValueWritten)
				_writer.Write(',');
			
			WriteEscapedString(name);
			
			_writer.Write(':');
		}
		
		/// <summary>
		/// Writes a null value
		/// </summary>
		public void WriteNullValue() {
			WriteComma();
			
			_writer.Write("null");
		}

		/// <summary>
		/// Writes a string value.
		/// </summary>
		/// <remarks>
		/// If the value of the string argument is null then a null literal is written instead.
		/// </remarks>
		/// <param name="value">
		/// The <see cref="String"/> to write.
		/// </param>
		public void WriteValue(String value) {
			
			WriteComma();
			
			if (value == null)
				_writer.Write("null");
			else
				WriteEscapedString(value);
		}
		
		/// <summary>
		/// Writes a decimal value.
		/// </summary>
		/// <param name="value">
		/// The <see cref="Decimal"/> value to write.
		/// </param>
		public void WriteValue(Decimal value) {
			WriteComma();

			_writer.Write(value.ToString());
		}
		
		/// <summary>
		/// Writes a boolean value
		/// </summary>
		/// <param name="value">
		/// The <see cref="System.Boolean"/> value to write.
		/// </param>
		public void WriteValue(bool value) {
			WriteComma();
			
			if (value)
				_writer.Write("true");
			else
				_writer.Write("false");
		}
		
		/// <summary>
		/// Writes the given white space.
		/// </summary>
		/// <param name="ws">
		/// A <see cref="String"/> containing the white space characters.
		/// </param>		
		public void WriteWhitespace(String ws) {
			if (_scopeStack.Count == 0)
				throw new Exception("Whitespace can only be written in containers.");

			_writer.Write(ws);
		}
		
		private void WriteEscapedString(String value) {
			_writer.Write('"');
			
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				
				if (c == '"')
					_writer.Write("\\\"");
				else if (c == '\r')
					_writer.Write("\\r");
				else if (c == '\n')
					_writer.Write("\\n");
				else if (c == '\t')
					_writer.Write("\\t");
				else if (c == '\b')
					_writer.Write("\\b");
				else if (c == '\f')
					_writer.Write("\\f");
				else if (c < 32 && c > 126)
				{
					_writer.Write("\\u");
					_writer.Write(Convert.ToString((int)c, 16).PadLeft(4, '0'));
				} else 
					_writer.Write(c);
			}

			_writer.Write('"');
		}
				
		private void WriteComma() {
			if (_scopeStack.Count == 0)
				throw new Exception("Values can only be written in containers.");

			Scope scope = _scopeStack.Peek();
			
			if ((scope.ScopeType == ScopeType.Object) && (!scope.NameWritten))
				throw new Exception();
			else
				scope.NameWritten = false;
			
			if ((scope.ScopeType == ScopeType.Array) && (scope.FirstValueWritten))
				_writer.Write(',');
			else
				scope.FirstValueWritten = true;
			
		}
	}
}

