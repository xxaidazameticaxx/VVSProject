#pragma checksum "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8378285204f78c17d999e7016238eb5d9a975989"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\_ViewImports.cshtml"
using Ayana;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\_ViewImports.cshtml"
using Ayana.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8378285204f78c17d999e7016238eb5d9a975989", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"173c1fb09878f2d54cb847bb1f18422e0ceac569", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-area", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Subscriptions", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("margin-left:80px;margin-top:30px;margin-bottom:25px; background-color:grey; border-color:darkgrey; -webkit-text-fill-color:white;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<style>
    body{
    background-color:white;
    font-family:'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;
    overflow:auto;
    }
    .boxed{
        background-color: #fbf8f9;
    }
    * {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

    .image-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        grid-gap: 10px;
    }

    .image-item {
        display: flex;
        justify-content: center;
        align-items: center;
    }

      
</style>
<div class=""row slideanim"" style=""transition:ease-in-out;"">
    <div  style=""background: #fbf8f9;text-align:left;padding:1em 0; margin-left:10px;  width:85%; height:400%;"">
        <h1 style=""margin-left:50px; margin-top:10px;font-style:italic;"">Say it with flowers</h1>
        <p style=""margin-left:50px;margin-top:10px;font-style:italic;"">Whatever the occasion, our flowers make it special!</p>
        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8378285204f78c17d999e7016238eb5d9a9759895761", async() => {
                WriteLiteral(" Subsription");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
   </div>
    <img  src=""https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQqAT5vptItn_pU8w3O25sGfAulMy1N9LZ8YnIQEOWSWNNd-38b"" style=""float:right; margin-right:20px;"" height=""137"">
</div>
<br>
<p style=""text-align:center; font-size:x-large; font-style:italic; font-weight:300;"">Best sellers</p>
<br>
<div class=""row text-center"">
");
#nullable restore
#line 46 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml"
     foreach (Product url in ViewBag.bestSellers)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"col-sm-4\">\r\n            <div>\r\n                <img src=\"https://e0.pxfuel.com/wallpapers/996/249/desktop-wallpaper-earth-white-tulip-white-tulips.jpg\"");
            BeginWriteAttribute("alt", " alt=\"", 1855, "\"", 1861, 0);
            EndWriteAttribute();
            WriteLiteral(" width=\"200\" height=\"200\">\r\n                <p><strong>");
#nullable restore
#line 51 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml"
                      Write(url.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong></p>\r\n                <p>BAM ");
#nullable restore
#line 52 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml"
                  Write(url.Price);

#line default
#line hidden
#nullable disable
            WriteLiteral(".00</p>\r\n            </div>\r\n        </div>\r\n");
#nullable restore
#line 55 "C:\Users\User\Desktop\FAKULTET\OOAD\Projekat-OOAD\Projekat-OOAD\Ayana\Views\Home\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
<div class=""row slideanim"" style=""transition:ease-in-out; margin-top:-2px; "" >
    <div style=""background: #ddeffb;text-align:left;padding:1em 0; margin-left:10px;  width:85%; height:400%;"">
        <h1 style=""margin-left:50px; margin-top:10px;font-style:italic;"">cheer someone up...</h1>
        <p style=""margin-left:50px;margin-top:10px;font-style:italic;"">Because you don't need a special occasion to suprise the one you love</p>
        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8378285204f78c17d999e7016238eb5d9a9759899661", async() => {
                WriteLiteral(" Shop now");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
    </div>
    <img src=""https://cdn.shopify.com/s/files/1/0507/3754/5401/t/1/assets/CLMD_LOL_preset_proflowers-mx-tile-wide-lv-new.jpeg?v=1662494477"" style=""float:right; margin-right:20px;"" height=""137"">
</div>
<br>
<p style=""text-align:center; font-size:x-large; font-style:italic; font-weight:300;"">Shop popular flowers</p>
<br>
<div class=""row text-center"">
    <div class=""col-sm-4"">
        <div>
            <img src=""https://i.pinimg.com/originals/2f/7e/01/2f7e0131e0127fdf2e1d114e3f487905.jpg?v=1680812238""");
            BeginWriteAttribute("alt", " alt=\"", 3222, "\"", 3228, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Roses </strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-4"">
        <div>
            <img src=""https://e0.pxfuel.com/wallpapers/996/249/desktop-wallpaper-earth-white-tulip-white-tulips.jpg""");
            BeginWriteAttribute("alt", " alt=\"", 3509, "\"", 3515, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Tulips</strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-4"">
        <div>
            <img src=""https://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Sunflower_sky_backdrop.jpg/1200px-Sunflower_sky_backdrop.jpg""");
            BeginWriteAttribute("alt", " alt=\"", 3821, "\"", 3827, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Sunflowers</strong></p>
            <p></p>
        </div>
    </div>
    </div>
<div class=""row slideanim"" style=""transition:ease-in-out;"">
    <div style=""background: #f7ede5;text-align:left;padding:1em 0; margin-left:10px;  width:85%; height:400%;"">
        <h1 style=""margin-left:50px; margin-top:10px;font-style:italic;"">happy birthday</h1>
        <p style=""margin-left:50px;margin-top:10px;font-style:italic;"">Celebrate your loved ones</p>

        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8378285204f78c17d999e7016238eb5d9a97598913394", async() => {
                WriteLiteral(" Shop now");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
    </div>
    <img src=""https://cdn.shopify.com/s/files/1/0507/3754/5401/t/1/assets/V5449D_LOL_preset_proflowers-mx-hero-lv-new.jpeg?v=1637201054&v=1680812238"" style=""float:right; margin-right:20px;"" height=""137"">
</div>
<br>
<p style=""text-align:center; font-size:x-large; font-style:italic; font-weight:300;"">Birthday best sellers</p>
<br>
<div class=""row text-center"">
    <div class=""col-sm-4"">
        <div>
            <img src=""https://placehold.it/150x80?text=IMAGE""");
            BeginWriteAttribute("alt", " alt=\"", 5047, "\"", 5053, 0);
            EndWriteAttribute();
            WriteLiteral(" width=\"200\" height=\"200\">\r\n            <p><strong></strong></p>\r\n            <p></p>\r\n        </div>\r\n    </div>\r\n    <div class=\"col-sm-4\">\r\n        <div>\r\n            <img src=\"https://placehold.it/150x80?text=IMAGE\"");
            BeginWriteAttribute("alt", " alt=\"", 5273, "\"", 5279, 0);
            EndWriteAttribute();
            WriteLiteral(" width=\"200\" height=\"200\">\r\n            <p><strong></strong></p>\r\n            <p></p>\r\n        </div>\r\n    </div>\r\n\r\n\r\n    <div class=\"col-sm-4\">\r\n        <div>\r\n            <img src=\"https://placehold.it/150x80?text=IMAGE\"");
            BeginWriteAttribute("alt", " alt=\"", 5503, "\"", 5509, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong></strong></p>
            <p></p>
        </div>
    </div>
</div>

<div class=""row slideanim"" style=""transition:ease-in-out;"">
    <div style=""background: #dae1f1;text-align:left;padding:1em 0; margin-left:10px;  width:85%; height:400%;"">
        <h1 style=""margin-left:50px; margin-top:10px;font-style:italic;"">for mom</h1>
        <p style=""margin-left:50px;margin-top:10px;font-style:italic;"">Suprise hers</p>

        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "8378285204f78c17d999e7016238eb5d9a97598916943", async() => {
                WriteLiteral(" Shop Mother\'s day");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
    </div>
    <img src=""https://cdn.shopify.com/s/files/1/0507/3754/5401/t/1/assets/L5496D_LOL_preset_proflowers-mx-hero-lv-new.jpeg?v=1674762178&v=1680812238"" style=""float:right; margin-right:20px;"" height=""137"">
</div>
<br>
<p style=""text-align:center; font-size:x-large; font-style:italic; font-weight:300;"">Shop by occasion</p>
<br>
<div class=""row text-center"">
    <div class=""col-sm-3"">
        <div>
            <img src=""https://thumbs.dreamstime.com/b/bouquet-colorful-roses-lisianthus-lilac-flowers-vector-illustration-pink-yellow-blue-white-hydrangea-green-leaves-50384792.jpg?v=1680812238""");
            BeginWriteAttribute("alt", " alt=\"", 6834, "\"", 6840, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Mother's day</strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-3"">
        <div>
            <img src=""https://img.freepik.com/premium-vector/flower-bouquet_463755-102.jpg?w=2000&v=1680812238""");
            BeginWriteAttribute("alt", " alt=\"", 7122, "\"", 7128, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>cheer someone up</strong></p>
            <p></p>
        </div>
    </div>


    <div class=""col-sm-3"">
        <div>
            <img src=""https://img.freepik.com/premium-vector/vintage-white-flowers-bouquet-with-green-leaves-painting-watercolor_150636-178.jpg?v=1680812238""");
            BeginWriteAttribute("alt", " alt=\"", 7464, "\"", 7470, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Valentine's day</strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-3"">
        <div>
            <img src=""https://previews.123rf.com/images/naddya/naddya1709/naddya170900016/87158697-vector-bouquet-of-roses-and-flowers.jpg?v=1680812238""");
            BeginWriteAttribute("alt", " alt=\"", 7796, "\"", 7802, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>Love & romance</strong></p>
            <p></p>
        </div>
    </div>
</div>
<br>
<br>
<div class=""row text-center"">
    <div class=""col-sm-3"">
        <div>
            <img src=""https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR0bVLNgQf7g6JRGp0maeZ2iDTm5wjyj3_Ix9EDvB5VYiT9UTnN""");
            BeginWriteAttribute("alt", " alt=\"", 8153, "\"", 8159, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>thank you</strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-3"">
        <div>
            <img src=""https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcTtqMddvmPggB43olPbwj4kalf5kkjqbVhwdtggBkamIuOcrtGT""");
            BeginWriteAttribute("alt", " alt=\"", 8454, "\"", 8460, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>anniversary</strong></p>
            <p></p>
        </div>
    </div>


    <div class=""col-sm-3"">
        <div>
            <img src=""https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTCEAC6WqOSHkc9MPDlqstzyNEEoYUUBzWQgg&usqp=CAU""");
            BeginWriteAttribute("alt", " alt=\"", 8756, "\"", 8762, 0);
            EndWriteAttribute();
            WriteLiteral(@" width=""200"" height=""200"">
            <p><strong>birthday</strong></p>
            <p></p>
        </div>
    </div>
    <div class=""col-sm-3"" style=""margin-bottom:100px;"">
        <div>
            <img src=""https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcSsD308WKkoZH9iv9NmYPZUBhzWVrdQlsPxuKKBDrmxzM95N-v1""");
            BeginWriteAttribute("alt", " alt=\"", 9085, "\"", 9091, 0);
            EndWriteAttribute();
            WriteLiteral(" width=\"200\" height=\"200\">\r\n            <p><strong>woman\'s day</strong></p>\r\n            <p></p>\r\n        </div>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
