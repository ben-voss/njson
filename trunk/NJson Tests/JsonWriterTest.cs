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
using NUnit.Framework;
using NJson;

namespace NJson.Tests
{
	[TestFixture]
	public class JsonWriterTest
	{
		[Test]
		public void StartArray() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.Flush();
			
			Assert.AreEqual("[", builder.ToString());
		}

		[Test]
		public void StartObject() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartObject();
			writer.Flush();
			
			Assert.AreEqual("{", builder.ToString());
		}
		
		[Test]
		public void StartEndArray() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[]", builder.ToString());
		}

		[Test]
		public void StartEndObject() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartObject();
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("{}", builder.ToString());
		}

		[Test]
		public void NestedStartEndObjectAndArray() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartObject();
			writer.WriteName("a");
			writer.WriteStartArray();
			writer.WriteStartObject();
			writer.WriteName("b");
			writer.WriteStartArray();
			writer.WriteEnd();
			writer.WriteEnd();
			writer.WriteEnd();
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("{\"a\":[{\"b\":[]}]}", builder.ToString());
		}
		
		[Test]
		[ExpectedException]
		public void NestedArrayMissingNameInObjectThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteStartArray();
		}
		
		[Test]
		[ExpectedException]
		public void NestedObjectMissingNameInObjectThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteStartObject();
		}

		[Test]
		[ExpectedException]
		public void TooManyEndsThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteEnd();
			writer.WriteEnd();
		}
		
		[Test]
		[ExpectedException]
		public void WriteNameTwiceThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteName("1");
			writer.WriteName("2");			
		}
		
		[Test]
		[ExpectedException]
		public void WriteNameWithArrayThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartArray();
			writer.WriteName("name");
		}
		
		[Test]
		[ExpectedException]
		public void WriteUnnamedValueWithObjectThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteValue("value");			
		}

		[Test]
		[ExpectedException]
		public void WriteValueTwiceWithObjectThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteStartObject();
			writer.WriteName("name");
			writer.WriteValue("value 1");			
			writer.WriteValue("value 2");			
		}
		
		[Test]
		public void WriteObjectWithOneValue() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartObject();
			writer.WriteName("name");
			writer.WriteValue("value");
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("{\"name\":\"value\"}", builder.ToString());
		}

		[Test]
		public void WriteObjectWithTwoValues() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartObject();
			writer.WriteName("name 1");
			writer.WriteValue("value 1");
			writer.WriteName("name 2");
			writer.WriteValue("value 2");
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("{\"name 1\":\"value 1\",\"name 2\":\"value 2\"}", builder.ToString());
		}
		
		
				[Test]
		public void WriteArrayWithOneValue() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue("value");
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[\"value\"]", builder.ToString());
		}

		[Test]
		public void WriteArrayWithTwoValues() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue("value 1");
			writer.WriteValue("value 2");
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[\"value 1\",\"value 2\"]", builder.ToString());
		}

		[Test]
		public void WriteNull() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteNullValue();
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[null]", builder.ToString());
		}
		
		[Test]
		public void WriteNullString() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue(null);
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[null]", builder.ToString());
		}

		[Test]
		public void WriteTrue() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue(true);
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[true]", builder.ToString());
		}
		
		[Test]
		public void WriteFalse() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue(false);
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[false]", builder.ToString());
		}
		
				
		[Test]
		public void WriteDecimal() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteValue(42.42M);
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[42.42]", builder.ToString());
		}
		
		[Test]
		public void WriteWhitespace() {
			StringBuilder builder = new StringBuilder();
			JsonWriter writer = JsonWriter.Create(builder);
			writer.WriteStartArray();
			writer.WriteWhitespace(" ");
			writer.WriteEnd();
			writer.Flush();
			
			Assert.AreEqual("[ ]", builder.ToString());
		}
		
		[Test]
		[ExpectedException]
		public void WriteDecimalNoContainerThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteValue(4.2M);
		}

		[Test]
		[ExpectedException]
		public void WriteNullNoContainerThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteNullValue();
		}
				
		[Test]
		[ExpectedException]
		public void WriteBoolNoContainerThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteValue(true);
		}

		[Test]
		[ExpectedException]
		public void WriteStringNoContainerThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteValue("test");
		}

		[Test]
		[ExpectedException]
		public void WriteWhitespaceNoContainerThrows() {
			JsonWriter writer = JsonWriter.Create(new StringBuilder());
			writer.WriteWhitespace(" ");
		}
	}
}

