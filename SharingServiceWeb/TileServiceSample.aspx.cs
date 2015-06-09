//-----------------------------------------------------------------------
// <copyright file="TileServiceSample.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Research.Wwt.SharingService.Web
{
    /// <summary>
    /// Code behind for the Tile service page which gets the details about tiles through WCF service.
    /// </summary>
    public partial class TileServiceSample : System.Web.UI.Page
    {
        #region Properties

        /// <summary>
        /// Gets or sets grid view direction
        /// </summary>
        public SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                {
                    ViewState["sortDirection"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["sortDirection"];
            }
            set
            {
                ViewState["sortDirection"] = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page_Load implementation which gets details about tiles.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gridViewTile.DataBound += new EventHandler(OnGridViewTileDataBound);

                string virtualDirectory = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;

                // Need to get the location where WCF services are hosted.
                string wcfServicePath = string.Format(
                        CultureInfo.InvariantCulture,
                        Constants.TileServiceWcfClientPathFormat,
                        Request.Url.Scheme,
                        Request.Url.Host,
                        Request.Url.Port,
                        virtualDirectory);
                Uri address = new Uri(Request.Url, new Uri(wcfServicePath));

                // Initialize the WCF service client with the current server's address.
                using (TileServiceClient client = new TileServiceClient("CustomBinding_ITileService", address.AbsoluteUri))
                {
                    try
                    {
                        PyramidDetails pyramidDetails = client.GetPyramidDetails();

                        if (pyramidDetails.Pyramids.Count > 0)
                        {
                            // By default the pyramid details is sorted descending order on Date created column.
                            GridViewSortDirection = SortDirection.Descending; 
                            List<Pyramid> orderedPyramids = pyramidDetails.Pyramids.OrderByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                            Session["pyramids"] = orderedPyramids;
                            labelErrorMessage.Visible = false;
                            gridViewTile.DataSource = orderedPyramids;
                            gridViewTile.DataBind();
                        }
                        else
                        {
                            labelErrorMessage.Visible = true;
                            labelError.Text = Properties.Resources.PyramidFolderNotFoundError;
                            this.HidePreLoader();
                        }
                    }
                    catch (FaultException)
                    {
                        labelErrorMessage.Visible = true;
                        labelError.Text = Properties.Resources.GenericErrorMessage;
                        HidePreLoader();
                    }
                    catch (TimeoutException)
                    {
                        labelErrorMessage.Visible = true;
                        labelError.Text = Properties.Resources.TimeoutErrorMessage;
                        HidePreLoader();
                    }
                }
            }
        }

        /// <summary>
        /// Event is fired after the grid view is bound with the data.
        /// </summary>
        /// <param name="sender">Tile grid view control</param>
        /// <param name="e">Event arguments</param>
        protected void OnGridViewTileDataBound(object sender, EventArgs e)
        {
            UpdateGridData();
            HidePreLoader();
        }

        /// <summary>
        /// On page load completes the pre loader is hidden.
        /// </summary>
        protected void HidePreLoader()
        {
            string script = "<SCRIPT LANGUAGE='JavaScript'> ";
            script += "HidePreloader()";
            script += "</SCRIPT>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "clientScript", script);
        }

        /// <summary>
        /// Event is fired on the sorting of the grid view columns
        /// </summary>
        /// <param name="sender">Tile details grid view</param>
        /// <param name="e">Grid view event arguments</param>
        protected void OnGridViewTileSorting(object sender, GridViewSortEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.SortExpression))
            {
                List<Pyramid> pyramidDetails = Session["pyramids"] as List<Pyramid>;
                ViewState["SortExpression"] = e.SortExpression;
                if (pyramidDetails != null)
                {
                    List<Pyramid> orderDetails = GetOrderedList(e.SortExpression, GridViewSortDirection, pyramidDetails);
                    if (orderDetails != null)
                    {
                        switch (GridViewSortDirection)
                        {
                            case SortDirection.Ascending:
                                GridViewSortDirection = SortDirection.Descending;
                                break;
                            case SortDirection.Descending:
                                GridViewSortDirection = SortDirection.Ascending;
                                break;
                            default:
                                break;
                        }

                        gridViewTile.DataSource = orderDetails;
                        gridViewTile.DataBind();

                        // The service path and labels has to be updated as row data bound is not called in 
                        // update panel.
                        UpdateGridData();
                    }
                }
            }
        }

        /// <summary>
        /// Event is fired after the grid is sorted based on the field.
        /// </summary>
        /// <param name="sender">Tile details grid</param>
        /// <param name="e">Event arguments</param>
        protected void OnGridViewTileSorted(object sender, EventArgs e)
        {
            foreach (DataControlField field in gridViewTile.Columns)
            {
                // Add the sort ascending/descending icon on the basis of sort expression
                if (field.SortExpression == (string)ViewState["SortExpression"])
                {
                    if (GridViewSortDirection == SortDirection.Ascending)
                    {
                        field.HeaderStyle.CssClass = Constants.SortAscendingStyle;
                    }
                    else
                    {
                        field.HeaderStyle.CssClass = Constants.SortDescendingStyle;
                    }
                }
                else
                {
                    field.HeaderStyle.CssClass = Constants.ColumnDefaultStyle;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the ordered list for the tile details on the basis of the
        /// sort expression.
        /// </summary>
        /// <param name="sortExpression">Sort expression on the basis of which the columns are decided for sorting</param>
        /// <param name="sortDirection">Sort direction(ASC/DESC) on the basis of which the columns are decided for sorting</param>
        /// <param name="pyramidDetails">Pyramid details</param>
        /// <returns>Ordered list of pyramid details</returns>
        private static List<Pyramid> GetOrderedList(string sortExpression, SortDirection sortDirection, List<Pyramid> pyramidDetails)
        {
            List<Pyramid> orderedPyramidDetails = null;
            if (sortDirection == SortDirection.Descending)
            {
                orderedPyramidDetails = GetAscendingSortList(sortExpression, pyramidDetails, orderedPyramidDetails);
            }
            else
            {
                orderedPyramidDetails = GetDescendingSortList(sortExpression, pyramidDetails, orderedPyramidDetails);
            }
            return orderedPyramidDetails;
        }

        /// <summary>
        /// Gets sorted list on ascending order for tile details
        /// </summary>
        /// <param name="sortExpression">Sort expression used to sorting</param>
        /// <param name="pyramidDetails">Pyramid details</param>
        /// <param name="orderedPyramidDetails">Ordered pyramid details</param>
        /// <returns>Ordered list in ascending order</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The code requires switch statements.")]
        private static List<Pyramid> GetDescendingSortList(string sortExpression, List<Pyramid> pyramidDetails, List<Pyramid> orderedPyramidDetails)
        {
            switch (sortExpression)
            {
                case "Name":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.Name).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "ProjectType":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.ProjectType).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "DateCreated":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "Levels":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => Convert.ToInt32(pyramid.WtmlDetails.Levels, CultureInfo.InvariantCulture)).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "IsDemEnabled":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.IsDemEnabled).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "Credit":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.Credit).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "CreditURL":
                    orderedPyramidDetails = pyramidDetails.OrderByDescending(pyramid => pyramid.WtmlDetails.CreditPath).ThenByDescending(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                default:
                    break;
            }
            return orderedPyramidDetails;
        }

        /// <summary>
        /// Gets sorted list on descending order for tile details
        /// </summary>
        /// <param name="sortExpression">Sort expression used to sorting</param>
        /// <param name="pyramidDetails">Pyramid details</param>
        /// <param name="orderedPyramidDetails">Ordered pyramid details</param>
        /// <returns>Ordered list in descending order</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The code requires switch statements.")]
        private static List<Pyramid> GetAscendingSortList(string sortExpression, List<Pyramid> pyramidDetails, List<Pyramid> orderedPyramidDetails)
        {
            switch (sortExpression)
            {
                case "Name":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.Name).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "ProjectType":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.ProjectType).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "DateCreated":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "Levels":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => Convert.ToInt32(pyramid.WtmlDetails.Levels, CultureInfo.InvariantCulture)).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "IsDemEnabled":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.IsDemEnabled).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "Credit":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.Credit).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                case "CreditURL":
                    orderedPyramidDetails = pyramidDetails.OrderBy(pyramid => pyramid.WtmlDetails.CreditPath).ThenBy(pyramid => pyramid.WtmlDetails.DateCreated).ToList();
                    break;
                default:
                    break;
            }
            return orderedPyramidDetails;
        }

        /// <summary>
        /// Updates the service path and DEM included and date created pattern.
        /// </summary>
        private void UpdateGridData()
        {
            foreach (GridViewRow gridRow in gridViewTile.Rows)
            {
                Literal dateCreatedLiteral = (Literal)gridRow.FindControl("lblDateCreated");
                if (dateCreatedLiteral != null)
                {
                    DateTime dateCreated;
                    if (DateTime.TryParse(dateCreatedLiteral.Text, out dateCreated))
                    {
                        dateCreatedLiteral.Text = dateCreated.ToLongDateString();
                    }
                }

                Literal demIncludedLiteral = (Literal)gridRow.FindControl("lblIsDemIncluded");
                if (demIncludedLiteral != null)
                {
                    bool isDemIncluded;
                    if (bool.TryParse(demIncludedLiteral.Text, out isDemIncluded))
                    {
                        if (isDemIncluded)
                        {
                            demIncludedLiteral.Text = Properties.Resources.DEMYes;
                        }
                        else
                        {
                            demIncludedLiteral.Text = Properties.Resources.DEMNo;
                        }
                    }
                }

                string virtualDirectory = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;
                string servicePath = string.Format(
                                 CultureInfo.InvariantCulture,
                                 Constants.TileServicePathFormat,
                                 Request.Url.Scheme,
                                 Request.Url.Host,
                                 Request.Url.Port,
                                 virtualDirectory);

                if (!string.IsNullOrEmpty(servicePath))
                {
                    // Updates the service path with virtual directory path
                    HyperLink downloadLink = (HyperLink)gridRow.FindControl("downloadLink");
                    if (downloadLink != null)
                    {
                        downloadLink.NavigateUrl = servicePath + downloadLink.NavigateUrl;
                    }

                    Image image = (Image)gridRow.FindControl("thumbnailImage");
                    if (image != null)
                    {
                        image.ImageUrl = servicePath + image.ImageUrl;
                    }
                }
            }
        }

        #endregion
    }
}