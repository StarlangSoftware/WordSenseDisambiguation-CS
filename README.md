# Word Sense Disambiguation Task

The task of choosing the correct sense for a word is called word sense disambiguation (WSD). WSD algorithms take an input word *w* in its context with a fixed set of potential word senses S<sub>w</sub> of that input word and produce an output chosen from S<sub>w</sub>. In the isolated WSD task, one usually uses the set of senses from a dictionary or theasurus like WordNet. 

In the literature, there are actually two variants of the generic WSD task. In the lexical sample task, a small selected set of target words is chosen, along with a set of senses for each target word. For each target word *w*, a number of corpus sentences (context sentences) are manually labeled with the correct sense of *w*. In all-words task, systems are given entire sentences and a lexicon with the set of senses for each word in each sentence. Annotators are then asked to disambiguate every word in the text.

In all-words WSD, a classifier is trained to label the words in the text with their set of potential word senses. After giving the sense labels to the words in our training data, the next step is to select a group of features to discriminate different senses for each input word.

The following Table shows an example for the word 'yüz', which can refer to the number '100', to the verb 'swim' or to the noun 'face'.

|Sense|Definition|
|---|---|
|yüz<sup>1</sup> (hundred)|The number coming after ninety nine|
|yüz<sup>2</sup> (swim)|move or float in water|
|yüz<sup>3</sup> (face)|face, visage, countenance|

For Developers
============

You can also see [Java](https://github.com/starlangsoftware/WordSenseDisambiguation), [Python](https://github.com/starlangsoftware/WordSenseDisambiguation-Py), [Cython](https://github.com/starlangsoftware/WordSenseDisambiguation-Cy), or [C++](https://github.com/starlangsoftware/WordSenseDisambiguation-CPP) repository.

## Requirements

* C# Editor
* [Git](#git)

### Git

Install the [latest version of Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Download Code

In order to work on code, create a fork from GitHub page. 
Use Git for cloning the code to your local or below line for Ubuntu:

	git clone <your-fork-git-link>

A directory called WordSenseDisambiguation-CS will be created. Or you can use below link for exploring the code:

	git clone https://github.com/starlangsoftware/WordSenseDisambiguation-CS.git

## Open project with Rider IDE

To import projects from Git with version control:

* Open Rider IDE, select Get From Version Control.

* In the Import window, click URL tab and paste github URL.

* Click open as Project.

Result: The imported project is listed in the Project Explorer view and files are loaded.


## Compile

**From IDE**

After being done with the downloading and opening project, select **Build Solution** option from **Build** menu. After compilation process, user can run WordSenseDisambiguation-CS.

Detailed Description
============

## ParseTree

In order to sense annotate a parse tree, one can use autoSemantic method of the TurkishTreeAutoSemantic class.

	ParseTreeDrawable parseTree = ...
	WordNet wordNet = new WordNet();
	FsmMorphologicalAnalyzer fsm = new FsmMorphologicalAnalyzer();
	TurkishTreeAutoSemantic turkishAutoSemantic = new TurkishTreeAutoSemantic(wordnet, fsm);
	turkishAutoSemantic.autoSemantic();

## Sentence

In order to sense annotate a parse tree, one can use autoSemantic method of the TurkishSentenceAutoSemantic class.

	AnnotatedSentence sentence = ...
	WordNet wordNet = new WordNet();
	FsmMorphologicalAnalyzer fsm = new FsmMorphologicalAnalyzer();
	TurkishSentenceAutoSemantic turkishAutoSemantic = new TurkishSentenceAutoSemantic(wordnet, fsm);
	turkishAutoSemantic.autoSemantic();

# Cite

	@INPROCEEDINGS{8093442,
  	author={O. {Açıkgöz} and A. T. {Gürkan} and B. {Ertopçu} and O. {Topsakal} and B. {Özenç} and A. B. {Kanburoğlu} and İ. {Çam} and B. {Avar} and G. {Ercan} 		and O. T. {Yıldız}},
  	booktitle={2017 International Conference on Computer Science and Engineering (UBMK)}, 
  	title={All-words word sense disambiguation for Turkish}, 
  	year={2017},
  	volume={},
  	number={},
  	pages={490-495},
  	doi={10.1109/UBMK.2017.8093442}}
