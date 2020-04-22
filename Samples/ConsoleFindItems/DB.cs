using System;
using eBay.Services;
using eBay.Services.Finding;
using Slf;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace ConsoleFindItems
{
    class SearchSrv
    {
        ILogger logger;
        ClientConfig config;
        FindingServicePortTypeClient client;
        int entriesPerPage;
        public SearchSrv(int _entriesPerPage)
        {
            entriesPerPage = _entriesPerPage;

            LoggerService.SetLogger(new ConsoleLogger());
            logger = LoggerService.GetLogger();
            config = new ClientConfig();
            config.EndPointAddress = "https://svcs.ebay.com/services/search/FindingService/v1";
            config.ApplicationId = "ZimingWu-comp0022-PRD-b69ec6afc-7ff00c17";
            client = FindingServiceClientFactory.getServiceClient(config);
        }
        public SearchItem[] GetBy(string keyword, string by)
        {
            try
            {
                // Create request object
                FindItemsByKeywordsRequest req = new FindItemsByKeywordsRequest();
                // Set request parameters
                req.keywords = keyword;
                PaginationInput pi = new PaginationInput();
                pi.entriesPerPage = entriesPerPage;
                pi.entriesPerPageSpecified = true;
                pi.pageNumber = 10;
                pi.pageNumberSpecified = true;
                req.paginationInput = pi;

                ItemFilter f1 = new ItemFilter();
                f1.name = ItemFilterType.ListingType;
                f1.value = new string[] { by };
                req.itemFilter = new ItemFilter[] { f1 };

                // Call the service
                FindItemsByKeywordsResponse response = client.findItemsByKeywords(req);

                // Show output
                logger.Info("Ack = " + response.ack);
                //logger.Info("Find " + response.searchResult.count + " items.");
                return response.searchResult.item;
            }
            catch (Exception ex)
            {
                // Handle exception if any
                logger.Error(ex);
                return null;
            }
        }

        static string CheckBadStr(string inputSTR)
        {
            if (inputSTR != null && inputSTR.Length > 0)
            {
                string[] bidStrlist = new string[24];
                bidStrlist[0] = "'";
                bidStrlist[1] = ";";
                bidStrlist[2] = ":";
                bidStrlist[3] = "%";
                bidStrlist[4] = "@";
                bidStrlist[5] = "&";
                bidStrlist[6] = "#";
                bidStrlist[7] = "\"";
                bidStrlist[8] = "net user";
                bidStrlist[9] = "exec";
                bidStrlist[10] = "net localgroup";
                bidStrlist[11] = "select";
                bidStrlist[12] = "asc";
                bidStrlist[13] = "char";
                bidStrlist[14] = "mid";
                bidStrlist[15] = "insert";
                bidStrlist[16] = "order";
                bidStrlist[17] = "exec";
                bidStrlist[18] = "delete";
                bidStrlist[19] = "drop";
                bidStrlist[20] = "truncate";
                bidStrlist[21] = "xp_cmdshell";
                bidStrlist[22] = "<";
                bidStrlist[23] = ">";

                string tmpStr = inputSTR.ToLower();

                bool replaced = false;
                foreach (string err in bidStrlist)
                    if (tmpStr.Contains(err))
                    {
                        replaced = true;
                        tmpStr = tmpStr.Replace(err, " ");
                    }
                if (replaced)
                    return tmpStr;
                else
                    return inputSTR;
            }
            return inputSTR;
        }
        public List<R> Rsl(string keyword, string by) // "FixedPrice"  "AuctionWithBIN"
        {
            SearchItem[] items = GetBy(keyword, by);

            List<R> rsl = new List<R>();

            if (items != null)
                foreach (SearchItem item in items)
                {
                    R r = new R(true);

                    // Item
                    // SellingState
                    // CATEGORY
                    Console.WriteLine("****************************");
                    r.itemId = item.itemId;
                    r.title = CheckBadStr( item.title );
                    r.categoryId = item.primaryCategory.categoryId;
                    r.categoryName = item.primaryCategory.categoryName;
                    r.sellingState = item.sellingStatus.sellingState;
                    r.viewItemURL = item.viewItemURL;
                    r.convertedCurrentPrice = item.sellingStatus.convertedCurrentPrice.Value.ToString();
                    // payment
                    // ITEM_PAYMENT
                    if (item.paymentMethod != null && item.paymentMethod.Length > 0)
                        r.paymentMethod = item.paymentMethod[0];
                    if (item.country != null)
                        r.country = item.country;
                    // CONDITIONS
                    if (item.condition != null)
                    {
                        r.conditionId = item.condition.conditionId.ToString();
                        r.conditionDisplayName = item.condition.conditionDisplayName;
                    }

                    bool got = false;
                    foreach (R each in rsl)
                        if (each.itemId == r.itemId)
                        {
                            got = true;
                            break;
                        }
                    if (!got)
                        rsl.Add(r);
                }

            return rsl;
        }
    }
    public struct RSL
    {
        public string A_itemId;
        public string A_title;
        public string A_convertedCurrentPrice;
        public string A_viewItemURL;
        public string F_itemId;
        public string F_title;
        public string F_convertedCurrentPrice;
        public string F_viewItemURL;
        public int score;
        public double priceGAP;
    }

    public struct R
    {
        public string itemId;
        public string title;
        public string categoryId;
        public string categoryName;
        public string sellingStateID_;
        public string sellingState;
        public string viewItemURL;
        public string convertedCurrentPrice;
        public string paymentMethodID_;
        public string paymentMethod;
        public string countryID_;
        public string country;
        public string conditionId;
        public string conditionDisplayName;

        public R(bool init)
        {
            itemId = "";
            title = "";
            categoryId = "";
            categoryName = "";
            sellingStateID_ = "";
            sellingState = "";
            viewItemURL = "";
            convertedCurrentPrice = "";
            paymentMethodID_ = "";
            paymentMethod = "";
            countryID_ = "";
            country = "";
            conditionId = "";
            conditionDisplayName = "";
        }
    }
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
        public void HDL_Payment(ref R r, SqlCommand cmd)
        {
            SqlDataReader reader;

            cmd.CommandText = "select paymentMethodID from PAYMENT where paymentMethod = '" + r.paymentMethod + "'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                r.paymentMethodID_ = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                cmd.CommandText = "insert PAYMENT (paymentMethod) values ('" + r.paymentMethod + "')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "select paymentMethodID from PAYMENT where paymentMethod = '" + r.paymentMethod + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    r.paymentMethodID_ = reader[0].ToString();
                reader.Close();
            }
        }
        public void HDL_Country(ref R r, SqlCommand cmd)
        {
            SqlDataReader reader;

            cmd.CommandText = "select countryID from COUNTRY where country = '" + r.country + "'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                r.countryID_ = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                cmd.CommandText = "insert COUNTRY (country) values ('" + r.country + "')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "select countryID from COUNTRY where country = '" + r.country + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    r.countryID_ = reader[0].ToString();
                reader.Close();
            }
        }
        public void HDL_Category(ref R r, SqlCommand cmd)
        {
            SqlDataReader reader;

            cmd.CommandText = "select categoryId from CATEGORY where categoryId = '" + r.categoryId + "'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
                reader.Close();
            else
            {
                reader.Close();
                cmd.CommandText = "insert CATEGORY (categoryId, categoryName) values ('" + r.categoryId + "', '" + r.categoryName + "') ";
                cmd.ExecuteNonQuery();
            }
        }
        public void HDL_Conditions(ref R r, SqlCommand cmd)
        {
            SqlDataReader reader;

            cmd.CommandText = "select conditionId from CONDITIONS where conditionId = '" + r.conditionId + "'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
                reader.Close();
            else
            {
                reader.Close();
                cmd.CommandText = "insert CONDITIONS (conditionId, conditionDisplayName) values ('" + r.conditionId + "', '" + r.conditionDisplayName + "') ";
                cmd.ExecuteNonQuery();
            }
        }
        public void HDL_SellingState(ref R r, SqlCommand cmd)
        {
            SqlDataReader reader;

            cmd.CommandText = "select sellingStateID from SELLINGSTATE where sellingState = '" + r.sellingState + "'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                r.sellingStateID_ = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                cmd.CommandText = "insert SELLINGSTATE (sellingState) values ('" + r.sellingState + "')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "select sellingStateID from SELLINGSTATE where sellingState = '" + r.sellingState + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    r.sellingStateID_ = reader[0].ToString();
                reader.Close();
            }
        }


        public bool NeedRedo_Keyword(string keyword, out string old_keywordId)
        {
            old_keywordId = "";
            using (SqlConnection conn = new SqlConnection(ConnSTR_PR))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Connection.Open();
                SqlDataReader reader;

                cmd.CommandText = "select keywordID, update_mark from KEYWORD_HIS where keyword = '" + keyword + "'";
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    old_keywordId = reader[0].ToString();
                    DateTime update_mark = DateTime.Parse(reader[1].ToString());
                    reader.Close();

                    cmd.CommandText = "select getdate()";
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    DateTime now = DateTime.Parse(reader[0].ToString());
                    reader.Close();

                    TimeSpan difTime = now - update_mark;
                    if (difTime.TotalMinutes > expireMin)
                        return true;
                    else 
                        using (SqlConnection conn_2 = new SqlConnection(ConnSTR_se))
                        {
                            SqlCommand cmd_2 = new SqlCommand();
                            cmd_2.Connection = conn_2;
                            cmd_2.Connection.Open();
                            SqlDataReader reader_2;

                            cmd_2.CommandText = "select keywordID, update_mark from KEYWORD_HIS where keywordID = '" + old_keywordId + "'";
                            reader_2 = cmd_2.ExecuteReader();
                            if (reader_2.Read())
                            {
                                reader_2.Close();
                                return false;
                            }
                            reader_2.Close();
                            return true;
                        } 
                }
                else
                {
                    reader.Close();
                    return true;
                }
            }
        }

        /// <summary>  
        ///  判断是否有非法字符 
        /// </summary>  
        /// <param name="strString"></param>  
        /// <returns>返回TRUE表示有非法字符，返回FALSE表示没有非法字符。</returns>  
        /// 
        static bool CheckBadStr(string strString)
        {
            if (strString != null && strString.Length > 0)
            {
                string[] bidStrlist = new string[24];
                bidStrlist[0] = "'";
                bidStrlist[1] = ";";
                bidStrlist[2] = ":";
                bidStrlist[3] = "%";
                bidStrlist[4] = "@";
                bidStrlist[5] = "&";
                bidStrlist[6] = "#";
                bidStrlist[7] = "\"";
                bidStrlist[8] = "net user";
                bidStrlist[9] = "exec";
                bidStrlist[10] = "net localgroup";
                bidStrlist[11] = "select";
                bidStrlist[12] = "asc";
                bidStrlist[13] = "char";
                bidStrlist[14] = "mid";
                bidStrlist[15] = "insert";
                bidStrlist[16] = "order";
                bidStrlist[17] = "exec";
                bidStrlist[18] = "delete";
                bidStrlist[19] = "drop";
                bidStrlist[20] = "truncate";
                bidStrlist[21] = "xp_cmdshell";
                bidStrlist[22] = "<";
                bidStrlist[23] = ">";
                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Length; i++)
                    if (tempStr.IndexOf(bidStrlist[i]) != -1)
                        //if (tempStr == bidStrlist[i]) 
                        return true;
            }
            return false;
        } 
        public List<RSL> Query(string keyword, int _entriesPerPage)
        {
            if (CheckBadStr(keyword))
                return null;

            keyword = keyword.Trim();
            string old_keywordId;
            if (NeedRedo_Keyword(keyword, out old_keywordId))
                using (SqlConnection conn = new SqlConnection(ConnSTR_PR))
                {
                    conn.Open();

                    SearchSrv ss = new SearchSrv(_entriesPerPage);
                    List<R> list_F = ss.Rsl(keyword, "FixedPrice");
                    List<R> list_A = ss.Rsl(keyword, "AuctionWithBIN");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn; 
                    SqlTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;

                    if (old_keywordId != "")
                    {
                        cmd.CommandText = "delete ITEM where itemid in (select itemid from KEYWORD_ITEM where keywordID = '" + old_keywordId + "')";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "delete KEYWORD_ITEM where keywordID = '" + old_keywordId + "'";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "delete KEYWORD_HIS where keyword = '" + keyword + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert KEYWORD_HIS (keyword, update_mark) values ('" + keyword + "', getdate())";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select keywordID from KEYWORD_HIS where keyword = '" + keyword + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    string new_keywordId = reader[0].ToString();
                    reader.Close();

                    for (int i = 0; i < list_F.Count; i++)
                    {
                        R refR = list_F[i];
                        HDL_Item(ref refR, cmd, new_keywordId, "F");
                    }
                    for (int i = 0; i < list_A.Count; i++)
                    {
                        R refR = list_A[i];
                        HDL_Item(ref refR, cmd, new_keywordId, "A");
                    }

                    tran.Commit();

                    // query :
                    List<RSL> rsl = GiveMeTable(conn, keyword);
                    conn.Close();
                    return rsl;
                }
            else
                using (SqlConnection conn = new SqlConnection(ConnSTR_se))
                {
                    conn.Open();

                    // query :
                    List<RSL> rsl = GiveMeTable(conn, keyword);
                    conn.Close();
                    return rsl;
                }
        }
        
        int GiveMeTable_Score(string[] a, string[] f)
        {
            int score = 0;
            if (a != null && f != null)
                foreach (string eacha in a)
                    foreach (string eachf in f)
                        if (eacha == eachf)
                            score++;
            return score;
        }
        string Refine_STR(string str)
        {
            str = str.Replace("<", " ");
            str = str.Replace(">", " ");
            str = str.Replace("/", " ");
            str = str.Replace(@"\", " ");
            str = str.Replace("|", " ");
            str = str.Replace("(", " ");
            str = str.Replace(")", " ");
            str = str.Replace("-", " ");

            return str;
        }
        string[] Refine(string[] str)
        {
            string[] rsl = new string[str.Length];
            int r = 0;
            int s = 0;
            foreach (string each in str)
            {
                if (each.Length > 1)
                {
                    rsl[r] = each;
                    r++;
                }
                s++;
            }

            string[] outcome = new string[r];
            for (int i = 0; i < outcome.Length; i++)
                outcome[i] = rsl[i];

            return outcome;
        }

        List<RSL> GiveMeTable(SqlConnection conn, string keyword)
        {
            List<RSL> A = GiveMeTable_Matrix(conn, keyword, "A");
            List<RSL> F = GiveMeTable_Matrix(conn, keyword, "F");

            for (int i = 0; i < A.Count; i++)
            {
                string[] cp_A       = Regex.Split(Refine_STR( A[i].A_title ), " ", RegexOptions.IgnoreCase);
                string[] cp_A_ref   = Refine(cp_A);


                int score = -1;
                foreach (RSL f in F)
                {
                    string[] cp_F       = Regex.Split(Refine_STR( f.F_title ), " ", RegexOptions.IgnoreCase);
                    string[] cp_F_ref   = Refine(cp_F);
                    int dyn_score = GiveMeTable_Score(cp_A_ref, cp_F_ref);
                    if (score < dyn_score)
                    {
                        score = dyn_score;

                        RSL Ai = A[i];
                        Ai.score        = score;
                        Ai.F_itemId     = f.F_itemId;
                        Ai.F_title      = f.F_title;
                        Ai.F_convertedCurrentPrice = f.F_convertedCurrentPrice;
                        Ai.F_viewItemURL = f.F_viewItemURL;

                        try {
                            Ai.priceGAP = double.Parse( Ai.F_convertedCurrentPrice ) - double.Parse( Ai.A_convertedCurrentPrice );
                        }
                        catch { }
                        A[i] = Ai;
                    }
                }
            }

            return A;
        }
        List<RSL> GiveMeTable_Matrix(SqlConnection conn, string keyword, string af)
        {
            string queryStr = @"select ITEM.itemid, ITEM.title, item.convertedCurrentPrice, item.viewItemURL
FROM 
item join KEYWORD_ITEM ON ITEM.ITEMID = KEYWORD_ITEM.ITEMID 
JOIN KEYWORD_HIS ON KEYWORD_ITEM.keywordID = KEYWORD_HIS.keywordID
WHERE KEYWORD_HIS.keyword = '" + keyword + "' and KEYWORD_ITEM.af = '"+af+"'";

            List<RSL> rsl_list = new List<RSL>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = queryStr;
                SqlDataReader reader =  cmd.ExecuteReader();
                while (reader.Read())
                {
                    RSL rsl = new RSL();
                    if (af == "A")
                    {
                        rsl.A_itemId            = reader[0].ToString();
                        rsl.A_title             = reader[1].ToString();
                        rsl.A_convertedCurrentPrice = reader[2].ToString();
                        rsl.A_viewItemURL       = reader[3].ToString();
                    }
                    else
                    { 
                        rsl.F_itemId            = reader[0].ToString();
                        rsl.F_title             = reader[1].ToString();
                        rsl.F_convertedCurrentPrice = reader[2].ToString();
                        rsl.F_viewItemURL       = reader[3].ToString();
                    }
                    rsl_list.Add(rsl);
                }
                reader.Close();
            }
            return rsl_list;
        }
    }
}
