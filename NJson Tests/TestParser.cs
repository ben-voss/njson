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
using NUnit.Framework;
using NJson;
using System.IO;
using System.Collections.Generic;

namespace NJsonTests
{
	[TestFixture()]
	public class Test
	{
		[Test]
		public void TestDictionaryEmpty1() {
			IDictionary<String, Object> result = (IDictionary<String, Object>)JsonParser.Parse("{}");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}
		
		[Test]
		public void TestDictionaryEmpty2() {
			IDictionary<String, Object> result = (IDictionary<String, Object>)JsonParser.Parse("{ }");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void TestDictionaryEmpty3() {
			IDictionary<String, Object> result = (IDictionary<String, Object>)JsonParser.Parse(" { }");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void TestDictionaryEmpty4() {
			IDictionary<String, Object> result = (IDictionary<String, Object>)JsonParser.Parse(" { } ");
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[Test]
		public void TestDictionaryOneValue ()
		{			
			IDictionary<String, Object> result = (IDictionary<String, object>)JsonParser.Parse("{\"Test\":\"Hello World\"}");
			
			Assert.IsNotNull(result);
			Assert.IsTrue(result.ContainsKey("Test"));
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("Hello World", result["Test"]);		
		}
		
		[Test()]
		public void TestDictionaryTwoValues ()
		{			
			IDictionary<String, Object> result = (IDictionary<String, object>)JsonParser.Parse("{\"Test 1\":\"Hello World\", \"Test 2\" : \"Second Value\" }");
			
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.ContainsKey("Test 1"));
			Assert.IsTrue(result.ContainsKey("Test 2"));
			Assert.AreEqual("Hello World", result["Test 1"]);			
			Assert.AreEqual("Second Value", result["Test 2"]);			
		}

		[Test()]
		[ExpectedException(typeof(Exception))]
		public void TestArrayMissingCloseBracket() {
			JsonParser.Parse("[");			
		}
		
		[Test()]
		public void TestArrayEmpty1() {
			IList<Object> array = (IList<Object>)JsonParser.Parse("[]");
			Assert.IsNotNull(array);
			Assert.AreEqual(0, array.Count);
		}

		[Test()]
		public void TestArrayEmpty2() {
			IList<Object> array = (IList<Object>)JsonParser.Parse("[ ]");
			Assert.IsNotNull(array);
			Assert.AreEqual(0, array.Count);
		}

		[Test()]
		public void TestArrayEmpty3() {
			IList<Object> array = (IList<Object>)JsonParser.Parse(" [ ]");
			Assert.IsNotNull(array);
			Assert.AreEqual(0, array.Count);
		}

		[Test()]
		public void TestArrayEmpty4() {
			IList<Object> array = (IList<Object>)JsonParser.Parse(" [ ] ");
			Assert.IsNotNull(array);
			Assert.AreEqual(0, array.Count);
		}
		
		[Test()]
		public void TestArrayOneString() {
			IList<Object> array = (IList<Object>)JsonParser.Parse("[\"Hello World\"]");
			Assert.IsNotNull(array);
			Assert.AreEqual(1, array.Count);
			Assert.AreEqual("Hello World", array[0]);
		}
		
		[Test()]
		public void TestArrayTwoStrings() {
			IList<Object> array = (IList<Object>)JsonParser.Parse("[\"Hello\", \"World\"]");
			Assert.IsNotNull(array);
			Assert.AreEqual(2, array.Count);
			Assert.AreEqual("Hello", array[0]);
			Assert.AreEqual("World", array[1]);
		}
		
		[Test]
		public void TestString() {
			String value = (String)JsonParser.Parse("\"Hello World\"");
			Assert.AreEqual("Hello World", value);
		}
		
		[Test]
		public void TestEmptyString() {
			String value = (String)JsonParser.Parse("\"\"");
			Assert.AreEqual(String.Empty, value);		
		}
		
		[Test]
		public void TestEscapedString() {
			String value = (String)JsonParser.Parse("\"\\t\\r\\n\\\"\\/\\b\\f\\u0042\"");
			Assert.AreEqual("\t\r\n\"/\b\f\u0042", value);
		}
		
		[Test]
		[ExpectedException(typeof(Exception))]
		public void TestUnterminatedString() {
			JsonParser.Parse("\"Hello World");
		}

		[Test]
		[ExpectedException(typeof(Exception))]
		public void TestUnterminatedEscapeString() {
			JsonParser.Parse("\"Hello World\\");
		}

		[Test]
		public void TestNumberOneDigit() {
			Decimal number = (Decimal)JsonParser.Parse("4");
			Assert.AreEqual(4M, number);
		}

		[Test]
		public void TestNumberTwoDigits() {
			Decimal number = (Decimal)JsonParser.Parse("42");
			Assert.AreEqual(42M, number);
		}
		
		[Test]
		public void TestNumberDecimalDigits() {
			Decimal number = (Decimal)JsonParser.Parse("4.2");
			Assert.AreEqual(4.2M, number);
		}

		[Test]
		public void TestNumberExponentDigits() {
			Decimal number = (Decimal)JsonParser.Parse("4e2");
			Assert.AreEqual(4e2M, number);
		}
		
		[Test]
		public void TestDictionaryWithArrayValue() {
			Dictionary<String, Object> dict = (Dictionary<String, Object>)JsonParser.Parse("{\"GlossSeeAlso\": [\"GML\", \"XML\"]}");
			Assert.IsNotNull(dict);
			Assert.AreEqual(1, dict.Count);
			Assert.IsTrue(dict.ContainsKey("GlossSeeAlso"));
			IList<Object> list = (IList<Object>)dict["GlossSeeAlso"];
			Assert.AreEqual(2, list.Count);
		}
		
		[Test]
		public void TestDictionaryWithDictionary() {
			JsonParser.Parse("{ \"a\" : { \"b\" : \"c\" } , \"d\" : \"e\" }");
		}

		[Test]
		public void TestArrayWithDictionary() {
			JsonParser.Parse("[ { \"a\" : \"b\" } , { \"c\" : \"d\" } ]");
			
		}

		[Test]
		public void TestText1() {
			DateTime start;
			DateTime end;
			
			String jsonString = File.ReadAllText("TestText1.txt");

			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
				JsonParser.Parse(new StringReader(jsonString));	
			end = DateTime.Now;
			Console.WriteLine(end - start);
		}

		[Test]
		public void TestText2() {
			DateTime start;
			DateTime end;

			String jsonString = File.ReadAllText("TestText2.txt");

			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
				JsonParser.Parse(new StringReader(jsonString));	
			end = DateTime.Now;
			Console.WriteLine(end - start);
		}
		
		[Test]
		public void TestTest3() {
			DateTime start;
			DateTime end;

			String jsonString = File.ReadAllText("TestText3.txt");

			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
				JsonParser.Parse(new StringReader(jsonString));	
			end = DateTime.Now;
			Console.WriteLine(end - start);
		}
				
		[Test]
		public void TestText4() {
			DateTime start;
			DateTime end;

			String jsonString = File.ReadAllText("TestText4.txt");

			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
				JsonParser.Parse(new StringReader(jsonString));	
			end = DateTime.Now;
			Console.WriteLine(end - start);
		}
		
		[Test]
		public void TestText5() {
			DateTime start;
			DateTime end;

			String jsonString = File.ReadAllText("TestText5.txt");

			start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
				JsonParser.Parse(new StringReader(jsonString));	
			end = DateTime.Now;
			Console.WriteLine(end - start);
		}
	}
}

