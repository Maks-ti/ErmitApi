

using NLog;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;


namespace ErmitApi;

[LayoutRenderer("intercept")]
public class LayoutRendererWrapper : WrapperLayoutRendererBase
{
    protected override string RenderInner(LogEventInfo logEvent)
    {
        return base.RenderInner(logEvent);
    }

    protected override string Transform(string text)
    {
        return text;
    }
}
