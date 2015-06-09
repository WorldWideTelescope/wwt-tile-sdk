<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TileServiceSample.aspx.cs"
    Inherits="Microsoft.Research.Wwt.SharingService.Web.TileServiceSample" Culture="auto"
    UICulture="auto" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<link href="Resources/TileStyle.css" rel="stylesheet" type="text/css" />
<head runat="server">
    <title>WWT - Tile Service Sample</title>
    <script type="text/javascript">
        function HidePreloader() {
            var divstyle = new String();
            var divstyle = document.getElementById('preloaderImage').style.visibility;
            if (divstyle.toLowerCase() == "visible" || divstyle == "") {
                document.getElementById('preloaderImage').style.visibility = "hidden";
            }
            else {
                document.getElementById('preloaderImage').style.visibility = "visible";
            }
        }    
    </script>
</head>
<body>
    <div id="preloaderImage">
        <div class="preLoaderWrapper">
            <div class="preloaderInnerDiv">
            </div>
            <div class="centerDiv">
                <img src="Resources/Preloader.gif" width="16" height="16" style="margin-top: 3px;" />
                <div class="loadingText">
                    Loading...
                </div>
            </div>
        </div>
    </div>
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
    <tr>
    <td>
         <div class="headerBg">
            <img src="Resources/TileTitle.png" width="317" height="100" alt="HeaderText" style="position: relative;
                float: left" />
            <img src="Resources/CommunityHeaderBG.png" width="391" height="100" style="position: relative;
                float: right;" />
        </div>
    </td>
    </tr>
    <tr>
    <td>
      <div>
            <div id="labelErrorMessage" class="errorMessage" runat="server">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Image CssClass="errorImageStyle" ImageUrl="Resources/Error.png" ID="errorIcon"
                                runat="server" meta:resourcekey="errorIconResource1" />
                        </td>
                        <td>
                            <asp:Literal ID="labelError" runat="server" meta:resourcekey="labelNoCommunitiesError" />
                        </td>
                    </tr>
                </table>
            </div>
            <table cellpadding="0" cellspacing="0" id="pyramidTable">
                <tr>
                    <td>
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
                        </asp:ScriptManager>
                        <asp:UpdatePanel ID="gdPyramidsPanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView ID="gridViewTile" runat="server" AutoGenerateColumns="False" CssClass="mainTable"
                                    AllowSorting="True" OnSorting="OnGridViewTileSorting" OnSorted="OnGridViewTileSorted"
                                    meta:resourcekey="gridViewPyramidResource1">
                                    <Columns>
                                        <asp:TemplateField meta:resourcekey="downloadLinkHeader">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="downloadTD" />
                                            <ItemTemplate>
                                                <div>
                                                    <asp:HyperLink class="downloadButtonLink" ID="downloadLink" NavigateUrl='<%# DataBinder.Eval(Container, "DataItem.WtmlPath") %>'
                                                      Width="97px" Height="45px"  runat="server"></asp:HyperLink>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="ThumbnailHeader">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="thumbnailTD" />
                                            <ItemTemplate>
                                                <div class="outerDiv">
                                                    <div class="innerDiv">
                                                        <asp:Image ID="thumbnailImage" ImageAlign="Middle" Width="97px" Height="45px" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.ThumbNailPath") %>'
                                                            runat="server" border="0" />
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="NameHeader" SortExpression="Name">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTD" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblImageTitle" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.Name") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="ProjectTypeHeader" SortExpression="ProjectType">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTDCenter" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblImageProjectType" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.ProjectType") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="DateCreatedHeader" SortExpression="DateCreated">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTDDate" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblDateCreated" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.DateCreated") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="LevelsHeader" SortExpression="Levels">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTDCenter" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblLevels" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.Levels") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="DemIncludedHeader" SortExpression="IsDemEnabled" HeaderStyle-Width="200">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTDCenter" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblIsDemIncluded" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.IsDemEnabled") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="CreditHeader" SortExpression="Credit">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTD" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblCredit" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.Credit") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField meta:resourcekey="CreditURLHeader" SortExpression="CreditURL">
                                            <HeaderStyle CssClass="columnHeader"></HeaderStyle>
                                            <ItemStyle CssClass="textTD" />
                                            <ItemTemplate>
                                                <asp:Literal ID="lblCreditURL" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.WtmlDetails.CreditPath") %>'></asp:Literal>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <RowStyle CssClass="itemContainerTableStyle"></RowStyle>
                                    <SortedAscendingHeaderStyle CssClass="sortAscending-header" />
                                    <SortedDescendingHeaderStyle CssClass="sortDescending-header" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </td>
    </tr>
    </table>
    <div>
        <!-- Gray Lines -->
        <!-- Image Header  -->
   
      
    </div>
    </form>
</body>
</html>
