using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ADO.Net_Mapper
{
	class Program
	{
		static void Main(string[] args)
		{
			SqlConnection conn = new SqlConnection("Server=localhost;Database=AuditedEntities;Trusted_Connection=True;");
			SqlDataAdapter adp = new SqlDataAdapter("select * from AuditReport", conn);

			DataTable tmp = new DataTable();
			adp.Fill(tmp);

			List<TempModel> tempList = new List<TempModel>();
			MapToEntity<TempModel>(tempList, tmp);

			Console.Read();
		}

		public static object GetObject<T>(T input)
		{
			var oType = input.GetType();
			if (oType == typeof(string)) return input as string;
			else return Activator.CreateInstance(oType);
		}

		public static void MapToEntity<T> (List<T> tList, DataTable dTable)
		{
			Console.WriteLine(typeof(T));
			foreach(DataRow trow in dTable.Rows)
			{
				T instance = (T) Activator.CreateInstance(typeof(T));

				foreach (DataColumn tcol in dTable.Columns)
				{
					Console.WriteLine(tcol.ColumnName + " --- " + trow[tcol]);

					var tProp = instance.GetType().GetProperties()
										.Where(x => x.Name == tcol.ColumnName)
										.SingleOrDefault();
					if(tProp != null)
					{
						try
						{
							var tInstance = GetObject(trow[tcol]);
							tProp.SetValue(instance, trow[tcol]);
						}
						catch (Exception ex) { }
					}
				}
				tList.Add(instance);
			}
		}
		public class TempModel
		{
			public long Id { get; set; }
			public string EntityName { get; set; }
			public string Column { get; set; }
			public string Type { get; set; }
			public string OldValue { get; set; }
			public string NewValue { get; set; }
			public string PrimaryKey { get; set; }
		}
	}
}
