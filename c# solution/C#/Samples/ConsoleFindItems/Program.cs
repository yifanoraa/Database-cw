using System;
using eBay.Services;
using eBay.Services.Finding;
using Slf;

namespace ConsoleFindItems
{
    class SearchSrv
    {
        ILogger logger;
        ClientConfig config;
        FindingServicePortTypeClient client;
        public SearchSrv()
        {
            LoggerService.SetLogger(new ConsoleLogger());
            logger = LoggerService.GetLogger();
            config = new ClientConfig();
            config.EndPointAddress = "https://svcs.ebay.com/services/search/FindingService/v1";
            config.ApplicationId = "ZimingWu-comp0022-PRD-b69ec6afc-7ff00c17";
            client = FindingServiceClientFactory.getServiceClient(config);
        }
        public SearchItem[] GetBy(string keyword, string by)
        {
            SearchItem[] rsl;
            try
            {
                // Create request object
                FindItemsByKeywordsRequest req = new FindItemsByKeywordsRequest();
                // Set request parameters
                req.keywords = keyword;
                PaginationInput pi = new PaginationInput();
                pi.entriesPerPage = 100;
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

        public struct R
        {
            public string itemId;
            public string title;
            public string categoryId;
            public string categoryName;
            public string sellingState;
            public string viewItemURL;
            public string convertedCurrentPrice;
            public string paymentMethod;
            public string country;
            public string conditionId;
            public string conditionDisplayName;

            public R(bool init)
            {
                itemId = "";
                title = "";
                categoryId = "";
                categoryName = "";
                sellingState = "";
                viewItemURL = "";
                convertedCurrentPrice = "";
                paymentMethod = "";
                country = "";
                conditionId = "";
                conditionDisplayName = "";
            }
        }


        public R[] Rsl(string keyword, string by) // "FixedPrice"  "AuctionWithBIN"
        {
            SearchItem[] items  =  GetBy(keyword, by);

            R[] rsl     = new R[items.Length];
            int rslIDX  = 0;

            foreach (SearchItem item in items)
            {
                R r = new R(true);

                // Item
                // SellingState
                // CATEGORY
                Console.WriteLine("****************************");
                r.itemId        = item.itemId;
                r.title         = item.title;
                r.categoryId    = item.primaryCategory.categoryId;
                r.categoryName  = item.primaryCategory.categoryName;
                // condition 在下面
                r.sellingState  = item.sellingStatus.sellingState;
                r.viewItemURL   = item.viewItemURL;
                r.convertedCurrentPrice = item.sellingStatus.convertedCurrentPrice.ToString();
                // payment
                // ITEM_PAYMENT
                if (item.paymentMethod != null && item.paymentMethod.Length > 0)
                    r.paymentMethod = item.paymentMethod[0];
                // SELLER  -  永远是空?
                if (item.country != null)
                    r.country = item.country;
                // CONDITIONS
                if (item.condition != null)
                {
                    r.conditionId = item.condition.conditionId.ToString();
                    r.conditionDisplayName = item.condition.conditionDisplayName;
                }

                rsl[rslIDX] = r;
                    rslIDX++;
            }

            return rsl;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SearchSrv ss = new SearchSrv();

            ss.Rsl("apple", "AuctionWithBIN");

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();
        }
        
    }
}
