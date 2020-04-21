using System;
using eBay.Services;
using eBay.Services.Finding;
using Slf;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

public class DB
    {
        public readonly string ConnSTR_PR;
        public readonly string ConnSTR_se;
        int expireMin;
        public DB(int _expireMin)
        {
            expireMin = _expireMin;

            SqlConnectionStringBuilder builder_2 = new SqlConnectionStringBuilder();
            builder_2.DataSource = "db-srv-04.database.windows.net";
            builder_2.InitialCatalog = "db-04";
            builder_2.UserID = "db-srv-admin";
            builder_2.Password = "a12345678!";
            ConnSTR_PR = builder_2.ConnectionString;

            SqlConnectionStringBuilder builder_3 = new SqlConnectionStringBuilder();
            builder_3.DataSource = "db-srv-05.database.windows.net";
            builder_3.InitialCatalog = "db-05";
            builder_3.UserID = "db-srv-admin";
            builder_3.Password = "a12345678!";
            ConnSTR_se = builder_3.ConnectionString;
        }
    
            public void Command(string queryString, string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand command = new SqlCommand(queryString, conn);
                command.Connection.Open();

                int rrr = command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();

                int stop;
                bool has = reader.HasRows;
                if (reader.Read())
                {
                    string c0 = reader[0].ToString();
                    string c1 = reader[1].ToString();
                    string c2 = reader[2].ToString();
                    stop = 1;
                }
                stop = 1;
            }
        }

        public int HDL_Item(ref R r, SqlCommand cmd, string new_keywordId, string af)
        {
            HDL_Payment(ref r, cmd);
            HDL_Country(ref r, cmd);
            HDL_Category(ref r, cmd);
            HDL_Conditions(ref r, cmd);
            HDL_SellingState(ref r, cmd);


            try
            {
                cmd.CommandText = @"insert ITEM (itemid, title, categoryId, sellingStateID, viewItemURL, 
                convertedCurrentPrice, paymentMethodID, countryID, conditionId)
                values('" + r.itemId + "', '" + r.title + "', '" + r.categoryId + "', '" + r.sellingStateID_ + "', '" + r.viewItemURL
                + "', '" + r.convertedCurrentPrice + "', '" + r.paymentMethodID_ + "', '" + r.countryID_ + "', '" + r.conditionId + "'); ";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Violation of PRIMARY KEY"))
                {
                    cmd.Cancel();
                    cmd.CommandText = "delete ITEM where itemid = '" + r.itemId + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"insert ITEM (itemid, title, categoryId, sellingStateID, viewItemURL, 
                convertedCurrentPrice, paymentMethodID, countryID, conditionId)
                values('" + r.itemId + "', '" + r.title + "', '" + r.categoryId + "', '" + r.sellingStateID_ + "', '" + r.viewItemURL
                    + "', '" + r.convertedCurrentPrice + "', '" + r.paymentMethodID_ + "', '" + r.countryID_ + "', '" + r.conditionId + "'); ";
                    cmd.ExecuteNonQuery();
                }
            }

            try
            {
                cmd.CommandText = "insert KEYWORD_ITEM (keywordID, itemid, af) values ('" + new_keywordId + "', '" + r.itemId + "', '" + af + "')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Violation of PRIMARY KEY"))
                {
                    cmd.Cancel();
                    cmd.CommandText = "delete KEYWORD_ITEM where keywordID = '" + new_keywordId + "' and itemid = '" + r.itemId + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert KEYWORD_ITEM (keywordID, itemid, af) values ('" + new_keywordId + "', '" + r.itemId + "', '" + af + "')";
                    cmd.ExecuteNonQuery();
                }
            }

            return 0;
        }

    }