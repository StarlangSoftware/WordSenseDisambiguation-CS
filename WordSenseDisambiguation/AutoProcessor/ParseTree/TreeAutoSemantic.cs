using AnnotatedTree;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public abstract class TreeAutoSemantic
    {
        protected abstract bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree);

        public void AutoSemantic(ParseTreeDrawable parseTree)
        {
            if (AutoLabelSingleSemantics(parseTree))
            {
                parseTree.Save();
            }
        }
    }
}