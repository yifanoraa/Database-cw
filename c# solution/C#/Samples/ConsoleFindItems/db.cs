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

    }