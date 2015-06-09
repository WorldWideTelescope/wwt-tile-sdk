<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="CommunityServiceSample.aspx.cs" Inherits="Microsoft.Research.Wwt.SharingService.Web.CommunityServiceSample" culture="auto" uiculture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <link href="Resources/Style.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><asp:Literal ID="Heading" Runat="server" meta:resourcekey="pageTitle"></asp:Literal></title>
</head>
<body>
    <form id="CommunityToolForm" runat="server">
   <!-- Gray Lines -->
   
    <!-- Image Header  -->
    <div class="headerBg">
        <img src="Resources/Title.PNG" width="317" height="100" alt="" style="position:relative;float:left" />
        <img src="Resources/CommunityHeaderBG.png" width="391" alt="" height="100" style=" position:relative;float:right;"/>
    </div>
        <div class="infoDiv">
            <span class="infoSpan"><asp:Literal ID="information" Runat="server" meta:resourcekey="spanInformation"></asp:Literal></span>
        </div>
        <div style="padding-left: 25px; padding-right: 25px;">
            <div id="labelNoCommunitiesDiv" class="errorMessage" runat="server">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td><asp:Image CssClass="imageStyle" ImageUrl="Resources/Error.png" ID="errorIcon" runat="server" /></td>
                        <td><asp:Literal ID="labelNoCommunities" runat="server" meta:resourcekey="labelNoCommunitiesResource" /></td>
                    </tr>
                </table>
            </div>
            <asp:GridView ID="gridViewCommunity" runat="server" CellPadding="0" 
                CellSpacing="0" AutoGenerateColumns="False" ShowHeader="False" 
                CssClass="mainTable" RowStyle-CssClass="ItemContainerTableStyle" >
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle BackColor="#1384c3" Width="3px" BorderWidth="0" Height="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="numberContainertd" />
                        <ItemTemplate>
                            <div class="numberContainer">
                                <asp:Literal ID="lblRowCount" Runat="server" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="thumbnailContainertd" />
                        <ItemTemplate>
                            <asp:HyperLink ID="thumbnailLink" NavigateUrl='<%# DataBinder.Eval(Container, "DataItem.SignUpFile") %>' runat="server">
                                <asp:Image CssClass="imageLinkStyle" ID="thumbnailImage" ImageAlign="Middle" Width="176px" Height="45px" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.Thumbnail") %>' Runat="server" />
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle CssClass="communityDetailsContainer" />
                        <ItemTemplate>
                            <div>
                                <div>
                                    <asp:HyperLink 
                                        ID="singupLink" 
                                        NavigateUrl='<%# DataBinder.Eval(Container, "DataItem.SignUpFile") %>' 
                                        Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>' 
                                        runat="server"/>
                                </div>
                                <div style="margin-left:5px; font-size:11px; margin-top:5px; color:#3e3e3e;">
                                    <asp:Literal ID="description" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' runat="server"/>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>