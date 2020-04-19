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

        public void Rsl(string keyword, string by) // "FixedPrice"  "AuctionWithBIN"
        {
            SearchItem[] items  =  GetBy(keyword, by);
            foreach (SearchItem item in items)
            {
                // Item
                // SellingState
                Console.WriteLine("****************************");
                Console.WriteLine(item.itemId);
                Console.WriteLine(item.title);
                Console.WriteLine(item.primaryCategory.categoryId);
                        // condition 在下面
                Console.WriteLine(item.sellingStatus.sellingState);
                Console.WriteLine(item.viewItemURL);
                Console.WriteLine(item.sellingStatus.convertedCurrentPrice);
                // payment
                // ITEM_PAYMENT
                if (item.paymentMethod != null)
                    foreach(string paymentMethod in item.paymentMethod)
                        Console.WriteLine(paymentMethod);
                // SELLER  -  永远是空?
                if (item.sellerInfo != null)
                {
                    Console.WriteLine(item.sellerInfo.feedbackScore);
                    Console.WriteLine(item.sellerInfo.positiveFeedbackPercent);
                    Console.WriteLine(item.sellerInfo.feedbackRatingStar);
                }
                // CATEGORY
                Console.WriteLine(item.primaryCategory.categoryId);
                Console.WriteLine(item.primaryCategory.categoryName);
                // CONDITIONS
                if (item.condition != null)
                {
                    Console.WriteLine(item.condition.conditionId);
                    Console.WriteLine(item.condition.conditionDisplayName);
                }
                    


                int stop = 1;
            }
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
