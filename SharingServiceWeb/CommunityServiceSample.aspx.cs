//-----------------------------------------------------------------------
// <copyright file="CommunityServiceSample.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Code behind for the Community service page which gets the details about communities through WCF service.
    /// </summary>
    public partial class CommunityServiceSample : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load implementation which gets details about communities.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            gridViewCommunity.RowDataBound += new GridViewRowEventHandler(GridViewCommunityRowDataBound);

            // Get the virtual directory path if any in case of service being hosted under any virtual directory.
            string virtualDirectory = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;
            
            // Need to get the location where WCF services are hosted.
            string wcfServicePath = string.Format(
                    CultureInfo.InvariantCulture, 
                    Constants.CommunityServiceWcfClientPathFormat, 
                    Request.Url.Scheme, 
                    Request.Url.Host, 
                    Request.Url.Port,
                    virtualDirectory);
            Uri address = new Uri(Request.Url, new Uri(wcfServicePath));

            // Initialize the WCF service client with the current server's address.
            using (CommunityServiceClient client = new CommunityServiceClient("CustomBinding_ICommunityService", address.AbsoluteUri))
            {
                try
                {
                    CommunityDetails communityDetails = client.GetAllCommunites();

                    if (communityDetails.Communities.Count > 0)
                    {
                        labelNoCommunitiesDiv.Visible = false;
                        gridViewCommunity.DataSource = communityDetails.Communities;
                        gridViewCommunity.DataBind();
                    }
                    else
                    {
                        labelNoCommunitiesDiv.Visible = true;
                    }
                }
                catch (FaultException ex)
                {
                    labelNoCommunitiesDiv.Visible = true;
                    labelNoCommunities.Text = ex.Message;
                }
                catch (TimeoutException)
                {
                    labelNoCommunitiesDiv.Visible = true;
                    labelNoCommunities.Text = Properties.Resources.TimeoutErrorMessage;
                }
            }
        }

        /// <summary>
        /// Generate the row number for each of the community getting bound.
        /// </summary>
        /// <param name="sender">Grid view row getting bound</param>
        /// <param name="e">Event arguments for row bound event</param>
        private void GridViewCommunityRowDataBound(object sender, GridViewRowEventArgs e)
        {
            Literal rowCountLiteral = (Literal)e.Row.FindControl("lblRowCount");
            if (rowCountLiteral != null)
            {
                rowCountLiteral.Text = Convert.ToString(e.Row.DataItemIndex + 1, CultureInfo.CurrentCulture);
            }
        }
    }
}
