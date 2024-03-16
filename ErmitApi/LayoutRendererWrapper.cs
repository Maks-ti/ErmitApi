

using NLog;
using NLog.LayoutRenderers.Wrappers;


namespace ErmitApi;

public class LayoutRendererWrapper : WrapperLayoutRendererBase
{
    protected override string RenderInner(LogEventInfo logEvent)
    {
        return "";
    }

    protected override string Transform(string text)
    {
        return text;
    }
}
