using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using eBay.Services;
using eBay.Services.Finding;
using Slf;
using ConsoleFindItems;

using System.Collections.Generic;
using System.Drawing;

namespace Example
{
    public partial class _Default : System.Web.UI.Page
    {
        private FindingServicePortTypeClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Init log
            LoggerService.SetLogger(new TraceLogger());

            // Get AppID and ServerAddress from Web.config
            string appID = System.Configuration.ConfigurationManager.AppSettings["AppID"];
            string findingServerAddress = System.Configuration.ConfigurationManager.AppSettings["FindingServerAddress"];
            
            ClientConfig config = new ClientConfig();
            // Initialize service end-point configration
            config.EndPointAddress = findingServerAddress;
            
            // set eBay developer account AppID
            config.ApplicationId = appID;

            // Create a service client
            client = FindingServiceClientFactory.getServiceClient(config);

            
        }

        protected void findItem_Click(object sender, EventArgs e)
        {
            try
            {

                DB db = new DB(15);
                List<RSL> rsl = db.Query(keyword.Text, 30);

                // Show output
                if (rsl.Count > 0)
                {
                    TableRow titlerow = new TableRow();

                    TableHeaderCell titlecell_0 = new TableHeaderCell();
                    titlecell_0.Text = "A_title"; 
                    titlecell_0.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_0);

                    TableHeaderCell titlecell_1 = new TableHeaderCell();
                    titlecell_1.Text = "A_convertedCurrentPrice";
                    titlecell_1.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_1);

                    TableHeaderCell titlecell_2 = new TableHeaderCell();
                    titlecell_2.Text = "A_viewItemURL";
                    titlecell_2.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_2);

                    TableHeaderCell titlecell_3 = new TableHeaderCell(); titlecell_3.BackColor = Color.Aqua; titlecell_3.ForeColor = Color.DarkSlateGray;
                    titlecell_3.Text = "F_title";
                    titlecell_3.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_3);

                    TableHeaderCell titlecell_4 = new TableHeaderCell(); titlecell_4.BackColor = Color.Aqua; titlecell_4.ForeColor = Color.DarkSlateGray;
                    titlecell_4.Text = "F_convertedCurrentPrice";
                    titlecell_4.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_4);

                    TableHeaderCell titlecell_5 = new TableHeaderCell(); titlecell_5.BackColor = Color.Aqua; titlecell_5.ForeColor = Color.DarkSlateGray;
                    titlecell_5.Text = "F_viewItemURL";
                    titlecell_5.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_5);

                    TableHeaderCell titlecell_6 = new TableHeaderCell(); titlecell_6.BackColor = Color.Aquamarine; titlecell_6.ForeColor = Color.DarkGray;
                    titlecell_6.Text = "score";
                    titlecell_6.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_6);

                    TableHeaderCell titlecell_7 = new TableHeaderCell(); titlecell_7.BackColor = Color.Aquamarine; titlecell_7.ForeColor = Color.DarkGray;
                    titlecell_7.Text = "priceGAP";
                    titlecell_7.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell_7);

                    result.Rows.Add(titlerow);

                    foreach (RSL r in rsl)
                    {
                        TableRow tblrow = new TableRow();
                        
                        TableCell cell_0 = new TableCell();
                        cell_0.Text = r.A_title; 
                        cell_0.BorderWidth = 1;
                        tblrow.Cells.Add(cell_0);

                        TableCell cell_1 = new TableCell();
                        cell_1.Text = r.A_convertedCurrentPrice;
                        cell_1.BorderWidth = 1;
                        tblrow.Cells.Add(cell_1);

                        TableCell cell_2 = new TableCell();
                        cell_2.Text = r.A_viewItemURL;
                        cell_2.BorderWidth = 1;
                        tblrow.Cells.Add(cell_2);

                        TableCell cell_3 = new TableCell();
                        cell_3.Text = r.F_title;
                        cell_3.BorderWidth = 1;
                        tblrow.Cells.Add(cell_3);

                        TableCell cell_4 = new TableCell();
                        cell_4.Text = r.F_convertedCurrentPrice;
                        cell_4.BorderWidth = 1;
                        tblrow.Cells.Add(cell_4);

                        TableCell cell_5 = new TableCell();
                        cell_5.Text = r.F_viewItemURL;
                        cell_5.BorderWidth = 1;
                        tblrow.Cells.Add(cell_5);

                        TableCell cell_6 = new TableCell();
                        cell_6.Text = r.score.ToString();
                        cell_6.BorderWidth = 1;
                        tblrow.Cells.Add(cell_6);

                        TableCell cell_7 = new TableCell();
                        cell_7.Text = r.priceGAP.ToString();
                        cell_7.BorderWidth = 1;
                        tblrow.Cells.Add(cell_7);



                        /*
                        TableCell tblcell5 = new TableCell();
                        tblcell5.Text = "<a href=AddToWatchList.aspx?itemId=" + items[i].itemId + ">" + "Add To Watch" + "</a>";
                        tblcell5.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell5);
                        */

                        result.Rows.Add(tblrow);
                    }
                }
                
            }
            catch (Exception ex)
            {
                errorMsg.Text = ex.Message;// +"          " +ex.StackTrace;
               
            }
        }
    }
}
