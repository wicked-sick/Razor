#pragma checksum "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/InjectWithModel.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a039b7091118c718dc3023b6ac58d9645cb58e59"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.TestFiles_IntegrationTests_CodeGenerationIntegrationTest_InjectWithModel), @"mvc.1.0.view", @"TestFiles/IntegrationTests/CodeGenerationIntegrationTest/InjectWithModel")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/TestFiles/IntegrationTests/CodeGenerationIntegrationTest/InjectWithModel.cshtml", typeof(AspNetCore.TestFiles_IntegrationTests_CodeGenerationIntegrationTest_InjectWithModel))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a039b7091118c718dc3023b6ac58d9645cb58e59", @"TestFiles/IntegrationTests/CodeGenerationIntegrationTest/InjectWithModel")]
    public class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_InjectWithModel : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MyModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public MyService<MyModel> Html { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public MyApp MyPropertyName { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
    }
}
#pragma warning restore 1591
