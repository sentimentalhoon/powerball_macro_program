//===================================================================================
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// This code is released under the terms of the CPOL license, 
//===================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace Csv.Serialization.Model
{
	public class Person
	{
		public Guid Id { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public long BitFlags { get; set; }

		public byte BitMask { get; set; }

		[CsvIgnore]
		public string BSB1 { get; set; }

		public string ContactName { get; set; }

		public string ContactTitle { get; set; }

		public string DepartmentCode { get; set; }

		[CsvIgnore]
		public string DirectAccount1 { get; set; }

		public DateTime DOB { get; set; }

		public string eMailAddress { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string MobilePhone { get; set; }

		public string PostCode { get; set; }

		public string State { get; set; }

		public string Suburb { get; set; }

		[CsvIgnore]
		public string Password { get; set; }

		[CsvIgnore]
		public List<Guid> Friends { get; set; }

		/// <summary>
		/// To String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("Name: {0} {1} {2},", ContactTitle, FirstName, LastName);

			return sb.ToString();
		}

		/// <summary>
		/// To Text
		/// </summary>
		/// <returns></returns>
		public string ToText()
		{
			var sb = new StringBuilder();

			sb.AppendLine(string.Format("ID: {0}", Id));
			sb.AppendLine(string.Format("\tName: {0} {1} {2},", ContactTitle, FirstName, LastName));
			sb.AppendLine(string.Format("\tDOB: {0}", DOB));
			sb.AppendLine(string.Format("\tMobilePhone: {0}", MobilePhone));
			sb.AppendLine(string.Format("\teMailAddress: {0}", eMailAddress));
			sb.AppendLine(string.Format("\tAddress1: {0}", Address1));
			sb.AppendLine(string.Format("\tAddress2: {0}", Address2));
			sb.AppendLine(string.Format("\tSuburb: {0}", Suburb));
			sb.AppendLine(string.Format("\tState: {0}", State));
			sb.AppendLine(string.Format("\tPostCode: {0}", PostCode));
			sb.AppendLine(string.Format("\tMobilePhone: {0}", MobilePhone));
			sb.AppendLine();

			return sb.ToString();
		}
	}
}