using System;
using System.Collections.Generic;
using System.Globalization;
using AnnotatedSentence;
using AnnotatedTree;
using AnnotatedTree.Processor;
using AnnotatedTree.Processor.Condition;
using MorphologicalAnalysis;
using WordNet;

namespace WordSenseDisambiguation.AutoProcessor.ParseTree
{
    public class MostFrequentTreeAutoSemantic : TreeAutoSemantic
    {
        private readonly WordNet.WordNet _turkishWordNet;
        private readonly FsmMorphologicalAnalyzer _fsm;

        /// <summary>
        /// Constructor for the {@link MostFrequentTreeAutoSemantic} class. Gets the Turkish wordnet and Turkish fst based
        /// morphological analyzer from the user and sets the corresponding attributes.
        /// </summary>
        /// <param name="turkishWordNet">Turkish wordnet</param>
        /// <param name="fsm">Turkish morphological analyzer</param>
        public MostFrequentTreeAutoSemantic(WordNet.WordNet turkishWordNet, FsmMorphologicalAnalyzer fsm)
        {
            this._turkishWordNet = turkishWordNet;
            this._fsm = fsm;
        }

        /// <summary>
        /// Returns the most frequent root word in the given synsets. In the wordnet, literals are ordered and indexed
        /// according to their usage. The most frequently used sense of the literal has sense number 1, then 2, etc. In order
        /// to get literal from root word, the algorithm checks root for a prefix and suffix. So, if the root is a prefix or
        /// suffix of a literal, it is included in the search.
        /// </summary>
        /// <param name="synSets">All possible synsets to search for most frequent literal.</param>
        /// <param name="root">Root word to be checked.</param>
        /// <returns>Synset storing most frequent literal either starting or ending with the given root form.</returns>
        private SynSet MostFrequent(List<SynSet> synSets, string root)
        {
            if (synSets.Count == 1)
            {
                return synSets[0];
            }

            var minSense = 50;
            SynSet best = null;
            foreach (var synSet in synSets)
            {
                for (int i = 0; i < synSet.GetSynonym().LiteralSize(); i++)
                {
                    if (synSet.GetSynonym().GetLiteral(i).GetName().ToLower(new CultureInfo("tr")).StartsWith(root)
                        || synSet.GetSynonym().GetLiteral(i).GetName().ToLower(new CultureInfo("tr")).EndsWith(" " + root))
                    {
                        if (synSet.GetSynonym().GetLiteral(i).GetSense() < minSense)
                        {
                            minSense = synSet.GetSynonym().GetLiteral(i).GetSense();
                            best = synSet;
                        }
                    }
                }
            }

            return best;
        }

        /// <summary>
        /// The method annotates the word senses of the words in the parse tree according to the baseline most frequent
        /// algorithm. The algorithm processes target words one by one. First, the algorithm constructs an array of
        /// all possible senses for the target word to annotate. Then the sense with the minimum sense index is selected. In
        /// the wordnet, literals are ordered and indexed according to their usage. The most frequently used sense of the
        /// literal has sense number 1, then 2, etc.
        /// </summary>
        /// <param name="parseTree">Parse tree to be annotated.</param>
        /// <returns>True, if at least one word is semantically annotated, false otherwise.</returns>
        protected override bool AutoLabelSingleSemantics(ParseTreeDrawable parseTree)
        {
            var nodeDrawableCollector =
                new NodeDrawableCollector((ParseNodeDrawable) parseTree.GetRoot(), new IsTurkishLeafNode());
            var leafList = nodeDrawableCollector.Collect();
            for (int i = 0; i < leafList.Count; i++)
            {
                var synSets = GetCandidateSynSets(_turkishWordNet, _fsm, leafList, i);
                if (synSets.Count > 0)
                {
                    var best = MostFrequent(synSets,
                        leafList[i].GetLayerInfo().GetMorphologicalParseAt(0).GetWord().GetName());
                    if (best != null)
                    {
                        leafList[i].GetLayerInfo().SetLayerData(ViewLayerType.SEMANTICS, best.GetId());
                    }
                }
            }

            return true;
        }
    }
}