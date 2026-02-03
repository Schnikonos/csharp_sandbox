using RazorLight;

namespace MyApp.Templating
{
    public class HtmlTemplateRenderer
    {
        private readonly RazorLightEngine engine;

        public HtmlTemplateRenderer()
        {
            engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(HtmlTemplateRenderer))
                .UseMemoryCachingProvider()
                .Build();
        }

        public Task<string> RenderAsync<T>(string templateKey, T model)
        {
            return engine.CompileRenderAsync(templateKey, model);
        }
    }
}
