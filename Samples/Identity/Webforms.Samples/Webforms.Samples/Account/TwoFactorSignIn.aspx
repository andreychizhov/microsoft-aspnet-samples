<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TwoFactorSignIn.aspx.cs" Async="true" Inherits="Webforms.Samples.Account.TwoFactorSignIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder runat="server" ID="sendcode">
        <section>
            <h2>Send Verification Code</h2>
            <div class="row">
                <div class="col-md-8">
                    Two Factor Authentication Provider:
            <asp:DropDownList runat="server" ID="Providers">
            </asp:DropDownList>
                    <asp:Button Text="Submit" ID="ProviderSubmit" OnClick="ProviderSubmit_Click" CssClass="btn btn-default" runat="server" />
                </div>
            </div>
        </section>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="verifycode" Visible="false">
        <section>
            <h2>Enter Verification Code</h2>
            <div>
                <asp:Label Text="" ID="DemoText" CssClass="text-danger" runat="server" />
            </div><div />
            <asp:HiddenField ID="SelectedProvider" runat="server" />
            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                <p class="text-danger">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
            </asp:PlaceHolder>
            <div class="form-group">
                <asp:Label Text="Code:" runat="server" AssociatedControlID="Code" CssClass="col-md-1 control-label" />
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="Code" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                    <div class="checkbox">
                        <asp:Label Text="Remember Browser" runat="server" />
                        <asp:CheckBox Text="" ID="RememberBrowser" runat="server" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                    <asp:Button Text="Submit" ID="CodeSubmit" OnClick="CodeSubmit_Click" CssClass="btn btn-default" runat="server" />
                </div>
            </div>
        </section>
    </asp:PlaceHolder>
</asp:Content>
