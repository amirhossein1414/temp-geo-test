DataTable dataTable = new DataTable();
            dataTable.Clear();
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("GeoData", typeof(SqlGeometry));

            using (SqlConnection sqlConn = new SqlConnection(GeoContext.ConnectionString))
            {
                sqlConn.Open();
                int j = 0;
                for (int i = 1; i < 100000; i++)
                {
                    var lat = Double.Parse($"0.{i}");
                    var lng = Double.Parse($"0.{i}");
                    var location = SqlGeometry.Point(lng, lat, 4326);

                    DataRow row = dataTable.NewRow();
                    row["Content"] = "" + i;
                    row["GeoData"] = location;
                    dataTable.Rows.Add(row);
                }

                using (SqlBulkCopy sqlbc = new SqlBulkCopy(sqlConn))
                {
                    sqlbc.DestinationTableName = "SuperMarkets";
                    sqlbc.ColumnMappings.Add("Content", "Content");
                    sqlbc.ColumnMappings.Add("GeoData", "GeoData");
                    sqlbc.WriteToServer(dataTable);
                }
            }
			
			//azadi canter : 35.699764, 51.338091
			//azadi north : 35.700116, 51.338088 // inserted 
			
			
			//eslam shahr : 35.554324, 51.235237
			// eslam shahr left : 35.554685, 51.235813 inserted